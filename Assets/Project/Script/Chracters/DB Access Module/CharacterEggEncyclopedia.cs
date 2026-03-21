

using System.Collections.Generic;

public class CharacterEggEncyclopedia : BaseDatabaseAccessModule<MagicalEggStaticManager, MagicalEggData>
{
    #region 저장할 데이터
    // 캐릭터는 한번에 하나의 알만 갖음
    private MagicalEggDataContainer curEgg;

    // 행복도 100%를 달성한 알을 컬렉션으로 갖음, egg정보는 GetData 메서드로 호출
    private HashSet<int> eggCollection;
    #endregion

    public bool TrySetCurEgg(int eggIndex)
    {
        // 이미 컬렉션에 존재하는 알은 취급 안함
        if (eggCollection.Contains(eggIndex)) return false;

        // 알데이터를 가져와서 현재 알로 설정
        MagicalEggData eggData = GetData(eggIndex);
        if (eggData != null)
        {
            curEgg = new MagicalEggDataContainer(eggData);
            return true;
        }
        return false;
    }

    public bool TryAddCollection()
    {
        if (curEgg == null || curEgg.happiness < 100) return false;

        // HashSet.Add : 내부적으로 중복 알아서 체크
        eggCollection.Add(curEgg.GetIndex());

        // 컬렉션에 추가했으니 현재 알은 삭제처리
        curEgg = null;
        return true;
    }
}