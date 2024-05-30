using System;
using UnityEngine;

namespace Creative_Vein_Studio.Tools.Scriptable_Objects.Brokers
{
    [CreateAssetMenu(fileName = "SceneBroker", menuName = "CVeinStudio/Brokers/SceneManager")]
    public class SoSceneManagerBroker : ScriptableObject
    {
        public Action<string> OnLoadSceneAsync;
        public Action OnLoadLobby;
        public Action OnLoadNextScene;
        public Action<string> OnLoadScene;
        public Action OnQuitGame;

        public void TriggerLoadSceneAsync(string val) => OnLoadSceneAsync?.Invoke(val);
        public void TriggerLoadScene(string val) => OnLoadScene?.Invoke(val);
        public void TriggerLoadNextScene() => OnLoadNextScene?.Invoke();
        public void TriggerLoadLobby() => OnLoadLobby?.Invoke();
        public void TriggerQuitGame() => OnQuitGame?.Invoke();
    }
}