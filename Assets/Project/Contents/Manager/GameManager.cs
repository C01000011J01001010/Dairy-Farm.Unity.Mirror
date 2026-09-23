using CoreEngine;
using CoreEngine.Facades;
using CoreEngine.Manager;
using Farm.Pool;
using System.Collections;
using UnityEngine;

namespace Farm.GameRule
{
    public class GameManager : BaseManager, IPriority
    {
        IEnumerator _routine;

        CharacterPoolManager characterPoolManager;

        public int Priority => (int)ManagerPriority.TopLevel;


        protected override IEnumerator OnInitialize()
        {
            yield return base.OnInitialize();
            characterPoolManager = CoreFacade.GetManager<CharacterPoolManager>();

            _routine = Initial();
            StartCoroutine(_routine);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_routine != null) StopCoroutine(_routine);
        }

        private IEnumerator Initial()
        {
            yield return new WaitUntil(CoreFacadeState.GetSceneInit);

            characterPoolManager.Spawn2D(CharacterPoolType.Tori, Vector2.zero, Quaternion.identity);
        }
    }
}

