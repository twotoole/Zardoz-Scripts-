using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletParent;
    [SerializeField]
    private float bulletHitMissDistance = 25f;
    [SerializeField]
    private Pooler bulletPool;
    [SerializeField]
    private RigBuilder rigBuilder; 
    [SerializeField]
    private float speed;
    [SerializeField]
    public GameObject gun;
    [SerializeField]
    public float aggroRange;
    [SerializeField]
    public float atkRange;
    [SerializeField]
    public float chaseRange;
    
    public Animator animator;

    public Transform barrelTransform;

    public NavMeshAgent agent;
    public GameObject player;

    public bool isHostile;
    public bool isMoving;
    public bool isShooting;
    public bool alive;

    public float health;
    private Rigidbody rb;

    
    LayerMask mask;

    void Awake()
    {   
        rb = GetComponent<Rigidbody>();
        alive = true;
        agent = GetComponent<NavMeshAgent>();
        mask = LayerMask.GetMask("Bullet", "Enemy");
    }

    void Start(){
        StartCoroutine(shootRoutine());
        setRigLayersToGunModel();
    }

    void Update()
    {   
        float dist = Vector3.Distance(player.transform.position, transform.position);

        if(dist < aggroRange){
            attack();
            isHostile = true;
        }
        if(dist < atkRange && isHostile){
            attack();
        }
        if(isHostile && dist > aggroRange)
        {
            chase();
        }
        if(isHostile && dist < chaseRange ){
            attack();
        }
        if(dist > chaseRange && isHostile){
            idle();
            isHostile = false;
        }
        if(alive == false){
        Destroy(this.gameObject, 5);
        }
    }




///// idle  chase   attack functions----------------------------------------------------
    void idle(){
        isShooting = false;
        return;
    }

    void chase(){
        agent.SetDestination(player.transform.position);
        Vector3 targetPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(targetPos);       
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsAiming", false);
    }
    void attack(){
        Vector3 targetPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(targetPos); 
        animator.SetBool("IsAiming", true);
        // Automatic();
        isShooting = true;
        Vector3 walkTargetPos = player.transform.position - transform.position;
        animator.SetFloat("MoveX", targetPos.normalized.x);
        animator.SetFloat("MoveZ", targetPos.normalized.z);
    }

// end of enemy state functions---------------------------------------------


////// shooting functions----------------------------------------------------------------
    IEnumerator shootRoutine(){
        while(true)
        {
            while(isShooting == true)
            {
                switch (gun.name)
                {
                case "M16":
                    ShootGun(Vector3.zero);
                    yield return new WaitForSeconds(0.5f);
                
                    break;
                case "webley":
                    ShootGun(Vector3.zero);
                    yield return new WaitForSeconds(1);
                    break;
                case "soubleBarrel":
                    ShootShotGun();
                    yield return new WaitForSeconds(2);
                    break;
                default:
                    break;
                }
            }    
        yield return new WaitForEndOfFrame(); 
        }
    }

    void ShootGun(Vector3 _randomRange)
    {
        //watch video on object pool
        RaycastHit hit;
       // GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
       GameObject bullet = bulletPool.GetObject();
       bullet.transform.position = barrelTransform.position;
       bullet.transform.rotation = Quaternion.identity;
       bullet.SetActive(true);

        Vector3 vec = (player.transform.position - this.transform.position) + _randomRange;

        EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
        //mathf.infinity should be hit distance of each respective gun
        if (Physics.Raycast(transform.position, vec, out hit, Mathf.Infinity, ~mask))
        {
             bulletController.target = hit.point;
             bulletController.hit = true;
        }
        else
        {
            bulletController.target = transform.position + vec * bulletHitMissDistance;
            bulletController.hit = false;
            // Debug.Log("miss");
        }
    }


    void ShootShotGun(){
        for(int i=0;i<10;i++){
            ShootGun(new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0));
        }
    }

///// end of shooting functions -----------------------------------

//// rig layers -------------------

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


    void setRigLayersToGunModel(){
        if(gun.GetComponent<gunSpecs>().isHandgun == true){
            setPistol();
        }
        else if(gun.GetComponent<gunSpecs>().isHandgun == false){
            setRifle();
        }
    }

//// end of  rig layers -----------------

   public void death(){
        if(health <= 0){
            alive = false;
            Destroy(rb);
            animator.SetBool("isDead", true);
        }
    }
}
