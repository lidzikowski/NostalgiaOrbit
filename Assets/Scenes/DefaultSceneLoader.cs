#if UNITY_EDITOR
using NostalgiaOrbitDLL;
using NostalgiaOrbitDLL.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class DefaultSceneLoader
{
    static DefaultSceneLoader()
    {
        EditorApplication.playModeStateChanged += LoadDefaultScene;
        
        //Debug.Log(PrefabTypes.phoenix_type.IsPrefabType(PrefabTypes.Phoenix_normal));
        //Debug.Log(PrefabTypes.phoenix_type.IsPrefabType(PrefabTypes.Phoenix_medium));
        //Debug.Log(PrefabTypes.phoenix_type.IsPrefabType(PrefabTypes.Phoenix_full));

        //Debug.Log(PrefabTypes.phoenix_type.IsPrefabType(PrefabTypes.Yamato_normal));
        //Debug.Log(PrefabTypes.phoenix_type.IsPrefabType(PrefabTypes.Admin));
        //Debug.Log(PrefabTypes.phoenix_type.IsPrefabType(PrefabTypes.Goliath_red_medium));

        //Debug.Log(PrefabTypes.playerShips.IsPrefabType(PrefabTypes.Phoenix_normal));
        //Debug.Log(PrefabTypes.playerShips.IsPrefabType(PrefabTypes.Admin));
    }

    static void LoadDefaultScene(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            EditorSceneManager.LoadScene(0);
        }
    }
}
#endif