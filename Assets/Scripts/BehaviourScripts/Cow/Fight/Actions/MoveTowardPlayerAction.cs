using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveTowardPlayer", story: "Agent Moves Towrads Player", category: "Action", id: "a6f52fee252e9876944b96ab9c598c16")]
public partial class MoveTowardPlayerAction : Action
{
    //using hash for animator parameters instead of string, string operations are expensive and unrealiable
    private int move = Animator.StringToHash("run");

    [SerializeField] private float stopDistance = 1.5f; // Distance at which the enemy stops moving

    [SerializeField]
    private float moveSpeed = 2f; // Speed of movement

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
        // Set the IsMoving bool to true when starting to move
        animator.SetBool(move, true);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // If player is not found, fail the action
        if (playerTransform == null)
            return Status.Failure;

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(enemyTransform.position, playerTransform.position);

        // Check if we're close enough to stop
        if (distanceToPlayer <= stopDistance)
        {
            
            // Set the IsMoving bool to true when starting to move
            animator.SetBool(move, true);
            return Status.Success;
        }

        // Calculate direction to player
        Vector3 directionToPlayer = (playerTransform.position - enemyTransform.position);

        // Rotate towards player
        enemyTransform.rotation = Quaternion.LookRotation(directionToPlayer);

        

        return Status.Running;
    }

    protected override void OnEnd()
    {
        // Reset animation speed when action ends
        if (animator != null)
        {
            animator.SetBool(move, false);

        }
    }
}

