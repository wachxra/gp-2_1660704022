using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private int id;
    public int ID
    { get { return id; } set { id = value; } }

    [SerializeField]
    private InventoryManager inventoryManager;

    [SerializeField]
    private ItemType itemType;
    public ItemType ItemType
    { get { return itemType; } set { itemType = value; } }

    private void Start()
    {
        inventoryManager = InventoryManager.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //GetItemA
        GameObject objA = eventData.pointerDrag;
        ItemDrag itemDragA = objA.GetComponent<ItemDrag>();
        InventorySlot slotA = itemDragA.IconParent.GetComponent<InventorySlot>();

        if (ItemType == ItemType.Shield)
        {
            if (itemDragA.Item.Type != itemType)
                return;
        }

        //ThereIsAnItemBInSlotB
        if (transform.childCount > 0)
        {
            GameObject objB = transform.GetChild(0).gameObject;
            ItemDrag itemDragB = objB.GetComponent<ItemDrag>();

            if (slotA.ItemType == ItemType.Shield)
            {
                if (itemDragB.Item.Type != slotA.ItemType)
                    return;
            }

            //Remove Item A From Slot A
            inventoryManager.RemoveItemInBag(slotA.ID);

            //SetItemBOnSlotA
            itemDragB.transform.SetParent(itemDragA.IconParent);
            itemDragB.IconParent = itemDragA.IconParent;
            inventoryManager.SaveItemInBag(slotA.ID, itemDragB.Item);

            //Remove item b from slot b
            inventoryManager.RemoveItemInBag(id);
        }

        else //Slot B is blank
        {
            //Remove Item A from slot A
            inventoryManager.RemoveItemInBag(slotA.ID);
        }

        itemDragA.IconParent = transform;
        inventoryManager.SaveItemInBag(id,itemDragA.Item);

        GameObject objDrop = eventData.pointerDrag;
        ItemDrag item = objDrop.GetComponent<ItemDrag>();
        item.IconParent = transform;
    }
}
