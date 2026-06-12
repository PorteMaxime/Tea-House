using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrdersPageScript : MonoBehaviour
{
    private GameObject mScrollView;
    private GameObject mScrollViewContent;
    private GameObject mCanvasOrderContent;

    private GameObject mOrderPrefab = null;

    private Tuple<Mail, GameObject> mSelectedMail = null;
    private Dictionary<Mail, GameObject> mMailsDictionary = new Dictionary<Mail, GameObject>();

    [SerializeField] private List<Order.State> mAvailableFilters;
    private List<Order.State> mCurrentFilters;
    private GameObject mCanvasFilters;
    private GameObject mCanvasFiltersContent;
    private GameObject mCanvasPopup;

    private GameObject mFilterPrefab = null;

    private void Awake()
    {
        mOrderPrefab = Resources.Load<GameObject>("PC/Order/CanvasOrderShort");

        Transform temp = transform.Find("ScrollView");
        if (temp != null)
            mScrollView = temp.gameObject;

        temp = transform.Find("ScrollView/Viewport/ContentScrollView");
        if (temp != null)
            mScrollViewContent = temp.gameObject;

        temp = transform.Find("CanvasOrderContent");
        if (temp != null)
            mCanvasOrderContent = temp.gameObject;

        temp = transform.Find("CanvasFilters");
        if (temp != null)
            mCanvasFilters = temp.gameObject;

        temp = transform.Find("CanvasFilters/CanvasPopup");
        if (temp != null)
            mCanvasPopup = temp.gameObject;

        temp = mCanvasPopup.transform.Find("CanvasFiltersContent");
        if (temp != null)
            mCanvasFiltersContent = temp.gameObject;

        mFilterPrefab = Resources.Load<GameObject>("PC/Order/CanvasFilter");

        foreach (Order.State state in mAvailableFilters)
        {
            CreateFilterPrefab(state);
        }

        mCurrentFilters = mAvailableFilters;

        ToggleOrderContent(false);

        MailManager.mOnMailExpired += OnMailExpired;
    }

    private void OnEnable()
    {
        if (MailManager.msInstance == null)
            return;

        List<Mail> mailsAvailable = MailManager.msInstance.GetMailsOfState(mAvailableFilters.ToArray());

        foreach (Mail mail in mailsAvailable)
        {
            if (!mMailsDictionary.ContainsKey(mail))
                CreateOrderPrefab(mail);
            else
                UpdateOrderPrefabState(mail);
        }

        UpdateMailsVisibility();

        if (mCanvasPopup != null)
            mCanvasPopup.SetActive(false);
    }

    public void CreateOrderPrefab(Mail mail)
    {
        GameObject prefab = Instantiate(mOrderPrefab, mScrollViewContent.transform, false);

        // Ajout action
        Transform prefabButton = prefab.transform.Find("ButtonShowOrder");
        if (prefabButton != null)
        {
            prefabButton.GetComponent<Button>().onClick.AddListener(() => SelectMail(mail));

            Transform prefabState = prefabButton.Find("TextState");
            if (prefabState != null)
                prefabState.GetComponent<Text>().text = mail.mOrder.mState.ToString();

            Transform prefabText = prefabButton.Find("TextDescription");
            if (prefabText != null)
                prefabText.GetComponent<Text>().text = mail.mText;

        }

        mMailsDictionary.Add(mail, prefab);
    }

    public void RemoveMailPrefab(Mail mail)
    {
        if (mMailsDictionary.ContainsKey(mail))
        {
            UnityEngine.Object.Destroy(mMailsDictionary[mail].gameObject);
            mMailsDictionary.Remove(mail);
        }

        if (mSelectedMail != null && mSelectedMail.Item1 == mail)
            DeSelectMail();
    }

    private void UpdateOrderPrefabState(Mail mail)
    {
        Transform temp = mMailsDictionary[mail].transform.Find("ButtonShowOrder/TextState");
        if (temp != null)
            temp.GetComponent<Text>().text = mail.mOrder.mState.ToString();

        Transform sendButton = mCanvasOrderContent.transform.Find("ButtonSend");
        if (sendButton != null)
            sendButton.GetComponent<Button>().interactable = mail.mOrder.mState == Order.State.Accepted ? true : false;

        UpdateMailsVisibility();
    }

    public void SelectMail(Mail mail)
    {
        if (!mMailsDictionary.ContainsKey(mail))
            return;

        // Si le mail est déjà sélectionné
        if (mSelectedMail != null && mail == mSelectedMail.Item1)
            return;

        mSelectedMail = new Tuple<Mail, GameObject>(mail, mMailsDictionary[mail]);

        Transform temp = mCanvasOrderContent.transform.Find("ImageOrderContent");
        //if (temp != null)
            //temp.GetComponent<Image>(); // changer image
        temp = mCanvasOrderContent.transform.Find("TextMailContent");
        if (temp != null)
            temp.GetComponent<Text>().text = mail.mText;

        // Active ou désactive le bouton "envoyer"
        Transform sendButton = mCanvasOrderContent.transform.Find("ButtonSend");
        if (sendButton != null)
            sendButton.GetComponent<Button>().interactable = mail.mOrder.mState == Order.State.Accepted ? true : false;

        ToggleOrderContent(true);
    }

    public void DeSelectMail()
    {
        ToggleOrderContent(false);
        mSelectedMail = null;
    }

    public void ToggleOrderContent(bool active)
    {
        mCanvasOrderContent.SetActive(active);

        mScrollView.GetComponent<RectTransform>().offsetMax = new Vector2(active ? -901 : -40, -120);
        mCanvasFilters.GetComponent<RectTransform>().offsetMax = new Vector2(active ? -901 : -40, 0);
    }

    private void OnMailExpired(Mail mail)
    {
        if (mMailsDictionary.ContainsKey(mail))
            UpdateOrderPrefabState(mail);
        else
            CreateOrderPrefab(mail);
    }

    #region Filters

    public void CreateFilterPrefab(Order.State state)
    {
        GameObject prefab = Instantiate(mFilterPrefab, mCanvasFiltersContent.transform, false);

        Toggle prefabToggle = prefab.transform.Find("ToggleFilter").GetComponent<Toggle>();

        if (prefabToggle != null)
            prefabToggle.onValueChanged.AddListener((bool clicked) => ToggleFilter(state, clicked));

        Text text = prefabToggle.transform.Find("Label").GetComponent<Text>();

        if (text != null)
            text.text = state.ToString();
    }

    public void ToggleFiltersContent()
    {
        ToggleFiltersContent(!mCanvasPopup.activeSelf);
    }

    public void ToggleFiltersContent(bool active)
    {
        mCanvasPopup.SetActive(active);
    }

    public void ToggleFilter(Order.State state, bool active)
    {
        if (active == true && !mAvailableFilters.Contains(state))
            mAvailableFilters.Add(state);
        else if (mAvailableFilters.Contains(state))
            mAvailableFilters.Remove(state);

        UpdateMailsVisibility();
    }
    private void UpdateMailsVisibility()
    {
        bool isMailInFilter = false;

        foreach (Mail mail in mMailsDictionary.Keys)
        {
            if (mCurrentFilters.Contains(mail.mOrder.mState))
            {
                isMailInFilter = true;
                mMailsDictionary[mail].SetActive(true);
            }
            else
                mMailsDictionary[mail].SetActive(false);
        }

        if (!isMailInFilter)
            DeSelectMail();
    }

    #endregion Filters

    public void SendOrder()
    {
        MailManager.msInstance.ChangeOrderState(mSelectedMail.Item1, Order.State.Completed);
        UpdateOrderPrefabState(mSelectedMail.Item1);

        mSelectedMail = null;
        ToggleOrderContent(false);
    }
}

/* A faire :
 * - Envoyer la commande -> SendOrder()
 * - Score de la commande lorsqu'elle est envoyée, afficher le score (à la place du bouton envoyer ?) -> SendOrder()
 * - modifier l'image fonction de la commande sélectionnée -> SelectMail()
 * - ...
*/
