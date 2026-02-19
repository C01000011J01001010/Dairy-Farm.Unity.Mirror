using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
 

public class FileManager : MonoBehaviour, IGlobalManager
{
    public static GraphicOptionValues savedGraphicOption { get; protected set; }

    // 프리팹은 Resources에 있는 객체를 참조하는 것이니 씬이 넘어가도 삭제되지 않음
    private static Dictionary<Type_Character, GameObject> CharacterPrefabs = new();
    //private static Dictionary<Type_Effect, GameObject> EffectPrefabs = new();
    //private static Dictionary<Type_Weapon, GameObject> WeaponPrefabs = new();

    private PathManager Path;

    public bool IsInit { get; set; }

    public void Exit()
    {
        GetComponent<GameObject>();
    }

    public IEnumerator Initialize()
    {
        Path = GameManager.GetManager<PathManager>();
        if (Path is null)
        {
            Debug.LogError("PathManager does not exist");
            yield break;
        }

#if UNITY_EDITOR
        // 데이터 저장 테스트
        GraphicOptionValues options = GraphicOptionValues.testOption;
        SaveFile(Path.directory.Option, Path.fileName.GraphicSettings, options.Struct2ByteArray());
#endif
        try
        {
            byte[] savedData = LoadFile_FromSaveFolder(Path.directory.Option, Path.fileName.GraphicSettings);
            savedGraphicOption = savedData.ByteArray2Struct<GraphicOptionValues>();
        }
        catch (Exception ex)
        {
            savedGraphicOption = GraphicOptionValues.defaultOption;
            Debug.LogWarning(ex);
        }


        //CheckStruct<GraphicOptionValues>(savedGraphicOption);


        yield return FillContainer(CharacterPrefabs, "Prefabs/Characters");
        //yield return FillContainer(EffectPrefabs, "Prefabs/Effects");
        //yield return FillContainer(WeaponPrefabs, "Prefabs/Weapons");
        yield break;
    }

    private void CheckStruct<T_Struct>(object obj)
    {
        if(typeof(T_Struct).IsStruct())
        {
            Type type = typeof(T_Struct);
            foreach (FieldInfo field in type.GetFields())
            {

                Debug.Log($"{field.Name} : {field.GetValue(obj)}");
            }
        }
        else
        {
            Debug.LogWarning($"{typeof(T_Struct).Name} is not struct");
        }
        
    }

    
    
    //// 캐릭터 프리팹 초기화
    //IEnumerator initializeCharacterPrefabs()
    //{
    //    if (CharacterPrefabs is not null) yield break;
    //    CharacterPrefabs = new();

    //    yield return FillContainer(CharacterPrefabs, "Prefabs/Characters");
    //}


    //IEnumerator initializeEffectPrefabs()
    //{
    //    if (EffectPrefabs is not null) yield break;
    //    EffectPrefabs = new();

    //    yield return FillContainer(EffectPrefabs, "Prefabs/Effects");
    //}
    //IEnumerator initializeWeaponPrefabs()
    //{
    //    if (WeaponPrefabs is not null) yield break;
    //    WeaponPrefabs = new();

    //    yield return FillContainer(WeaponPrefabs, "Prefabs/Weapons");
    //}

    public IEnumerator FillContainer<T_Enum, T_PrefabType>
        (Dictionary<T_Enum, T_PrefabType> targetDictionary, string directory)
        where T_Enum : struct, Enum
        where T_PrefabType : UnityEngine.Object
    {
        int lastTime = Environment.TickCount;
        Type enumType = typeof(T_Enum);
        foreach (T_Enum current in Enum.GetValues(enumType))
        {
            string fileName = current.ToString();
            if (fileName == "Length" || fileName.Contains("__")) continue;
            T_PrefabType prefab = GetPrefab_ByEnum(directory, fileName, current, targetDictionary);

            int currentTime = Environment.TickCount;
            // 마지막부터 0.1초 뒤 현재시간
            if (lastTime + 100 < currentTime)
            {
                lastTime = currentTime;
                yield return null; // 다음프레임에 계속
            }
        }
    }


    public static void SaveFile(string directory, string fileName, params byte[] data) // params : 가변매개변수 키워드
    {
        // directory(경로) -> 폴더
        // 경로가 확정되어야 파일을 그 안에 만들 수 있음
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        string totalDirectory = $"{directory}/{fileName}";

        /*
         * create는 FileStream을 반환
         * FileStream->데이터 전송 또는 통신
         * 생성된 파일을 잡고 있는 FileStream이 존재하게 됨
         * 이미 하나의 fileStream을 가지고 있을 때에 같은 프로그램이라도
         * 여러개의 fileStream으로 하나의 파일을 관리할 수 없음!(동시접근 제한)
         * FileStream을 제대로 안끄고 나가면 프로그램을 껐음에도 불구하고 파일이 안지워짐
         * => 컴퓨터를 재부팅하거나 시간이 자나면 => 윈도우가 안쓰는 메모리를 정리
        */

        if (!File.Exists(totalDirectory)) File.Create(totalDirectory).Close();

        File.WriteAllBytes(totalDirectory, data);
    }

    public static byte[] LoadFile_FromSaveFolder(string directory, string fileName)
    {
        if (Directory.Exists(directory))
        {
            string totalDirectory = $"{directory}/{fileName}";

            if(File.Exists(totalDirectory))
            {
                return File.ReadAllBytes(totalDirectory);
            }
        }
        return null;
    }


    public static T_FileType LoadFile_FromResources<T_FileType>(string path)
        where T_FileType : UnityEngine.Object
        => Resources.Load<T_FileType>(path);

    public static T_FileType LoadFile_FromResources<T_FileType>(string directory, string fileName)
        where T_FileType : UnityEngine.Object
        => Resources.Load<T_FileType>($"{directory}/{fileName}");

    public static GameObject GetPrefab(string path)
        => LoadFile_FromResources<GameObject>(path);

    private T_File GetPrefab_ByEnum<T_Enum, T_File>(string Directory, string fileName,
        T_Enum fileEnum, Dictionary<T_Enum, T_File> container = null)
        where T_Enum : struct, Enum
        where T_File : UnityEngine.Object // Resources.Load는 UnityEngine.Object를 상속받은 타입만 파일로 읽어올 수 있음
    {
        T_File file = LoadFile_FromResources<T_File>(Directory, fileName);

        if (file is null)
        {
            Debug.LogError($"{Directory}/{fileName} doesn't exist in Resources");
            return null;
        }

        else if (container is not null)
        {
            container.TryAdd(fileEnum, file);
        }
        return file;
    }

    public static GameObject GetCharacterPrefab(Type_Character type) 
        => CharacterPrefabs.TryGetValue(type, out var result) ? result : null;
    //public static GameObject GetEffactPrefab(Type_Effect type)
    //    => EffectPrefabs.TryGetValue(type, out var result) ? result : null;
    //public static GameObject GetWeaponPrefab(Type_Weapon type)
    //    => WeaponPrefabs.TryGetValue(type, out var result) ? result : null;
}
