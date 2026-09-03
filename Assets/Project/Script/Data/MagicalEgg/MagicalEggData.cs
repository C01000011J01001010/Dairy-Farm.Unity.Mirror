
using System.Collections.Generic;
using UnityEngine;
using CoreEngine.Data;

public class MagicalEggData : BaseData_ForUi
{
    // 알 상호작용으로 열리는 퀘스트창에 전달
    [SerializeField] protected int[] questIndexs;

    [SerializeField] protected BaseReward[] reward;
}