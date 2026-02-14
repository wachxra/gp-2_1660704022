using UnityEngine;

public class RightClick : MonoBehaviour
{
    public static RightClick instance;

    private Camera cam;
    public LayerMask layerMask;

    private LeftClick leftClick;

    private void Awake()
    {
        leftClick = GetComponent<LeftClick>();
    }

    private void Start()
    {
        instance = this;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Ground", "Character", "Build");
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            TryCommand(Input.mousePosition);
        }    
    }

    private void CommandToWalk(RaycastHit hit,Character c)
    {
        if (c != null)
            c.WalkToPosition(hit.point);

        CreateVFX(hit.point,VFXManager.Instance.DoubleRingMarker);
    }

    private void CommandToAttack(RaycastHit hit, Character c)
    {
        if (c == null)
            return;

        Character target = hit.collider.GetComponent<Character>();
        Debug.Log("Attack");

        if (target != null)
            c.ToAttackCharacter(target);
    }

    private void TryCommand(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Ground":
                    CommandToWalk(hit, leftClick.CurChar); break;
                case "Enemy":
                    CommandToAttack(hit, leftClick.CurChar); break;
            }  
        }
    }    

    private void CreateVFX(Vector3 pos,GameObject vfxPrefab)
    {
        if (vfxPrefab == null)
            return;

        Instantiate(vfxPrefab,pos + new Vector3(0f,0.1f,0f),Quaternion.identity);
    }
}
