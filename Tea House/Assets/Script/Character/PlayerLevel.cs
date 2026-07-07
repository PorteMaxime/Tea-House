using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    int level = 0;
    int xpAmount = 0;
    int xpNextLevel = 10;

    public List<ItemData> itemLevel1;
    public List<ItemData> itemLevel2;
    public List<ItemData> itemLevel3;
    public List<ItemData> itemLevel4;
    public List<ItemData> itemLevel5;

    public void CheckLevel() 
    {
        if(xpAmount >= xpNextLevel)
        {
            level++;

            if(level == 1)
            {
                foreach(ItemData item in itemLevel1)
                {
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
        }
    }
}
