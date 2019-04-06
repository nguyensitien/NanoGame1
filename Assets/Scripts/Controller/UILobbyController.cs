using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILobbyController : MonoBehaviour {

    [SerializeField]
    private Transform contentItem = null;

    [SerializeField]
    private GameObject itemUiNodePrefab = null;

    [SerializeField]
    private RectTransform logoScreen = null, campaignScreen = null;

    private void Start()
    {
        //List<UserDataNode> list = DataController.Instance.UserDataNodeList;
        //for (int i = 0; i < list.Count; i++)
        //{
        //    Debug.Log(list[i].idNode+":"+list[i].numStar);
        //}
        //DataController.Instance.UserData.idNodeHighest = 5;
        //DataController.Instance.UserDataNodeList[0].numStar = 3;
        //DataController.Instance.UserDataNodeList[1].numStar = 2;
        //DataController.Instance.UserDataNodeList[2].numStar = 1;
        //Debug.Log("UserDataNodeList:"+Application.persistentDataPath);
        Init();
    }
    private void Init()
    {
        DataBaseMap dataMap = ObjectDataController.Instance.DataBaseMap;
        for (int i = 0; i < dataMap.nodeList.Length; i++)
        {
            GameObject obj = Instantiate(itemUiNodePrefab,contentItem);
            int idNode = i + 1;
            ItemUINode itemUINode = obj.GetComponent<ItemUINode>();
            itemUINode.Init(idNode);
        }
    }

    public void OnBtnLogin()
    {
        SceneManager.LoadScene("Login");
    }

    public void GotoLogoScreen()
    {
        logoScreen.gameObject.SetActive(true);
        campaignScreen.gameObject.SetActive(false);
    }

    public void GotoCampaignScreen()
    {
        campaignScreen.gameObject.SetActive(true);
        logoScreen.gameObject.SetActive(false);
    }
}
