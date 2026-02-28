using UnityEngine;

//                                                       유니티 기본 클래스
public abstract class ObjectShower<T> : MonoBehaviour where T : class//Object
{
    T connectObject = null;

    public bool IsConnected => connectObject is not null;
    public T GetConnectedObject() => connectObject;

    protected virtual bool OnConnected(T target) { Visualize(target); return true; }
    public bool Connect(T target) 
    {
        // 변수가 정확히 null일 때에만 작동, 대상이 파괴되어서 댕글링 포인터 상태일 때에는 if가 true
        if (connectObject is not null) Disconnect(); //연결이 이미 되었다면 끊고 진행!
        if (target == null) return false; //연결 대상이 없으니까 비우고 끝!
        if (OnConnected(target))//만약 연결이 완료되었다면!
        {
            connectObject = target;
            return true; //연결 대상을 저장하고 연결 되었다고 알림!
        }
        else
        {
            connectObject = null;
            return false; //연결 대상을 없애고 연결이 끊겼다고 알림!
        }
    }

    protected virtual void OnDisconnected(T target) { Visualize(target); }

    public void Disconnect() 
    {
        if (!IsConnected) //연결이 안되어있을 때!
        {
            OnDisconnected(null); //비어있는 Disconnected를 보내면! 아무튼 끊어졌다 표시!
            return;
        }
        OnDisconnected(connectObject); //연결 끊겼을 때 할 일을 하렴!
        connectObject = null; //연결이 끊어졌으니까 끊어졌다고 표시하기!
    }

    public abstract void Visualize(T target);
}
