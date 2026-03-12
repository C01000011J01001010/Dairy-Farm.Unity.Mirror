using UnityEngine;
using System;

public abstract class ItemEffectSO : ScriptableObject
{
    // 클릭 즉시 실행되는 효과 (애니메이션 트리거, 개수 감소 등)
    public abstract void OnUse(BaseCharacter character, InfoItemContainer item);

    // 애니메이션 이벤트 타이밍에 맞춰 실행되는 효과 (실제 땅 파기, 물 주기 등)
    public abstract void OnAnimEvent(BaseCharacter character, InfoItemContainer item);
}