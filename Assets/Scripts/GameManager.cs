using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] heroPrefabs;
    public GameObject[] HeroPrefabs { get { return heroPrefabs; }}

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (Setting.isNewGame)
        {
            Setting.isNewGame = false;
            GeneratePlayerHero();
            AudioManager.instance.PlayBGM(1);
        }

        if (Setting.isWarping)
        {
            Setting.isWarping = false;
            WarpPlayer();
        }

        RemoveAlreadyRecruitedHeroNPCs();
    }

    private void GeneratePlayerHero()
    {
        Setting.recruitedHeroPrefabIds.Clear();

        int i = Setting.playerPrefabId;

        GameObject heroObj = Instantiate(heroPrefabs[i],
            new Vector3(44f, 10f, 35f), Quaternion.identity);

        heroObj.tag = "Player";

        Character hero = heroObj.GetComponent<Character>();
        PartyManager.instance.Members.Add(hero);

        hero.CharInit(VFXManager.Instance, UIManager.instance,
            InventoryManager.instance, PartyManager.instance);

        /*InventoryManager.instance.AddItem(hero, 0);
        InventoryManager.instance.AddItem(hero, 2);*/

        PartyManager.instance.FixedStartLoadout();
    }

    private void WarpPlayer()
    {
        PartyManager.instance.LoadAllHeroData();
    }

    private void RemoveAlreadyRecruitedHeroNPCs()
    {
        Hero[] heroesInScene = FindObjectsByType<Hero>(FindObjectsSortMode.None);

        foreach (Hero hero in heroesInScene)
        {
            if (hero == null)
                continue;

            if (hero.CompareTag("Player"))
                continue;

            if (!hero.CompareTag("Hero"))
                continue;

            if (PartyManager.instance.Members.Contains(hero))
                continue;

            if (Setting.recruitedHeroPrefabIds.Contains(hero.PrefabID))
            {
                Destroy(hero.gameObject);
            }
        }
    }
}
