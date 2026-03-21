using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TileData
{
    public TileBase tileBase;
    public Vector3Int position;
    public bool isWalkable;
    public GameObject occupant; // 유닛이나 장애물
}

/// <summary>
/// 타일의 변환 처리, 타일 위치의 객체 위치 처리
/// </summary>
public class TileManager : MonoBehaviour, IScenedManager
{
    // 씬마다 다른 타일 데이터를 불러옴
    // 각 타일을 눈금으로 구별
    private Dictionary<Vector2Int, TileData> checkerboard;

    [SerializeField] int _priority;
    public int Priority => _priority;

    /// <summary>
    /// 현재씬의 buildIndex를 key로 모눈 데이터를 불러옴
    /// </summary>
    public Dictionary<Vector2Int, TileData> GetCheckerboard(Scene CurScene)
    {
        int key = CurScene.buildIndex;
        // TODO : FileManager로부터 데이터 불러오기
        return null;
    }

    // 특정 좌표에 객체가 있는지 확인하는 함수
    public bool IsTileEmpty(Vector2Int gridPosition)
    {
        TileData tileData = null;
        if(checkerboard.TryGetValue(gridPosition, out tileData))
        {
            if (tileData.occupant) return false;
            return true;
        }

        // 타일이 없는 경우 비어있는지 확인할 문제가 아님
        return false;
    }

    // 타일을 차지하는 객체를 저장
    public void SetOccupant(Vector2Int gridPosition, GameObject existObject)
    {
        checkerboard[gridPosition].occupant = existObject;
    }

    public void ClearOccupant(Vector2Int gridPosition)
    {
        checkerboard[gridPosition].occupant = null;
    }

    public IEnumerator Initialize()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator PostInitialize()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}