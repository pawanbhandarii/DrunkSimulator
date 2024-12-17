using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public class ChangeAnimatorState : MonoBehaviour
{
    private static readonly int Run = Animator.StringToHash("run");

    public Animator animator;
    public BehaviorGraphAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        //behaviourGraph = GetComponent<Behaviour>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeAnimatorToRun()
    {
        animator.SetBool(Run,true);
    }
    public void enableGraph()
    {
       // behaviourGraph.enabled = true;

    }
    public void enableAgent()
    {
        agent.enabled = true;
    }
}
