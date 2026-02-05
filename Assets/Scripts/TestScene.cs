using UnityEngine;

public class TestScene : MonoBehaviour
{
    [SerializeField] private Character[] characters;

    public void SetIdle()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetState(CharState.Idle);
        }                
    }
    public void SetWalk()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetState(CharState.Walk);
        }
    }
    public void setAttack()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetState(CharState.Attack);
            characters[i].Anim.SetTrigger("Attack");
        }
    }
    public void setDie()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetState(CharState.Die);
            characters[i].Anim.SetTrigger("Die");
        }
    }
}
