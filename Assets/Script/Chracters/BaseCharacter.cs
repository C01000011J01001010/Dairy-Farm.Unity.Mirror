using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;


[RequireComponent(typeof(baseCharacterAnim))]
[RequireComponent(typeof(CharacterStateController))]
[RequireComponent(typeof(Rigidbody2D))]
public class BaseCharacter : MonoBehaviour, IScenedInitialize
{
    [SerializeField] protected int _priority = 11;
    public int Priority => _priority;


    public baseCharacterAnim anim {  get; protected set; }
    protected CharacterStateController stateController;
    protected Rigidbody2D rigidBody;

    public Vector2 inputMove;
    public bool isMove;
    public bool isSprint;
    public float moveSpeed = 3f;
    public float SprintMul = 1.5f;

    protected bool isReady;


    private void OnDisable()
    {
        if (isReady) Unsubscribe();
    }

    private void OnEnable()
    {
        if (isReady) Subscribe();
    }

    public virtual void Exit()
    {
        if (isReady)
        {
            Unsubscribe();

            // OnDisable에서 다시 Unsubscribe하지 않도록 플래그를 끄기
            isReady = false; 
        }
    }

    public virtual IEnumerator Initialize()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<baseCharacterAnim>();
        stateController = GetComponent<CharacterStateController>();
        yield return null;
    }
    public IEnumerator LateInitialize()
    {
        stateController.Initialize(CharacterState.Idle);
        yield return null;
        isReady = true;
        Subscribe();
    }

    protected void Subscribe()
    {
        CharacterManager mng = WorldManager.GetManager<CharacterManager>();
        mng?.AddList(this);
    }

    protected void Unsubscribe()
    {
        CharacterManager mng = WorldManager.GetManager<CharacterManager>();
        mng?.RemoveList(this);
    }

    public virtual void Move(Vector2 input)
    {
        inputMove = input;
        isMove = input.sqrMagnitude > 0.01f;

        // 3. Scale -1을 이용한 좌우 반전 로직
        // x값이 0일 때는 마지막 방향을 유지하기 위해 '0이 아닐 때만' 업데이트
        if (input.x != 0)
        {
            float direction = input.x > 0 ? 1f : -1f;

            // 부모의 Scale을 뒤집어 하위 무기, 이펙트 위치까지 한꺼번에 반전
            transform.localScale = new Vector3(direction, 1f, 1f);
        }
        // 위 아래 일 시 정상 scale로 변경
        else if(input.y != 0)
        {
            transform.localScale = Vector3.one;
        }
    }

    public void SprintHold(bool value) => isSprint = value;

    //public void SprintToggle() => isRun = !isRun;

    public virtual void Tick() { }

    public virtual void FixedTick()
    {
        if(isMove)
        {
            Physics_Move();
        }
    }

    private void Physics_Move()
    {
        Vector2 nextVec = inputMove.normalized * moveSpeed * Time.fixedDeltaTime;
        if (isSprint) nextVec *= SprintMul;
        Debug.Log("다음 이동량 : " + nextVec);
        rigidBody.MovePosition(rigidBody.position + nextVec);
    }
}
