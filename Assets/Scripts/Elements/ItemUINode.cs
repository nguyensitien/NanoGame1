using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemUINode : MonoBehaviour {
    [SerializeField]
    private Color colorUnlock, colorLock;
    [SerializeField]
    private Text txtLv = null;

    [SerializeField]
    private Image[] starList;

    [SerializeField]
    private GameObject starTran = null;

    [SerializeField]
    private CanvasGroup canvasGroup = null;
    private int idNode;
    private bool isUnlocked;
    public void Init(int idNode)
    {
        this.idNode = idNode;
        InitStar();
        txtLv.text = (idNode) + "";
    }

    private void InitStar()
    {
        if (idNode <= DataController.Instance.UserData.idNodeHighest)
        {
            //da danh roi hoac dang danh
            isUnlocked = true;
            canvasGroup.alpha = 1;
            starTran.SetActive(true);
            for (int i = 0; i < starList.Length; i++)
            {
                starList[i].color = colorLock;
            }
            List<UserDataNode> list = DataController.Instance.UserDataNodeList;
            if (idNode <= list.Count)
            {
                int length = list[idNode - 1].numStar;
                for (int i = 0; i < length; i++)
                {
                    starList[i].color = colorUnlock;
                }
            }
        }
        else
        {
            //chua danh
            isUnlocked = false;
            canvasGroup.alpha = 0.3f;
            starTran.SetActive(false);
        }
    }

    public void OnClickPlay()
    {
        if (isUnlocked == true)
        {
            GameObject mapPrefab = Resources.Load<GameObject>("Maps/Node"+idNode);
            if (mapPrefab)
            {
                ObjectDataController.Instance.idNodeFighting = idNode;
                ObjectDataController.Instance.nodeMapFighting = mapPrefab;
                SceneManager.LoadScene("Gameplay");
            }
            else
            {
                UIPopupController.Instance.notifycationController.Show("Bản đồ này chưa được tạo!");
            }
        }
        else
        {
            UIPopupController.Instance.notifycationController.Show("Bạn cần hoàn thành mục tiêu trước!");
        }
    }
}
