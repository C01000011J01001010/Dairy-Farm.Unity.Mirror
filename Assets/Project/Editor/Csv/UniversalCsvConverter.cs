using System;
using System.IO;
using System.Linq;
using System.Reflection; // 리플렉션 사용을 위해 추가
using UnityEditor;
using UnityEngine;

public class UniversalCsvConverter : BaseCsvConverter
{
    const string target = "Universal(reflection)";
    protected override string ConverterTarget => target;

    protected Type targetType;
    protected override Type TargetType => targetType;

    

    [MenuItem(defaultMenu + target)]
    public static void ShowWindow() => GetWindow<UniversalCsvConverter>("CSV Converter");

    protected override void OnEnable()
    {
        base.OnEnable();
        SetTargetType(TypeName);
    }

    // 문자열을 지정된 타입으로 안전하게 변환하는 헬퍼 함수
    private object ConvertValue(string value, Type type)
    {
        try
        {
            if (type == typeof(int)) return int.Parse(value);
            if (type == typeof(float)) return float.Parse(value);
            if (type == typeof(string)) return value;
            if (type == typeof(bool)) return bool.Parse(value);
            if (type.IsEnum) return Enum.Parse(type, value, true);
        }
        catch
        {
            Debug.LogWarning($"값 변환 실패: '{value}'를 {type.Name} 타입으로 바꿀 수 없습니다.");
        }
        return null; // 지원하지 않는 타입이거나 실패하면 null 반환 (기존 값 유지)
    }

    protected override void CheckTypeNameForReflection()
    {
        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                // 1. 텍스트 입력 칸 (여기서는 글자를 치더라도 TypeName 변수만 바뀌고 아무 연산도 안 함)
                TypeName = EditorGUILayout.TextField("타입 이름 입력", TypeName);

                if (GUILayout.Button("완료", GUILayout.Width(50)))
                {
                    TypeName = TypeName.Trim();

                    SetTargetType(TypeName); // 무거운 리플렉션 연산 실행
                    UpdateSetting();         // 세팅 저장
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                // 1. 텍스트 입력 칸 (여기서는 글자를 치더라도 TypeName 변수만 바뀌고 아무 연산도 안 함)
                EditorGUILayout.LabelField("현재 데이터 타입", TargetType?.Name ?? "존재하지 않는 데이터 타입");
            }
        }
        

        GUILayout.Space(10);
    }

    protected void SetTargetType(string targetTypeStr)
    {
        // 어셈블리 분리로 사용 못함(기본 cs vs 에디터cs)
        //targetType = Type.GetType(targetTypeStr);

        targetType = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes())
                        .FirstOrDefault(type => type.Name == targetTypeStr);

        if (targetType == null)
        {
            Debug.LogWarning($"Type({targetTypeStr})이 존재하지 않음");
        }
    }

    protected override void ConvertDetails(ScriptableObject asset, int rowNum, string[] cols)
    {
        // 리플렉션을 통한 데이터 자동 매칭 및 할당
        // id와 name은 BaseCsvConverter에서 해결
        for (int j = 2; j < attributeCount; j++)
        {
            if (j >= cols.Length || Empty(rowNum, j, cols)) continue;

            string headerName = headers[j].Trim();
            string stringValue = cols[j].Trim();

            // 해당 이름의 변수가 targetType에 존재하는지 검색
            FieldInfo field = targetType.GetField(headerName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                object convertedValue = ConvertValue(stringValue, field.FieldType);
                if (convertedValue != null)
                {
                    field.SetValue(asset, convertedValue);
                }
            }
        }
    }

    // 현재 프로젝트의 모든 어셈블리에서 이름으로 클래스 타입을 찾는 함수
    private Type GetTypeByName(string className)
    {
        if (string.IsNullOrWhiteSpace(className)) return null;

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.Name == className) return type;
            }
        }
        return null;
    }
}