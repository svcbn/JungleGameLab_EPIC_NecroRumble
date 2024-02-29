using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace LOONACIA.Unity.Managers
{
    public enum SceneType
    {
        Unknown,
        Title,
        Tutorial,
        Game,
    }
    
    public static class SceneManagerEx
    {
        public static Action<Scene> OnSceneChanging;
        
        private static BaseScene _currentScene;
        public static BaseScene CurrentScene 
        {
            get
            {
                if (_currentScene == null)
                {
                    _currentScene = GameObject.FindObjectOfType<BaseScene>();
                }
                return _currentScene;
            } 
        }
        public static void LoadScene(SceneType sceneType)
        {
            CurrentScene.Clear();
            
            LoadScene(GetSceneName(sceneType));
        }
        static string GetSceneName(SceneType sceneType_)
        {
            string name = System.Enum.GetName(typeof(SceneType), sceneType_);
            return name;
        }

        public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            OnSceneChanging?.Invoke(currentScene);
            
            SceneManager.LoadScene(sceneName, mode);
        }

        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            OnSceneChanging?.Invoke(currentScene);

            return SceneManager.LoadSceneAsync(sceneName, mode);
        }

        public static void ReloadScene()
        {
            //DOTween.KillAll();

            LoadScene(SceneManager.GetActiveScene().name);
        }

    }
}