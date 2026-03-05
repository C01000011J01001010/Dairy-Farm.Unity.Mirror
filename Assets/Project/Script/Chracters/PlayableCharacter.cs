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
        PlayerCotroller user = WorldManager.GetObject<PlayerCotroller>();
        user.character ??= this;
        TryAddCharacterModule<CharacterTileChecker>();

        yield return base.Initialize();
        yield return null;
    }
}
