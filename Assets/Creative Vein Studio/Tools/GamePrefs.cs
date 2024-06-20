using System;
using Newtonsoft.Json;
using UnityEngine;

namespace HeistEscape
{
    [System.Serializable]
    public class GamePrefs : MonoBehaviour
    {
        private const string _MUSICLEVEL = "Music_Level";
        private const string _UserPrefs = "UPrefs";

        //------------------------------------
        //  Setters
        //------------------------------------
        public static void SavePrefs<T>(T obj)
        {
            var scriptable = JsonConvert.SerializeObject(obj);
#if UNITY_EDITOR
            Debug.Log($"Serialized Object NEWTONSOFT: {scriptable}");
#endif
            PlayerPrefs.SetString(_UserPrefs, scriptable);
            PlayerPrefs.Save();
        }

        //------------------------------------
        // Getters
        //------------------------------------
        public static T GetPrefs<T>()
        {
            var data = PlayerPrefs.GetString(_UserPrefs);
            if (string.IsNullOrEmpty(data)) return default;
            return JsonConvert.DeserializeObject<T>(data);
        }

        //------------------------------------
        // DELETE
        //------------------------------------
        public static void DeleteData()
        {
            PlayerPrefs.DeleteKey(_UserPrefs);
        }

        //------------------------------------
        // Saving changes
        //------------------------------------
        public static void SavePrefChanges()
        {
            PlayerPrefs.Save();
        }
    }
}