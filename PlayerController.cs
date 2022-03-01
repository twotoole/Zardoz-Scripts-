using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]

public class PlayerController : MonoBehaviour
{

    // [SerializeField]
    // private float jumpHeight = 1.0f;
    // [SerializeField]
    // private float gravityValue = -9.81f;
    // [SerializeField]
    // private float rotationSpeed = 5f;

    [SerializeField]
    private float speed = 2.0f;
    [SerializeField]
    private float turnSmoothTime = 0.1f;
    [SerializeField]
    private float rotationSpeed = 5f;
    [SerializeField]
    private float animationSmoothTime = 0.1f;



    private CharacterController controller;
    private Transform cam;
    private PlayerInput playerInput;
    public bool isAiming;

    private InputAction moveAction;
    private InputAction aimAction;
    private InputAction jumpAction;

    public Animator animator;

    int moveXAnimationParameterId;
    int moveZAnimationParameterId;
    
    float turnSmoothVelocity;
    Vector2 currentAninmationBlendVector;
    Vector2 animationVelocity;

    
    public float gravity = 9.9f;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cam = Camera.main.transform;

        jumpAction = playerInput.actions["Jump"];
        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];

        jumpAction.performed += _ => jump();
        jumpAction.canceled -= _ => jump();

        Cursor.lockState = CursorLockMode.Locked;
        //Animations
        moveXAnimationParameterId = Animator.StringToHash("MoveX");
        moveZAnimationParameterId = Animator.StringToHash("MoveZ");        
    }

    void Start()
    {
    }




       void Update()
    {
        Aiming();
        Movement();
        Gravity();
        
    }

    void Movement(){
         Vector2 input = moveAction.ReadValue<Vector2>();
        currentAninmationBlendVector = Vector2.SmoothDamp(currentAninmationBlendVector, input, ref animationVelocity, animationSmoothTime);
        Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle =Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            if(isAiming == false)
            {
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            }
            
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        animator.SetFloat(moveXAnimationParameterId, currentAninmationBlendVector.x);
        animator.SetFloat(moveZAnimationParameterId, currentAninmationBlendVector.y);
        
        if(isAiming == true)
        {
        //rotate player towards camera direction
        Quaternion targetRoatation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRoatation, rotationSpeed * Time.deltaTime);
        }
        
        animator.SetFloat("Speed", direction.magnitude);
    }

    private void Aiming()
    {
        aimAction.performed += _ => isAiming = true;
        aimAction.canceled += _ => isAiming = false;

        animator.SetBool("IsAiming", isAiming);
    }


    void jump(){

        // playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

    void Gravity()
    {
        controller.Move(Vector3.down * gravity * Time.deltaTime);
    }

}
