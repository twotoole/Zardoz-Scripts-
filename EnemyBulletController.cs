using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{    
    
    [SerializeField]
    private GameObject bulletDecal;

    private Pooler pool;
    private float speed = 50f;
    private float timeToDestroy = 2f;

    public Vector3 target;
    public bool hit;

    private void OnEnable()
    {
        StartCoroutine(DestroyBulletAfterTime());
    }

    IEnumerator DestroyBulletAfterTime()
    {
        yield return new WaitForSeconds(timeToDestroy);
        pool.ReturnObject(gameObject);
    }

    private void Start()
    {
        pool = transform.parent.GetComponent<Pooler>();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if(!hit && Vector3.Distance(transform.position, target) < 0.1f)
        {
            pool.ReturnObject(gameObject);
        }    
    }


    //draws decal
    private void OnCollisionEnter(Collision other)
    {
            ContactPoint contact = other.GetContact(0);
            GameObject.Instantiate(bulletDecal, contact.point + contact.normal * 0.001f, Quaternion.LookRotation(contact.normal));
            pool.ReturnObject(gameObject);
            
    }
    
    void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Player"){
            col.gameObject.GetComponent<HealthController>().health -= 10;
            col.gameObject.GetComponent<HealthController>().setHealth();
            col.gameObject.GetComponent<HealthController>().death();
            col.gameObject.GetComponent<HealthController>().setHealth();
            pool.ReturnObject(gameObject);
        }
        // if(col.gameObject.tag == "Enemy"){
        //     col.gameObject.GetComponent<EnemyController>().health -= 10;
        //     pool.ReturnObject(gameObject);
        // }

    }
}
