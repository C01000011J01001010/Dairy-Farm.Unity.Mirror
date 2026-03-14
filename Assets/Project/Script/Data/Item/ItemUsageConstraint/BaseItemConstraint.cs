using UnityEngine;
using System;

public abstract class BaseItemConstraint : ScriptableObject
{
    /// <summary>
    /// 아이템을 사용할 수 없는가 체크
    /// </summary>
    public abstract bool IsViolated(BaseCharacter character);
}