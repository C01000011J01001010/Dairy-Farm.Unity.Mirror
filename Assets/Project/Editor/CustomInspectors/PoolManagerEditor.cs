#if UNITY_EDITOR
using NUnit.Framework.Internal;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


// 커스텀 인스펙터는 "누구의" 커스텀 인스펙터인지 명시!
// CustomEditor를 명시하면 자동으로 빌드 대상에서 제외
[CustomEditor(typeof(Disabled_PoolManager))]
public class PoolManagerEditor : Editor
{
    struct TableState
    {
        public SerializedProperty property;
        public int lastCount;
        public bool isOpen;

        public void Set(SerializedProperty target)
        {
            property = target;
            lastCount = target.arraySize;
        }
    }
    private TableState[] tables;
    private int tableCount;


    // 인스펙터 창이 활성화된 순간
    private void OnEnable()
    {
        /* 여기에서는 이 "인스펙터"클래스에서 쓸 수 있는 변수 뿐만 아니라
         * 이 "UI"가 원본의 변수를 가져올 수 있어야 함
         * 원본의 변수의 내용을 확인하거나 바꿔줄 수 있는거?
         * 유니티의 Inspector는 모두 리플렉션으로 작동함
         * 유니티에서 Inspector창에 "보이게"하려면 무슨 효과가 필요했는가?
         * => serialize된 field여야함
         * => == 직렬화된 필드
         * 여기에서는 무조건 직렬화된 오브젝트만 받을 수 있음
         */
        //SetProperty();
    }
    private void SetProperty()
    {
        List<SerializedProperty> properties = new(4);
        properties.TryAdd(serializedObject.FindProperty("requestCharacter"));
        properties.TryAdd(serializedObject.FindProperty("requestController"));
        properties.TryAdd(serializedObject.FindProperty("requestEffect"));
        properties.TryAdd(serializedObject.FindProperty("requestWeapon"));

        tableCount = properties.Count;
        tables = new TableState[tableCount];
        for (int i = 0; i < tableCount; i++)
        {
            tables[i].Set(properties[i]);
        }
    }


    // 인스펙터 창을 그려달라는 요청! Graphic User Interface => GUI
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DrawCustomInspector();
    }

    public void DrawCustomInspector()
    {
        target.TryDrawScriptOpenButton();


        // 인스펙터가 보여주고 있는 대상을 업데이트
        serializedObject.Update();

        // 테이블들을 그려주도록 합시다
        for (int i = 0; i < tableCount; i++)
        {
            DrawTable(ref tables[i]);
        }

        //프로퍼티의 적용
        // 여기서 뭔가 바꾸는건 PoolManager를 바꾸는 것이 아니라 에디터인스턴스를 바꾸는것이다.
        // PoolManager에게 "변경"을 보내줘야한다.
        // 1. 바꾸면 저장
        // 2. 버튼을 눌러 저장
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTable(ref TableState state)
    {
        

        #region TitleBar
        DrawTitle(ref state);
        #endregion
        //DrawDescription();
        #region Row
        if (state.isOpen)
        {
            //EditorGUILayout.BeginHorizontal();

            if (state.lastCount > 0)
            {
                for (int i = 0; i < state.lastCount; i++)
                {
                    
                    DrawRow(state.property.GetArrayElementAtIndex(i), out bool isDeleted);
                    
                    if (isDeleted)
                    {
                        state.property.DeleteArrayElementAtIndex(i);
                        state.lastCount--;
                    }
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("List is Empty");
                EditorGUILayout.EndHorizontal();
            }

            //EditorGUILayout.EndHorizontal();

        }
        #endregion
    }

    void DrawTitle(ref TableState state)
    {
        SerializedProperty property = state.property;

        EditorGUILayout.BeginHorizontal();

        //EditorGUILayout.LabelField(name, EditorStyles.boldLabel);

        // Foldout은 눌리면 원래 상태에서 반대 상태로 이동함
        state.isOpen = EditorGUILayout.Foldout(state.isOpen, state.property.displayName);

        // 마지막 카운트에서 바뀌는 순간을 체크해서 디버그로 찍어주세요
        // IntField 는 인풋필드의 값으 바꾸기만 하면 바로 적용(실시간 적용)
        // DelayedIntField 는 Enter를 누르거나 입력을 종료하면 적용(신뢰성 있는 반응)
        int currentCount = EditorGUILayout.DelayedIntField(state.lastCount, GUILayout.Width(40.0f));
        if (currentCount < 0) currentCount = 0;
        OnCountChanged(ref state, currentCount);

        bool isPressed = GUILayout.Button("+", GUILayout.Width(20.0f));
        if (isPressed)
        {
            OnAddButtonClick(ref state);
        }

        EditorGUILayout.EndHorizontal();
    }

    void DrawDescription()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("생성하길 원하는 객체타입과 개수를 설정할 수 있습니다.");
        EditorGUILayout.EndHorizontal();
    }

    /// <summary> </summary>
    /// <param name="row">각 행에 표현될 프로퍼티</param>
    /// <param name="isDeleted">x버튼이 눌렸을 때 true를 반환</param>
    void DrawRow(SerializedProperty row, out bool isDeleted)
    {
        EditorGUILayout.BeginHorizontal();

        SerializedProperty wantType = row.FindPropertyRelative("_wantType");
        SerializedProperty amount   = row.FindPropertyRelative("_amount");

        // 근데 wantType은 Enum이라고 하는 확신이 없잖아? 그냥 이름이 wantType인거지
        // 만약 확신할 수 없다면 그냥 대충 저희가 클래스 멤버변수를 만들면 띄워주는 것처럼
        // 프로퍼티를 그냥 보여주는거 만들면 안될까?
        EditorGUILayout.PropertyField(wantType); // 기본값 그리기
        amount.intValue = EditorGUILayout.DelayedIntField(amount.intValue, GUILayout.Width(60.0f));
        isDeleted = GUILayout.Button("x", GUILayout.Width(30.0f));

        EditorGUILayout.EndHorizontal();
    }

    void OnCountChanged(ref TableState state, int count)
    {
        if (state.lastCount == count) return;
        Debug.Log($"{name} : {count}");

        state.lastCount = state.property.arraySize = count;
    }

    private void OnAddButtonClick(ref TableState state)
    {
        OnCountChanged(ref state, state.lastCount + 1);
    }


}
#endif