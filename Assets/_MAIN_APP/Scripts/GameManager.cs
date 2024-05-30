using System;
using _MAIN_APP.Scripts.ScriptableObjects;
using CreativeVeinStudio.Simple_Dialogue_System.Attributes;
using UnityEngine;

namespace _MAIN_APP.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField, ExposeSo] internal SoBiomeList ownedBiomes;
        [SerializeField, ExposeSo] internal SoBiomeList availableBiomes;
        [SerializeField] private bool registered;

        [FieldTitle("Activated items")]
        [SerializeField, ExposeSo]
        internal SoBiomeDetails _activeBiome;
        [SerializeField, ExposeSo] internal SoAudioSceneDetails _activeScene;

        private void Awake()
        {
            Singleton();
        }

        private void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            if (AddressableManager.Instance != null) AddressableManager.Instance.ReleaseAssets();
        }

        // initiate app
        private void Init()
        {
            ShowNews();

            //TODO get user 
            GetUserData();
        }

        // get user info
        private void GetUserData()
        {
            // check to see if User's device is registered
            var deviceId = SystemInfo.deviceUniqueIdentifier;

            // get user data if so
            //TODO perform api request qith device unique ID to see if registered and has data
            if (registered)
            {
                GetActiveBiomes();
            }
            else
            {
                //TODO - have user register and use device id for validation
                // have user register                
            }
        }

        // get active biomes
        private void GetActiveBiomes()
        {
            // check to see what Biome has been downloaded and add it to the owned list 
            foreach (var biome in availableBiomes.Biomes)
            {
                if (biome.IsActive)
                    ownedBiomes.AddBiome(biome);
            }
        }

        // show new messages
        private void ShowNews()
        {
            // get new news info from URL
        }

        private void Singleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}