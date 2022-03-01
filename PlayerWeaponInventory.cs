using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerWeaponInventory : MonoBehaviour
{

    [SerializeField]
    protected List<GameObject> weapons = new List<GameObject>();
    [SerializeField]
    private Transform rifleParent;
    [SerializeField]
    private Transform pistolParent;
    [SerializeField]
    private RigBuilder rigBuilder; 
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private GameObject deafaultWeapon;

    private InputAction weaponScroll;
    private int currentWeaponIndex;

    public GameObject currentGun;

    public Image gunImage;
    public Sprite M16img;
    public Sprite Shotgunimg;
    public Sprite ZedRevolverimg;
    public AudioSource gunSound;

    // Start is called before the first frame update
    void Awake()
    {
      weaponScroll = playerInput.actions["SwitchWeapon"];
      weaponScroll.performed += _ => changeWeapon(weaponScroll.ReadValue<float>());
      weaponScroll.performed += _ => displayUIimage();
      weaponScroll.performed += _ => assignGunSound(currentGun);
        //    weaponScroll.performed += _ =>  Debug.Log("INDEX: " + currentWeaponIndex);
        //  weaponScroll.performed += _ =>  Debug.Log("COUNT: " + weapons.Count);    
        weapons.Add(deafaultWeapon);
        activeWeapon(deafaultWeapon);
        displayUIimage();
        assignGunSound(deafaultWeapon);
    }


    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Current Gun is: " + currentGun);
    }

    void OnTriggerEnter(Collider col){

        if(col.tag == "Weapon"){
            GameObject gun;
            gun = col.gameObject;
            // activeWeapon(gun);   

            //  if(col.GetComponent<gunSpecs>().isHandgun == true){
            //      setPistol();
            //  }     
            //  else if(col.GetComponent<gunSpecs>().isHandgun == false){
            //      setRifle();
            //  }     

            for(int i = 0; i<weapons.Count; i++){
                if(col.gameObject.name == weapons[i].name){
                    return;
                }
            }

            if(col.GetComponent<gunSpecs>().isHandgun == true){
                // gun = Instantiate(gunPrefab, pistolParent);
                gun.transform.SetParent(pistolParent);
                gun.transform.localPosition = Vector3.zero;
                gun.transform.localRotation = Quaternion.identity;
                gun.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (col.GetComponent<gunSpecs>().isHandgun == false){
                //    gun = Instantiate(gunPrefab, rifleParent);
                gun.transform.SetParent(rifleParent);
                gun.transform.localPosition = Vector3.zero;
                gun.transform.localRotation = Quaternion.identity;
                gun.transform.localScale = new Vector3(1, 1, 1);
            }
            weapons.Add(gun);
            activeWeapon(currentGun);
            assignGunSound(currentGun);
        }


    }

    void setRifle(){
            rigBuilder.layers[1].active = true;
            rigBuilder.layers[2].active = true;
            rigBuilder.layers[3].active = true;

            rigBuilder.layers[4].active = false;
            rigBuilder.layers[5].active = false;
            rigBuilder.layers[6].active = false;
    }

    void setPistol(){
            rigBuilder.layers[1].active = false;
            rigBuilder.layers[2].active = false;
            rigBuilder.layers[3].active = false;

            rigBuilder.layers[4].active = true;
            rigBuilder.layers[5].active = true;
            rigBuilder.layers[6].active = true;
    }


    void activeWeapon(GameObject _gun){

        if(currentGun != null){
            //  Debug.Log("Current Gun is: " + _gun.name);
        }

        for(int i = 0; i<weapons.Count; i++){
            if(_gun.name != weapons[i].name){
                weapons[i].SetActive(false);
            }
            else if(_gun.name == weapons[i].name){
                weapons[i].SetActive(true);
                if(weapons[i].GetComponent<gunSpecs>().isHandgun == true){
                    setPistol();
                }
                else if(weapons[i].GetComponent<gunSpecs>().isHandgun == false){
                    setRifle();
                }                
                currentWeaponIndex = i;
                currentGun = weapons[i];
            }
        }
    }

    void changeWeapon(float _val){  

        if(weapons.Count > 0){    

            if(_val < 0){
                if(currentWeaponIndex == 0){
                    currentWeaponIndex = weapons.Count;
                }
                activeWeapon(weapons[currentWeaponIndex - 1]);
            }      
            if(_val > 0){
                if(currentWeaponIndex >= weapons.Count - 1){
                    currentWeaponIndex =-1;
                }
                activeWeapon(weapons[currentWeaponIndex + 1]);
            }
        }
        // Debug.Log("Inventory is giving: " + currentGun);
    }

   void displayUIimage(){

       if(currentGun.name == "Zed'sRevolver"){
           gunImage.sprite = ZedRevolverimg;
       }
       if(currentGun.name == "M16"){
           gunImage.sprite = M16img;
       }
       if(currentGun.name == "doubleBarrel"){
           gunImage.sprite = Shotgunimg;
       }
    }

    void assignGunSound(GameObject _gun){
        gunSound = currentGun.GetComponent<AudioSource>();
    }

}
