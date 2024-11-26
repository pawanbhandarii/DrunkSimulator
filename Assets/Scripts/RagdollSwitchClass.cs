using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RagdollSwitchClass : MonoBehaviour
{
    [SerializeField] Collider myCollider;
    [SerializeField] float respawnTime = 3.0f;
    Rigidbody[] rigidbodies;
    bool bIsRagdoll = false;
    string ragdollTag = "RagdollTrigger";
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        ToggleRagdoll(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ToggleRagdoll( bool bisAnimating)
    {
        bIsRagdoll = !bisAnimating;
        myCollider.enabled = bisAnimating;
        foreach(Rigidbody ragdollBone in rigidbodies)
        {
            ragdollBone.gameObject.GetComponent<CapsuleCollider>().enabled = bisAnimating;
            ragdollBone.isKinematic = bisAnimating;
        }
        GetComponent<Animator>().enabled = bisAnimating;
        {
            if(bisAnimating)
            {
                GetUpAnimation();
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("oncollisionEntered");
        if (!bIsRagdoll && collision.gameObject.tag == "RagdollTrigger")
        {
            ToggleRagdoll(false);
            StartCoroutine(GetBackUp());
        }
    }
    private void GetUpAnimation()
    {
        //play get up animation here
    }
    private IEnumerator GetBackUp()
    {
        yield return new WaitForSeconds(respawnTime);
        ToggleRagdoll(true);
    }
}
