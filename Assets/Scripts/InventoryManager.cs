using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] itemPrefabs;
    public GameObject[] ItemPrefabs
    { get { return itemPrefabs; } set { itemPrefabs = value; } }

    [SerializeField]
    private ItemData[] itemData;
    public ItemData[] ItemData
    {  get { return itemData; } set { itemData = value; } }

    public const int MAXSLOT = 18;

    public static InventoryManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        AddAllItemsToNPCShop(0);
        AddAllItemsToNPCShop(1);
    }

    private void AddAllItemsToNPCShop(int npcId)
    {
        if (QuestManager.instance == null)
            return;

        if (QuestManager.instance.NPCPerson == null)
            return;

        if (npcId < 0 || npcId >= QuestManager.instance.NPCPerson.Length)
            return;

        NPC npc = QuestManager.instance.NPCPerson[npcId];

        if (npc == null)
            return;

        npc.ShopItems.Clear();

        for (int i = 0; i < 16; i++)
        {
            if (i >= itemData.Length)
                break;

            Item item = new Item(itemData[i]);
            npc.ShopItems.Add(item);
        }
    }

    public bool AddItem(Character character, int id)
    {
        Item item = new Item(itemData[id]);

        for (int i = 0; i < character.InventoryItems.Length; i++)
        {
            if (character.InventoryItems[i] == null)
            {
                character.InventoryItems[i] = item;
                return true;
            }
        }

        Debug.Log("Inventory Full");
        return false;
    }

    public void SaveItemInBag(int index, Item item)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
        {
            return;
        }

        PartyManager.instance.SelectChars[0].InventoryItems[index] = item;

        switch (index)
        {
            case 16:
                PartyManager.instance.SelectChars[0].EquipShield(item);
                AudioManager.instance.PlaySFX(4);
                break;

            case 17:
                PartyManager.instance.SelectChars[0].EquipWeapon(item);
                AudioManager.instance.PlaySFX(3);
                break;

            default:
                AudioManager.instance.PlaySFX(0);
                break;
        }
    }

    public void RemoveItemInBag(int index)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return;

        PartyManager.instance.SelectChars[0].InventoryItems[index] = null;

        switch (index)
        {
            case 16:
                PartyManager.instance.SelectChars[0].UnEquipShield();
                AudioManager.instance.PlaySFX(4);
                break;

            case 17:
                PartyManager.instance.SelectChars[0].UnEquipWeapon();
                AudioManager.instance.PlaySFX(3);
                break;

            default:
                AudioManager.instance.PlaySFX(0);
                break;
        }
    }

    public bool RemoveItemFromParty(int id)
    {
        Item item = new Item(itemData[id]);
        Debug.Log($"Finding {item.ItemName}");

        List<Character> party = PartyManager.instance.Members;

        foreach (Character hero in party)
        {
            for (int i = 0; i < hero.InventoryItems.Length; i++)
            {
                if (hero.InventoryItems[i] == null)
                    continue;

                if (hero.InventoryItems[i].ID == item.ID)
                {
                    Debug.Log($"Removing {hero.InventoryItems[i].ItemName}");
                    hero.InventoryItems[i] = null;
                    Debug.Log($"Removed {hero.InventoryItems[i]}");
                    return true;
                }
            }
        }
        return false;
    }

    void SpawnDropItem(Item item, Vector3 pos)
    {
        int dropItemId;

        switch (item.Type)
        {
            case ItemType.Consumable:
                dropItemId = 0;
                break;

            default:
                dropItemId = 16;
                break;
        }

        if (dropItemId >= itemData.Length)
            dropItemId = 0;

        Item dropItem = new Item(itemData[dropItemId]);

        int prefabId = 0;

        if (dropItem.Type == ItemType.Consumable)
            prefabId = 0;
        else
            prefabId = 1;

        if (prefabId >= ItemPrefabs.Length)
            prefabId = 0;

        GameObject itemObj = Instantiate(ItemPrefabs[prefabId], pos, Quaternion.identity);
        itemObj.AddComponent<ItemPick>();

        MeshCollider meshCol = itemObj.GetComponent<MeshCollider>();
        if (meshCol != null)
        {
            Bounds bounds = meshCol.bounds;
            float bottomY = bounds.min.y;

            Vector3 adjust = new Vector3(0, pos.y - bottomY, 0);
            itemObj.transform.position += adjust;
        }

        ItemPick itemPick = itemObj.GetComponent<ItemPick>();
        itemPick.Init(dropItem, instance, PartyManager.instance);
    }

    public void SpawnDropInventory(Item[] items, Vector3 pos)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                SpawnDropItem(items[i], pos);
        }
    }

    public void DrinkConsumableItem(Item item, int slotId)
    {
        string s = string.Format("Drink: {0}", item.ItemName);
        Debug.Log(s);

        if(PartyManager.instance.SelectChars.Count >0)
        {
            PartyManager.instance.SelectChars[0].Recovery(item.Power);
            RemoveItemInBag(slotId);
        }
    }

    public bool CheckPartyForItem(int id)
    {
        Item item = new Item(itemData[id]);
        Debug.Log(item.ItemName);

        List<Character> party = PartyManager.instance.Members;

        foreach (Character hero in party)
        {
            for (int i = 0; i < hero.InventoryItems.Length; i++)
            {
                if (hero.InventoryItems[i] == null)
                    continue;
                Debug.Log(hero.InventoryItems[i].ItemName);

                if (hero.InventoryItems[i].ID == item.ID)
                    return true;
            }
        }
        return false;
    }

    private void AddItemShopToNPC(int npcId, int itemId)
    {
        Item item = new Item(itemData[itemId]);
        QuestManager.instance.NPCPerson[npcId].ShopItems.Add(item);
    }
}
