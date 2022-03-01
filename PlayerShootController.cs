using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShootController : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletParent;
    [SerializeField]
    private float bulletHitMissDistance = 25f;
    [SerializeField]
    private Pooler bulletPool;

    public Transform barrelTransform;

    private Transform cam;
    private InputAction shootAction;

    private PlayerInput playerInput;

    private bool isShooting = false;

    private string currentGun;

    LayerMask mask;

    private AudioSource currentGunSound;


    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        shootAction = playerInput.actions["Shoot"];
        shootAction.performed += _ => registerWeaponChange();  
        shootAction.performed += _ => shootSwitch();
        shootAction.canceled -= _ => shootSwitch();
        // shootAction.performed += _ => shootSwitch();  
        // shootAction.performed += _ => isShooting = true;
        shootAction.canceled += _ => isShooting = false;  
        cam = Camera.main.transform;

        mask = LayerMask.GetMask("Player", "Bullet");
    }

    void Start()
    {
        // StartCoroutine(shootCoroutine());
        currentGunSound = GetComponent<PlayerWeaponInventory>().gunSound;
    }

    void Update(){            
    }

    private void ShootGun(Vector3 _randomRange)
    {
        //watch video on object pool
        RaycastHit hit;
       // GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        GameObject bullet = bulletPool.GetObject();
        bullet.transform.position = barrelTransform.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.SetActive(true);

        Vector3 vec = cam.transform.forward + _randomRange;

        BulletController bulletController = bullet.GetComponent<BulletController>();
        //mathf.infinity should be hit distance of each respective gun
        if (Physics.Raycast(cam.transform.position, vec, out hit, Mathf.Infinity, ~mask))
        {
             bulletController.target = hit.point;
             bulletController.hit = true;
            //  Debug.Log("hit");
        }
        else
        {
            bulletController.target = cam.transform.position + vec * bulletHitMissDistance;
            bulletController.hit = false;
            // Debug.Log("miss");
        }
    }


    void ShootShotGun(){
        for(int i=0;i<10;i++){
            ShootGun(new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), 0));
        }
    }


    void shootSwitch(){
                switch (currentGun)
                {
                case "M16":
                    shootAction.performed += _ => isShooting = true;
                    // Debug.Log(currentGun);
                    IEnumerator auto = autoCoroutine();
                    StartCoroutine(auto);
                    currentGunSound = GetComponent<PlayerWeaponInventory>().gunSound;
                    currentGunSound.Play();
                    shootAction.canceled += _ => StopCoroutine(auto);
                    shootAction.canceled += _ => currentGunSound.Stop();
                    break;
                case "Zed'sRevolver":
                    //   Debug.Log(currentGun);
                    currentGunSound = GetComponent<PlayerWeaponInventory>().gunSound;
                    currentGunSound.Play();
                    ShootGun(Vector3.zero);
                    break;
                case "doubleBarrel":
                    //  Debug.Log(currentGun);
                    currentGunSound = GetComponent<PlayerWeaponInventory>().gunSound;
                    currentGunSound.Play();
                    ShootShotGun();
                    break;
                case "webley":
                    //  Debug.Log(currentGun);
                    currentGunSound = GetComponent<PlayerWeaponInventory>().gunSound;
                    currentGunSound.Play();
                   ShootGun(Vector3.zero);
                    break;
                default:
                    // Debug.Log("No appropriate weapon name");
                    break;
            }
    }

     IEnumerator autoCoroutine()
    {   
        while(true)
        {
            while(isShooting == true)
            {
                ShootGun(Vector3.zero);
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForEndOfFrame();
        }
    }


    
    void registerWeaponChange(){
        currentGun = GetComponent<PlayerWeaponInventory>().currentGun.name;
    }




}
