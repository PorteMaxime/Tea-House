using SQLite;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using static ItemData;

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


// money

public class DatabaseManager : MonoBehaviour
{
    [SerializeField] private Daylight mDaylight;

    private SQLiteConnection database;

    [SerializeField] private MailSystem mMailSystem;
    [SerializeField] private MailCreator mMailCreator;

    private void Awake()
    {
        database = new SQLiteConnection($"{Application.persistentDataPath}/Data.db");

        Debug.Log("Database path : " + database.DatabasePath);
    }

    void Start()
    {
        //CreateTables();

        LoadDaylight();

        LoadInventory();

        LoadMails();
    }

    private void CreateTables()
    {
        if (database == null)
            return;

        // Creation commandes
        database.CreateTable<MailRandomData>();

        // Daylight
        database.Execute("CREATE TABLE Time (id int PRIMARY KEY, currentDay int);");
        database.Execute("INSERT INTO Time VALUES (1, 0);");

        // Mails
        database.CreateTable<Mail>();
        database.CreateTable<Order>();
        database.Execute("CREATE UNIQUE INDEX IF NOT EXISTS uniqueIndex ON Order(mailId);");
        database.CreateTable<StatValue>();
        database.Execute("CREATE UNIQUE INDEX IF NOT EXISTS uniqueIndexes ON Stat(stat, mailId);");

        // Inventaire
        database.CreateTable<ItemSave>();
    }

    public Dictionary<string, List<string>> LoadMailRandomValues()
    {
        Dictionary<string, List<string>> randomValues = new Dictionary<string, List<string>>();

        foreach (MailRandomData singleResult in database.Query<MailRandomData>("SELECT * FROM MailRandomData;"))
        {
            if (!randomValues.ContainsKey(singleResult.type))
                randomValues.Add(singleResult.type, new List<string>());

            randomValues[singleResult.type].Add(singleResult.value);
        }

        return randomValues;
    }

    public void LoadDaylight()
    {
        int currentDay = database.ExecuteScalar<int>("SELECT currentDay FROM Time WHERE id = 1;");

        mDaylight.mCurrentDay = currentDay;
    }

    public void SaveDaylight()
    {
        database.Execute("UPDATE Time SET currentDay = " + mDaylight.mCurrentDay + " WHERE id = 1;");
    }

    public void LoadInventory()
    {
        var rows = database.Table<ItemSave>();

        Inventory inventory = Inventory.msInstance;

        foreach (var itemSave in rows)
        {
            if (inventory.mItemsLookUp.TryGetValue(itemSave.name, out var item) == true)
            {
                inventory.mItems[item] = itemSave.amount;
                item.mIsUnlocked = itemSave.isUnlocked;
            }
        }
    }

    public void SaveInventory()
    {
        ItemSave itemSave = new ItemSave();

        foreach (var item in Inventory.msInstance.mItems)
        {
            itemSave.name = item.Key.mName;
            itemSave.amount = item.Value;
            itemSave.isUnlocked = item.Key.mIsUnlocked;

            database.InsertOrReplace(itemSave);
        }
    }

    public void LoadMails()
    {
        var mails = database.Table<Mail>().OrderBy(mail => mail.id).ToList();
        var orders = database.Table<Order>().ToList().GroupBy(order => order.mailId).ToDictionary(g => g.Key, g => g.Last());
        var stats = database.Table<StatValue>().ToList().GroupBy(stat => stat.mMailId).ToDictionary(g => g.Key, g => g.ToList());

        Mail.mailID = mails.Count();

        foreach (Mail mail in mails)
        {
            mail.SetOrder(orders[mail.id]);
            mail.mOrder.mOrderData.mStats = stats[mail.id];
            mMailSystem.mMails[mail.mOrder.mState].Add(mail);
        }
    }

    public void SaveMails()
    {
        foreach (List<Mail> mails in mMailSystem.mMails.Values)
            foreach (Mail mail in mails)
                SaveMail(mail);

        foreach (List<Mail> mails in mMailCreator.mCalendar)
            foreach (Mail mail in mails)
                SaveMail(mail);
    }

    private void SaveMail(Mail mail)
    {
        database.InsertOrReplace(mail);
        database.InsertOrReplace(mail.mOrder);
        foreach (StatValue stat in mail.mOrder.mOrderData.mStats)
        {
            string query = "INSERT OR REPLACE INTO Stat (mailId, stat, value) VALUES (?, ?, ?);";
            database.Execute(query, mail.id, stat.mStat, stat.mValue);
        }
    }
}
