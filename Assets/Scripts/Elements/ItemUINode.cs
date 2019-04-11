using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemUINode : MonoBehaviour {
    [SerializeField]
    private Sprite starUnlock = null, starLock = null;

    [SerializeField]
    private Image imgBg = null;
    [SerializeField]
    private Sprite spritesBgLock = null,spriteBgUnlock = null,spriteBgChosing = null;
    [SerializeField]
    private Text txtLv = null;
    [SerializeField]
    private Color colorLvLock , colorLvUnlock , colorLvChosing ;
    [SerializeField]
    private Image[] starList;

    [SerializeField]
    private GameObject starTran = null;

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
            if(idNode == DataController.Instance.UserData.idNodeHighest)
            {
                imgBg.sprite = spriteBgChosing;
                txtLv.color = colorLvChosing;
            }
            else
            {
                imgBg.sprite = spriteBgUnlock;
                txtLv.color = colorLvUnlock;

            }
            starTran.SetActive(true);
            for (int i = 0; i < starList.Length; i++)
            {
                starList[i].sprite = starLock;
                starList[i].SetNativeSize();
            }
            List<UserDataNode> list = DataController.Instance.UserDataNodeList;
            if (idNode <= list.Count)
            {
                int length = list[idNode - 1].numStar;
                for (int i = 0; i < length; i++)
                {
                    starList[i].sprite = starUnlock;
                    starList[i].SetNativeSize();
                }
            }
        }

        else
        {
            //chua danh
            isUnlocked = false;
            starTran.SetActive(false);
            imgBg.sprite = spritesBgLock;
            txtLv.color = colorLvLock;
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
                //UIPopupController.Instance.notifycationController.Show("Bản đồ này chưa được tạo!");
            }
        }
        else
        {
            //UIPopupController.Instance.notifycationController.Show("Bạn cần hoàn thành mục tiêu trước!");
        }
    }
}
