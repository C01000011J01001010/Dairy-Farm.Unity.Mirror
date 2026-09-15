using CoreEngine;
using CoreEngine.Actor;
using CoreEngine.Pool;
using Farm.Character.Move;
using Farm.Character.StateMachine;
using Farm.Egg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Farm.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BaseCharacter : BaseActorHost, IActorHost, IPoolable, ITickable, IFixedTickable
    {
        #region featrue
        [SerializeField] protected CharacterAnimFeature animFeature = new();
        [SerializeField] protected CharacterStateController stateController = new();
        [SerializeField] protected CharacterTileChecker tileChecker = new();
        [SerializeField] protected CharacterInventory inventory = new();
        [SerializeField] protected CharacterQuestBook questBook = new();
        [SerializeField] protected CharacterCropDataSheet cropDataSheet = new();
        [SerializeField] protected CharacterEggEncyclopedia eggEncyclopedia = new();
        [SerializeField] protected CharacterActionController actionController = new();
        [SerializeField] protected CharacterMoveFeature moveFeature = new();
        #endregion

        public TickGroup TickGroup => TickGroup.Character;

        public FixedTickGroup FixedTickGroup => FixedTickGroup.Physics;

        public IPoolReleaser Releaser { get; set; }


        protected override void Awake()
        {
            base.Awake();
        }

        private void OnDestroy()
        {
            FeatureHandler.Dispose_RegisteredFeatures();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            OnSpawn();
        }

        public void OnSpawn()
        {
            FeatureHandler.RegisterFeature(animFeature);
            FeatureHandler.RegisterFeature(stateController);
            FeatureHandler.RegisterFeature(tileChecker);
            FeatureHandler.RegisterFeature(inventory);
            FeatureHandler.RegisterFeature(questBook);
            FeatureHandler.RegisterFeature(cropDataSheet);
            FeatureHandler.RegisterFeature(eggEncyclopedia);
            FeatureHandler.RegisterFeature(actionController);
            FeatureHandler.RegisterFeature(moveFeature);

            FeatureHandler.Initialize_RegisteredFeatures();

            stateController.StartState();
        }

        private void InitializeFeaure()
        {

        }

        public void OnDespawn()
        {
            
        }

        

        //public void SprintHold(bool value) => isSprint = value;

        //public void SprintToggle() => isRun = !isRun;

        public virtual void Tick(float deltaTime)
        {
            FeatureHandler.Tick_InitializedFeatures(deltaTime);
            //tileChecker.Tick(deltaTime);
            //stateController.Tick(deltaTime);
        }

        public virtual void FixedTick(float fixedDeltaTime)
        {
            FeatureHandler.Tick_InitializedFeatures(fixedDeltaTime);
            //moveFeature.FixedTick(fixedDeltaTime);
            //stateController.FixedTick(fixedDeltaTime);
        }

        

        protected override void OnValidate()
        {
            base.OnValidate();
            stateController.OnValidate();
        }
    }
}


