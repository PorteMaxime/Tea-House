using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MailCanvasScript : MonoBehaviour
{
    MailSystem mMailSystem;

    // Page mails
    [Header("Page Mails")]

    [SerializeField] private GameObject mMailPrefab = null;
    [SerializeField] private GameObject mMailsScrollView;
    [SerializeField] private GameObject mMailsScrollViewContent;

    [SerializeField] private GameObject mCanvasMailContent;
    [SerializeField] private GameObject mMailImageProfile;
    [SerializeField] private GameObject mMailTextSubject;
    [SerializeField] private GameObject mMailTextSender;
    [SerializeField] private GameObject mTextMailContent;

    private Tuple<Mail, GameObject> mSelectedMail = null;
    public Dictionary<Mail, GameObject> mMailsDictionary = new Dictionary<Mail, GameObject>();

    // Page orders
    [Header("Page Orders")]

    [SerializeField] private GameObject mOrderPrefab = null;
    [SerializeField] private GameObject mOrderImageProfile;
    [SerializeField] private GameObject mOrdersScrollView;
    [SerializeField] private GameObject mOrdersScrollViewContent;

    [SerializeField] private GameObject mCanvasOrderContent;

    private Tuple<Mail, GameObject> mSelectedOrder = null;
    public Dictionary<Mail, GameObject> mOrdersDictionary = new Dictionary<Mail, GameObject>();

    [SerializeField] private GameObject mFilterPrefab = null;
    [SerializeField] private GameObject mCanvasFilters;
    [SerializeField] private GameObject mCanvasFiltersContent;
    [SerializeField] private GameObject mCanvasFiltersPopup;

    [SerializeField] private List<Order.State> mAvailableFilters;
    private List<Order.State> mCurrentFilters;


    private Inventory_Station mPackInv = null;

    //

    void Awake()
    {
        mMailSystem = GetComponent<MailSystem>();

        ToggleMailContent(false);

        ToggleOrderContent(false);

        ToggleFiltersContent(false);

        // Creation filtres
        foreach (Order.State state in mAvailableFilters)
            CreateFilterPrefab(state);

        mCurrentFilters = new List<Order.State> { Order.State.Accepted };
    }

    void Start()
    {
        mPackInv = GameObject.Find("Inventory_Station").GetComponent<Inventory_Station>();
    }

    #region Page Mails

    public void SelectMail(Mail mail)
    {
        if (!mMailsDictionary.ContainsKey(mail))
            return;

        // Si le mail est deja selectionne
        if (mSelectedMail != null && mail == mSelectedMail.Item1)
            return;

        mSelectedMail = new Tuple<Mail, GameObject>(mail, mMailsDictionary[mail]);

        // Enlever image "non lu"
        Transform temp = mSelectedMail.Item2.transform.Find("ButtonShowMail/ImageNotRead");
        if (temp != null && temp.gameObject.activeSelf == true)
            temp.gameObject.SetActive(false);

        // Mise a jour informations mail
        mMailImageProfile.GetComponent<Image>().sprite = mail.mPicture;
        mMailTextSubject.GetComponent<Text>().text = mail.mText;
        mMailTextSender.GetComponent<Text>().text = mail.mName;
        mTextMailContent.GetComponent<Text>().text = mail.mText;

        ToggleMailContent(true);
    }

    public void DeSelectMail()
    {
        ToggleMailContent(false);
        mSelectedMail = null;
    }

    public void ToggleMailContent(bool active)
    {
        mCanvasMailContent.SetActive(active);

        mMailsScrollView.GetComponent<RectTransform>().offsetMax = new Vector2(active ? -1051 : -20, -120);
    }

    public void CreateMailPrefab(Mail mail)
    {
        GameObject prefab = Instantiate(mMailPrefab, mMailsScrollViewContent.transform, false);

        // Ajout action
        Transform prefabButton = prefab.transform.Find("ButtonShowMail");
        if (prefabButton != null)
        {
            prefabButton.GetComponent<Button>().onClick.AddListener(() => SelectMail(mail));

            // Image et texte
            Transform prefabImage = prefabButton.transform.Find("ImageProfileOutside/ImageProfile");
            if (prefabImage != null)
                prefabImage.GetComponent<Image>().sprite = mail.mPicture;

            Transform prefabText = prefabButton.transform.Find("TextContent");
            if (prefabText != null)
                prefabText.GetComponent<Text>().text = mail.mText;

            // image non lu

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

    public void AcceptMail()
    {
        mMailSystem.ChangeOrderState(mSelectedMail.Item1, Order.State.Accepted);

        CreateOrderPrefab(mSelectedMail.Item1);

        RemoveMailPrefab(mSelectedMail.Item1);

        mMailSystem.mMails[Order.State.Accepted] = mMailSystem.mMails[Order.State.Accepted].OrderBy(mail => mail.mExpiration).ToList();
    }

    public void RejectMail()
    {
        mMailSystem.ChangeOrderState(mSelectedMail.Item1, Order.State.Rejected);

        CreateOrderPrefab(mSelectedMail.Item1);

        RemoveMailPrefab(mSelectedMail.Item1);
    }

    #endregion Page Mails

    #region Page Orders

    public void SelectOrder(Mail mail)
    {
        if (!mOrdersDictionary.ContainsKey(mail))
            return;

        // Si l'order est d�j� s�lectionn�
        if (mSelectedOrder != null && mail == mSelectedOrder.Item1)
            return;

        mSelectedOrder = new Tuple<Mail, GameObject>(mail, mOrdersDictionary[mail]);

        mOrderImageProfile.GetComponent<Image>().sprite = mail.mPicture;

        Transform temp = mCanvasOrderContent.transform.Find("TextMailContent");
        if (temp != null)
            temp.GetComponent<Text>().text = mail.mText;

        // Active ou d�sactive le bouton "envoyer"
        Transform sendButton = mCanvasOrderContent.transform.Find("ButtonSend");
        if (sendButton != null)
            sendButton.GetComponent<Button>().interactable = mail.mOrder.mState == Order.State.Accepted ? true : false;

        ToggleOrderContent(true);
    }

    public void DeSelectOrder()
    {
        ToggleOrderContent(false);
        mSelectedOrder = null;
    }

    public void ToggleOrderContent(bool active)
    {
        mCanvasOrderContent.SetActive(active);

        mOrdersScrollView.GetComponent<RectTransform>().offsetMax = new Vector2(active ? -1051 : -40, -120);
    }

    public void CreateOrderPrefab(Mail mail)
    {
        GameObject prefab = Instantiate(mOrderPrefab, mOrdersScrollViewContent.transform, false);

        // Ajout action
        Transform prefabButton = prefab.transform.Find("ButtonShowOrder");
        if (prefabButton != null)
        {
            prefabButton.GetComponent<Button>().onClick.AddListener(() => SelectOrder(mail));

            Transform prefabState = prefabButton.Find("TextState");
            if (prefabState != null)
                prefabState.GetComponent<Text>().text = mail.mOrder.mState.ToString();

            Transform prefabText = prefabButton.Find("TextDescription");
            if (prefabText != null)
                prefabText.GetComponent<Text>().text = mail.mText;
        }

        if (!mCurrentFilters.Contains(mail.mOrder.mState))
            prefab.SetActive(false);

        mOrdersDictionary.Add(mail, prefab);
    }

    public void RemoveOrderPrefab(Mail mail)
    {
        if (mOrdersDictionary.ContainsKey(mail))
        {
            UnityEngine.Object.Destroy(mOrdersDictionary[mail].gameObject);
            mOrdersDictionary.Remove(mail);
        }

        if (mSelectedOrder != null && mSelectedOrder.Item1 == mail)
            DeSelectOrder();
    }

    public void UpdatePrefabState(Mail mail)
    {
        Transform temp = mOrdersDictionary[mail].transform.Find("ButtonShowOrder/TextState");
        if (temp != null)
            temp.GetComponent<Text>().text = mail.mOrder.mState.ToString();

        Transform sendButton = mCanvasOrderContent.transform.Find("ButtonSend");
        if (sendButton != null)
            sendButton.GetComponent<Button>().interactable = mail.mOrder.mState == Order.State.Accepted ? true : false;

        UpdateOrdersVisibility();
    }

    public void TrySendOrder()
    {
        PackageScript package = mPackInv.GetPackage(mSelectedOrder.Item1);
        if (package == null)
        {
            Debug.Log("Order can't be send");
            return; // Not completed
        }

        int score = package.GetCompletionScore(); // => function to complete with scoring system
        Destroy(package.gameObject);
        // TODO: Consequences of completion
        mMailSystem.ChangeOrderState(mSelectedOrder.Item1, Order.State.Completed);

        UpdatePrefabState(mSelectedOrder.Item1);

        if (!mCurrentFilters.Contains(Order.State.Completed))
        {
            mSelectedOrder = null;
            ToggleOrderContent(false);
        }
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLevel>().Addxp(10);
    }

    #region Filters

    public void CreateFilterPrefab(Order.State state)
    {
        GameObject prefab = Instantiate(mFilterPrefab, mCanvasFiltersContent.transform, false);

        Toggle prefabToggle = prefab.transform.Find("PanelFilter/ToggleFilter").GetComponent<Toggle>();

        if (prefabToggle != null)
        {
            if (state == Order.State.Accepted)
                prefabToggle.isOn = true;
            prefabToggle.onValueChanged.AddListener((bool clicked) => ToggleFilter(state, clicked));
        }

        Text text = prefabToggle.transform.Find("Label").GetComponent<Text>();

        if (text != null)
            text.text = state.ToString();
    }

    public void ToggleFiltersContent()
    {
        ToggleFiltersContent(!mCanvasFiltersPopup.activeSelf);
    }

    public void ToggleFiltersContent(bool active)
    {
        mCanvasFiltersPopup.SetActive(active);
    }

    public void ToggleFilter(Order.State state, bool active)
    {
        if (active == true && !mCurrentFilters.Contains(state))
            mCurrentFilters.Add(state);
        else if (mCurrentFilters.Contains(state))
        {
            mCurrentFilters.Remove(state);

            if (mSelectedOrder != null && !mCurrentFilters.Contains(mSelectedOrder.Item1.mOrder.mState))
                DeSelectOrder();
        }

        UpdateOrdersVisibility();
    }

    private void UpdateOrdersVisibility()
    {
        bool isMailInFilter = false;

        foreach (Mail mail in mOrdersDictionary.Keys)
        {
            if (mCurrentFilters.Contains(mail.mOrder.mState))
            {
                isMailInFilter = true;
                mOrdersDictionary[mail].SetActive(true);
            }
            else
                mOrdersDictionary[mail].SetActive(false);
        }

        if (!isMailInFilter)
            DeSelectMail();
    }

    #endregion Filters

    #endregion Page Orders

}
