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
    public int PartyMoney { get { return partyMoney; }set { partyMoney = value; } }

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
    private void Start()
    {
        /*if (members.Count > 0)
        {
            SelectSingleHero(0);
            UIManager.instance.ShowMagicToggles();
        }

        RandomStartLoadout();*/
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (selectChars.Count > 0)
            {
                selectChars[0].IsMagicMode = true;
                selectChars[0].CurMagicCast = selectChars[0].MagicSkills[0];
            }
        }
    }

    public void SelectSingleHero(int i)
    {
        foreach (Character c in selectChars)
            c.ToggleRingSelection(false);

        selectChars.Clear();

        selectChars.Add(members[i]);
        selectChars[0].ToggleRingSelection(true);
    }

    public void HeroSelectMagicSkill(int i)
    {
        if (selectChars.Count <= 0)
            return;

        selectChars[0].IsMagicMode = true;
        selectChars[0].CurMagicCast = selectChars[0].MagicSkills[i];
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
    }

    public void UnSelectSingleHeroByToggle(int i)
    {
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
        if(selectChars.Contains(members[id]))
            selectChars.Remove(members[id]);

        members.Remove(members[id]);
    }

    public void DistributeTotalExp(int n)
    {
        totalExp = n;
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

        for (int i = 0; i < Setting.partyCount; i++)
        {
            GameObject heroObj =
                Instantiate(GameManager.instance.HeroPrefabs[heroData[i].prefabId],
                pos, Quaternion.identity);

            if (i == 0)
                heroObj.gameObject.tag = "Player";

            Hero hero = heroObj.GetComponent<Hero>();
            hero.CharInit(VFXManager.Instance, UIManager.instance, InventoryManager.instance,
                this);
            hero.CurHP = heroData[i].curHp;

            for (int j = 0; j < heroData[i].magicIds.Count; j++)
            {
                int magicId = heroData[i].magicIds[j];
                hero.MagicSkills.Add(new Magic(VFXManager.Instance.MagicData[magicId]));
            }

            for (int k = 0; k < heroData[i].inventoryItemIds.Length; k++)
            {
                int itemId = heroData[i].inventoryItemIds[k];
                if (itemId != -1)
                    hero.InventoryItems[k] =
                        new Item(InventoryManager.instance.ItemData[itemId]);
            }

            hero.AttackDamage = heroData[i].attackDamage;
            hero.DefensePower = heroData[i].defendsePower;
            hero.Exp = heroData[i].exp;
            hero.Level = heroData[i].level;
            hero.NextExp = heroData[i].nextExp;
            members.Add(hero);
        }
    }

    public void RandomStartLoadout()
    {
        if (members.Count == 0)
            return;

        List<int> skillPool = new List<int>();

        for (int i = 0; i < 8; i++)
        {
            skillPool.Add(i);
        }

        List<int> weaponPool = new List<int>();

        for (int i = 0; i < 16; i++)
        {
            weaponPool.Add(i);
        }

        ShuffleList(weaponPool);

        int weaponIndex = 0;

        foreach (Hero hero in members)
        {
            hero.MagicSkills.Clear();

            List<int> availableSkills = new List<int>(skillPool);
            ShuffleList(availableSkills);

            for (int i = 0; i < 4; i++)
            {
                int skillId = availableSkills[i];
                hero.MagicSkills.Add(new Magic(VFXManager.Instance.MagicData[skillId]));
            }

            for (int i = 0; i < 2; i++)
            {
                if (weaponIndex >= weaponPool.Count)
                    break;

                int weaponId = weaponPool[weaponIndex];
                weaponIndex++;

                Item weapon = new Item(InventoryManager.instance.ItemData[weaponId]);
                AddWeaponToHero(hero, weapon);
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

    private void AddWeaponToHero(Hero hero, Item weapon)
    {
        if (hero == null || weapon == null)
            return;

        for (int i = 0; i < hero.InventoryItems.Length; i++)
        {
            if (hero.InventoryItems[i] == null)
            {
                hero.InventoryItems[i] = weapon;
                break;
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}