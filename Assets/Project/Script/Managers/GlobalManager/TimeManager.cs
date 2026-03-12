using System;
using System.Collections;
using UnityEngine;

// 매니저 클래스중 가장 마지막에 초기화
public class TimeManager : BaseGlobalManager, IGlobalManager
{
    // 현실 24분(1440초) = 게임 내 하루

    /// <summary>
    /// 게임 속 하루를 현실 시간 초로 환산한 값<br/>
    /// *현실의 1분은 게임속 1시간
    /// </summary>
    public static float DayLength { get; private set; } = 24 * 60;

    /// <summary>
    /// 플레이어 새게임을 시작하고 경과한 총 시간
    /// </summary>
    public double TotalGameSeconds { get; private set; }

    public event System.Action<int/*분*/> Event_OnGameMinutesPassed; // 현실 1초마다 발생
    public event System.Action<int/*분*/> Event_OnGameHoursPassed;   // 현실 1분마다 발생

    private float timerForMinute = 0f;
    private float timerForHour = 0f;

    // 현실 1초 = 게임 1분 / 현실 60초 = 게임 1시간
    private const int REAL_SECONDS_PER_GAME_MINUTE = 1;
    private const int REAL_SECONDS_PER_GAME_HOUR = 60;

    public void Exit()
    {
        GameManager.UPDATE_Initial -= OnUpdate;
    }

    public IEnumerator Initialize()
    {
        GameManager.UPDATE_Initial += OnUpdate;

        // TODO: 세이브 파일 로드 로직 (lastSaveTime 가져오기)
        DateTime lastSaveTime = DateTime.UtcNow; // 임시: 세이브 데이터에서 가져와야 함
        TimeSpan offlineDelta = DateTime.UtcNow - lastSaveTime;
        UpdateTime((float)offlineDelta.TotalSeconds);
        yield break;
    }

    private void OnUpdate()
    {
        UpdateTime(Time.deltaTime);
    }

    private void UpdateTime(float deltaTime)
    {
        TotalGameSeconds += deltaTime;
        timerForMinute += deltaTime;
        timerForHour += deltaTime;

        if (timerForMinute >= REAL_SECONDS_PER_GAME_MINUTE)
        {
            int deltaMinutes = ((int)timerForMinute) / REAL_SECONDS_PER_GAME_MINUTE;
            timerForMinute -= (deltaMinutes * REAL_SECONDS_PER_GAME_MINUTE);
            Event_OnGameMinutesPassed?.Invoke(deltaMinutes);
        }

        if (timerForHour >= REAL_SECONDS_PER_GAME_HOUR)
        {
            int deltaHours = ((int)timerForHour) / REAL_SECONDS_PER_GAME_HOUR;
            timerForHour -= (deltaHours * REAL_SECONDS_PER_GAME_HOUR);
            Event_OnGameHoursPassed?.Invoke(deltaHours);
        }
    }


    // 게임속에서 현재가 몇 일째인지 반환
    public int GetCurrentDay() => (int)(TotalGameSeconds / DayLength) + 1;

    public void SetWorldTime(float dayLength)
    {
        DayLength = dayLength;
    }

    //public IEnumerator EventLoopbyTime(int loopCount, float LoopIntervel, System.Action Event)
    //{
    //    int count = 0;
    //    Debug.Log($"{loopCount}번의 Event({Event.Method.Name})루프 시작");
    //    while (count < loopCount)
    //    {
    //        count++;
    //        yield return new WaitForSecondsRealtime(LoopIntervel);
    //        Event?.Invoke();
    //    }
    //}
}
