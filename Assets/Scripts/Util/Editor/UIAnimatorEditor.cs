using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Virtupad;

[CustomEditor(typeof(UIAnimator))]
public class UIAnimatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Auto Fix all start points"))
            AutoFixStartPoints();
    }

    private void AutoFixStartPoints()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            GameObject[] gameObjects = scene.GetRootGameObjects();
            for (int j = 0; j < gameObjects.Length; j++)
            {
                UIAnimator[] animators = gameObjects[j].GetComponentsInChildren<UIAnimator>(true);
                for (int k = 0; k < animators.Length; k++)
                {
                    animators[k].AutoCalcStartAndEndPoint();
                }
            }
        }
        EditorSceneManager.MarkAllScenesDirty();
    }
}
