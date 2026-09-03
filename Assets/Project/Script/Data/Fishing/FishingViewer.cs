using UnityEngine;

namespace Farm.Fishing
{
    public class FishingViewer : MonoBehaviour
    {
        //인스펙터에서 설정 가능하게 선언
        [SerializeField] private Animator animator;
        [SerializeField] private FishModule fishModule;

        private void OnEnable()
        {
            //이벤트에 내 함수를 구독
            fishModule.Event_OnFishingResult += OnFishingResult;
            Debug.Log("구독완료");
        }

        private void OnDisable()
        {
            // 메모리 누수/ 중복 구독 방지
            fishModule.Event_OnFishingResult -= OnFishingResult;
        }

        private void OnFishingResult(bool isFishing)
        {
            Debug.Log("호출완료");
            if (isFishing)
            {
                Debug.Log("애니메이션 출력");
                //낚시중이라면 Fish트리거를 켜 애니메이션 재생 요청
                animator.SetTrigger("Fish");
            }
        }
    }
}
