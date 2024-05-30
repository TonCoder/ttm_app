using System;
using UnityEngine;

namespace Creative_Vein_Studio.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ScenePathInfoAttribute : PropertyAttribute
    {
        public ScenePathInfoAttribute()
        {
        }
    }
}