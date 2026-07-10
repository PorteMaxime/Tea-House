using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class MailCreator : MonoBehaviour
{
    [SerializeField] private Daylight mDaylight;
    [SerializeField] private DatabaseManager mDatabaseManager;

    private Dictionary<int, List<MailPreset>> mMailsPresets = new Dictionary<int, List<MailPreset>>();

    private Vector2Int mIngredientsAmount = new Vector2Int(1, 3);
    private Vector2Int mDurationRange = new Vector2Int(2, 7);

    private int mPresetProbability = 8;
    private int mTorrefiedProbability = 25;

    // Informations pour creer les commandes
    [SerializeField] public List<Sprite> mImages;
    Dictionary<string, List<string>> mMailRandomContent;

    public List<Mail>[] mCalendar = new List<Mail>[7];

    public MailCreator()
    {
        for (int i = 0; i < mCalendar.Length; i++)
            mCalendar[i] = new List<Mail>();
    }

    public void Start()
    {
        // Chargement des donnees depuis la bdd
        mMailRandomContent = mDatabaseManager.LoadMailRandomValues();

        // Recuperation des presets de mails
        MailPreset[] mailsPresets = Resources.LoadAll<MailPreset>("MailsPresets");

        foreach (MailPreset mail in mailsPresets)
        {
            if (!mMailsPresets.ContainsKey(mail.mPresetLevel))
                mMailsPresets.Add(mail.mPresetLevel, new List<MailPreset>());

            mMailsPresets[mail.mPresetLevel].Add(mail);
        }
    }

    public void CreateDailyMails()
    {
        for (int i = 0; i < Random.Range(10, 70); i++)
            CreateMail();
    }

    public void CreateMail()
    {
        Mail mail = null;

        // Si preset mail
        if (Random.Range(0, 101) <= mPresetProbability && mMailsPresets.Count > 0)
        {
            int playerLevel = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLevel>().level;

            List<MailPreset> availableMails = new List<MailPreset>();

            foreach (var pair in mMailsPresets)
                if (pair.Key <= playerLevel)
                    availableMails.AddRange(pair.Value);

            if (availableMails.Count > 0)
            {
                mail = availableMails[Random.Range(0, availableMails.Count)].CreateMail();
                CreateMailData(mail);
            }
        }

        if (mail == null)
        {
            mail = new Mail();

            CreateMailData(mail);

            CreateOrder(mail);

            CreateMailDescription(mail);
        }

        mCalendar[mail.mSendDay % 7].Add(mail);
    }

    public void CreateMailData(Mail mail)
    {
        Sprite mailSprite = mImages[Random.Range(0, mImages.Count)];
        mail.mPicture = mailSprite;
        mail.mPictureName = mailSprite.name;
        if (mail.mName == "" || mail.mName == null)
            mail.mName = GetRandomValue("FirstName") + " " + GetRandomValue("LastName");
        mail.mSendDay = mDaylight.mCurrentDay + 1 + Random.Range(0, mCalendar.Length);

        if (mail.mDuration <= 0)
            mail.mDuration = Random.Range(mDurationRange.x, mDurationRange.y);
    }

    public void CreateMailDescription(Mail mail)
    {
        // Pour tests
        /*
        if (mail.mOrder == null)
        {
            mail.mText = "-> Order is not set";
            return;
        }

        mail.mText += "\nCreated day " + mail.mSendDay + " | Lasting " + mail.mDuration + " days | Expiring on day " + (mail.mSendDay + mail.mDuration);

        foreach (StatValue stat in mail.mOrder.mOrderData.mStats)
            mail.mText += "\n" + stat.mStat.ToString() + " : " + stat.mValue;
        /**/

        // Formule de politesse
        int politeRand = Random.Range(0, 5);
        if (politeRand <= 1)
            mail.mText = GetRandomValue("Greetings") + "\n";

        // Raison + pour quand
        int forWhenRand = Random.Range(0, 5);
        if (forWhenRand <= 1)
        {
            // Version courte
            mail.mText += GetRandomValue("Reason") + ".\n";
            mail.mText = Regex.Replace(mail.mText, @"\{long\((.*?)\)\}", "");
        }
        else
        {
            // Version longue
            mail.mText += GetRandomValue("Reason") + " " + GetRandomValue("Day+" + mail.mDuration.ToString()) + ".\n";
            mail.mText = Regex.Replace(mail.mText, @"\{long\((.*?)\)\}", "$1");
        }

        // Ingredients
        bool first = true;
        int rand;
        foreach (StatValue stat in mail.mOrder.mOrderData)
        {
            if (stat.mValue <= 0 ||stat.mStat == ItemData.Stat.Prestige)
                continue;

            if (first)
            {
                mail.mText += GetRandomValue("StatMain");
                mail.mText = mail.mText.Replace("{isFirst}", "");
                first = false;
            }

            if (mail.mText.Contains("{Stat") != true)
            {
                if (mail.mText.Contains("{sub") == true)
                {
                    if (mail.mText.Contains("{sub}") == true)
                        mail.mText = mail.mText.Replace("{sub}", " " + GetRandomValue("StatSub"));
                    else if (mail.mText.Contains("{subSimple}") == true)
                    {
                        mail.mText = mail.mText.Replace("{subSimple}", " " + GetRandomValue("StatSub"));
                        mail.mText = mail.mText.Replace("{Stat}", "{StatSimple}");
                    }
                }
                else
                {
                    mail.mText += GetRandomValue("StatMain");
                    mail.mText = mail.mText.Replace("{isFirst}", "also ");
                }
            }

            string StatLimit = stat.mValue > mail.mOrder.mLimitHighAmount ? "StatHigh" : stat.mValue < mail.mOrder.mLimitHighAmount ? "StatLow" : "";
            string statAmount = "";

            if (mail.mText.Contains("{Stat}"))
            {
                if (StatLimit != "")
                    statAmount = GetRandomValue(StatLimit);
                mail.mText = mail.mText.Replace("{Stat}", statAmount + stat.mStat.ToString());
            }
            else if (mail.mText.Contains("{StatSimple}"))
            {
                if (StatLimit != "")
                    statAmount = GetRandomValue(StatLimit + "Simple");
                mail.mText = mail.mText.Replace("{StatSimple}", statAmount + GetRandomValue(stat.mStat.ToString()));
            }
        }
        mail.mText = mail.mText.Replace("{sub}", "");
        mail.mText = mail.mText.Replace("{subSimple}", "");

        // Information suppl�mentaire
        rand = Random.Range(0, 3);
        if (rand <= 2)
        {
            mail.mText += "\n";

            if (mail.mOrder.mLimitHighAmount - 2 <= 1)
                mail.mText += GetRandomValue("CommandLowAmount");
            else if (mail.mOrder.mLimitHighAmount - 2 >= 3)
                mail.mText += GetRandomValue("CommandHighAmount");
            else
            {
                foreach (StatValue stat in mail.mOrder.mOrderData)
                {
                    if (stat.mValue > mail.mOrder.mLimitHighAmount)
                    {
                        mail.mText += GetRandomValue("CommandIntense");
                        break;
                    }
                }
            }
        }

        if (forWhenRand <= 1)
        {
            mail.mText += "\n" + GetRandomValue("DayEnd");
            mail.mText = mail.mText.Replace("{day}", GetRandomValue("Day+" + mail.mDuration.ToString()));
        }

        // Formule de politesse
        if (politeRand <= 1)
        mail.mText += "\n" + GetRandomValue("Ending");
    }

    private string GetRandomValue(string type)
    {
        if (mMailRandomContent.TryGetValue(type, out List<string> value) != true)
            return "(" + type + " not found)";

        return mMailRandomContent[type][Random.Range(0, mMailRandomContent[type].Count)];
    }

    public void CreateOrder(Mail mail)
    {
        Order order = new Order();

        mail.SetOrder(order);

        // Base
        List<BaseData> bases = Inventory.msInstance.GetItemsOfType<BaseData>();
        BaseData baseIngredient = bases[Random.Range(0, bases.Count)];
        //mail.mText = baseIngredient.mName; // Pour tests

        order.AddStats(baseIngredient.mStats);

        // Ingredients
        //mail.mText += "\n";
        List<IngredientData> ingredients = Inventory.msInstance.GetItemsOfType<IngredientData>();

        int ingredientAmount = Random.Range(mIngredientsAmount.x, mIngredientsAmount.y + 1);
        mail.mOrder.mLimitHighAmount += ingredientAmount;

        for (int i = 0; i < ingredientAmount; i++)
        {
            IngredientData ingredient = ingredients[Random.Range(0, ingredients.Count)];
            //mail.mText += " " + ingredient.mName; // Pour tests

            if (Inventory.msInstance.isTorrefactionUnlocked == true && Random.Range(0, 101) <= mTorrefiedProbability)
            {
                //mail.mText += " torrefied"; // Pour tests

                List<StatValue> torrefiedStats = ingredient.GetTorrefiedStats();

                order.AddStats(torrefiedStats);
            }
            else
                order.AddStats(ingredient.mStats);
            //mail.mText += " ; "; // Pour tests
        }

        for (int i = 0; i < order.mOrderData.Count; i++)
        {
            if (order.mOrderData[i].mValue != 0)
            {
                int oppositeStat = ItemData.GetOppositeIndex(order.mOrderData[i].mStat);

                if (oppositeStat >= 0 && order.mOrderData[oppositeStat].mValue != 0)
                {
                    int rand = Random.Range(0, 2);

                    if (rand == 0)
                        order.mOrderData[i].mValue = 0;
                    else
                        order.mOrderData[oppositeStat].mValue = 0;
                }
            }
        }
    }
}
