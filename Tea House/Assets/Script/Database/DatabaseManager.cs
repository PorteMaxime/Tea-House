using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class MailRandomData
{
    public string type { get; set; }

    public string value { get; set; }
}

[Table("Inventory")]
public class ItemTempInfos
{
    [PrimaryKey]
    public string name { get; set; }

    public int amount { get; set; }
    public bool isUnlocked { get; set; }
}

public class DatabaseManager : MonoBehaviour
{
    [SerializeField] public Daylight mDaylight;

    SQLiteConnection database;

    private void Awake()
    {
        database = new SQLiteConnection($"{Application.persistentDataPath}/Data.db");

        Debug.Log("Database path : " + database.DatabasePath);
    }

    void Start()
    {
        //CreateTables();
        //LoadInventory();
        //SaveInventory()
    }

    private void CreateTables()
    {
        if (database == null)
            return;

        database.CreateTable<MailRandomData>();

        database.CreateTable<Mail>();
        database.Execute("ALTER TABLE Mail ADD COLUMN IF NOT EXISTS state int");

        database.Execute("CREATE TABLE Stat (mailId int, type int, value int, CONSTRAINT foreignId FOREIGN KEY (mailId) REFERENCES Mail(id))");

        database.Execute("CREATE TABLE Inventory (name string, amount int, isUnlocked bool)");
        database.CreateTable<ItemTempInfos>();

        database.Execute("CREATE TABLE Time (id int PRIMARY KEY, currentDay int)");
        database.Execute("INSERT INTO Time VALUES (1, 0)");
    }

    public Dictionary<string, List<string>> LoadMailRandomValues()
    {
        Dictionary<string, List<string>> randomValues = new Dictionary<string, List<string>>();

        foreach (MailRandomData singleResult in database.Query<MailRandomData>("SELECT * FROM MailRandomData"))
        {
            if (!randomValues.ContainsKey(singleResult.type))
                randomValues.Add(singleResult.type, new List<string>());

            randomValues[singleResult.type].Add(singleResult.value);
        }

        return randomValues;
    }

    public void SaveDaylight()
    {
        database.Execute("UPDATE Time SET currentDay = " + mDaylight.mCurrentDay + " WHERE id = 1");
    }

    public void LoadDaylight()
    {
        int currentDay = database.ExecuteScalar<int>("SELECT currentDay FROM Time WHERE id = " + 1);

        mDaylight.mCurrentDay = currentDay;
    }

    public void SaveInventory()
    {
        ItemTempInfos itemInfos = new ItemTempInfos();

        foreach (var item in Inventory.msInstance.mItems)
        {
            itemInfos.name = item.Key.mName;
            itemInfos.amount = item.Value;
            itemInfos.isUnlocked = item.Key.mIsUnlocked;

            database.InsertOrReplace(itemInfos);
        }
    }

    public void LoadInventory()
    {
        var rows = database.Table<ItemTempInfos>().ToList();

        Inventory inventory = Inventory.msInstance;

        foreach (var itemInfos in rows)
        {
            if (inventory.mItemsLookUp.TryGetValue(itemInfos.name, out var item) == true)
            {
                inventory.mItems[item] = itemInfos.amount;
                item.mIsUnlocked = itemInfos.isUnlocked;
            }
        }
    }

    public void SaveMails()
    {
        //database.Query<Mail>();
    }
}
