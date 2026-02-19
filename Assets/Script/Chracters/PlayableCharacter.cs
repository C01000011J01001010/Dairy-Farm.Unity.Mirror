using UnityEngine;

[RequireComponent(typeof(MaleAnim))]
[RequireComponent(typeof(CharacterMove))]
public class PlayableCharacter : MonoBehaviour
{
    public MaleAnim anim;
    public CharacterMove move;

    private void Awake()
    {
        anim = GetComponent<MaleAnim>();
    }

    public void IsMove()
    {

    }

    public void SetMoveDir(CharacterDirection dir)
    {
        anim.MoveDir((int)dir);
    }
}
