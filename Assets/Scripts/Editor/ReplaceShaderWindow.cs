using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ReplaceShaderWindow : EditorWindow
{
    [MenuItem("Tools/Replace Shader")]
    public static void Open()
    {
        GetWindow<ReplaceShaderWindow>();
    }

    Shader shader;
    Shader newShader;

    string shaderToReplace = "Hidden/InternalErrorShader";
    Shader newShader2;

    List<string> materials = new List<string>();

    void OnGUI()
    {
        EditorGUILayout.LabelField("Replace selected shader");

        shader = EditorGUILayout.ObjectField(shader, typeof(Shader), false) as Shader;
        newShader = EditorGUILayout.ObjectField(newShader, typeof(Shader), false) as Shader;

        if (GUILayout.Button("Replace"))
        {
            if (shader == null)
                Debug.LogError("Shader field can not be empty");

            if (newShader == null)
                Debug.LogError("New Shader field can not be empty");

            materials.Clear();

            string shaderPath = AssetDatabase.GetAssetPath(shader);
            string[] allMaterials = AssetDatabase.FindAssets("t:Material");

            for (int i = 0; i < allMaterials.Length; i++)
            {
                allMaterials[i] = AssetDatabase.GUIDToAssetPath(allMaterials[i]);
                string[] dep = AssetDatabase.GetDependencies(allMaterials[i]);

                if (ArrayUtility.Contains(dep, shaderPath))
                {
                    materials.Add(allMaterials[i]);
                }
            }

            foreach (string materialPath in materials)
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                mat.shader = newShader;
            }

            Debug.Log($"{materials.Count} materials replaced.");
        }

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Replace typed in shader");

        shaderToReplace = EditorGUILayout.TextField(shaderToReplace);
        newShader2 = EditorGUILayout.ObjectField(newShader2, typeof(Shader), false) as Shader;

        if (GUILayout.Button("Replace"))
        {
            if (shaderToReplace == null && shaderToReplace == "")
                Debug.LogError("Shader To Replace field can not be empty");

            if (newShader2 == null)
                Debug.LogError("New Shader 2 field can not be empty");

            materials.Clear();

            string[] allMaterials = AssetDatabase.FindAssets("t:Material");

            for (int i = 0; i < allMaterials.Length; i++)
            {
                allMaterials[i] = AssetDatabase.GUIDToAssetPath(allMaterials[i]);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(allMaterials[i]);

                if (mat.shader.name == shaderToReplace)
                {
                    materials.Add(allMaterials[i]);
                    mat.shader = newShader2;
                }
            }

            Debug.Log($"{materials.Count} materials replaced.");
        }

        EditorGUILayout.Space(20);

        if (GUILayout.Button("Print out all shaders used in project materials"))
        {
            materials.Clear();

            string[] allMaterials = AssetDatabase.FindAssets("t:Material");

            for (int i = 0; i < allMaterials.Length; i++)
            {
                allMaterials[i] = AssetDatabase.GUIDToAssetPath(allMaterials[i]);
                materials.Add(allMaterials[i]);
            }

            List<string> shaders = new List<string>();

            foreach (string materialPath in materials)
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

                if (!shaders.Contains(mat.shader.name))
                {
                    shaders.Add(mat.shader.name);
                }
            }

            shaders = shaders.OrderBy(x => x).ToList();

            foreach (var item in shaders)
            {
                Debug.Log($"{item}");
            }
        }

        GUILayout.BeginScrollView(new Vector2(0, 0), GUIStyle.none);
        {
            for (int i = 0; i < materials.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(Path.GetFileNameWithoutExtension(materials[i]));
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Show"))
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(materials[i], typeof(Material)));
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
    }
}