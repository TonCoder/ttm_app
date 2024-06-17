using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _MAIN_APP.Scripts
{
    public class AddressableManager : MonoBehaviour
    {
        [SerializeField, Space(10)] List<AddressableAssetEntry> availableReferences = new List<AddressableAssetEntry>();

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
            availableReferences?.ForEach(x => x.aReference?.ReleaseAsset());
            Addressables.InitializeAsync().Completed -= InitComplete;
        }

        private void InitComplete(AsyncOperationHandle<IResourceLocator> obj)
        {
            // Perform actions since its completed loading
            Debug.Log("Ready to get addressable objects");
        }

        #region MAIN FUNCTIONS

        public GameObject CreateInstance(AssetReference reference, Transform parent = null)
        {
            if (reference == null) return null;
            var exist = availableReferences.Find(x => x.aReference == reference);
            if (exist != null && exist.Go)
            {
                return availableReferences.Find(x => x.aReference == reference)?.Go;
            }
            else if (exist != null)
            {
                exist.Go = reference.InstantiateAsync(parent).WaitForCompletion();
                return exist.Go;
            }

            availableReferences.Add(new AddressableAssetEntry()
                { aReference = reference, Go = reference.InstantiateAsync(parent).WaitForCompletion() });

            return availableReferences.Last().Go;
        }

        public void UnLoadAndDestroy(AssetReference reference)
        {
            if (!availableReferences.Any(x => x.aReference == reference && x.Go != null)) return;

            var obj = availableReferences.Find(x => x.aReference == reference);
            Addressables.ReleaseInstance(obj.Go);
            obj.aReference.ReleaseAsset();
            availableReferences.Remove(obj);
        }

        public void UnLoadAndDestroy(int goInstanceID)
        {
            if (availableReferences.All(x => x.Go.GetInstanceID() != goInstanceID)) return;

            var obj = availableReferences.Find(x => x.Go.GetInstanceID() != goInstanceID);
            Addressables.ReleaseInstance(obj.Go);
            obj.aReference.ReleaseAsset();
            availableReferences.Remove(obj);
        }

        public void UnLoadAndDestroy(ref List<AssetReference> references)
        {
            references?.ForEach(reference =>
            {
                var obj = availableReferences.Find(x => x.aReference == reference);
                if (obj != null)
                {
                    if (obj?.Go) Addressables.ReleaseInstance(obj.Go);
                    obj?.aReference.ReleaseAsset();
                    availableReferences.Remove(obj);
                }
            });
        }

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

        public void LoadAsync<T>(AssetReference val, Action<T> callback)
        {
            // if the reference val exist in the reference list then return it
            if (availableReferences.Any(x => x.aReference == val))
            {
                callback?.Invoke(availableReferences.FirstOrDefault(x => x.aReference == val)!.aReference
                    .LoadAssetAsync<T>()
                    .WaitForCompletion());
                return;
            }

            // else add it to the list and then load it and return it
            availableReferences.Add(new AddressableAssetEntry() { aReference = val });
            callback?.Invoke(availableReferences.Last()!.aReference.LoadAssetAsync<T>().WaitForCompletion());
        }

        public AsyncOperationHandle<object> LoadAsync(AssetReference val)
        {
            // else add it to the list and then load it and return it
            availableReferences.Add(new AddressableAssetEntry() { aReference = val });
            return availableReferences.Last().aReference.LoadAssetAsync<object>();
        }

        public void ReleaseAsset([CanBeNull] AssetReference val)
        {
            if (val == null) return;
            var asset = availableReferences.FirstOrDefault(x => x.aReference == val);
            if (asset != null)
            {
                Debug.Log($"Addressable released {val.SubObjectName}");
                asset.aReference.ReleaseAsset();
                availableReferences.Remove(asset);
            }
        }

        public void ReleaseAsset(string assetId)
        {
            var asset = availableReferences.FirstOrDefault(x => x.aReference.AssetGUID.Equals(assetId));
            if (asset != null)
            {
                asset.aReference.ReleaseAsset();
                availableReferences.Remove(asset);
            }
        }

        public void ReleaseAssets()
        {
            availableReferences?.ForEach(x => x.aReference.ReleaseAsset());
            availableReferences?.Clear();
        }

        #endregion

        #region Delete Cache and downloaded

        public void DeleteCacheAndDownload(AssetReference reference)
        {
            // Addressables.Release(reference);
            var status = Addressables.ClearDependencyCacheAsync(reference.RuntimeKey, true).WaitForCompletion();

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

            Debug.Log("Releasing downloaded asset");
            Debug.Log("This is performed to prevent hoggin of memory while asset is not in use");
            ReleaseAsset(reference);
            dn.Completed -= onComplete;
        }

        #endregion

        public void DownloadAddressableList(int id, AssetReference[] references,
            Action<float> progress,
            Action onComplete,
            Action<string> onError = null)
        {
            try
            {
                if (_itemToLoadAsync.ContainsKey(id)) return;

                Debug.Log("Downloading list...");
                _startedList = new AsyncOperationHandle<object>[] { };
                // start download process
                for (int i = 0; i < references.Length; i++)
                {
                    _startedList[i] = LoadAsync(references[i]);
                }

                // store into list
                _itemToLoadAsync.Add(id, _startedList);

                Debug.Log("Getting status...");

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
            Debug.Log($"Looping through loading items.. current ID: {id}");

            for (int i = 0; i < _itemToLoadAsync[id].Count(); i++)
            {
                while (!_itemToLoadAsync[id][i].IsDone)
                {
                    var status = _itemToLoadAsync[id][i].GetDownloadStatus();
                    progress?.Invoke(status.Percent / _itemToLoadAsync[id].Count());
                    yield return null;
                }

                Debug.Log($"{(i + 1)} Item done of {_itemToLoadAsync[id].Count()}");
            }

            onComplete?.Invoke();
            Debug.Log("Releasing downloaded asset");
            Debug.Log("This is performed to prevent hoggin of memory while asset is not in use");
            foreach (AsyncOperationHandle<object> vHandle in _itemToLoadAsync[id])
            {
                ReleaseAsset((AssetReference)vHandle.Result);
            }

            _itemToLoadAsync.Remove(id);
        }
    }

    [Serializable]
    public class AddressableAssetEntry
    {
        public AssetReference aReference;
        public GameObject Go;
        public AsyncOperationHandle<object> Status;
    }
}