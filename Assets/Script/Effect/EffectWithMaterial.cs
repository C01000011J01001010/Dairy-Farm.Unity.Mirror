using System.Collections;
using System.Threading;
using UnityEngine;




public abstract class EffectWithMaterial : MonoBehaviour
{
    [Tooltip("이팩트를 위해 연결해야하는 Material")]
    public Material targetMaterial;

    protected Renderer _renderer;
    protected Material _origin;
    protected Material _effect;

    public bool IsDone { get; protected set; }

    protected void Awake()
    {
        _renderer = gameObject.GetComponent<Renderer>();
        _origin = _renderer.sharedMaterial;
        _effect = new Material(targetMaterial);
        InitilaizEffect();
    }

    protected void OnDestroy()
    {
        Destroy(_effect);
    }

    protected void InitilaizEffect()
    {
        int baseColor = Shader.PropertyToID("_BaseColor");
        int baseMap = Shader.PropertyToID("_BaseMap");

        // BaseMap 복사
        Texture originBaseMap = _origin.GetTexture(baseMap);
        _effect.SetTexture(baseMap, originBaseMap);

        // BaseColor 복사
        Color originBaseColor = _origin.GetColor(baseColor);
        _effect.SetColor(baseColor, originBaseColor);
    }

    // 코드 중복 방지
    protected IEnumerator CommonEffectRoutine(IEnumerator subRoutine)
    {
        // Routine 시작
        IsDone = false;

        // 타겟 복사본 부착
        _renderer.sharedMaterial = _effect;

        // 이펙트 시작
        yield return subRoutine;

        // 원본으로 복귀
        _renderer.sharedMaterial = _origin;

        // Routine 종료
        IsDone = true;
    }
}
