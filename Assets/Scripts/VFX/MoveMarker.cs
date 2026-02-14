using UnityEngine;

public class MoveMarker : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
