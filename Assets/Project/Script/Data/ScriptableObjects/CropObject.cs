using UnityEngine;

/// <summary>
/// 농작물에 대한 정적 데이터를 저장하는 SO 스크립트
/// </summary>
public class CropObject : DataObject
{
    public int season; // 제철인 계절
    public int days_ToMaturity; // 수확까지 걸리는 날짜 (계산할때는 시간으로 환산)
    public int result_ItemIndex; // 수확하여 얻을 수 있는 아이템의 인덱스

    [SerializeField]
    Sprite[] _growth; // 식물 성장 구간 이미지

    public Sprite[] GetSprites() => _growth;
}
