using CoreEngine.CameraSystem;
using CoreEngine.EventBus;
using CoreEngine.Pool;
using System.Collections;
using UnityEngine;

namespace Farm.Character
{
    public enum CharacterRequest { RequestControl, RequestRelease}
    public struct CharacterRequestEvent : IEvent
    {
        public readonly PlayableCharacter requester;
        public readonly CharacterRequest Request;
        public CharacterRequestEvent(PlayableCharacter changedCharacter, CharacterRequest request)
        {
            requester = changedCharacter;
            Request = request;
        }
    }
    public class PlayableCharacter : BaseCharacter, ISpawnable
    {
        public override void OnSpawn()
        {
            base.OnSpawn();

            EventBus<CharacterRequestEvent>.Publish(
                new CharacterRequestEvent(this, CharacterRequest.RequestControl));

            System.Type cameraType = typeof(TargetCameraController2D);
            EventBus<SetCameraTargetEvent>.Publish(
                new(transform, cameraType));

            EventBus<SwitchCameraEvent>.Publish(
                new SwitchCameraEvent(cameraType, null));
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            var evt = new CharacterRequestEvent(this, CharacterRequest.RequestRelease);
            EventBus<CharacterRequestEvent>.Publish(evt);
        }
    }
}

