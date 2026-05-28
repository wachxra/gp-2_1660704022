using UnityEngine;

public class Hero : Character
{
    [SerializeField]
    private int prefabId;
    public int PrefabID {  get { return prefabId; } }

    [SerializeField]
    private int exp;
    public int Exp { get { return exp; } set { exp = value; } }

    [SerializeField]
    private int level;
    public int Level { get { return level; } set { level = value; } }

    [SerializeField]
    private int nextExp;
    public int NextExp { get { return nextExp; } set { nextExp = value; } }

    [SerializeField]
    private int strength;
    public int Strength { get { return strength; } set { strength = value; } }

    [SerializeField]
    private int dexterity;
    public int Dexterity { get { return dexterity; } set { dexterity = value; } }

    [SerializeField]
    private int constitution;
    public int Constitution { get { return constitution; } set { constitution = value; } }

    [SerializeField]
    private int intelligence;
    public int Intelligence { get { return intelligence; } set { intelligence = value; } }

    [SerializeField]
    private int wisdom;
    public int Wisdom { get { return wisdom; } set { wisdom = value; } }

    [SerializeField]
    private int charisma;
    public int Charisma { get { return charisma; } set { charisma = value; } }

    private void Update()
    {
        switch(state)
        {
            case CharState.Walk:
                WalkUpdate();
                break;
            case CharState.WalkToEnemy:
                WalkToEnemyUpdate(); 
                break;
            case CharState.Attack:
                AttackUpdate();
                break;
            case CharState.WalkToMagicCast:
                WalkToMagicCastUpdate();
                break;
            case CharState.WalkToNPC:
                WalkToNPCUpdate();
                break;
        }    
    }

    protected void WalkToNPCUpdate()
    {
        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance <= 2f)
        {
            navAgent.isStopped = true;
            SetState(CharState.Idle);

            NPC npc = curCharTarget.GetComponent<NPC>();

            if(npc != null)
            {
                if (npc.IsShopKeeper)
                    uiManager.PrepareShopPanel(npc, this);
                else
                    uiManager.PrepareDialogueBox(npc);
            }
            else
            {
                Hero hero = curCharTarget.GetComponent<Hero>();

                if (hero != null)
                {
                    if (Setting.recruitedHeroPrefabIds.Contains(hero.PrefabID))
                    {
                        Debug.Log($"{hero.name} already recruited.");
                        return;
                    }

                    uiManager.PrepareHeroJoinParty(hero);
                }
            }
        }
    }

    public void SaveItemInInventory(Item item)
    {
        for (int i = 0; i < 16; i++)
        {
            if (InventoryItems[i] == null)
            {
                InventoryItems[i] = item;
                return;
            }
        }
    }

    public void ReceiveExp(int n)
    {
        exp += n;
        CheckLevel(exp);
    }

    private void UpdateStat()
    {
        attackDamage++;
        defensePower++;
        maxHP++;

        if(strength >= Random.Range(1, 20))
            attackDamage++;
        if(dexterity >= Random.Range(1, 20))
            defensePower++;
        if(constitution >= Random.Range(1, 20))
            maxHP++;
    }

    private void CheckLevel(int exp)
    {
        nextExp = level * 30;

        if (exp >= nextExp)
        {
            level++;
            nextExp = level * 30;
            UpdateStat();

            switch (level)
            {
                case 5:
                    magicSkills.Add(new Magic(vfxManager.MagicData[0]));
                    uiManager.ShowMagicToggles();
                    break;
            }
        }
    }
}
