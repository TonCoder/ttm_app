using System;
using System.Collections;
using System.Collections.Generic;
using Creative_Vein_Studio.PropertyAttributes;
using CVStudio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Creative_Vein_Studio.Tools
{
    public class SubSceneLoader : MonoBehaviour
    {
        [SerializeField] private bool loadSubScenesOnStart;
        [SerializeField, ScenePathInfo] private List<string> coreScenes = new List<string>();
        [SerializeField, ScenePathInfo] private List<string> subScenesToLoad = new List<string>();

        [SerializeField] private UEvents.EFloat onLoadProgress;

        private readonly List<AsyncOperation> _scenesToLoadAsync = new List<AsyncOperation>();
        private float _loadProgress = 0;

        private void Awake()
        {
            RemoveExistingScenes(ref coreScenes);
            for (int i = 0; i < coreScenes.Count; i++)
            {
                SceneManager.LoadSceneAsync((coreScenes[i]), LoadSceneMode.Additive);
            }

            if (loadSubScenesOnStart)
            {
                RemoveExistingScenes(ref subScenesToLoad);
                LoadSubScenes();
            }
        }

        private void RemoveExistingScenes(ref List<string> col)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var sName = SceneManager.GetSceneAt(i);
                if (col.Contains(sName.path))
                {
                    col.Remove(sName.path);
                }
            }
        }

        public void LoadSubScenes()
        {
            for (int i = 0; i < subScenesToLoad.Count; i++)
            {
                _scenesToLoadAsync.Add(
                    SceneManager.LoadSceneAsync(subScenesToLoad[i], LoadSceneMode.Additive));
            }

            if (onLoadProgress != null)
            {
                StartCoroutine(LoadSceneStatus());
            }
        }

        public void UnloadSubScene(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.None);
        }

        private IEnumerator LoadSceneStatus()
        {
            _loadProgress = 0;
            for (int i = 0; i < _scenesToLoadAsync.Count; i++)
            {
                while (!_scenesToLoadAsync[i].isDone)
                {
                    _loadProgress += _scenesToLoadAsync[i].progress;
                    onLoadProgress?.Invoke(_loadProgress / _scenesToLoadAsync.Count);
                    yield return null;
                }
            }

            _scenesToLoadAsync.Clear();
        }
    }
}