using System.Collections.Generic;
using UnityEngine;

namespace Farm.Fishing
{
    [CreateAssetMenu(fileName = "FishData", menuName = "Fishing/FishTable")]
    public class FishTable : ScriptableObject
    {
        public List<FishData> basic;
        public List<FishData> low;
        public List<FishData> mid;
        public List<FishData> high;

        public List<FishData> GetTier(int reelTier) => reelTier switch
        {
            0 => basic,
            1 => low,
            2 => mid,
            _ => high
        };
    }
}
