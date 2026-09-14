using UnityEngine;
using System;
using Farm.Character;

namespace Farm.GameData.Item
{
    public abstract class BaseItemEffect : ScriptableObject
    {
        public abstract void ApplyEffect(BaseCharacter character, ItemDataContainer item);
    }
}
