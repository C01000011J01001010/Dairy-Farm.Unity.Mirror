using UnityEngine;

namespace Farm.Fishing
{
    [CreateAssetMenu(fileName = "FishData", menuName = "Fishing/FishData")]
    public class FishData : ScriptableObject 
    {
        public string fishName;
        public Sprite icon;

        [Tooltip("물고기 등급")]
        public float Grade = 10f;
    }
    
}
