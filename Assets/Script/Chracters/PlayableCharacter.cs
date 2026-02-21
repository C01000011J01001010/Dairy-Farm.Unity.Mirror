using System.Collections;
using UnityEngine;


public class PlayableCharacter : BaseCharacter, IScenedInitialize
{
    
    public override void Exit()
    {

    }

    public override IEnumerator Initialize()
    {
        // 현재 조작가능한 캐릭터가 없다면 해당 캐릭터를 사용하도록 함
        PlayerCotroller owner = WorldManager.GetObject<PlayerCotroller>();
        owner.character ??= this;

        yield return base.Initialize();
        yield return null;
    }

    

    public override void Tick()
    {
        stateController.UpdateFromOwner();
    }
}
