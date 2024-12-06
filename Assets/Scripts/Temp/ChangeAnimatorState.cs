using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimatorState : MonoBehaviour
{
    private static readonly int Run = Animator.StringToHash("run");

    public Animator animator;   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeAnimatorToRun()
    {
        animator.SetBool(Run,true);
    }
}
