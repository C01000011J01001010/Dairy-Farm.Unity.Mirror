using CoreEngine;
using CoreEngine.EventBus;
using CoreEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class TitleSelection : BaseSelection, IInitialize, IScenedUi
{
    private BaseButton[] _buttonList;
    [SerializeField] private SceneReference _sceneReference;


    public override IEnumerator Initialize()
    {
        _buttonList = gameObject.GetComponentsInChildren<BaseButton>();

        foreach (BaseButton button in _buttonList)
        {
            yield return button.Initialize();
        }

        SetButtonCallback();

        Debug.Log("TitleSelection 초기화 성공");
        yield return null;
    }

    protected override void SetButtonCallback()
    {
        _buttonList[0].SetCallback(CALLBACK_StatNewGame);
        _buttonList[1].SetCallback(CALLBACK_OpenGameSettingsWindow);
        _buttonList[2].SetCallback(CALLBACK_ExitGame);
    }

    protected override void ClearButtonCallback()
    {
        foreach (BaseButton button in _buttonList)
        {
            button.ClearCallback();
        }
    }

    public void CALLBACK_StatNewGame()
    {
        EventBus<SceneLoadRequestEvent>.Publish(new SceneLoadRequestEvent(_sceneReference));
    }

    public void CALLBACK_OpenGameSettingsWindow()
    {

    }

    public void CALLBACK_ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
