using UnityEngine;

public class MaleAnim : BaseAnim
{
    int moveDirHash;


    protected override void GetAnimPrarmHash()
    {
        moveDirHash = Animator.StringToHash("MoveDir");
    }

    private void Awake()
    {
        GetAnimPrarmHash();
    }

    public void MoveDir(int dir)
    {
        SetParam(moveDirHash, dir);
    }
}
