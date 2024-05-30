using System.Collections;
using Creative_Vein_Studio.Tools.Scriptable_Objects.Brokers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Creative_Vein_Studio.Tools
{
    public class SceneManagement : MonoBehaviour
    {
        public bool makeSingleton = false;
        public static SceneManagement instance;
        public SoSceneManagerBroker sceneBroker;

        public string LobbyScene = "01_Lobby";
        public int lobbyIndex = 0;

        public Scene GetActiveScene => SceneManager.GetActiveScene();

        private int _currentSceneIndex;

        private void Awake()
        {
            if (makeSingleton) Singleton();
            if (sceneBroker != null)
            {
                sceneBroker.OnLoadSceneAsync += LoadSceneAsync;
                sceneBroker.OnLoadLobby += LoadLobby;
                sceneBroker.OnLoadScene += LoadScene;
                sceneBroker.OnQuitGame += QuitGame;
                sceneBroker.OnLoadNextScene += LoadNextScene;
            }

            lobbyIndex = SceneManager.GetSceneByName(LobbyScene).buildIndex;
        }

        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadYourAsyncScene(sceneName));
        }

        public void LoadNextScene()
        {
            SceneManager.LoadScene(_currentSceneIndex + 1);
        }

        IEnumerator LoadYourAsyncScene(string _sceneName)
        {
            // The Application loads the Scene in the background as the current Scene runs.
            // This is particularly good for creating loading screens.
            // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
            // a sceneBuildIndex of 1 as shown in Build Settings.

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneName);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        private void LoadLobby()
        {
            SceneManager.LoadScene(lobbyIndex);
        }

        public void LoadScene(string sceneName)
        {
            _currentSceneIndex = SceneManager.GetSceneByName(LobbyScene).buildIndex;
            SceneManager.LoadScene(sceneName);
        }

        public void LoadScene(int index)
        {
            _currentSceneIndex = index;
            SceneManager.LoadScene(index);
        }

        public void LoadFirstIndexScene(int index = 0)
        {
            _currentSceneIndex = index;
            SceneManager.LoadScene(index);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void Singleton()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}