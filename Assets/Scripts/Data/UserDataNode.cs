using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class UserDataNode  {

    public int idNode;
    public int numStar;
    public UserDataNode(int idNode,int numStar)
    {
        this.idNode = idNode;
        this.numStar = numStar;
    }
    
}
