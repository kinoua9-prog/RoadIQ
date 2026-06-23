using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SmartLevelGenerator))]
public class SmartLevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SmartLevelGenerator generator = (SmartLevelGenerator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Smart Level"))
        {
            generator.GenerateSmartLevel();

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                generator.gameObject.scene
            );
        }
    }
}