using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{

    [SerializeField]
    private Slider healthSlider; 
    [SerializeField]
    private Animator animator;

    public float health;
    private Rigidbody rb;
    public bool alive;




    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        setHealth();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setHealth(){
        healthSlider.value = health;
    }

    public void death(){
        if(health <= 0){
            alive = false;
            Destroy(rb);
            animator.SetBool("isDead", true);
        }
    }
}
