using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeftClick : MonoBehaviour
{
    public static LeftClick instance;

    private Camera cam;

    [SerializeField]
    private RectTransform boxSelection;
    private Vector2 oldAnchoredPos;
    private Vector2 startPos;

    [SerializeField]
    private LayerMask layerMask;

    private void Start()
    {
        instance = this;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Ground", "Character", "Building", "Item");

        boxSelection = UIManager.instance.SelectionBox;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            ClearEverything();
        }

        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox(Input.mousePosition);
            TrySelect(Input.mousePosition);
        }
    }

    private int SelectCharacter(RaycastHit hit)
    {

        ClearEverything();

        Character hero = hit.collider.GetComponent<Character>();

        int i = PartyManager.instance.FindIndexFromClass(hero);

        UIManager.instance.ToggleAvatar[i].isOn = true;
        return i;
    }

    private void TrySelect(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        int i = 0;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Player":
                case "Hero":
                    i = SelectCharacter(hit);
                    break;
            }
        }

        if (PartyManager.instance.SelectChars.Count == 0)
            UIManager.instance.ToggleAvatar[i].isOn = true;
    }

    private void ClearRingSelection()
    {
        foreach (Character h in PartyManager.instance.SelectChars)
            h.ToggleRingSelection(false);
    }

    private void ClearEverything()
    {
        foreach (Toggle t in UIManager.instance.ToggleAvatar)
            t.isOn = false;

        ClearRingSelection();
        PartyManager.instance.SelectChars.Clear();
    }

    private void UpdateSelectionBox(Vector2 mousePos)
    {
        if (!boxSelection.gameObject.activeInHierarchy)
            boxSelection.gameObject.SetActive(true);

        float width = mousePos.x - startPos.x;
        float height = mousePos.y - startPos.y;

        boxSelection.anchoredPosition = startPos + new Vector2(width / 2,height /2);

        width = Mathf.Abs(width);
        height = Mathf.Abs(height);

        boxSelection.sizeDelta = new Vector2(width,height);

        oldAnchoredPos = boxSelection.anchoredPosition;
    }

    private void ReleaseSelectionBox(Vector2 mousePos)
    {
        boxSelection.gameObject.SetActive(false);

        Vector2 corner1 = boxSelection.anchoredPosition - (boxSelection.sizeDelta / 2);
        Vector2 corner2 = boxSelection.anchoredPosition + (boxSelection.sizeDelta / 2);

        for (int m = 0; m < PartyManager.instance.Members.Count; m++)
        {
            Character member = PartyManager.instance.Members[m];

            Vector2 unitPos = cam.WorldToScreenPoint(member.transform.position);

            if ((unitPos.x > corner1.x && unitPos.x < corner2.x)
                && (unitPos.y > corner1.y && unitPos.y < corner2.y))
            {
                Debug.Log($"Found in box: {member.name}");

                if (!PartyManager.instance.SelectChars.Contains(member))
                {
                    PartyManager.instance.SelectChars.Add(member);
                    member.ToggleRingSelection(true);
                }

                int i = PartyManager.instance.FindIndexFromClass(member);
                UIManager.instance.ToggleAvatar[i].SetIsOnWithoutNotify(true);
            }
        }

        boxSelection.sizeDelta = new Vector2(0, 0);
    }
}