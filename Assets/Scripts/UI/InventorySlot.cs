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
        //RemoveItemAFromSlotA
        inventoryManager.RemoveItemInBag(slotA.ID);

        //ThereIsAnItemBInSlotB
        if (transform.childCount > 0)
        {
            GameObject objB = transform.GetChild(0).gameObject;
            ItemDrag itemDragB = objB.GetComponent<ItemDrag>();

            //SetItemBOnSlotA
            itemDragB.transform.SetParent(itemDragA.IconParent);
            itemDragB.IconParent = itemDragA.IconParent;
            inventoryManager.SaveItemInBag(slotA.ID, itemDragB.Item);
        }

        itemDragA.IconParent = transform;
        inventoryManager.SaveItemInBag(id,itemDragA.Item);

        GameObject objDrop = eventData.pointerDrag;
        ItemDrag item = objDrop.GetComponent<ItemDrag>();
        item.IconParent = transform;
    }
}
