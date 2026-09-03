using CoreEngine;
using CoreEngine.EventBus;
using CoreEngine.Facades;
using CoreEngine.Hub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CoreEngine.Ui
{
    public class UiController : BaseLeaf, IUi, IPriority
    {
        [SerializeField] private int _priority = -1;
        public int Priority => _priority;

        [SerializeField] private UiSelection _uiSelection;

        private readonly List<AsyncOperationHandle<GameObject>> _loadedHandles = new();

        public bool IsActive { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            // 3계층 말단(Leaf) 객체로서 EventBus를 통해 상향식으로 자신을 등록[cite: 8, 28]
            ModuleRegistrationEvent evt = new ModuleRegistrationEvent(this, true, myScope);
            EventBus<ModuleRegistrationEvent>.Publish(evt);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ModuleRegistrationEvent evt = new ModuleRegistrationEvent(this, false, myScope);
            EventBus<ModuleRegistrationEvent>.Publish(evt);
        }

        public IEnumerator Initialize()
        {
            // 수정됨: 인자 없이 내부 캡슐화 리스트 순회
            foreach (var assetRef in _uiSelection.GetValidUis())
            {
                if (assetRef.RuntimeKeyIsValid())
                {
                    yield return LoadAndOpenInitialUi(assetRef);
                }
            }
            IsActive = true;
        }

        public IEnumerator LateInitialize()
        {
            yield break;
        }

        private IEnumerator LoadAndOpenInitialUi(AssetReferenceGameObject assetRef)
        {
            var handle = assetRef.InstantiateAsync(transform);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedHandles.Add(handle);

                if (handle.Result.TryGetComponent(out BaseUi ui))
                {
                    yield return ui.Initialize();
                    //ui.ClaimOpen();
                }
            }
            else
            {
                Debug.LogError($"[UiController] UI 로드 실패: {assetRef.RuntimeKey}");
            }
        }

        // ====================================================================
        // Facade API 
        // ====================================================================

        public T OpenUi<T>() where T : BaseUi, IUi
        {
            T ui = CoreFacade.GetUi<T>();
            //ui?.ClaimOpen();
            return ui;
        }

        public void CloseUi(BaseUi ui)
        {
            //ui?.ClaimClose();
        }

        public void SetActive(bool active)
        {
            IsActive = active;
        }

        public void Exit()
        {
            foreach (var handle in _loadedHandles)
            {
                if (handle.IsValid())
                {
                    Addressables.ReleaseInstance(handle);
                }
            }
            _loadedHandles.Clear();
            IsActive = false;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            // 인스펙터 변경 시 에디터 단에서 즉시 타입/중복 검증 수행
            _uiSelection?.Validate<IUi>(this, nameof(_uiSelection));
        }
#endif
    }
}