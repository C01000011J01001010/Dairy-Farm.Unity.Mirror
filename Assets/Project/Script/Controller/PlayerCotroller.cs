using System.Collections;
using UnityEngine;


public class PlayerCotroller : MonoBehaviour, IScenedInitialize
{
    [SerializeField] private int _priority = 10;

    public PlayableCharacter character;
    //public Vector2 input;
    UserInputManager inputManager;

    public int Priority => _priority;

    

    private void OnEnable()
    {
        GameManager.UPDATE_OnController += CALLBACK_UPDATE;
    }

    private void OnDisable()
    {
        GameManager.UPDATE_OnController -= CALLBACK_UPDATE;
    }

    public void Exit()
    {
        
    }

    public IEnumerator Initialize()
    {
        inputManager = GameManager.GetManager<UserInputManager>();
        yield return null;
    }
    public IEnumerator PostInitialize() 
    { 
        yield break; 
    }

    private void CALLBACK_UPDATE()
    {
        InputMove();
        InputSprint();
    }

    private void InputMove()
    {
        character?.Move(inputManager.Move);
    }

    private void InputSprint()
    {
        character?.SprintHold(inputManager.Sprint);
    }

    
}
