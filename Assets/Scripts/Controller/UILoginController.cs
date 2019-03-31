using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoginController : Singleton<UILoginController> {

    [SerializeField]
    private CanvasGroup[] splashList;
    [SerializeField]
    private float timeTween;
    [SerializeField]
    private GameObject logoGame;
    private void Start()
    {
        Screen.SetResolution(1280,1920,true);
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 4;

        for (int i = 0; i < splashList.Length; i++)
        {
            int index = i;
            splashList[i].alpha = 0;
            DOTween.To(() => splashList[index].alpha, (x) => splashList[index].alpha = x, 1, timeTween).SetEase(Ease.Linear).SetDelay(index * timeTween*2.5f + 0.5f).OnComplete(()=> {
                DOTween.To(() => splashList[index].alpha, (x) => splashList[index].alpha = x, 0, timeTween).SetEase(Ease.Linear).OnComplete(()=> {
                    if (index == splashList.Length - 1)
                    {
                        Debug.Log("Complete");
                        logoGame.SetActive(true);
                    }
                });
            });
            

        }
    }

    public void OnClickPlay()
    {
        SceneManager.LoadScene("Lobby");
    }
}
