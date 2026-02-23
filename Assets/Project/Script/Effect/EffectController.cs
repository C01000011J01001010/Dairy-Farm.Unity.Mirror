using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    // 인스펙터에서 조작하고 게임 시작시 파괴
    [SerializeField] private SpawnEffectSplit[] _splits;

    // 함수 입력이 없을 시 기본값
    private const float defaultDelay = 1;

    // 개별 이펙트 필요 변수
    private Dictionary<SpawnEffectType, List<EffectSpawning>> effectsDict = new();
    private Dictionary<SpawnEffectType, float/*splitValue*/> splitValue = new();

    private Coroutine mainRoutine;
    private List<Coroutine> subRoutines = new();

    public event Action OnSpawnStart;
    public event Action OnSpawnEnd;
    public event Action OnDespawnStart;
    public event Action OnDespawnEnd;

    private void OnDisable()
    {
        if (mainRoutine != null) StopCoroutine(mainRoutine);
        ClearSubRoutine();
    }
    public virtual void Exit()
    {
        
    }
    public virtual void Initialize()
    {
        foreach (var eff in GetComponentsInChildren<EffectSpawning>())
        {
            // 해당 타입의 리스트가 없으면 만들어주고 리스트에 넣기
            if (!effectsDict.ContainsKey(eff.type)) effectsDict[eff.type] = new();
            effectsDict[eff.type].Add(eff);
        }

        foreach (var split in _splits)
        {
            splitValue.Add(split.effectType, split.splitValue);
        }
        // 인스펙터 조작용이니 다 썼으면 제거
        _splits = null;
        //yield break;
    }

    // 테스트용 함수
#if UNITY_EDITOR
    public void PlaySpawnEffect_Phase()=> PlaySpawnEffect(SpawnEffectType.Phase);
    public void PlaySpawnEffect_Dissolve()=> PlaySpawnEffect(SpawnEffectType.Dissolve);
    public void PlayDespawnEffect_Phase()=> PlayDespawnEffect(SpawnEffectType.Phase);
    public void PlayDespawnEffect_Dissolve()=> PlayDespawnEffect(SpawnEffectType.Dissolve);
#endif

    public void PlaySpawnEffect(SpawnEffectType effectType, float delay = defaultDelay)
    {
        if (IsEffectReady(effectType))
        {
            IEnumerator effectRoutine = SpawnEffect(effectsDict[effectType], splitValue[effectType], delay);
            PlayEffect(effectRoutine);
        }
    }

    public void PlayDespawnEffect(SpawnEffectType effectType, float delay = defaultDelay)
    {
        if (IsEffectReady(effectType))
        {
            IEnumerator effectRoutine = DespawnEffect(effectsDict[effectType], splitValue[effectType], delay);
            PlayEffect(effectRoutine);
        }
    }

    public bool IsEffectReady(SpawnEffectType effectType)
    {
        bool result =   effectsDict.ContainsKey(effectType) &&
                        splitValue.ContainsKey(effectType) &&
                        effectsDict[effectType] != null &&
                        effectsDict[effectType].Count > 0;

        // 이펙트가 사용할 수 없는 경우 알려주기
        if (!result) Debug.LogWarning($"Effect({effectType}) of {gameObject.name} is not ready");
        return result;
    }

    private void PlayEffect(IEnumerator routine)
    {
        // 오브젝트가 꺼져있으면 켜주기
        if(!gameObject.activeSelf) gameObject.SetActive(true);

        // 이미 실행중인 코루틴이 있으면 강제종료
        if (mainRoutine != null) StopCoroutine(mainRoutine);

        // 새로운 코루틴 시작
        mainRoutine = StartCoroutine(routine);
    }

    private IEnumerator SpawnEffect(List<EffectSpawning> effects, float split,  float delay = defaultDelay)
    {
        OnPlaySpawnEffectStart();

        // 모든 코루틴 동시에 실행
        foreach (var effect in effects)
        {
            IEnumerator routine = effect.PlaySpawnEffect(delay, split);
            subRoutines.Add(StartCoroutine(routine));
        }

        yield return WaitUntillFinished(effects);

        OnPlaySpawnEffectEnd();
    }

    private IEnumerator DespawnEffect(List<EffectSpawning> effects, float split, float delay = defaultDelay)
    {
        OnPlayDespawnEffectStart();

        // 모든 코루틴 동시에 실행
        foreach (var effect in effects)
        {
            IEnumerator routine = effect.PlayDespawnEffect(delay, split);
            subRoutines.Add(StartCoroutine(routine));
        }
        yield return WaitUntillFinished(effects);

        OnPlayDespawnEffectEnd();
    }

    private IEnumerator WaitUntillFinished(List<EffectSpawning> effects)
    {
        // 모든 effect의 IsDone이 True가 되면 함수 탈출
        yield return new WaitUntil(() => effects.All(effect => effect.IsDone));
    }

    private void ClearSubRoutine()
    {
        foreach (var curRoutine in subRoutines)
        {
            if (curRoutine != null) StopCoroutine(curRoutine);
        }
        subRoutines.Clear();
    }

    #region 공통적인 이펙트 이벤트
    protected virtual void OnEffectStart()
    {
        // 이미 실행한 이벤트가 있다면 종료
        ClearSubRoutine();
    }
    protected virtual void OnEffectEnd()
    {
        subRoutines.Clear();
    }
    #endregion

    #region 개별적인 이펙트 이벤트
    protected virtual void OnPlaySpawnEffectStart()
    {
        OnEffectStart();
        OnSpawnStart?.Invoke();
    }
    protected virtual void OnPlaySpawnEffectEnd()
    {
        OnEffectEnd();
        OnSpawnEnd?.Invoke();

    }
    protected virtual void OnPlayDespawnEffectStart()
    {
        OnEffectStart();
        OnDespawnStart?.Invoke();
    }
    protected virtual void OnPlayDespawnEffectEnd()
    {
        OnEffectEnd();
        OnDespawnEnd?.Invoke();
        gameObject.SetActive(false);
    }
    #endregion



}
