using UnityEngine;
using System;
using Farm.Character;

namespace Farm.StaticData.Item
{
    public abstract class BaseItemEffect : ScriptableObject
    {
        public abstract void ApplyEffect(BaseCharacter character, ItemDataContainer item);
    }
}
