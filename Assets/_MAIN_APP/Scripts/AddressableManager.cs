using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyButtons;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace _MAIN_APP.Scripts
{
    public class AddressableManager : MonoBehaviour
    {
        [SerializeField, Space(10)] List<AddressableAssetEntry> referencesLoaded = new List<AddressableAssetEntry>();

        private readonly Dictionary<int, AsyncOperationHandle<object>[]> _itemToLoadAsync = new();

        public static AddressableManager Instance;
        private AsyncOperationHandle<object>[] _startedList;
        static readonly List<bool> _result = new List<bool>();

        private void Awake()
        {
            Addressables.InitializeAsync().Completed += InitComplete;

            if (Instance == null && Instance != this)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            referencesLoaded?.ForEach(x => x.aReference?.ReleaseAsset());
            Addressables.InitializeAsync().Completed -= InitComplete;
        }

        private void InitComplete(AsyncOperationHandle<IResourceLocator> obj)
        {
            // Perform actions since its completed loading
            Debug.Log("Ready to get addressable objects");
        }


        #region STATICS

        public static bool WasAssetDownloaded(AssetReference val)
        {
            var result = Addressables.GetDownloadSizeAsync(val).WaitForCompletion();
            return result <= 0;
        }

        public static bool WereAssetsDownloaded(AssetReference[] references)
        {
            _result.Clear();
            _result.AddRange(from t in references
                where Addressables.GetDownloadSizeAsync(t).WaitForCompletion() <= 0
                select true);

            return _result.Any(x => x);
        }

        #endregion

        #region LOAD

        public GameObject CreateInstance(AssetReference reference, Transform parent = null)
        {
            if (reference == null) return null;
            var exist = referencesLoaded.Find(x => x.aReference == reference);
            if (exist != null && exist.Go)
            {
                return referencesLoaded.Find(x => x.aReference == reference)?.Go;
            }
            else if (exist != null)
            {
                exist.Go = reference.InstantiateAsync(parent).WaitForCompletion();
                return exist.Go;
            }

            referencesLoaded.Add(new AddressableAssetEntry()
                { aReference = reference, Go = reference.InstantiateAsync(parent).WaitForCompletion() });

            return referencesLoaded.Last().Go;
        }

        public void LoadAsync<T>(AssetReference val, Action<T> callback)
        {
            // if the reference val exist in the reference list then return it
            if (referencesLoaded.Any(x => x.aReference == val))
            {
                callback?.Invoke(referencesLoaded.FirstOrDefault(x => x.aReference == val)!.aReference
                    .LoadAssetAsync<T>()
                    .WaitForCompletion());
                return;
            }

            // else add it to the list and then load it and return it
            referencesLoaded.Add(new AddressableAssetEntry() { aReference = val });
            callback?.Invoke(referencesLoaded.Last()!.aReference.LoadAssetAsync<T>().WaitForCompletion());
        }

        public AsyncOperationHandle<object> LoadAsync(AssetReference val)
        {
            if (referencesLoaded.All(x => x.aReference != val))
            {
                // else add it to the list and then load it and return it
                referencesLoaded.Add(new AddressableAssetEntry() { aReference = val });
            }

            // val.ReleaseAsset();
            return referencesLoaded.Last().aReference.LoadAssetAsync<object>();
        }

        public void DownloadList(int id, AssetReference[] references,
            Action<float> progress,
            Action onComplete,
            Action<string> onError = null)
        {
            try
            {
                if (_itemToLoadAsync.ContainsKey(id)) return;

                _startedList = new AsyncOperationHandle<object>[references.Length];
                // start download process
                for (int i = 0; i < references.Length; i++)
                {
                    if (!WasAssetDownloaded(references[i]))
                        _startedList[i] = LoadAsync(references[i]);
                }

                // store into list
                _itemToLoadAsync.Add(id, _startedList);

                StartCoroutine(DownloadStatus(id, progress, onComplete));
            }
            catch (Exception e)
            {
                onError?.Invoke(e.Message);
            }
        }

        private IEnumerator DownloadStatus(int id,
            Action<float> progress,
            Action onComplete)
        {
#if UNITY_EDITOR
            Debug.Log($"Looping through loading items.. current ID: {id}");
#endif
            for (int i = 0; i < _itemToLoadAsync[id].Count(); i++)
            {
                while (!_itemToLoadAsync[id][i].IsDone)
                {
                    var status = _itemToLoadAsync[id][i].GetDownloadStatus();
                    progress?.Invoke(status.Percent / _itemToLoadAsync[id].Count());
                    yield return null;
                }

#if UNITY_EDITOR
                Debug.Log($"{(i + 1)} Item done of {_itemToLoadAsync[id].Count()}");
#endif
            }

            progress?.Invoke(1);
            onComplete?.Invoke();
            _itemToLoadAsync.Remove(id);
        }

        #endregion

        #region UNLOAD

        public void ReleaseAsset([CanBeNull] AssetReference val)
        {
            if (val == null) return;
            var asset = referencesLoaded.FirstOrDefault(x => x.aReference == val);
            if (asset != null)
            {
                Debug.Log($"Addressable released {val.SubObjectName}");
                asset.aReference.ReleaseAsset();
                referencesLoaded.Remove(asset);
            }
        }

        public void ReleaseAsset(string assetId)
        {
            var asset = referencesLoaded.FirstOrDefault(x => x.aReference.AssetGUID.Equals(assetId));
            if (asset != null)
            {
                asset.aReference.ReleaseAsset();
                referencesLoaded.Remove(asset);
            }
        }

        public void ReleaseAssets()
        {
            referencesLoaded?.ForEach(x => x.aReference.ReleaseAsset());
            referencesLoaded?.Clear();
        }

        public void ReleaseAssetList(ref AssetReference[] references)
        {
            for (int i = 0; i < references.Length; i++)
            {
                ReleaseAsset(references[i]);
            }
        }

        public void UnLoadAndDestroy(AssetReference reference)
        {
            if (!referencesLoaded.Any(x => x.aReference == reference && x.Go != null)) return;

            var obj = referencesLoaded.Find(x => x.aReference == reference);
            Addressables.ReleaseInstance(obj.Go);
            obj.aReference.ReleaseAsset();
            referencesLoaded.Remove(obj);
        }

        public void UnLoadAndDestroy(int goInstanceID)
        {
            if (referencesLoaded.All(x => x.Go.GetInstanceID() != goInstanceID)) return;

            var obj = referencesLoaded.Find(x => x.Go.GetInstanceID() != goInstanceID);
            Addressables.ReleaseInstance(obj.Go);
            obj.aReference.ReleaseAsset();
            referencesLoaded.Remove(obj);
        }

        public void UnLoadAndDestroy(ref List<AssetReference> references)
        {
            references?.ForEach(reference =>
            {
                var obj = referencesLoaded.Find(x => x.aReference == reference);
                if (obj != null)
                {
                    if (obj?.Go) Addressables.ReleaseInstance(obj.Go);
                    obj?.aReference.ReleaseAsset();
                    referencesLoaded.Remove(obj);
                }
            });
        }

        #endregion

        #region Delete Cached and downloaded

        public void DeleteCacheAndDownload(AssetReference reference)
        {
            // Addressables.Release(reference);
            var status = Addressables.ClearDependencyCacheAsync(reference.RuntimeKey, true).WaitForCompletion();
            // Addressables.CleanBundleCache(new[] { reference.AssetGUID });
#if UNITY_EDITOR
            Debug.Log($"Cache cleared-- {status}");
#endif
        }

        #endregion

        #region DOWNLOAD ACTIONS

        /// <summary>
        /// Gets the reference provided and starts to download it from the server initially setup
        /// CALL <remarks>WasAssetDownloaded</remarks> to see if this step is required
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="onComplete"></param>
        /// <param name="progress"></param>
        /// <param name="onError"></param>
        public void DownloadAddressable(AssetReference reference,
            Action<float> progress,
            Action<AsyncOperationHandle<object>> onComplete,
            Action<string> onError = null)
        {
            try
            {
                StartCoroutine(ProgressStatus(reference, progress, onComplete));
            }
            catch (Exception e)
            {
                onError?.Invoke(e.Message);
            }
        }


        private IEnumerator ProgressStatus(AssetReference reference,
            Action<float> progress,
            Action<AsyncOperationHandle<object>> onComplete)
        {
            AsyncOperationHandle<object> dn = LoadAsync(reference);

            dn.Completed += onComplete;

            while (!dn.IsDone)
            {
                var status = dn.GetDownloadStatus();
                progress?.Invoke(status.Percent);
                // loadInfoTxt.text = $"{(int)(progress * 100)}%";
                yield return null;
            }

#if UNITY_EDITOR
            Debug.Log("Releasing downloaded asset");
            Debug.Log("This is performed to prevent hoggin of memory while asset is not in use");
#endif

            ReleaseAsset(reference);
            dn.Completed -= onComplete;
        }

        #endregion

#if UNITY_EDITOR
        [Button]
        public void ReleaseReference(int index)
        {
            if (referencesLoaded[index].Go)
                UnLoadAndDestroy(referencesLoaded[index].aReference);
            else
                ReleaseAsset(referencesLoaded[index].aReference);
        }

#endif
    }


    [Serializable]
    public class AddressableAssetEntry
    {
        public AssetReference aReference;
        public GameObject Go;
        public AsyncOperationHandle<object> Status;
    }
}