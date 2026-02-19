using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : BaseUi
{
    [SerializeField] private Image loadingImage;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingText;
    private Coroutine loadingAnimCoroutine;

    private int currentAmount = 0;
    private int maxAmount = 0;
    public float currentAmountPerMax
        => maxAmount <= 0 ? 1.0f : (float)currentAmount / maxAmount;

    public bool isLoading {get; protected set; }

    public override MyUi UiType => MyUi.LoadingScreen;

    public override void Exit()
    {
        UIManager.OnLoadingCall -= Loading_Call;
        UIManager.OnLoadingNext -= Loading_Next;
        UIManager.OnLoadingNextPercent -= Loading_NextPercent;
        UIManager.OnLoadingEnd -= Loading_End;
    }

    public override IEnumerator Initialize()
    {
        loadingSlider.onValueChanged.RemoveAllListeners();

        UIManager.OnLoadingCall -= Loading_Call;
        UIManager.OnLoadingCall += Loading_Call;
        UIManager.OnLoadingNext -= Loading_Next;
        UIManager.OnLoadingNext += Loading_Next;
        UIManager.OnLoadingNextPercent -= Loading_NextPercent;
        UIManager.OnLoadingNextPercent += Loading_NextPercent;
        UIManager.OnLoadingEnd -= Loading_End;
        UIManager.OnLoadingEnd += Loading_End;

        yield return null;
    }

    

    public bool Active(bool value)
    {
        return isLoading;
    }

    public bool SetLoadingState(bool value)
    {
        gameObject.SetActive(value);
        Debug.Log(gameObject.activeSelf ? "로딩 Start!" : "로딩 End!");
        isLoading = value;
        return value;
    }

    // 정수개의 초기화를 진행할 때 사용 -> GameManager와 기타 매니저 초기화
    void Loading_Call(int processAmount)
    {
        SetLoadingState(true);
        loadingAnimCoroutine = StartCoroutine(LoadingAnimation());
        if (processAmount != -1) // -1이면 정수를 안쓰고 퍼센트만 쓰겠다는 의미
        {
            currentAmount = 0;
            maxAmount = processAmount;
        }
        Visulize("시스템 로딩 중...", 0.0f);
    }

    void Loading_Next(string loadingContext, int skipAmount = 1)
    {
        currentAmount += skipAmount;
        Visulize(loadingContext, currentAmountPerMax);
    }
    void Loading_NextPercent(string loadingContext, float percent)
    {
        Visulize(loadingContext, percent);
    }

    void Loading_End()
    {
        Visulize("시스템 로딩 완료", 1.0f);
        StopCoroutine(loadingAnimCoroutine);
        SetLoadingState(false);
    }

    /// <param name="context">로딩화면에 띄울 메세지</param>
    /// <param name="unitInterval">0~1 사이의 로딩진행도</param>
    public void Visulize(string context, float unitInterval)
    {
        loadingText.text = $"{context}\n{(unitInterval * 100f).ToString("F2")}%";
        loadingSlider.value = unitInterval;
    }

    public IEnumerator LoadingAnimation()
    {
        float interval = 1.0f / 60.0f; // 60프레임 기준 1프레임마다 적용되도록
        float plusRot = 720 * interval;
        float curRot = 0;
        while (true)
        {
            curRot += plusRot % 360;

            // Quaternion.AngleAxis(angle(몇 degree 돌릴 것인지), axis(기준축은 어디인지));
            loadingImage.transform.rotation = Quaternion.AngleAxis(curRot, Vector3.forward);
            yield return new WaitForSeconds(interval); 
        }
    }
}
