using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CoreEngine.Data;

public class FieldShower : BaseObjectViewer<ProductField>
{
    [SerializeField] GameObject toggleWindow;
    [SerializeField] Slider slider_Time;
    [SerializeField] TextMeshProUGUI text_Count;
    [SerializeField] TextMeshProUGUI text_NextTime;
    float receivedNextTime;
    float receivedInterval;

    void Awake()
    {
        FieldManager.OnFieldFocusChanged -= SetConnect;
        FieldManager.OnFieldFocusChanged += SetConnect;
        //보고 있는 필드가 바뀌면 그 대상과 연결하도록!
    }

    void Update()
    {
        if(ConnectObject) UpdateNextTime(receivedNextTime, receivedInterval);
    }

    public void SetConnect(ProductField target) => Connect(target);

    public virtual void UpdateNextTime(float nextTime)
    {
        receivedNextTime = nextTime; //얘네들은 그냥 보는 용도! 마지막으로 들어온 정보!
    }

    public virtual void UpdateInterval(float interval)
    {
        receivedInterval = interval; //얘네들은 그냥 보는 용도! 마지막으로 들어온 정보!
    }

    public virtual void UpdateCount(int current, int max) 
        => text_Count.SetText($"개수 : ({current}/{max})");

    public virtual string GetTimeText(float wantTime)
    {
        if (wantTime < 0) return null;
        else if (wantTime > 60)
        {
            int timeAsInt = (int)wantTime;
            int minute = timeAsInt / 60;
            int seconds = timeAsInt % 60;
            return $"{minute}:{seconds,00}";
        }
        else
        {
            return $"{wantTime}";
        }
    }

    public virtual void UpdateNextTime(float nextTime, float maxTime)
    {
        float leftTime = nextTime - Time.time; //남은 시간 확인!
        string leftText = GetTimeText(leftTime); // 남은 시간을 텍스트로 변환!
        //변환 해봤더니 없던데?!              그럼 글자 비우고!
        if (string.IsNullOrEmpty(leftText)) text_NextTime.SetText("");
        //있네?  남은 시간이 있는 거니까  남은시간 / 최대시간 텍스트
        else text_NextTime.SetText($"{leftText:f1} / {GetTimeText(maxTime):f1}");
        //슬라이더 돌렸음 ㅎ
        slider_Time.value = maxTime > 0 ? 1 - (leftTime / maxTime) : 1;
    }

    public override void UpdateView()
    {
        transform.position = ConnectObject.transform.position;
        UpdateCount(ConnectObject.GetCountCurrent(), ConnectObject.GetCountMax());
        float newInterval = ConnectObject.GetProductInterval();
        float newNextTime = ConnectObject.GetProductNextTime();
        UpdateInterval(newInterval);
        UpdateNextTime(newNextTime);
    }

    protected override void OnConnected()
    {
        if(!IsEmpty())
        {
            toggleWindow.SetActive(true);
            ConnectObject.OnCountChanged -= UpdateCount;
            ConnectObject.OnCountChanged += UpdateCount; //카운트 바뀌었으니까 업데이트 할래!
            ConnectObject.OnIntervalChanged -= UpdateInterval;
            ConnectObject.OnIntervalChanged += UpdateInterval; //생산 간격이 바뀌었어? 업데이트!
            ConnectObject.OnNextTimeChanged -= UpdateNextTime;
            ConnectObject.OnNextTimeChanged += UpdateNextTime; //생산 시간 어떻게 되었어? 업데이트!
        }
    }
    protected override void OnDisconnected(ProductField target)
    {
        if(target)
        {
            target.OnCountChanged -= UpdateCount;
            target.OnIntervalChanged -= UpdateInterval;
            target.OnNextTimeChanged -= UpdateNextTime;
        }
        toggleWindow.SetActive(false);
        base.OnDisconnected(target);
    }

    public virtual void ClaimHarvestCurrentTile()
    {
        WorkController.ClaimHarvestField(GetConnectedObject());
    }
}
