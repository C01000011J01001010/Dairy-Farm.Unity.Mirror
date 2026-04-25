using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

// 점유물의 종류를 구분하여 이동 가능 여부를 판단하기 위한 Enum
public enum OccupantType
{
    None,
    Crop,       // 농작물 (이동 가능, 중복 배치 불가)
    Item,       // 바닥에 둔 아이템 (이동 제한, 중복 배치 불가)
    Obstacle    // 바위, 나무 등 장애물 (이동 제한)
}

/// <summary>
/// 개별 타일의 논리적 데이터를 담는 클래스
/// (Unity의 TileData 구조체와 이름 충돌을 피하기 위해 GridTileInfo로 명명)
/// </summary>
[System.Serializable]
public class GridTileInfo
{
    // 로컬 Save/Load를 위한 문자열 ID (예: "Grass_01", "Soil_Plowed")
    public string tileID;
    public TileBase tileBase;

    // 점유물 정보
    public GameObject occupant;
    public OccupantType occupantType = OccupantType.None;
}

public class TileManager : MonoBehaviour //, IScenedManager (인터페이스 주석 처리)
{
    [Tooltip("실제 타일이 그려지는 유니티 타일맵 컴포넌트")]
    [SerializeField] private Tilemap groundTilemap;

    [SerializeField] int _priority;
    public int Priority => _priority;

    // 타일 논리 데이터 저장소
    private Dictionary<Vector2Int, GridTileInfo> checkerboard = new Dictionary<Vector2Int, GridTileInfo>();

    /// <summary>
    /// 플레이어가 해당 타일로 이동할 수 있는지 확인 (농작물은 통과 가능)
    /// </summary>
    public bool CanMoveTo(Vector2Int gridPosition)
    {
        if (checkerboard.TryGetValue(gridPosition, out GridTileInfo tileInfo))
        {
            // 점유물이 없거나, 점유물이 '농작물'인 경우에만 이동 허용
            if (tileInfo.occupant == null || tileInfo.occupantType == OccupantType.Crop)
            {
                return true;
            }
            return false; // 아이템이나 장애물이 있으면 이동 불가
        }
        return false; // 맵 범위를 벗어남 (타일 데이터가 없음)
    }

    /// <summary>
    /// 해당 타일에 무언가(농작물/아이템)를 배치할 수 있는지 확인
    /// </summary>
    public bool CanPlaceObject(Vector2Int gridPosition)
    {
        if (checkerboard.TryGetValue(gridPosition, out GridTileInfo tileInfo))
        {
            // 점유물이 아예 없어야만 새로운 객체 배치 가능
            return tileInfo.occupant == null;
        }
        return false;
    }

    /// <summary>
    /// 타일에 객체(농작물, 아이템 등) 배치
    /// </summary>
    public bool SetOccupant(Vector2Int gridPosition, GameObject newObject, OccupantType type)
    {
        if (!CanPlaceObject(gridPosition))
        {
            Debug.LogWarning("이미 자리를 차지한 객체가 있어 배치할 수 없습니다.");
            return false;
        }

        GridTileInfo tileInfo = checkerboard[gridPosition];
        tileInfo.occupant = newObject;
        tileInfo.occupantType = type;

        // 물리적 위치 조정 (타일의 중앙으로 객체 이동)
        Vector3 worldPos = groundTilemap.GetCellCenterWorld((Vector3Int)gridPosition);
        newObject.transform.position = worldPos;

        SaveTileData(); // 상태가 변경되었으므로 로컬 저장 트리거
        return true;
    }

    /// <summary>
    /// 타일의 점유물 제거 (수확, 아이템 줍기 등)
    /// </summary>
    public void ClearOccupant(Vector2Int gridPosition)
    {
        if (checkerboard.TryGetValue(gridPosition, out GridTileInfo tileInfo))
        {
            tileInfo.occupant = null;
            tileInfo.occupantType = OccupantType.None;
            SaveTileData(); // 로컬 저장 트리거
        }
    }

    /// <summary>
    /// 실제 타일맵의 타일(시각적 요소) 및 논리적 데이터 변경 (예: 풀밭 -> 논밭)
    /// </summary>
    public void ChangeTileBase(Vector2Int gridPosition, TileBase newTileBase, string newTileID)
    {
        Vector3Int pos3D = (Vector3Int)gridPosition;

        // 1. 실제 유니티 Tilemap 시각적 업데이트
        groundTilemap.SetTile(pos3D, newTileBase);

        // 2. 논리 데이터 갱신
        if (!checkerboard.ContainsKey(gridPosition))
        {
            checkerboard[gridPosition] = new GridTileInfo();
        }

        checkerboard[gridPosition].tileBase = newTileBase;
        checkerboard[gridPosition].tileID = newTileID;

        SaveTileData(); // 로컬 저장 트리거
    }

    /// <summary>
    /// 데이터 저장 트리거 함수
    /// </summary>
    private void SaveTileData()
    {
        // TODO: FileManager를 호출하여 현재 checkerboard 데이터를 JSON 등으로 로컬에 저장
        // Dictionary 자체는 직렬화가 안 되므로 List나 배열 형태로 변환하여 저장해야 합니다.
        Debug.Log("로컬 데이터 저장됨...");
    }

    // --- IScenedManager 인터페이스 구현부 ---

    public IEnumerator Initialize()
    {
        // 씬 로드 시 타일맵을 스캔하거나 FileManager에서 저장된 데이터를 불러와 checkerboard를 세팅합니다.
        yield return null;
    }

    // ... (나머지 인터페이스 메서드 생략)
}