using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Transform[] enterPoints;
    public Transform[] EnterPoints { get { return enterPoints; } }

    public static MapManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void GoToMap(string mapName, int enterPointId)
    {
        Setting.isWarping = true;
        Setting.enterPointId = enterPointId;
        Setting.partyCount = PartyManager.instance.Members.Count;

        PartyManager.instance.SaveAllHeroData();

        AudioManager.instance.PlaySFX(9);

        switch (mapName)
        {
            case "SampleScene":
                AudioManager.instance.PlayBGM(1);
                break;

            case "Forest01":
                AudioManager.instance.PlayBGM(3);
                break;
        }

        SceneManager.LoadScene(mapName);
    }
}