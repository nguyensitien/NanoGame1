using UnityEngine;
using System.Collections;

public class DestroyObjectInTime : MonoBehaviour
{
    public float time;
    public bool isAnim;
    public bool isDestroy;
    // Use this for initialization
    private WaitForSeconds wait;
    private void Awake()
    {
        wait = new WaitForSeconds(time);
    }
    private Coroutine corou;
    void OnEnable()
    {
        if (!isAnim)
        {
            if (corou != null) StopCoroutine(corou);
            corou = this.StartCoroutine(DestroyGameObject());
        }
    }

    public IEnumerator DestroyGameObject()
    {
        yield return wait;
        if (isDestroy)
        {
            Destroy(gameObject);
        }
        else
        {
            //gameObject.SetActive(false);
            SmartPool.Instance.Despawn(gameObject);
        }
    }


}
