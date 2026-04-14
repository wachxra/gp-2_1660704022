using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private NPC[] npcPerson;
    public NPC[] NPCPerson {  get { return npcPerson; } set { npcPerson = value; } }

    [SerializeField]
    private QuestData[] questData;
    public QuestData[] QuestData {get { return questData; } set { questData = value; } }

    [SerializeField]
    private NPC curNpc;
    public NPC CurNPC { get { return curNpc; } set { curNpc = value; } }

    [SerializeField]
    private Quest curQuest;
    public Quest CurQuest { get { return curQuest; } set {curQuest = value; } }

    public static QuestManager instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        AddQuestToNPC(npcPerson[0],questData[0]); //Give Golem - Give Potion Quest
    }

    private void AddQuestToNPC(NPC npc, QuestData questData)
    {
        Quest quest = new Quest(questData);
        npc.QuestsToGive.Add(quest);
    }

    public Quest CheckForQuest(NPC npc,QuestStatus status)
    {
        curNpc = npc;

        Quest quest = npc.CheckQuestList(status);
        curQuest = quest;

        return quest;
    }

    private bool CheckItemToDelivery()
    {
        return InventoryManager.instance.CheckPartyForItem(curQuest.QuestItemId);
    }

    public bool CheckIfFinishQuest()
    {
        bool success = false;

        Debug.Log(curQuest.Type);

        switch (curQuest.Type)
        {
            case QuestType.Delivery:
                success = CheckItemToDelivery(); break;
        }
        return success;
    }

    public bool CheckLastDialogue(int i)
    {
        if (i == curQuest.QuestDialogue.Length - 1)
            return true;
        else 
            return false;
    }

    public string NextDialogue(int i) //Map with buttonNext
    {
        if (i < curQuest.QuestDialogue.Length)
            return curQuest.QuestDialogue[i];
        else
            return "";
    }

    public void RejectQuest() //map with ButtonReject
    {
        curQuest.Status = QuestStatus.Reject;
    }

    public void AcceptQuest() //Map with ButtonAccept
    {
        curQuest.Status = QuestStatus.InProgress;
        PartyManager.instance.QuestList.Add(curQuest);
    }

    public bool DeliverItem()
    {
        return InventoryManager.instance.RemoveItemFromParty(curQuest.QuestItemId);
    }

    public bool NpcGiveReward()
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return false;
            
        Character hero = PartyManager.instance.SelectChars[0];

        Item item = new Item(InventoryManager.instance.ItemData[CurQuest.RewardItemId]);

        for (int i = 0; i < 16; i++)
        {
            if (hero.InventoryItems[i] == null)
            {
                hero.InventoryItems[i] = item;
                curQuest.Status = QuestStatus.Finish;

                UIManager.instance.ItemRewardPanel(item.ItemName, item.Icon);

                return true;
            }          
        }
        return false;
    } 
}
                                                            