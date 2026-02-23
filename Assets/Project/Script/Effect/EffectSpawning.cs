using System.Collections;
using System.Threading;
using UnityEngine;

public enum SpawnEffectType
{
    Phase,
    Dissolve,
}

// 이펙트 사용을 위한 변수
public class SpawnEffectVal
{
    public SpawnEffectType effectType = SpawnEffectType.Dissolve;
    public float delay = 1f;

    public static implicit operator bool(SpawnEffectVal val)
        => val != null;
    public static bool operator true(SpawnEffectVal val)
        => val != null;
    public static bool operator false(SpawnEffectVal val)
        => val == null;
}


[System.Serializable]
public class SpawnEffectSplit
{
    public SpawnEffectType effectType;
    public float splitValue;
}

public abstract class EffectSpawning : EffectWithMaterial
{
    // 이펙트 컨트롤러에서 접근하기 위한 열거자
    public abstract SpawnEffectType type { get; }

    private readonly int _splitId = Shader.PropertyToID("_SplitValue");

    public IEnumerator PlaySpawnEffect(float delay, float targetValue)
        => CommonEffectRoutine(SpawnEffectRoutine(delay, targetValue));
    public IEnumerator PlayDespawnEffect(float delay, float targetValue)
        => CommonEffectRoutine(DespawnEffectRoutine(delay, targetValue));
    private IEnumerator SpawnEffectRoutine(float delay, float targetValue)
    {
        float time = 0;
        // 적용된 딜레이 시간동안 Split값을 변환
        while (time < delay)
        {
            time += Time.deltaTime;
            _effect.SetFloat(_splitId, (time / delay) * targetValue);
            yield return null;
        }
    }
    private IEnumerator DespawnEffectRoutine(float delay, float targetValue)
    {
        float time = 0;
        // 적용된 딜레이 시간동안 Split값을 변환
        while (time < delay)
        {
            time += Time.deltaTime;
            _effect.SetFloat(_splitId, ((delay - time) / delay) * targetValue);
            yield return null;
        }
    }
}
