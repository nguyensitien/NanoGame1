using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[System.Serializable]
public class UserData {

    public int idNodeHighest;
    public int attemptNodeCur;
    public UserData(int idNodeHighest = 1,int attackNodeCur = 0)
    {
        this.idNodeHighest = idNodeHighest;
        this.attemptNodeCur = attackNodeCur;
    }

    
}
