using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField]
    private GameObject doubleRingMarker;
    public GameObject DoubleRingMarker {  get { return doubleRingMarker; } }

    public static VFXManager Instance;

    private void Start()
    {
        Instance = this;
    }
}
