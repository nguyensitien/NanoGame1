using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataController : SingletonDontDestroy<DataController> {

    private UserData userData;
    public UserData UserData {
        get {
            if (userData == null)
            {
                userData = LoadUserData();
            }
            if (userData == null)
            {
                userData = new UserData();
            }
            return userData;
        }
    }

    private UserData LoadUserData()
    {
        string path = Application.persistentDataPath + "/UserData.dat";
        FileStream file;
        if (File.Exists(path)) file = File.OpenRead(path);
        else return null;
        BinaryFormatter bi = new BinaryFormatter();
        UserData data = (UserData)bi.Deserialize(file);
        file.Close();
        return data;
    }

    private void SaveUserData()
    {
        string path = Application.persistentDataPath + "/UserData.dat";
        FileStream file;
        if (File.Exists(path)) file = File.OpenWrite(path);
        else file = File.Create(path);
        BinaryFormatter bi = new BinaryFormatter();
        bi.Serialize(file,UserData);
        file.Close();
    }

    private List<UserDataNode> userDataNodeList;
    public List<UserDataNode> UserDataNodeList
    {
        get {
            if (userDataNodeList == null)
            {
                userDataNodeList = LoadUserDataNodeList();
                
            }
            if (userDataNodeList == null)
            {
                userDataNodeList = new List<UserDataNode>();
                userDataNodeList.Add(new UserDataNode(1));
            }
            int origin = userDataNodeList.Count;
            int max = ObjectDataController.Instance.DataBaseMap.nodeList.Length;
            for (int i = origin; i <= UserData.idNodeHighest; i++)
            {
                if (i <= max - 1)
                {
                    userDataNodeList.Add(new UserDataNode(i));
                }
                else
                {
                    return userDataNodeList;
                }
                
            }
            return userDataNodeList;
        }
    }

    private List<UserDataNode> LoadUserDataNodeList()
    {
        string path = Application.persistentDataPath + "/UserDataNode.dat";
        FileStream file;
        if (File.Exists(path)) file = File.OpenRead(path);
        else return null;
        BinaryFormatter bi = new BinaryFormatter();
        List<UserDataNode> result = (List<UserDataNode>)bi.Deserialize(file);
        file.Close();
        return result;
    }

    private void SaveUserDataNodeList()
    {
        string path = Application.persistentDataPath + "/UserDataNode.dat";
        FileStream file;
        if (File.Exists(path)) file = File.OpenWrite(path);
        else file = File.Create(path);
        BinaryFormatter bi = new BinaryFormatter();
        bi.Serialize(file,UserDataNodeList);
        file.Close();
    }
    private void OnApplicationQuit()
    {
        SaveUserData();
        SaveUserDataNodeList();
    }
}
