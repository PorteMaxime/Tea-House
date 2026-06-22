//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class MailsPageScript : MonoBehaviour
//{
//    private GameObject mScrollView;
//    private GameObject mScrollViewContent;
//    private GameObject mCanvasMailContent;

//    private GameObject mMailPrefab = null;

//    private List<Mail> mMailsToCreate = new List<Mail>();

//    private Tuple<Mail, GameObject> mSelectedMail = null;
//    private Dictionary<Mail, GameObject> mMailsDictionary = new Dictionary<Mail, GameObject>();

//    private MailSystem mMailSystem;

//    private void Awake()
//    {
//        mMailPrefab = Resources.Load<GameObject>("PC/Mail/CanvasMailShort");

//        Transform temp = transform.Find("ScrollView");
//        if (temp != null)
//            mScrollView = temp.gameObject;

//        temp = transform.Find("ScrollView/Viewport/ContentScrollView");
//        if (temp != null)
//            mScrollViewContent = temp.gameObject;

//        temp = transform.Find("CanvasMailContent");
//        if (temp != null)
//            mCanvasMailContent = temp.gameObject;

//        ToggleMailContent(false);

//        mMailSystem = gameObject.GetComponentInParent<MailSystem>();

//        mMailSystem.mOnMailSent += OnMailSent;
//        mMailSystem.mOnMailExpired += OnMailExpired;
//    }

//    private void OnEnable()
//    {
//        foreach (Mail mail in mMailsToCreate)
//            CreateMailPrefab(mail);

//        mMailsToCreate.Clear();
//    }

//    public void SelectMail(Mail mail)
//    {
//        if (!mMailsDictionary.ContainsKey(mail))
//            return;

//        // Si le mail est déjà sélectionné
//        if (mSelectedMail != null && mail == mSelectedMail.Item1)
//            return;

//        mSelectedMail = new Tuple<Mail, GameObject>(mail, mMailsDictionary[mail]);

//        // Enlever image "non lu"
//        Transform temp = mSelectedMail.Item2.transform.Find("ButtonShowMail/ImageNotRead");
//        if (temp != null && temp.gameObject.activeSelf == true)
//            temp.gameObject.SetActive(false);

//        // Mise à jour informations mail
//        temp = mCanvasMailContent.transform.Find("ImageMailTitle/ImageProfile");
//        if (temp != null)
//            temp.GetComponent<Image>().sprite = mail.mPicture;
//        temp = mCanvasMailContent.transform.Find("ImageMailTitle/TextSubject");
//        if (temp != null)
//            temp.GetComponent<Text>().text = mail.mText;
//        temp = mCanvasMailContent.transform.Find("ImageMailTitle/TextSender");
//        if (temp != null)
//            temp.GetComponent<Text>().text = mail.mName;
//        temp = mCanvasMailContent.transform.Find("TextMailContent");
//        if (temp != null)
//            temp.GetComponent<Text>().text = mail.mText;

//        ToggleMailContent(true);
//    }

//    public void DeSelectMail()
//    {
//        ToggleMailContent(false);
//        mSelectedMail = null;
//    }

//    public void ToggleMailContent(bool active)
//    {
//        mCanvasMailContent.SetActive(active);

//        mScrollView.GetComponent<RectTransform>().offsetMax = new Vector2(active ? -1051 : -20, -120);
//    }

//    public void AcceptMail()
//    {
//        mMailSystem.ChangeOrderState(mSelectedMail.Item1, Order.State.Accepted);
//        RemoveMailPrefab(mSelectedMail.Item1);
//    }

//    public void RejectMail()
//    {
//        mMailSystem.ChangeOrderState(mSelectedMail.Item1, Order.State.Rejected);
//        RemoveMailPrefab(mSelectedMail.Item1);
//    }

//    public void CreateMailPrefab(Mail mail)
//    {
//        GameObject prefab = Instantiate(mMailPrefab, mScrollViewContent.transform, false);

//        // Ajout action
//        Transform prefabButton = prefab.transform.Find("ButtonShowMail");
//        if (prefabButton != null)
//        {
//            prefabButton.GetComponent<Button>().onClick.AddListener(() => SelectMail(mail));

//            // Image et texte
//            Transform prefabImage = prefabButton.transform.Find("ImageProfile");
//            if (prefabImage != null)
//                prefabImage.GetComponent<Image>().sprite = mail.mPicture;

//            Transform prefabText = prefabButton.transform.Find("TextContent");
//            if (prefabText != null)
//                prefabText.GetComponent<Text>().text = mail.mText;

//            // image non lu

//        }

//        mMailsDictionary.Add(mail, prefab);
//    }

//    public void RemoveMailPrefab(Mail mail)
//    {
//        if (mMailsDictionary.ContainsKey(mail))
//        {
//            UnityEngine.Object.Destroy(mMailsDictionary[mail].gameObject);
//            mMailsDictionary.Remove(mail);
//        }

//        if (mSelectedMail != null && mSelectedMail.Item1 == mail)
//            DeSelectMail();
//    }

//    private void OnMailSent(Mail mail)
//    {
//        if (gameObject.activeSelf == true)
//            CreateMailPrefab(mail);
//        else
//            mMailsToCreate.Add(mail);
//    }

//    private void OnMailExpired(Mail mail)
//    {
//        if (mMailsToCreate.Contains(mail))
//            mMailsToCreate.Remove(mail);
//        else if (mMailsDictionary.ContainsKey(mail))
//            RemoveMailPrefab(mail);
//    }
//}

///* A faire :
// * - modifier arrière plan prefab quand il est sélectionné ? (afficher arrière plan différent ?) -> SelectMail() DeSelectMail()
// * - mettre image non lue aà la création du préfab
// * - ...
//*/
