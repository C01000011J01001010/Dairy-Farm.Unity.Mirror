using System.Collections;
using UnityEngine;

public class InventoryScreen : BaseUi, IGlobalUi
{

    public override void Exit()
    {
        
    }

    public override IEnumerator Initialize()
    {
        yield return null;
    }

    public override void ClaimOpen()
    {
        base.ClaimOpen();
        GameManager.SetPauseUpdate(true);
    }

    public override void ClaimClose()
    {
        base.ClaimClose();
        GameManager.SetPauseUpdate(false);
    }

}