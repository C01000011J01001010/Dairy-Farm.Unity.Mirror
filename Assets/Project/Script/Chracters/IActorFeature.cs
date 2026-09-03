using System;
using UnityEngine;

public interface IActorFeature
{
    // 주인 참조
    BaseCharacter Owner { get; }

    // 모듈 활성화 여부
    bool IsActive { get; }
    void SetActive(bool active);

    event Action<bool/*active*/> Evnet_OnSetActive;

    // 초기화 단계
    void Initialize(BaseCharacter owner);
    void PostInitialize();

    // 업데이트 루프
    void Tick(float deltaTime);
    void FixedTick(float fixedDeltaTime);

    // 종료 및 메모리 정리
    void Exit();
}
