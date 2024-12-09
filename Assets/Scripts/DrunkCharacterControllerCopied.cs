using Unity.VisualScripting;
using UnityEngine;



[AddComponentMenu("")] // Don't display in add component menu
public class DrunkCharacterControllerCopied : MonoBehaviour
{

    RaycastHit EvaGiriJumpHit;
    [SerializeField] private GameObject EvaGiriJumpRaycastLocation;
    public bool useCharacterForward = false;
    public bool lockToCameraForward = false;
    public float turnSpeed = 10f;
    public KeyCode sprintJoystick = KeyCode.JoystickButton2;
    public KeyCode sprintKeyboard = KeyCode.LeftShift;
    public KeyCode interactKeyboard = KeyCode.E;
    public KeyCode jumpKeyboard = KeyCode.Space;

    private float turnSpeedMultiplier;
    private float speed = 0f;
    private float direction = 0f;
    private bool isSprinting = false;
    public bool isInteracting = false;
    private bool isRunningJumping = false;
    private bool takeInput = true;//this is to check whether to take input from user or not, game does not take input when character is in ragdoll or recovery phase
    public bool canPerformEvaGiriJump;
    private Vector3 targetDirection;
    private Vector2 input;
    private Quaternion freeRotation;
    private Camera mainCamera;
    private float velocity;

    #region Animation Related References
    [SerializeField] AnimationClip defaultAnimationClip;
    [SerializeField] AnimationClip idleAnimationClip;
    [SerializeField] AnimationClip walkAnimationClip;
    private Animator anim;
    private AnimationSystem animationSystem;
    #endregion
    

    #region Event Bindings
    EventBinding<EnemyAttack> attackEventBinding;
    #endregion
    private void OnEnable()
    {
        attackEventBinding = new EventBinding<EnemyAttack>(handleEnemyAttackEvent);
        EventBus<EnemyAttack>.Register(attackEventBinding);
    }
    private void OnDisable()
    {
        EventBus<EnemyAttack>.Deregister(attackEventBinding);
    }
    #region Functions To Handle Events
    private void handleEnemyAttackEvent(EnemyAttack enemyAttack)
    {
        anim.SetBool("DoubleTakeDown-VictimBool", true);
        Debug.Log($"attack Event Received animation to play: { enemyAttack.animName}, Damage:{enemyAttack.damage}");
    }
    #endregion
    // Use this for initialization
    void Start ()
	{
	    anim = GetComponent<Animator>();
	    mainCamera = Camera.main;
        animationSystem = new AnimationSystem(anim, idleAnimationClip, walkAnimationClip);
	}
    private void Update()
    {
        PerformRaycasts();
        HandleAnimationStartings();
        HandleAnimationEndings();
    }
    // Update is called once per frame
    void FixedUpdate ()
	{
#if ENABLE_LEGACY_INPUT_MANAGER
        if(takeInput)
        {
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
        }
       // Debug.Log(input);

		// set speed to both vertical and horizontal inputs
        if (useCharacterForward)
            speed = Mathf.Abs(input.x) + input.y;
        else
            speed = Mathf.Abs(input.x) + Mathf.Abs(input.y);

        speed = Mathf.Clamp(speed, 0f, 1f);
        speed = Mathf.SmoothDamp(anim.GetFloat("Speed"), speed, ref velocity, 0.1f);
        anim.SetFloat("Speed", speed);

	    if (input.y < 0f && useCharacterForward)
            direction = input.y;
	    else
            direction = 0f;

        anim.SetFloat("Direction", direction);

        // set sprinting
	    isSprinting = ((Input.GetKey(sprintJoystick) || Input.GetKey(sprintKeyboard)) && input != Vector2.zero && direction >= 0f);
       // isRunningJumping = ((Input.GetKeyDown(jumpKeyboard)) && input != Vector2.zero );
        


        anim.SetBool("isSprinting", isSprinting);
            //set interact
            isInteracting = ((Input.GetKey(interactKeyboard)));
            anim.SetBool("isSittingDrinking", isInteracting);

            // Update target direction relative to the camera view (or not if the Keep Direction option is checked)
            UpdateTargetDirection();
        if (input != Vector2.zero && targetDirection.magnitude > 0.1f)
        {
            Vector3 lookDirection = targetDirection.normalized;
            freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
            var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
            var eulerY = transform.eulerAngles.y;

            if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
            var euler = new Vector3(0, eulerY, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), turnSpeed * turnSpeedMultiplier * Time.deltaTime);
        }
#else
        InputSystemHelper.EnableBackendsWarningMessage();
#endif
	}

    public virtual void UpdateTargetDirection()
    {
        if (!useCharacterForward)
        {
            turnSpeedMultiplier = 1f;
            var forward = mainCamera.transform.TransformDirection(Vector3.forward);
            forward.y = 0;

            //get the right-facing direction of the referenceTransform
            var right = mainCamera.transform.TransformDirection(Vector3.right);

            // determine the direction the player will face based on input and the referenceTransform's right and forward directions
            targetDirection = input.x * right + input.y * forward;
        }
        else
        {
            turnSpeedMultiplier = 0.2f;
            var forward = transform.TransformDirection(Vector3.forward);
            forward.y = 0;

            //get the right-facing direction of the referenceTransform
            var right = transform.TransformDirection(Vector3.right);
            targetDirection = input.x * right + Mathf.Abs(input.y) * forward;
        }
    }
    private void HandleAnimationStartings()
    {
        if ((Input.GetKeyDown(jumpKeyboard)) && input != Vector2.zero)
        {
            anim.SetBool("RunJump", true);

        }
        if ((Input.GetKeyDown(jumpKeyboard))  && canPerformEvaGiriJump)
        {
            anim.SetBool("EvaGiriJumpDown", true);

        }
        if (Input.GetMouseButtonDown(0) && input == Vector2.zero)
        {
            anim.SetBool("ZombiePunch", true);
        }
        if (Input.GetMouseButtonDown(1) && input == Vector2.zero)
        {
            anim.SetBool("RumbaDance", true);
            takeInput = false;
        }

    }
    private void HandleAnimationEndings()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).IsName("getUpFromFlat") || anim.GetCurrentAnimatorStateInfo(0).IsName("getUpFromFlat 0"))
        {
            anim.SetBool("FlatFall", false);
            takeInput = true;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).IsName("RunJump")|| anim.GetCurrentAnimatorStateInfo(0).IsName("RunJump 0"))
        {
            anim.SetBool("RunJump", false);
            // takeInput = true;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).IsName("ZombiePunch"))
        {
            anim.SetBool("ZombiePunch", false);
            // takeInput = true;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).IsName("RumbaDance"))
        {
            anim.SetBool("RumbaDance", false);
            takeInput = true;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).IsName("EvaGiriJumpDown1"))
        {
            anim.SetBool("EvaGiriJumpDown", false);
            takeInput = true;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).IsName("SittingDrinking"))
        {
            anim.SetBool("drinkDrink", false);
            anim.SetBool("isDrunk", true);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).IsName("DoubleTakeDown-Victim"))
        {
            anim.SetBool("DoubleTakeDown-VictimBool", false);
            
        }
    }
    private void PerformRaycasts()
    {
        canPerformEvaGiriJump = !Physics.Raycast(EvaGiriJumpRaycastLocation.transform.position, Vector3.down, 0.5f);
        Debug.DrawRay(EvaGiriJumpRaycastLocation.transform.position, Vector3.down * 0.5f, Color.red);
        /*if (!(Physics.Raycast(EvaGiriJumpRaycastLocation.transform.position, Vector3.down, 4f)))
        {
            canPerformEvaGiriJump = true;
        }
        else
        {
            canPerformEvaGiriJump = false;
        }*/

    }
    
    private void OnTriggerEnter(Collider collision)
    {
        switch (collision.gameObject.tag)
        {
            case "RagdollTrigger":
                {
                    //Debug.Log("on collision entered");
                    anim.SetBool("FlatFall", true);
                    //takeInput = false;
                    break;
                }
            case "DrunkTrigger":
                {
                    //Debug.Log("on collision entered");

                    // bool currentValue = anim.GetBool("isDrunk");
                    //anim.SetBool("isDrunk", !currentValue);
                    anim.SetBool("drinkDrink", true);
                    //takeInput = false;
                    break;
                }

        }
        /*if (collision.gameObject.tag=="RagdollTrigger")
        {
            //Debug.Log("on collision entered");
            anim.SetBool("FlatFall", true);
            //takeInput = false;

        }*/
    }
}


