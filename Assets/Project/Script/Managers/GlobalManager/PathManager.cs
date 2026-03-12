using System.Collections;
using UnityEngine;

public class PathManager : BaseGlobalManager, IGlobalManager
{
    public class Directory
    {
        // 런타임에 초기화
        private string _main;
        public string Save { get; private set; }
        public string Option { get; private set; }

        public Directory()
        {
            // 응용 프로그램에서 사용하는 데이터 경로
            // Executable Application
            _main = $"{Application.persistentDataPath}/Datas"; //Application은 런타임 객체
#if UNITY_EDITOR
            Debug.Log("저장 경로 : " + _main);
#endif

            Save = $"{_main}/Saves";
            Option = $"{_main}/Options";
        }
    }

    public class FileName
    {
        public string GraphicSettings = "GraphicSettings.save";
    }

    // 디렉터리
    public Directory directory { get; private set; }
    public FileName fileName { get; private set; }

    public void Exit()
    {

    }

    public IEnumerator Initialize()
    {
        directory = new();
        fileName = new();
        yield return null;
    }
}
