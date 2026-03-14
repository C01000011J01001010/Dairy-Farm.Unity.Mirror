using UnityEngine;
using System;

public abstract class BaseItemCondition : ScriptableObject
{
    /// <summary>
    /// 아이템 사용 조건을 만족했는가 체크
    /// </summary>
    public abstract bool IsSatisfied(BaseCharacter character);
}