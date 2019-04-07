using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumLoopAtEnd : StateMachineBehaviour {
    [SerializeField]
    private string nameParam;
    [SerializeField]
    private bool valueParam;
    [SerializeField]
    private float duration;
    private float timeCur;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        timeCur = 0;
        animator.SetBool(nameParam, false);
    }


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        timeCur += Time.deltaTime;
        if(timeCur >= duration)
        {
            animator.SetBool(nameParam,valueParam);
            timeCur = 0;
        }
    }
}
