using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack Player", story: "Attack Player", category: "Action", id: "2e5d50226e9c90d2764071d45d3c1eb2")]
public partial class AttackPlayerAction : Action
{
    //move all these data to blackboard pattern ASAP
    //using hash for animator parameters instead of string, string operations are expensive and unrealiable
    private int attack = Animator.StringToHash("DoubleLegTakeDownBool");

    private Transform enemyTransform;
    private Transform playerTransform;
    private Animator animator;


    protected override Status OnStart()
    {
        // Find the enemy's transform (assuming this script is on the enemy)
        enemyTransform = this.GameObject.transform;

        // Find the player's transform
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Get the animator component
        animator = GameObject.GetComponent<Animator>();

        // Check if player or required components are missing
        if (playerTransform == null || animator == null)
        {
            Debug.LogError("Player or Animator component not found!");
            return Status.Failure;
        }
        // Calculate direction to player
        Vector3 directionToPlayer = (playerTransform.position - enemyTransform.position); // do not normalize this vector, normalization doesn't work for some reason, it just return indentity vector in original direction

        // Rotate towards player
        enemyTransform.rotation = Quaternion.LookRotation(directionToPlayer);
        // Set the IsMoving bool to true when starting to move
        animator.SetBool(attack, true);
        Debug.Log("Attack Started");

        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        // If player is not found, fail the action
        if (playerTransform == null)
            return Status.Failure;

        

        // Check if we're close enough to stop
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime>0.9 && animator.GetCurrentAnimatorStateInfo(0).IsName("DoubleLegTakeDown"))
            {

            // Set the IsMoving bool to true when starting to move
            animator.SetBool(attack, false);
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        // Reset animation speed when action ends
        if (animator != null)
        {
            animator.SetBool(attack, false);

        }
    }
}

