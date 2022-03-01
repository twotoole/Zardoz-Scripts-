using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using Cinemachine;


public class PlayerShootAnimation : MonoBehaviour
{
 
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private float waitTime = 5;

    

    public GameObject player;
    public CinemachineVirtualCamera vCam1;
    public CinemachineVirtualCamera vCam2;


    private InputAction aimAction;
    private InputAction shootAction;
    private InputAction lookAction;

    public Rig aimLayer;
    public Rig PoseLayer;

    private bool isHoldingAim;
    private bool isHipFiring;

    void Awake(){
        playerInput = GetComponent<PlayerInput>();
        aimAction = playerInput.actions["Aim"];
        shootAction = playerInput.actions["Shoot"];
        lookAction = playerInput.actions["Look"];

        aimAction.performed += _ => isHoldingAim = true;
        aimAction.canceled += _ => isHoldingAim = false;

        shootAction.performed += _ => isHipFiring = true;
        shootAction.performed += _ => waitTime = 0;
        shootAction.canceled += _ => waitTime = 5;
    }

    // Start is called before the first frame update
    void Start()
    {
        isHipFiring = false;
        isHoldingAim = false;
        waitTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        releaseAimAfterSeconds();


        if(isHoldingAim == true){
        shootAction.Disable();
        }
        else if(isHoldingAim == false){
            shootAction.Enable();
        }


        if(isHoldingAim == true || isHipFiring == true){
            aimIn();
        }
        else if(isHoldingAim == false){
            aimOut();
        }
    }

    void aimIn(){
        aimLayer.weight = 1;
        PoseLayer.weight = 0;
        vCam1.Priority = 1;
        vCam2.Priority = 10;
        player.GetComponent<PlayerController>().isAiming = true;
    }


    void aimOut(){
        aimLayer.weight = 0;
        PoseLayer.weight = 1;
        vCam1.Priority = 10;
        vCam2.Priority = 1;
        player.GetComponent<PlayerController>().isAiming = false;
    }

    void releaseAimAfterSeconds(){
        if(isHipFiring == true && isHoldingAim == false && waitTime > 0){
            waitTime -= 0.1f;
            if(waitTime < 0){
                aimOut();
                isHipFiring = false;
            }
        }

    }

    // void logs(){
    // Debug.Log("isHoldingAim = " + isHoldingAim);
    // Debug.Log("isHipFiring = "+ isHipFiring);
    // }


}