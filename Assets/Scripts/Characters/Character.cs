using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum CharState
{
    Idle,
    Walk,
    WalkToEnemy,
    Attack,
    WalkToMagicCast,
    MagicCast,
    Hit,
    Die,
    WalkToNPC
}

public abstract class Character : MonoBehaviour
{
    protected NavMeshAgent navAgent;

    protected Animator anim;
    public Animator Anim { get { return anim; } }

    [SerializeField]
    protected Sprite avatarPic;
    public Sprite AvatarPic { get { return avatarPic; } }

    [SerializeField]
    protected string charName;
    public string CharName { get { return charName; } }

    [SerializeField]
    protected CharState state;
    public CharState State { get { return state; } }

    [SerializeField]
    protected GameObject ringSelection;
    public GameObject RingSelection { get { return ringSelection; } }

    [SerializeField]
    protected int curHP = 10;
    public int CurHP { get { return curHP; } set { curHP = value; } }

    [SerializeField]
    protected Character curCharTarget;
    public Character CurCharTarget { get { return curCharTarget; } set { curCharTarget = value; } }

    [SerializeField]
    protected float attackRange = 2f;
    public float AttackRange { get { return attackRange; } }
    [SerializeField]
    protected int attackDamage = 3;
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    [SerializeField]
    protected float attackCooldown = 2f;
    [SerializeField]
    protected float attackTimer = 0f;
    [SerializeField]
    protected float findingRange = 20f;
    public float FindingRange { get { return findingRange; } }
    [SerializeField]
    protected List<Magic> magicSkills = new List<Magic>();
    public List<Magic> MagicSkills
    { get { return magicSkills; } set { magicSkills = value; } }

    [SerializeField]
    protected Magic curMagicCast = null;
    public Magic CurMagicCast
    { get { return curMagicCast; } set { curMagicCast = value; } }

    [SerializeField]
    protected bool isMagicMode = false;
    public bool IsMagicMode
    { get { return isMagicMode; } set { isMagicMode = value; } }

    [Header("Inventory")]
    [SerializeField]
    protected Item[] inventoryItems;
    public Item[] InventoryItems
    { get { return inventoryItems; } set { inventoryItems = value; } }

    [SerializeField]
    protected Item mainWeapon;
    public Item MainWeapon { get { return mainWeapon; } set { mainWeapon = value; } }

    [SerializeField]
    protected Item shield;
    public Item Shield { get { return shield; } set { shield = value; } }

    [SerializeField]
    protected int maxHP = 100;
    public int MaxHP { get { return maxHP; } }

    [SerializeField]
    protected Transform shieldHand;

    [SerializeField]
    protected GameObject shieldObj;

    [SerializeField]
    protected int defensePower = 0;
    public int DefensePower { get { return defensePower; } set { defensePower = value; } }

    [SerializeField]
    protected Transform weaponHand;

    [SerializeField]
    protected GameObject weaponObj;

    [SerializeField]
    protected int weaponPower = 0;

    protected VFXManager vfxManager;
    protected UIManager uiManager;
    protected InventoryManager invManager;
    protected PartyManager partyManager;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void SetState(CharState s)
    {
        state = s;

        if (state == CharState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }

    public void WalkToPosition(Vector3 dest)
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(dest);
            navAgent.isStopped = false;
        }
        SetState(CharState.Walk);
    }

    protected void WalkUpdate()
    {
        float distance = Vector3.Distance(transform.position, navAgent.destination);
        Debug.Log(distance);

        if (distance <= navAgent.stoppingDistance)
            SetState(CharState.Idle);
    }

    public void ToggleRingSelection(bool flag)
    {
        ringSelection.SetActive(flag);
    }

    public void ToAttackCharacter(Character target)
    {
        if (curHP <= 0 || state == CharState.Die)
            return;

        curCharTarget = target;

        navAgent.SetDestination(target.transform.position);
        navAgent.isStopped = false;

        if (IsMagicMode)
            SetState(CharState.WalkToMagicCast);
        else
            SetState(CharState.WalkToEnemy);
    }

    protected void WalkToEnemyUpdate()
    {
        if (curCharTarget == null)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.SetDestination(curCharTarget.transform.position);
        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance <= attackRange)
        {
            SetState(CharState.Attack);
            Attack();
        }
    }

    protected void WalkToMagicCastUpdate()
    {
        if (curCharTarget == null || curMagicCast == null)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.SetDestination(curCharTarget.transform.position);

        float disrance = Vector3.Distance(transform.position,curCharTarget
            .transform.position);

        if (disrance <= curMagicCast.Range)
        {
            navAgent.isStopped = true;
            SetState(CharState.MagicCast);

            MagicCast(curMagicCast);
        }
    }

    public void ToTalkToNPC(Character npc)
    {
        if (curHP <= 0 || state == CharState.Die)
            return;

        curCharTarget = npc;

        navAgent.SetDestination(npc.transform.position);
        navAgent.isStopped = false;

        SetState(CharState.WalkToNPC);
    }

    protected void Attack()
    {
        transform.LookAt(curCharTarget.transform);
        anim.SetTrigger("Attack");

        AudioManager.instance.PlaySFX(3);

        AttackLogic();
    }

    protected void AttackUpdate()
    {
        if (curCharTarget == null)
            return;

        if (curCharTarget.CurHP <= 0)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.isStopped = true;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            Attack();
        }

        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);
        if (distance > attackRange)
        {
            SetState(CharState.WalkToEnemy);
            navAgent.SetDestination(curCharTarget.transform.position);
            navAgent.isStopped = false;
        }
    }

    protected void AttackLogic()
    {
        Character target = curCharTarget.GetComponent<Character>();
        if (target != null)
            target.ReceiveDamage(attackDamage);
    }

    private void MagicCast(Magic curMagicCast)
    {
        transform.LookAt(curCharTarget.transform);
        anim.SetTrigger("MagicAttack");

        StartCoroutine(LoadMagicCast(curMagicCast));
    }

    protected void MagicCastLogic(Magic magic)
    {
        Character target = curCharTarget.GetComponent<Character>();

        if (target != null)
            target.ReceiveDamage(magic.Power);
    }

    public void ReceiveDamage(int damage)
    {
        if (curHP <= 0 || state == CharState.Die)
            return;

        int damageAfter = damage - defensePower;
        if (damageAfter < 0)
        {
            damageAfter = 0;
        }

        curHP -= damageAfter;

        if (damageAfter > 0)
            AudioManager.instance.PlaySFX(6);

        if (curHP <= 0)
        {
            curHP = 0;
            Die();
        }
    }

    public bool IsMyEnemy(string targetTag)
    {
        string myTag = gameObject.tag;
        if ((myTag == "Hero" || myTag == "Player") && targetTag == "Enemy")
            return true;
        if (myTag == "Enemy" && (targetTag == "Hero" || targetTag == "Player"))
            return true;
        return false;
    }

    protected virtual IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private IEnumerator ShootMagicCast(Magic curMagicCast)
    {
        AudioManager.instance.PlaySFX(10);

        if (vfxManager != null)
        {
            Vector3 chestOffset = Vector3.up * 1.2f;

            vfxManager.ShootMagic(
                curMagicCast.ShootID,
                transform.position + chestOffset,
                curCharTarget.transform.position + chestOffset,
                curMagicCast.ShootTime
            );
        }

        yield return new WaitForSeconds(curMagicCast.ShootTime);

        MagicCastLogic(curMagicCast);

        isMagicMode = false;

        SetState(CharState.Idle);

        if (uiManager != null)
            uiManager.IsOnCurToggleMagic(false);
    }

    private IEnumerator LoadMagicCast(Magic curMagicCast)
    {
        AudioManager.instance.PlaySFX(8);

        if (vfxManager != null)
        {
            Vector3 chestOffset = Vector3.up * 1.2f;

            vfxManager.LoadMagic(
                curMagicCast.LoadID,
                transform.position + chestOffset,
                curMagicCast.LoadTime
            );
        }

        yield return new WaitForSeconds(curMagicCast.LoadTime);

        StartCoroutine(ShootMagicCast(curMagicCast));
    }

    protected virtual void Die()
    {
        navAgent.isStopped = true;
        SetState(CharState.Die);

        anim.SetTrigger("Die");

        AudioManager.instance.PlaySFX(6);

        invManager.SpawnDropInventory(inventoryItems, transform.position);

        StartCoroutine(DestroyObject());
    }

    public void CharInit(VFXManager vfxM,UIManager uiM,InventoryManager invM,PartyManager partyM)
    {
        vfxManager = vfxM;
        uiManager = uiM;
        invManager = invM;
        partyManager = partyM;

        inventoryItems = new Item[InventoryManager.MAXSLOT];
    }

    public void Recovery (int n)
    {
        curHP += n;

        if(curHP > maxHP)
            curHP = maxHP;
    }

    public void EquipShield(Item item)
    {
        shieldObj = Instantiate(invManager.ItemPrefabs[item.PrefabID], shieldHand);

        shieldObj.transform.localPosition = new Vector3(-8.5f, -4f, 3f);
        shieldObj.transform.Rotate(-90f, 0f, 180f, Space.Self);

        defensePower += item.Power;
        shield = item;
    }

    public void UnEquipShield()
    {
        if (shield != null)
        {
            defensePower -= shield.Power;
            shield = null;
            Destroy(shieldObj);
        }
    }

    public void EquipWeapon(Item item)
    {
        weaponObj = Instantiate(invManager.ItemPrefabs[item.PrefabID], weaponHand);

        weaponObj.transform.localPosition = new Vector3(7.5f, 2f, 8f);
        weaponObj.transform.Rotate(0f, 90f, -90f, Space.Self);

        weaponPower += item.Power;
        mainWeapon = item;
    }

    public void UnEquipWeapon()
    {
        if (mainWeapon != null)
        {
            weaponPower -= mainWeapon.Power;
            mainWeapon = null;
            Destroy(weaponObj);
        }
    }
}
