using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour, IScenedManager
{
    [SerializeField] private int _priority;
    public int Priority => _priority;


    List<BaseCharacter> _characterList;

    public void Exit()
    {
        GameManager.UPDATE_OnCharacter -= CALLBACK_Update;
        GameManager.UPDATE_Physics -= CALLBACK_FixedUpdate;
    }

    public IEnumerator Initialize()
    {
        _characterList = new();
        GameManager.UPDATE_OnCharacter -= CALLBACK_Update;
        GameManager.UPDATE_OnCharacter += CALLBACK_Update;
        GameManager.UPDATE_Physics -= CALLBACK_FixedUpdate;
        GameManager.UPDATE_Physics += CALLBACK_FixedUpdate;

        yield return null;
    }

    public IEnumerator LateInitialize()
    {
        yield break;
    }

    public void AddList(BaseCharacter character)
    {
        if (!_characterList.Contains(character))
        {
            _characterList.Add(character);
        }
    }

    public void RemoveList(BaseCharacter character)
    {
        _characterList.Remove(character);
    }

    public virtual void CALLBACK_Update()
    {
        for(int i = _characterList.Count-1; i >= 0; --i)
        {
            _characterList[i].Tick();
        }
    }

    public virtual void CALLBACK_FixedUpdate()
    {
        for (int i = _characterList.Count - 1; i >= 0; --i)
        {
            _characterList[i].FixedTick();
        }
    }
}
