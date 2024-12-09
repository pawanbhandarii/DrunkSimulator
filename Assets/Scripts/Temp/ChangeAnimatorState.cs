using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimatorState : MonoBehaviour
{
    private static readonly int Run = Animator.StringToHash("run");

    public Animator animator;
    public GameObject some;
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
        behaviourGraph.enabled = true;

    }
}
