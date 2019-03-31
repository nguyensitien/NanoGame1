using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDataController : SingletonDontDestroy<ObjectDataController> {

    private DataBaseMap _dataBaseMap;
    public DataBaseMap DataBaseMap
    {
        get {
            if (_dataBaseMap == null)
            {
                _dataBaseMap = Resources.Load<DataBaseMap>("DataMap/DataBaseMap");
            }
            return _dataBaseMap;
        }
    }

    public int idNodeFighting = -1;
    public GameObject nodeMapFighting;
}
