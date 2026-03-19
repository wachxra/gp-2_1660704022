using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform selectionBox;
    public RectTransform SelectionBox { get { return selectionBox; } }

    [SerializeField]
    private Toggle togglePauseUnpause;

    [SerializeField]
    private Toggle[] toggleMagic;
    public Toggle[] ToggleMagic { get { return toggleMagic; } }

    [SerializeField]
    private int curToggleMagicTD = -1;

    [SerializeField]
    private GameObject blackImage;

    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private GameObject itemUIPrefab;

    [SerializeField]
    private GameObject[] slots;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            togglePauseUnpause.isOn = !togglePauseUnpause.isOn;
    }

    public void ToggleAi(bool isOn)
    {
        foreach (Character member in PartyManager.instance.Members)
        {
            AttackAI ai = member.gameObject.GetComponent<AttackAI>();

            if (ai != null)
                ai.enabled = isOn;
        }
    }   
    
    public void SelectAll()
    {
        PartyManager.instance.SelectChars.Clear();

        foreach (Character member in PartyManager.instance.Members)
        {
            if (member.CurHP >0)
            {
                member.ToggleRingSelection(true);
                PartyManager.instance.SelectChars.Add(member);
            }
        }    
    }

    public void PauseUnpause(bool isOn)
    {
        Time.timeScale = isOn ? 0 : 1;
    }

    /*    public void ShowMagicToggles()
        {
            if (PartyManager.instance.SelectChars.Count <= 0)
                return;

            //Show Magic skill only the single selected hero
            Character hero = PartyManager.instance.SelectChars[0];

            for (int i = 0; i < hero.MagicSkills.Count; i++)
            {
                toggleMagic[i].interactable = true;
                toggleMagic[i].isOn = false;
                toggleMagic[i].GetComponent<Text>().text = hero.MagicSkills[i].Name;
            }
        }*/

    // Hot Fixed

    public void ShowMagicToggles()
    {
        if (PartyManager.instance.SelectChars.Count <= 0)
            return;

        Character hero = PartyManager.instance.SelectChars[0];

        for (int i = 0; i < toggleMagic.Length; i++)
        {
            if (i < hero.MagicSkills.Count)
            {
                toggleMagic[i].interactable = true;
                toggleMagic[i].SetIsOnWithoutNotify(false);

                Text txt = toggleMagic[i].GetComponentInChildren<Text>();
                if (txt != null)
                    txt.text = hero.MagicSkills[i].Name;
                toggleMagic[i].targetGraphic.GetComponent<Image>().sprite = hero.MagicSkills[i].Icon;
            }
            else
            {
                toggleMagic[i].interactable = false;
                toggleMagic[i].SetIsOnWithoutNotify(false);
            }
        }
    }

    public void SelectMagicSkill(int i)
    {
        curToggleMagicTD = i;
        PartyManager.instance.HeroSelectMagicSkill(i);
    }

    public void IsOnCurToggleMagic(bool flag)
    {
        toggleMagic[curToggleMagicTD].isOn = flag;
    }    

    public void ToggleInventoryPanel()
    {
        if (!inventoryPanel.activeInHierarchy)
        {
            inventoryPanel.SetActive(true);
            blackImage.SetActive(true);
            ShowInventory();
        }
        else
        {
            inventoryPanel.SetActive(false);
            blackImage.SetActive(false);
            ClearInventory();
        }
    }

    public void ClearInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                Transform child = slots[i].transform.GetChild(0);
                Destroy(child.gameObject);
            }
        }
    }
    public void ShowInventory()
    {
        if (PartyManager.instance.SelectChars.Count <= 0)
            return;

        //Show Inventory only the single selected hero
        Character hero = PartyManager.instance.SelectChars[0];

        //Show items
        for (int i = 0; i < hero.InventoryItems.Length; i++)
        {
            if (hero.InventoryItems[i] != null)
            {
                GameObject itemObj = Instantiate(itemUIPrefab,slots[i].transform);
                itemObj.GetComponent<Image>().sprite = hero.InventoryItems[i].Icon;
            }
        }
    }
}
