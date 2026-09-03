using System.Collections;
using UnityEngine;

namespace Farm.Character
{
    public class PlayableCharacter : BaseCharacter
    {
        public event System.Action Event_OnControllTargetSet;
        public event System.Action Event_OnControllTargetRemoved;

        CharacterTileChecker tileChecker;
        CharacterActionController actionController;
        CharacterQuestBook questBook;


        public override IEnumerator PostInitialize()
        {
            yield return base.PostInitialize();

            Debug.Log("EventBus로 컨트롤러 연결 필요");
            //if (user.curTargetCharacter == null)
            //{
            //    user.SetControllTarget(this);
            //}
        }

        // 새로 연결되는 캐릭터만 해당하니 이벤트 등록 안함
        public virtual void OnControllTargetSet()
        {
            Event_OnControllTargetSet?.Invoke();
        }

        // 연결된 캐릭터만 제거하는거니 이벤트로 등록 안함
        public virtual void OnControllTargetRemoved()
        {
            Event_OnControllTargetRemoved?.Invoke();
        }
    }
}

