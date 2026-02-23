using UnityEngine;

public class Constants
{
    #region Resources 디렉터리
    public const string DIRECTORY_Prefabs_Characters = "Prefabs/Characters";
    public const string DIRECTORY_Prefabs_Controllers = "Prefabs/Controllers";
    public const string DIRECTORY_Prefabs_Uis = "Prefabs/UIs";
    #endregion

    #region Scene이름
    public const string SCENE_NAME_GlobalScene = "GlobalScene";
    public const string SCENE_NAME_TitleScene = "TitleScene";
    public const string SCENE_NAME_SampleScene = "SampleScene";
    #endregion


    #region Layer Name
    public const string LAYER_PlayerCharacter = "PlayerCharacter";
    #endregion    

    #region LayerMask
    public static LayerMask LAYERMASK_PlayerCharacter = GetLayer(Layer.PlayerCharacter);
    #endregion

    #region 태그
    public const string TAG_StartPoint = "StartPoint";
    public const string TAG_PlayerCharacter = "PlayerChracter";
    #endregion

    private static LayerMask GetLayer(params Layer[] index)
    {
        LayerMask result = 0;
        foreach(Layer i in index)
        {
            result |= 1 << (int)i;
        }
        return result;
    }
}

public enum Layer
{
    PlayerCharacter
}
