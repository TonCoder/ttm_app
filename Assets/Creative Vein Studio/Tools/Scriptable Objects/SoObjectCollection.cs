using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectList", menuName = "CVeinStudio/List/Objects")]
public class SoObjectCollection : ScriptableObject
{
   [SerializeField] private List<GameObject> list;
}
