using UnityEngine;

public enum CharacterDirection
{
    Up,
    RightUp,
    RightDown,
    Down,
    LeftDown,
    LeftUp,
}


public class PlayableCharacterCotroller : MonoBehaviour
{
    public PlayableCharacter character;
    public CharacterDirection curDir;
    public Vector2 input;
    UserInputManager inputManager;

    private void Start()
    {
        inputManager = UserInputManager.tempInst;
    }

    private void Update()
    {
        input = inputManager.Move;
        curDir = (CharacterDirection)GetDirectionIndex(input);
        character.SetMoveDir(curDir);
        character.move.InputVec(input);
    }

    int GetDirectionIndex(Vector2 input)
    {
        // 입력이 너무 작으면 방향 없음 처리
        if (input.sqrMagnitude < 0.01f)
            return -1;

        // 각도 구하기 (-180 ~ 180)
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

        // 반시계→시계 반전
        // 위쪽을 0으로
        angle = -angle + 90f;

        // -90~270을 0~360 범위로 정규화
        angle = (angle + 360f) % 360f;

        // 60도 단위로 나누기 (6방향), 위에서 우로 30도는 6이 나와서 결과가 0 이됨
        int index = Mathf.FloorToInt((angle + 30f) / 60f) % 6;

        return index;
    }
}
