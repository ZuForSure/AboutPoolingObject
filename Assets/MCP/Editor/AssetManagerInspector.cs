using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using MCP.DataModels.BaseModels;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

[CustomEditor(typeof(AssetManager))]
public class AssetManagerInspector : Editor
{
    bool fold1 = false;
    bool fold2 = false;
    bool fold3 = false;
    bool fold4 = false;
    bool fold5 = false;

    void OnEnable()
    {
        DataHolder.Instance().Init();

    }

    public override void OnInspectorGUI()
    {
        //EditorGUILayout.BeginVertical("box");
        fold1 = EditorGUILayout.Foldout(fold1, "Sprites");
        if (fold1)
        {
            if (GUILayout.Button("Import Sprite", GUILayout.Width(200)))
            {

                string des = "";
                if (PlayerPrefs.HasKey("AssetManagerSpriteImport"))
                {
                    des = PlayerPrefs.GetString("AssetManagerSpriteImport");
                }

                des = EditorUtility.OpenFolderPanel("Select Sprites Folder", des, "");
                ImportSprites(des);

                if (des != "")
                    PlayerPrefs.SetString("AssetManagerSpriteImport", des);
            }

            if (GUILayout.Button("Clear all", GUILayout.Width(200)))
            {
                EditorUtility.SetDirty(((AssetManager)target));
                ((AssetManager)target).sprites.Clear();
            }

            int order = 1;
            foreach (var item in ((AssetManager)target).sprites)
            {
                EditorGUILayout.BeginHorizontal();
                if (item.Value != null)
                {
                    Texture2D s = item.Value.texture;
                    EditorGUILayout.ObjectField(GUIContent.none, s, typeof(Texture2D), false, GUILayout.MaxWidth(100));
                    EditorGUILayout.LabelField(order.ToString() + ". " + item.Key);
                }

                order++;
                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    EditorUtility.SetDirty(((AssetManager)target));
                    ((AssetManager)target).sprites.Remove(item.Key);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        fold2 = EditorGUILayout.Foldout(fold2, "Prefabs");
        if (fold2)
        {
            if (GUILayout.Button("Import Prefabs", GUILayout.Width(200)))
            {

                string des = "";
                if (PlayerPrefs.HasKey("AssetManagerPrefabImport"))
                {
                    des = PlayerPrefs.GetString("AssetManagerPrefabImport");
                }

                des = EditorUtility.OpenFolderPanel("Select Prefabs Folder", des, "");
                ImportPrefab(des);

                if (des != "")
                    PlayerPrefs.SetString("AssetManagerPrefabImport", des);
            }

            if (GUILayout.Button("Clear all", GUILayout.Width(200)))
            {
                EditorUtility.SetDirty(((AssetManager)target));
                ((AssetManager)target).prefabs.Clear();
            }

            int order = 1;
            foreach (var item in ((AssetManager)target).prefabs)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(order.ToString() + ". " + item.Key);
                order++;
                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    EditorUtility.SetDirty(((AssetManager)target));
                    ((AssetManager)target).prefabs.Remove(item.Key);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        fold3 = EditorGUILayout.Foldout(fold3, "Audio Clip");
        if (fold3)
        {
            if (GUILayout.Button("Import Audio", GUILayout.Width(200)))
            {

                string des = "";
                if (PlayerPrefs.HasKey("AssetManagerAudioImport"))
                {
                    des = PlayerPrefs.GetString("AssetManagerAudioImport");
                }

                des = EditorUtility.OpenFolderPanel("Select Audios Folder", des, "");
                ImportAudio(des);

                if (des != "")
                    PlayerPrefs.SetString("AssetManagerAudioImport", des);
            }

            if (GUILayout.Button("Clear all", GUILayout.Width(200)))
            {
                EditorUtility.SetDirty(((AssetManager)target));
                ((AssetManager)target).audioClips.Clear();
            }

            int order = 1;
            foreach (var item in ((AssetManager)target).audioClips)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(order.ToString() + ". " + item.Key);
                order++;
                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    EditorUtility.SetDirty(((AssetManager)target));
                    ((AssetManager)target).audioClips.Remove(item.Key);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        

        fold5 = EditorGUILayout.Foldout(fold5, "PrefabsLifetime");
        if (fold5)
        {
            if (GUILayout.Button("Import PrefabsLifetime", GUILayout.Width(200)))
            {

                string des = "";
                if (PlayerPrefs.HasKey("AssetManagerPrefabsLifetimeImport"))
                {
                    des = PlayerPrefs.GetString("AssetManagerPrefabsLifetimeImport");
                }

                des = EditorUtility.OpenFolderPanel("Select PrefabsLifetime Folder", des, "");
                ImportPrefabsLifetime(des);

                if (des != "")
                    PlayerPrefs.SetString("AssetManagerPrefabsLifetimeImport", des);
            }

            if (GUILayout.Button("Clear all", GUILayout.Width(200)))
            {
                EditorUtility.SetDirty(((AssetManager)target));
                //((AssetManager)target).prefabLifeTimes.Clear();
            }

            //int order = 1;
            //foreach (var item in ((AssetManager)target).prefabLifeTimes)
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    EditorGUILayout.LabelField(order.ToString() + ". " + item.Key);
            //    order++;
            //    if (GUILayout.Button("X", GUILayout.Width(30)))
            //    {
            //        EditorUtility.SetDirty(((AssetManager)target));
            //        ((AssetManager)target).prefabLifeTimes.Remove(item.Key);
            //        break;
            //    }
            //    EditorGUILayout.EndHorizontal();
            //}
        }

        //EditorGUILayout.EndVertical();
    }

    private void ImportSprites(string folderPath)
    {
        EditorUtility.SetDirty(((AssetManager)target));

        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
        {
            Debug.LogError("Invalid folder path");
            return;
        }

        string[] spritePaths = Directory.GetFiles(folderPath, "*.png");
        spritePaths = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);

        if (spritePaths.Length == 0)
        {
            Debug.LogWarning("No sprite assets found in the specified folder");
            return;
        }

        foreach (string spritePath in spritePaths)
        {
            var path = "Assets" + spritePath.Split("/Assets")[1];
            Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
            for (int i = 0; i < sprites.Length; i++)
            {
                Sprite sprite = sprites[i] as Sprite;
                if (sprite != null && !((AssetManager)target).sprites.ContainsKey(sprite.name))
                    ((AssetManager)target).sprites.Add(sprite.name, sprite);
            }
        }

        Debug.Log("Sprites imported successfully!");
    }

    private void ImportAudio(string folderPath)
    {
        EditorUtility.SetDirty(((AssetManager)target));

        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
        {
            Debug.LogError("Invalid folder path");
            return;
        }

        string[] mp3Path = Directory.GetFiles(folderPath, "*.mp3");
        string[] wavePath = Directory.GetFiles(folderPath, "*.wav");

        List<string> audioPaths = new List<string>();
        audioPaths.AddRange(mp3Path.ToList());
        audioPaths.AddRange(wavePath.ToList());

        if (audioPaths.Count == 0)
        {
            Debug.LogWarning("No audio assets found in the specified folder");
            return;
        }

        foreach (string audioPath in audioPaths)
        {
            var path = "Assets" + audioPath.Split("/Assets")[1];
            string audioName = Path.GetFileNameWithoutExtension(path);
            AudioClip audio = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (audio != null && !((AssetManager)target).audioClips.ContainsKey(audioName))
            {
                Debug.Log(audioName);
                ((AssetManager)target).audioClips.Add(audioName, audio);
            }
        }

        Debug.Log("AudioClips imported successfully!");
    }

    private void ImportPrefab(string folderPath)
    {
        EditorUtility.SetDirty(((AssetManager)target));

        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
        {
            Debug.LogError("Invalid folder path");
            return;
        }

        string[] prefabPaths = Directory.GetFiles(folderPath, "*.prefab");

        if (prefabPaths.Length == 0)
        {
            Debug.LogWarning("No prefab assets found in the specified folder");
            return;
        }

        foreach (string prefabPath in prefabPaths)
        {
            var path = "Assets" + prefabPath.Split("/Assets")[1];
            string prefabName = Path.GetFileNameWithoutExtension(path);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null && !((AssetManager)target).prefabs.ContainsKey(prefabName))
            {
                Debug.Log(prefabName);
                ((AssetManager)target).prefabs.Add(prefabName, prefab);
            }
        }

        Debug.Log("Prefabs imported successfully!");
    }

    

    private void ImportPrefabsLifetime(string folderPath)
    {
        EditorUtility.SetDirty(((AssetManager)target));

        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
        {
            Debug.LogError("Invalid folder path");
            return;
        }

        string[] prefabPaths = Directory.GetFiles(folderPath, "*.prefab");

        if (prefabPaths.Length == 0)
        {
            Debug.LogWarning("No prefab assets found in the specified folder");
            return;
        }

        foreach (string prefabPath in prefabPaths)
        {
            var path = "Assets" + prefabPath.Split("/Assets")[1];
            string prefabName = Path.GetFileNameWithoutExtension(path);
            //PrefabLifeTime prefabLifeTimes = AssetDatabase.LoadAssetAtPath<PrefabLifeTime>(path);

            //if (prefabLifeTimes != null && !((AssetManager)target).prefabLifeTimes.ContainsKey(prefabName))
            //{
            //    Debug.Log(prefabName);
            //    ((AssetManager)target).prefabLifeTimes.Add(prefabName, prefabLifeTimes);
            //}
        }

        Debug.Log("Prefabs LifeTime imported successfully!");
    }
}
