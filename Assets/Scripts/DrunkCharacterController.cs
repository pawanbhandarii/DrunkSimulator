using UnityEngine;
using UnityEngine.InputSystem;

public class DrunkCharacterController : MonoBehaviour
{
    DrunkCharacterInput input;

    Animator animator;

    int isWalkingHash;

    Vector2 currentMovement;
    bool movementPressed;

    private void Awake()
    {
        input = new DrunkCharacterInput();

        input.CharacterControls.Movement.performed += ctx =>
        {
            currentMovement = ctx.ReadValue<Vector2>();
            movementPressed =  currentMovement.x!=0 || currentMovement.y!=0;
        };
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking2");
    }
    private void Update()
    {
        handleMovement();
        handleRotation();
    }
    void handleMovement()
    {
        bool _isWalking = animator.GetBool(isWalkingHash);
        if (movementPressed && !_isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        if(!movementPressed && _isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }
        
    }
    void handleRotation()
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = new Vector3(currentMovement.x, 0, currentMovement.y);
        Vector3 positionToLookAt = currentPosition+newPosition;
        transform.LookAt(positionToLookAt);
    }
    private void OnEnable()
    {
        input.CharacterControls.Enable();
    }
    void OnDisable()
    {
        input.CharacterControls.Disable();
    }
}