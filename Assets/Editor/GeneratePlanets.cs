using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

class MyWindow : EditorWindow
{
    [MenuItem("My Project/Generate Planet Prefabs")]
    public static void GeneratePlanetPrefabs()
    {
        ArrayList spriteArr = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/Sprites/Planets");

        int count = 1;
        foreach (string filePath in fileEntries)
        {
            if (!filePath.ToLower().Contains("meta"))
            {
                int assetPathIndex = filePath.IndexOf("Assets");
                string localPath = filePath.Substring(assetPathIndex);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(localPath);
                GameObject prefab = PrefabUtility.CreatePrefab("Assets/Editor/Output/planet_" + count.ToString("D3") + ".prefab", GameObject.Find("CirclePlanet"), ReplacePrefabOptions.ConnectToPrefab);
                prefab.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
                count++;
            }
            
        }
    }

    void OnGUI()
    {
        // The actual window code goes here
    }
}
