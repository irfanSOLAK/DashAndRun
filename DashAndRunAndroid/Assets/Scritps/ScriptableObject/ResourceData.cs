using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData", menuName = "ScriptableObjects/ResourceData", order = 1)]
public class ResourceData : ScriptableObject
{
    public List<PrefabData> prefabs=new List<PrefabData>(); // Prefablar� i�eren liste
}
// PrefabData s�n�f�
[System.Serializable]
public class PrefabData
{
    public int id;
    public GameObject prefab;
    public string description;
}