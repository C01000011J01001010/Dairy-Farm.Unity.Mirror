using UnityEngine;
using CoreEngine.Pool;

namespace Farm.Pool
{
    public enum CharacterPoolType
    {
        Tori,
    }
    public class CharacterPoolManager : BaseObjectPoolManager<CharacterPoolType>
    {
    }
}
