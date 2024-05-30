using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Creative_Vein_Studio.Tools.Scriptable_Objects
{
    /// <summary>
    /// This ScriptableObject (S.O) is used to hold a list of scripts that register to it for ease of access from other classes
    /// Simply create the S.O asset in your project and then assign it as a property to any script that you want to register.
    /// Then access the Register and Unregister in the Start and OnDestroy of the class using it
    /// </summary>
    [CreateAssetMenu(fileName = "ReferenceManager", menuName = "CVeinStudio/ReferenceManager")]
    public class SoReferenceManager : ScriptableObject
    {
        [SerializeField] List<ContentInfo> gameContent = new List<ContentInfo>();

        private void OnDestroy()
        {
            gameContent.Clear();
        }

        /// <summary>
        /// Use this function to register the class you want to store; Run in the Awake or Start function.
        /// example of use within given class:
        ///<code>
        /// [SerializeField] private SoReferenceManager _soRefManager; -- SET IN INSPECTOR
        /// private Start(){
        ///   ver self = this;
        ///   _soRefManager.Register(ref self, self.GetInstanceID());
        /// }
        ///</code>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        public void Register<T>(ref T obj, in int id)
        {
            if (ReturnMatch(id) == null)
                gameContent.Add(new ContentInfo() { id = id, obj = obj, type = typeof(T) });
        }

        /// <summary>
        /// Use this function to UN-register the class you previously stored by passing its instance ID; Run in the OnDestroy function.
        /// Fore more on the Instance ID,
        /// see: <see langword="https://docs.unity3d.com/ScriptReference/Object.GetInstanceID.html" href="https://docs.unity3d.com/ScriptReference/Object.GetInstanceID.html"/>
        /// <example>Example of use within given class:
        ///<code>
        /// private OnDestroy(){
        ///   _soRefManager.UnRegister(self.GetInstanceID());
        /// }
        ///</code>
        /// </example>
        /// </summary>
        /// <param name="id"></param>
        public void UnRegister(in int id)
        {
            var obj = ReturnMatch(id);
            if (obj != null)
                gameContent.Remove(obj);
        }

        /// <summary>
        /// This function will allow the user to retrieve all registered objects of the provided type.
        ///<example>
        /// Example of use:
        ///<code>
        /// private void RandomFunction(){
        ///     var EnemyControllerList = referenceManager.GetObjectOfType &lt;EnemyController&gt;();'
        ///     //... do whatever with values
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="type"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>IEnumerable&lt;T&gt;</returns>
        public IEnumerable<T> GetObjectOfType<T>()
        {
            if (gameContent.Count <= 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"The gamecontent collection is EMPTY");
#endif
                return null;
            }

            if (!gameContent.Any(x => x.type == typeof(T)))
            {
#if UNITY_EDITOR
                Debug.LogError($"The Type {typeof(T)} is not present in the list");
#endif
                return null;
            }

            return gameContent.Where(x => x.type == typeof(T)).Select(c => c.obj).Cast<T>();
        }

        public ContentInfo GetObjectByID(int id)
        {
            return gameContent.Find(x => x.id == id);
        }

        private ContentInfo ReturnMatch(in int id)
        {
            for (int i = 0; i < gameContent.Count; i++)
            {
                if (gameContent[i].id == id)
                {
                    return gameContent[i];
                }
            }

            return null;
        }

        [Serializable]
        public class ContentInfo
        {
            public int id;
            public Type type;
            public object obj;
        }
    }
}