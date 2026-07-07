using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public int level = 0;
    public int xpAmount = 0;
    public int xpNextLevel = 10;

    public List<ItemData> itemLevel1;
    public List<ItemData> itemLevel2;
    public List<ItemData> itemLevel3;
    public List<ItemData> itemLevel4;
    public List<ItemData> itemLevel5;

    public void Addxp(int amount)
    {
        xpAmount += amount;
        while(xpAmount >= xpNextLevel)
        {
            level++;
            xpAmount -= xpNextLevel;
        }

        CheckLevel();
    }
    public void CheckLevel()
    {
        if(level == 1)
        {
            Debug.Log("test");
            foreach(ItemData item in itemLevel1)
            {
                Debug.Log(item.name);
                item.mIsUnlocked = true;
            }
        }
        else if(level == 2)
        {
            foreach (ItemData item in itemLevel2)
            {
                item.mIsUnlocked = true;
            }
        }
        else if (level == 3)
        {
            foreach (ItemData item in itemLevel3)
            {
                item.mIsUnlocked = true;
            }
        }
        else if (level == 4)
        {
            foreach (ItemData item in itemLevel4)
            {
                item.mIsUnlocked = true;
            }
        }
        else if (level == 5)
        {
            foreach (ItemData item in itemLevel5)
            {
                item.mIsUnlocked = true;
            }
        }

        JarsScript.instance.UpdateJars();

    }
}
