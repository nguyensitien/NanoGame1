using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DataBaseMap",menuName = "Data/DataBaseMap")]
public class DataBaseMap : ScriptableObject {

    public DataBaseNode[] nodeList;
}


[System.Serializable]
public class DataBaseNode  {
    public int numSketch = 3;
}