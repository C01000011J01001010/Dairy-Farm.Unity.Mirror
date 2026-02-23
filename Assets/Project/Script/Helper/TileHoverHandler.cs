using UnityEngine;
using UnityEngine.Tilemaps; // 타일맵 관련 기능을 위해 필수!
using UnityEngine.InputSystem;

public class TileHoverHandler : MonoBehaviour
{

    [SerializeField] private Tilemap targetTilemap;   // 현재 사용 중인 타일맵
    [SerializeField] private GameObject highlight;   // 마우스를 따라다닐 단 하나의 오브젝트

    void Update()
    {
        // 1. 마우스 위치를 월드 좌표로 변환
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        // 2. 월드 좌표를 타일맵의 '셀(칸) 좌표'로 변환
        Vector3Int cellPosition = targetTilemap.WorldToCell(mouseWorldPos);

        // 3. 해당 셀에 타일이 있는지 확인 (선택 사항: 타일이 있는 곳만 하이라이트 하고 싶을 때)
        if (targetTilemap.HasTile(cellPosition))
        {
            highlight.SetActive(true);

            // 4. 셀 좌표의 중앙 월드 좌표를 가져와서 하이라이트 오브젝트 위치 수정
            highlight.transform.position = targetTilemap.GetCellCenterWorld(cellPosition);
        }
        else
        {
            // 타일이 없는 허공이라면 하이라이트 끄기
            highlight.SetActive(false);
        }
    }
}