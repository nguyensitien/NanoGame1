using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDataController : SingletonDontDestroy<ObjectDataController> {
    private const string IdNode = "IdNode";
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
    private int idNodeFighting = -1;
    public int IdNodeFighting
    {
        get
        {
            if (idNodeFighting == -1)
            {
                idNodeFighting = PlayerPrefs.GetInt(IdNode,1);
                if (idNodeFighting > DataController.Instance.UserDataNodeList.Count)
                {
                    idNodeFighting = DataController.Instance.UserDataNodeList.Count - 1;
                }
            }
            return idNodeFighting;
        }
        set
        {
            idNodeFighting = value;
            
            PlayerPrefs.SetInt(IdNode, idNodeFighting);
        }
    }
    public GameObject nodeMapFighting;

    private ConfigGame _configGame;
    public ConfigGame ConfigGame
    {
        get
        {
            if (_configGame == null)
            {
                _configGame = Resources.Load<ConfigGame>("ConfigGame");
            }
            return _configGame;
        }
    }
    
}
