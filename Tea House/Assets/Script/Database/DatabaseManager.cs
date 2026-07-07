using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MailRandomData
{
    public string type { get; set; }

    public string value { get; set; }
}

[Table("Inventory")]
public class ItemSave
{
    [PrimaryKey]
    public string name { get; set; }

    public int amount { get; set; }
    public bool isUnlocked { get; set; }
}

public class DatabaseManager : MonoBehaviour
{
    [SerializeField] private Daylight mDaylight;

    private SQLiteConnection mDatabase;
    bool mIsDatabaseInUse = false;

    [SerializeField] private MailSystem mMailSystem;
    [SerializeField] private MailCreator mMailCreator;
    [SerializeField] private MailCanvasScript mMailCanvasScript;

    public bool mIsLoaded = false;

    private void Awake()
    {
        if (File.Exists($"{Application.persistentDataPath}/Data.db") != true)
            File.Copy($"{Application.streamingAssetsPath}/Save/Data.db", $"{Application.persistentDataPath}/Data.db");

        mDatabase = new SQLiteConnection($"{Application.persistentDataPath}/Data.db");
        mDatabase.CreateTable<Mail>();

        Debug.Log("Database path : " + mDatabase.DatabasePath);
    }

    async void Start()
    {
        // Chargement des données
        LoadingScript.msInstance.StartLoading();

        try
        {
            LoadingScript.msInstance.mCanvasGroup.alpha = 1;
            await LoadGame();
        }
        finally
        {
            LoadingScript.msInstance.StopLoading();

            // Création de mails si c'est le premier jour
            if (mDaylight.mCurrentDay <= 0)
                mMailCreator.CreateDailyMails();

            mIsLoaded = true;
        }
    }

    private void CreateTables()
    {
        if (mDatabase == null)
            return;

        // Creation commandes
        mDatabase.CreateTable<MailRandomData>();

        // Daylight
        mDatabase.Execute("CREATE TABLE Others (id string PRIMARY KEY, value int);");
        mDatabase.Execute("INSERT INTO Others VALUES ('currentDay', 0);");
        mDatabase.Execute("INSERT INTO Others VALUES ('money', 20);");

        // Mails
        mDatabase.CreateTable<Mail>();
        mDatabase.CreateTable<Order>();
        mDatabase.CreateTable<StatValue>();
        mDatabase.Execute("CREATE UNIQUE INDEX IF NOT EXISTS uniqueIndexes ON Stat(stat, mailId);");

        // Inventaire
        mDatabase.CreateTable<ItemSave>();
    }

    public Dictionary<string, List<string>> LoadMailRandomValues()
    {
        Dictionary<string, List<string>> randomValues = new Dictionary<string, List<string>>();

        foreach (MailRandomData singleResult in mDatabase.Query<MailRandomData>("SELECT * FROM MailRandomData;"))
        {
            if (!randomValues.ContainsKey(singleResult.type))
                randomValues.Add(singleResult.type, new List<string>());

            randomValues[singleResult.type].Add(singleResult.value);
        }

        return randomValues;
    }

    public async Task LoadGame()
    {
        if (mIsDatabaseInUse == true)
            return;

        mIsDatabaseInUse = true;

        try
        {
            await LoadDaylight();

            await LoadInventory();

            await LoadMails();
        }
        finally
        {
            mIsDatabaseInUse = false;
        }
    }

    public async Task SaveGame()
    {
        if (mIsDatabaseInUse == true)
            return;

        mIsDatabaseInUse = true;

        try
        {
            await SaveDaylight();

            await SaveInventory();

            await SaveMails();
        }
        finally
        {
            mIsDatabaseInUse = false;
        }
    }

    public async Task LoadDaylight()
    {
        int currentDay = 0;

        await Task.Run(() =>
        {
            string query = "SELECT value FROM Others WHERE id = 'currentDay';";
            currentDay = mDatabase.ExecuteScalar<int>(query);
        });

        mDaylight.mCurrentDay = currentDay;
    }

    public async Task SaveDaylight()
    {
        int day = mDaylight.mCurrentDay;

        await Task.Run(() =>
        {
            string query = "UPDATE Others SET value = ? WHERE id = 'currentDay';";
            mDatabase.Execute(query, day);
        });
    }

    public async Task LoadInventory()
    {
        int moneyAmount = 0;
        List<ItemSave> rows = null;
        
        await Task.Run(() =>
        {
            string query = "SELECT value FROM Others WHERE id = 'money';";
            moneyAmount = mDatabase.ExecuteScalar<int>(query);

            rows = mDatabase.Table<ItemSave>().ToList();
        });

        Inventory inventory = Inventory.msInstance;

        inventory.mMoneyAmount = moneyAmount;

        foreach (var itemSave in rows)
        {
            if (inventory.mItemsLookUp.TryGetValue(itemSave.name, out var item) == true)
            {
                inventory.mItems[item] = itemSave.amount;
                item.mIsUnlocked = itemSave.isUnlocked;
            }
        }
    }

    public async Task SaveInventory()
    {
        var tempItems = Inventory.msInstance.mItems.ToList();

        int money = Inventory.msInstance.mMoneyAmount;

        await Task.Run(() =>
        {
            ItemSave itemSave = new ItemSave();

            foreach (var item in tempItems)
            {
                itemSave.name = item.Key.mName;
                itemSave.amount = item.Value;
                itemSave.isUnlocked = item.Key.mIsUnlocked;

                mDatabase.InsertOrReplace(itemSave);
            }

            string query = "UPDATE Others SET value = ? WHERE id = 'money';";
            mDatabase.Execute(query, money);
        });
    }

    public async Task LoadMails()
    {
        List<Mail> mails = await Task.Run(LoadMailsDatabase);

        LoadMailsUnity(mails);
    }

    private List<Mail> LoadMailsDatabase()
    {
        var mails = mDatabase.Table<Mail>().OrderBy(mail => mail.id).ToList();
        var orders = mDatabase.Table<Order>().ToList().GroupBy(order => order.mailId).ToDictionary(g => g.Key, g => g.Last());
        var stats = mDatabase.Table<StatValue>().ToList().GroupBy(stat => stat.mMailId).ToDictionary(g => g.Key, g => g.ToList());

        foreach (Mail mail in mails)
        {
            mail.SetOrder(orders[mail.id]);
            mail.mOrder.mOrderData = stats[mail.id];
        }

        return mails;
    }

    private void LoadMailsUnity(List<Mail> mails)
    {
        Mail.mailID = mails.Count();

        foreach (Mail mail in mails)
        {
            mail.mPicture = mMailCreator.mImages.FirstOrDefault(image => image.name == mail.mPictureName);

            if (mail.mSendDay <= mDaylight.mCurrentDay)
            {
                mMailSystem.mMails[mail.mOrder.mState].Add(mail);

                if (mail.mOrder.mState == Order.State.InBox)
                    mMailCanvasScript.CreateMailPrefab(mail);
                else if (mail.mOrder.mState != Order.State.NotSent)
                    mMailCanvasScript.CreateOrderPrefab(mail);
            }
            else
                mMailCreator.mCalendar[mail.mSendDay % 7].Add(mail);
        }

        mMailSystem.mMails[Order.State.NotSent] = mMailSystem.mMails[Order.State.NotSent].OrderBy(mail => mail.mSendTime).ToList();

        mMailSystem.mMails[Order.State.Accepted] = mMailSystem.mMails[Order.State.Accepted].OrderBy(mail => mail.mExpiration).ToList();
        mMailSystem.mMails[Order.State.InBox] = mMailSystem.mMails[Order.State.InBox].OrderBy(mail => mail.mExpiration).ToList();
    }

    public async Task SaveMails()
    {
        List<Mail> mails = GetMails();

        await Task.Run(() => SaveMailsDatabase(mails));
    }

    private List<Mail> GetMails()
    {
        List<Mail> mails = new List<Mail>();

        foreach (List<Mail> list in mMailSystem.mMails.Values)
            mails.AddRange(list);

        foreach (List<Mail> list in mMailCreator.mCalendar)
            mails.AddRange(list);

        return mails;
    }

    private void SaveMailsDatabase(List<Mail> mails)
    {
        foreach (Mail mail in mails)
        {
            mDatabase.InsertOrReplace(mail);
            mDatabase.InsertOrReplace(mail.mOrder);
            foreach (StatValue stat in mail.mOrder.mOrderData)
            {
                string query = "INSERT OR REPLACE INTO Stat (mailId, stat, value) VALUES (?, ?, ?);";
                mDatabase.Execute(query, mail.id, stat.mStat, stat.mValue);
            }
        }
    }
}
