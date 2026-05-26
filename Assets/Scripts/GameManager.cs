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
        if(Setting.isNewGame)
        {
            Setting.isNewGame = false;
            GeneratePlayerHero();
            AudioManager.instance.PlayBGM(1);
        }
        if(Setting.isWarping)
        {
            Setting.isWarping = false;
            WarpPlayer();
        }
    }

    private void GeneratePlayerHero()
    {
        int i = Setting.playerPrefabId;

        GameObject heroObj = Instantiate(heroPrefabs[i],
            new Vector3(44f,10f,35f),Quaternion.identity);

        heroObj.tag = "Player";

        Character hero = heroObj.GetComponent<Character>();
        PartyManager.instance.Members.Add(hero);

        hero.CharInit(VFXManager.Instance,UIManager.instance,
            InventoryManager.instance,PartyManager.instance);

        InventoryManager.instance.AddItem(hero, 0);
        InventoryManager.instance.AddItem(hero, 2);
    }

    private void WarpPlayer()
    {
        PartyManager.instance.LoadAllHeroData();
    }
}
