using UnityEngine;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
    [SerializeField]
    private List<Character> selectChars = new List<Character>();
    public List<Character> SelectChars { get { return selectChars; } }

    [SerializeField]
    private List<Character> members = new List<Character>();
    public List<Character> Members { get { return members; } }

    [SerializeField]
    private List<Quest> questList = new List<Quest>();
    public List<Quest> QuestList { get { return questList; } }

    [SerializeField]
    private int partyMoney = 1000;
    public int PartyMoney { get { return partyMoney; } set { partyMoney = value; } }

    [SerializeField]
    private int totalExp;

    [SerializeField]
    private HeroData[] heroData;
    public HeroData[] HeroData { get { return heroData; } }

    public static PartyManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (selectChars.Count > 0 && selectChars[0].MagicSkills.Count > 0)
            {
                selectChars[0].IsMagicMode = true;
                selectChars[0].CurMagicCast = selectChars[0].MagicSkills[0];
            }
        }
    }

    public void FixedStartLoadout()
    {
        if (members.Count == 0)
            return;

        int[,] heroMagicIds =
        {
            { 0, 1, 2, 3 },
            { 4, 5, 6, 7 },
            { 0, 2, 4, 6 },
            { 1, 3, 5, 7 }
        };

        int[,] heroItemIds =
        {
            { 0, 1, 2, 3 },
            { 4, 5, 6, 7 },
            { 8, 9, 10, 11 },
            { 12, 13, 14, 15 }
        };

        int heroCount = Mathf.Min(members.Count, 4);

        for (int heroIndex = 0; heroIndex < heroCount; heroIndex++)
        {
            Hero hero = members[heroIndex] as Hero;

            if (hero == null)
                continue;

            hero.MagicSkills.Clear();

            for (int i = 0; i < 4; i++)
            {
                int magicId = heroMagicIds[heroIndex, i];

                if (magicId < 0 || magicId >= VFXManager.Instance.MagicData.Length)
                    continue;

                hero.MagicSkills.Add(new Magic(VFXManager.Instance.MagicData[magicId]));
            }

            for (int i = 0; i < hero.InventoryItems.Length; i++)
            {
                hero.InventoryItems[i] = null;
            }

            for (int i = 0; i < 4; i++)
            {
                int itemId = heroItemIds[heroIndex, i];

                if (itemId < 0 || itemId >= InventoryManager.instance.ItemData.Length)
                    continue;

                hero.InventoryItems[i] = new Item(InventoryManager.instance.ItemData[itemId]);
            }
        }

        SelectSingleHero(0);

        if (selectChars.Count > 0 && selectChars[0].MagicSkills.Count > 0)
        {
            selectChars[0].IsMagicMode = true;
            selectChars[0].CurMagicCast = selectChars[0].MagicSkills[0];
        }

        UIManager.instance.ShowMagicToggles();
    }

    public void SelectSingleHero(int i)
    {
        if (i < 0 || i >= members.Count)
            return;

        foreach (Character c in selectChars)
            c.ToggleRingSelection(false);

        selectChars.Clear();

        selectChars.Add(members[i]);
        selectChars[0].ToggleRingSelection(true);

        if (selectChars[0].MagicSkills.Count > 0)
        {
            selectChars[0].IsMagicMode = true;
            selectChars[0].CurMagicCast = selectChars[0].MagicSkills[0];
        }

        UIManager.instance.ShowMagicToggles();
    }

    public void HeroSelectMagicSkill(int i)
    {
        if (selectChars.Count <= 0)
            return;

        if (i < 0 || i >= selectChars[0].MagicSkills.Count)
            return;

        selectChars[0].IsMagicMode = true;
        selectChars[0].CurMagicCast = selectChars[0].MagicSkills[i];

        Debug.Log("CurMagicCast = " + selectChars[0].CurMagicCast.Name);
    }

    public int FindIndexFromClass(Character hero)
    {
        for (int i = 0; i < members.Count; i++)
        {
            if (members[i] == hero)
                return i;
        }

        return 0;
    }

    public void SelectSingleHeroByToggle(int i)
    {
        if (i < 0 || i >= members.Count)
            return;

        if (selectChars.Contains(members[i]))
        {
            members[i].ToggleRingSelection(true);
            UIManager.instance.ShowMagicToggles();
        }
        else
        {
            selectChars.Add(members[i]);
            members[i].ToggleRingSelection(true);
            UIManager.instance.ShowMagicToggles();
        }

        if (members[i].MagicSkills.Count > 0)
        {
            members[i].IsMagicMode = true;
            members[i].CurMagicCast = members[i].MagicSkills[0];
        }
    }

    public void UnSelectSingleHeroByToggle(int i)
    {
        if (i < 0 || i >= members.Count)
            return;

        if (selectChars.Contains(members[i]))
        {
            selectChars.Remove(members[i]);
            members[i].ToggleRingSelection(false);
        }
    }

    public void RemoveHeroFromParty(int id)
    {
        if (id == -1 || id == 0)
            return;

        if (id < 0 || id >= members.Count)
            return;

        if (selectChars.Contains(members[id]))
            selectChars.Remove(members[id]);

        members.Remove(members[id]);
    }

    public void DistributeTotalExp(int n)
    {
        totalExp = n;

        if (members.Count == 0)
            return;

        int eachHeroExp = totalExp / members.Count;

        foreach (Hero hero in members)
        {
            hero.ReceiveExp(eachHeroExp);
        }
    }

    public bool HeroJoinParty(Character hero)
    {
        if (hero == null)
            return false;

        if (!hero.CompareTag("Hero"))
            return false;

        Hero h = hero as Hero;
        if (h == null)
            return false;

        if (members.Count >= 6)
            return false;

        if (members.Contains(hero))
            return false;

        if (Setting.recruitedHeroPrefabIds.Contains(h.PrefabID))
            return false;

        Setting.recruitedHeroPrefabIds.Add(h.PrefabID);

        hero.CharInit(VFXManager.Instance, UIManager.instance,
            InventoryManager.instance, this);

        members.Add(hero);

        return true;
    }

    public void SaveAllHeroData()
    {
        for (int i = 0; i < members.Count; i++)
        {
            Hero hero = (Hero)members[i];

            heroData[i].prefabId = hero.PrefabID;
            heroData[i].curHp = hero.CurHP;

            heroData[i].magicIds.Clear();

            for (int j = 0; j < hero.MagicSkills.Count; j++)
            {
                heroData[i].magicIds.Add(hero.MagicSkills[j].ID);
            }

            for (int k = 0; k < hero.InventoryItems.Length; k++)
            {
                if (hero.InventoryItems[k] == null)
                    heroData[i].inventoryItemIds[k] = -1;
                else
                    heroData[i].inventoryItemIds[k] = hero.InventoryItems[k].ID;
            }

            heroData[i].attackDamage = hero.AttackDamage;
            heroData[i].defendsePower = hero.DefensePower;
            heroData[i].exp = hero.Exp;
            heroData[i].level = hero.Level;
            heroData[i].nextExp = hero.NextExp;
        }
    }

    public void LoadAllHeroData()
    {
        int enterId = Setting.enterPointId;
        Vector3 pos = MapManager.instance.EnterPoints[enterId].position;

        members.Clear();
        selectChars.Clear();

        for (int i = 0; i < Setting.partyCount; i++)
        {
            GameObject heroObj =
                Instantiate(GameManager.instance.HeroPrefabs[heroData[i].prefabId],
                pos, Quaternion.identity);

            if (i == 0)
                heroObj.gameObject.tag = "Player";
            else
                heroObj.gameObject.tag = "Hero";

            Hero hero = heroObj.GetComponent<Hero>();

            hero.CharInit(VFXManager.Instance, UIManager.instance,
                InventoryManager.instance, this);

            hero.CurHP = heroData[i].curHp;

            for (int j = 0; j < heroData[i].magicIds.Count; j++)
            {
                int magicId = heroData[i].magicIds[j];

                if (magicId >= 0 && magicId < VFXManager.Instance.MagicData.Length)
                    hero.MagicSkills.Add(new Magic(VFXManager.Instance.MagicData[magicId]));
            }

            for (int k = 0; k < heroData[i].inventoryItemIds.Length; k++)
            {
                int itemId = heroData[i].inventoryItemIds[k];

                if (itemId != -1 && itemId < InventoryManager.instance.ItemData.Length)
                {
                    Item item = new Item(InventoryManager.instance.ItemData[itemId]);
                    hero.InventoryItems[k] = item;

                    if (k == 16)
                        hero.EquipShield(item);

                    if (k == 17)
                        hero.EquipWeapon(item);
                }
            }

            hero.AttackDamage = heroData[i].attackDamage;
            hero.DefensePower = heroData[i].defendsePower;
            hero.Exp = heroData[i].exp;
            hero.Level = heroData[i].level;
            hero.NextExp = heroData[i].nextExp;

            members.Add(hero);
        }

        if (members.Count > 0)
            SelectSingleHero(0);
    }
}