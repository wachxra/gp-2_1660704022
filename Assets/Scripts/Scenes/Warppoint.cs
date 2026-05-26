using UnityEngine;

public class Warppoint : MonoBehaviour
{
    [SerializeField]
    private string toMapName;

    [SerializeField]
    private int enterPointId;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Player enters Warp");
            MapManager.instance.GoToMap(toMapName, enterPointId);
        }
    }
}
