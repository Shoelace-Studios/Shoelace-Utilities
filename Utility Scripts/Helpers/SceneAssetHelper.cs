#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShoelaceStudios.Utilities.Helpers
{
    public static class SceneAssetHelper
    {
        public static string SceneDataFolder => "Assets/Scenes/SceneData/";

        public static string GetSceneFolder()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            return Path.Combine(SceneDataFolder, sceneName);
        }

        public static T GetOrCreateAsset<T>(string name) where T : ScriptableObject
        {
            string folder = GetSceneFolder();
            EnsureFolderExists(folder);

            string path = Path.Combine(folder, name + ".asset");
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"Created {typeof(T).Name} for scene at {path}");
            }

            return asset;
        }

        private static void EnsureFolderExists(string folderPath)
        {
            folderPath = folderPath.Replace("\\", "/"); // normalize for Unity
            if (AssetDatabase.IsValidFolder(folderPath)) return;

            string parent = Path.GetDirectoryName(folderPath).Replace("\\", "/");
            if (!AssetDatabase.IsValidFolder(parent))
                EnsureFolderExists(parent);

            string newFolderName = Path.GetFileName(folderPath);
            AssetDatabase.CreateFolder(parent, newFolderName);
        }
    }
}
#endif
