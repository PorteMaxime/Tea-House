using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MailsPageScript : MonoBehaviour
{
    private GameObject mScrollView;
    private GameObject mScrollViewContent;
    private GameObject mCanvasMailContent;

    private GameObject mMailPrefab = null;

    private Tuple<Mail, GameObject> mSelectedMail = null;
    private Dictionary<Mail, GameObject> mMailsDictionary = new Dictionary<Mail, GameObject>();

    private void Awake()
    {
        mMailPrefab = Resources.Load<GameObject>("Mail/CanvasMailShort");

        Transform temp = transform.Find("ScrollView");
        if (temp != null)
            mScrollView = temp.gameObject;

        temp = transform.Find("ScrollView/Viewport/ContentScrollView");
        if (temp != null)
            mScrollViewContent = temp.gameObject;

        temp = transform.Find("CanvasMailContent");
        if (temp != null)
            mCanvasMailContent = temp.gameObject;

        ToggleMailContent(false);
    }

    private void OnEnable()
    {
        List<Mail> mailsToDisplay = MailManager.msInstance.GetMailsOfState(Order.State.Waiting);

        foreach (Mail mail in mailsToDisplay)
        {
            if (!mMailsDictionary.ContainsKey(mail))
                CreateMailPrefab(mail);
        }
    }

    public void SelectMail(GameObject prefabShortMail)
    {
        if (!mMailsDictionary.ContainsValue(prefabShortMail))
            return;

        if (mSelectedMail != null && prefabShortMail == mMailsDictionary[mSelectedMail.Item1])
            return;

        mSelectedMail = new Tuple<Mail, GameObject>(mMailsDictionary.FirstOrDefault(x => x.Value == prefabShortMail).Key, prefabShortMail);

        ToggleMailContent(true);

        // Enlever image "non lu"
        Transform temp = mSelectedMail.Item2.transform.Find("ButtonShowMail/ImageNotRead");
        if (temp != null)
            temp.gameObject.SetActive(false);

        // Mise à jour informations mail
        temp = mCanvasMailContent.transform.Find("ImageMailTitle/ImageProfile");
        //if (temp != null)
        //temp.GetComponent<Image>(); // changer image
        temp = mCanvasMailContent.transform.Find("ImageMailTitle/TextSubject");
        if (temp != null)
            temp.GetComponent<Text>().text = "SUJET";
        temp = mCanvasMailContent.transform.Find("ImageMailTitle/TextSender");
        if (temp != null)
            temp.GetComponent<Text>().text = "QUELQU'UN";
        temp = mCanvasMailContent.transform.Find("TextMailContent");
        if (temp != null)
            temp.GetComponent<Text>().text = "CONTENU MAIL";
    }

    public void DeSelectMail()
    {
        ToggleMailContent(false);
        mSelectedMail = null;
    }

    public void ToggleMailContent(bool active)
    {
        mCanvasMailContent.SetActive(active);

        mScrollView.GetComponent<RectTransform>().offsetMax = new Vector2(active ? -701 : 0, 0);
    }

    public void AcceptMail()
    {
        mSelectedMail.Item1.mOrder.mState = Order.State.Accepted;
        RemoveMailPrefab(mSelectedMail.Item1);
    }

    public void RejectMail()
    {
        mSelectedMail.Item1.mOrder.mState = Order.State.Rejected;
        RemoveMailPrefab(mSelectedMail.Item1);
    }

    public void CreateMailPrefab(Mail mail)
    {
        GameObject prefab = Instantiate(mMailPrefab, mScrollViewContent.transform, false);

        // Ajout action
        Button prefabButton = prefab.GetComponent<Transform>().Find("ButtonShowMail").GetComponent<Button>();
        if (prefabButton != null)
            prefabButton.onClick.AddListener(() => SelectMail(prefab));

        mMailsDictionary.Add(mail, prefab);
    }

    public void RemoveMailPrefab(Mail mail)
    {
        if (mMailsDictionary.ContainsKey(mail))
        {
            UnityEngine.Object.Destroy(mSelectedMail.Item2.transform.gameObject);

            if (mSelectedMail.Item1 == mail)
                DeSelectMail();

            mMailsDictionary.Remove(mail);
        }
    }
}

/* A faire :
 * - suppression prefabs quand les mails n'existent plus (appeler fonction via le MailManager.cs ?)
 * - mettre à jour les infos sur l'interface par rapport au mail sélectionné -> SelectMail()
 * - modifier arrière plan prefab quand il est sélectionné (afficher arrière plan différent ?) -> SelectMail() DeSelectMail()
 * - ...
*/
