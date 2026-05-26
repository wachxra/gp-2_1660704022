using UnityEngine;
using UnityEngine.TextCore.Text;
using System.Collections.Generic;

public class NPC : Character
{
    [SerializeField]
    private List<Quest> questToGive = new List<Quest>();
    public List<Quest> QuestsToGive { get { return questToGive; } set { questToGive = value; } }

    [SerializeField]
    private bool isShopKeeper;
    public bool IsShopKeeper { get { return isShopKeeper; } }

    [SerializeField]
    private List<Item> shopItems = new List<Item>();
    public List<Item> ShopItems { get { return shopItems; } set { shopItems = value; } }

    [SerializeField]
    private int npcMoney = 3000;
    public int NpcMoney { get { return npcMoney; } set { npcMoney = value; } }

    public Quest CheckQuestList(QuestStatus status)
    {
        foreach(Quest quest in questToGive)
        {
            if (quest.Status == status)
                return quest;
        }
        return null;
    }
}
