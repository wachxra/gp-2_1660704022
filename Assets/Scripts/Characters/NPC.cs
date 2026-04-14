using UnityEngine;
using UnityEngine.TextCore.Text;
using System.Collections.Generic;

public class NPC : Character
{
    [SerializeField]
    private List<Quest> questToGive = new List<Quest>();
    public List<Quest> QuestsToGive { get { return questToGive; } set { questToGive = value; } }

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
