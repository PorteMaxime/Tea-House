using Palmmedia.ReportGenerator.Core.Parser.Filtering;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        mOrderPrefab = Resources.Load<GameObject>("Order/CanvasOrderShort");

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

        mFilterPrefab = Resources.Load<GameObject>("Order/CanvasFilter");

        foreach (Order.State state in mAvailableFilters)
        {
            CreateFilterPrefab(state);
        }

        mCurrentFilters = mAvailableFilters;

        ToggleOrderContent(false);
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
                UpdateOrderState(mail);
        }

        UpdateMailsVisibility();

        mCanvasPopup.SetActive(false);
    }

    public void CreateOrderPrefab(Mail mail)
    {
        GameObject prefab = Instantiate(mOrderPrefab, mScrollViewContent.transform, false);

        Transform temp = prefab.transform.Find("ButtonShowOrder/TextState");
        if (temp != null)
            temp.GetComponent<Text>().text = mail.mOrder.mState.ToString();

        // Ajout action
        Button prefabButton = prefab.transform.Find("ButtonShowOrder").GetComponent<Button>();
        if (prefabButton != null)
            prefabButton.onClick.AddListener(() => SelectMail(mail));

        mMailsDictionary.Add(mail, prefab);
    }
    private void UpdateOrderState(Mail mail)
    {
        Transform temp = mMailsDictionary[mail].transform.Find("ButtonShowOrder/TextState");
        if (temp != null)
            temp.GetComponent<Text>().text = mail.mOrder.mState.ToString();

        UpdateMailsVisibility();
    }

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

    public void SelectMail(Mail mail)
    {
        if (!mMailsDictionary.ContainsKey(mail))
            return;

        // Si le mail est déjà sélectionné
        if (mSelectedMail != null && mail == mSelectedMail.Item1)
            return;

        mSelectedMail = new Tuple<Mail, GameObject>(mail, mMailsDictionary[mail]);
        // Mise à jour informations mail
        Transform temp = mCanvasOrderContent.transform.Find("ImageOrderContent");
        //if (temp != null)
            //temp.GetComponent<Image>(); // changer image
        temp = mCanvasOrderContent.transform.Find("TextMailContent");
        if (temp != null)
            temp.GetComponent<Text>().text = "CONTENU MAIL ( / order)";

        Button sendButton = mCanvasOrderContent.transform.Find("ButtonSend").GetComponent<Button>();
        if (sendButton != null)
        {
            if ( mail.mOrder.mState == Order.State.Accepted)
                sendButton.interactable = true;
            else
                sendButton.interactable = false;
        }

        ToggleOrderContent(true);
    }

    public void ToggleOrderContent(bool active)
    {
        mCanvasOrderContent.SetActive(active);

        mScrollView.GetComponent<RectTransform>().offsetMax = new Vector2(active ? -701 : 0, -60);
        mCanvasFilters.GetComponent<RectTransform>().offsetMax = new Vector2(active ? -701 : 0, 0);
    }

    public void SetOrderState(Mail mail, Order.State newState)
    {
        mail.mOrder.mState = newState;

        Transform temp = mMailsDictionary[mail].transform.Find("ButtonShowOrder/TextState");
        if (temp != null)
            temp.GetComponent<Text>().text = mail.mOrder.mState.ToString();
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
        foreach (Mail mail in mMailsDictionary.Keys)
        {
            if (mCurrentFilters.Contains(mail.mOrder.mState))
                mMailsDictionary[mail].SetActive(true);
            else
                mMailsDictionary[mail].SetActive(false);
        }
    }

    public void SendOrder()
    {
        Debug.Log("envoyer commande");

        mSelectedMail.Item1.mOrder.mState = Order.State.Completed;
        UpdateOrderState(mSelectedMail.Item1);

        mSelectedMail = null;
        ToggleOrderContent(false);
    }
}

/* A faire :
 * - Envoyer la commande -> SendOrder()
 * - Score de la commande lorsqu'elle est envoyée, afficher le score (à la place du bouton envoyer ?) -> SendOrder()
 * - modifier l'image et le texte en fonction de la commande sélectionnée -> SelectMail()
 * - mettre à jour l'état des commandes lorsqu'elles expirent si les commandes peuvent expirer en pleine journée -> MailManager.cs
 * - ...
*/
