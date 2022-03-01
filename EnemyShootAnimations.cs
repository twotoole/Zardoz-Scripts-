using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnemyShootAnimations : MonoBehaviour
{

    public GameObject player;
    public Rig aimLayer;
    public Rig PoseLayer;


    // Update is called once per frame
    void Update()
    {
       if(player.GetComponent<EnemyController>().isShooting == true){
           aimIn();
       }
       else if(player.GetComponent<EnemyController>().isShooting == false){
           aimOut();
       }

    }

    void aimIn(){
        aimLayer.weight = 1;
        PoseLayer.weight = 0;
    }


    void aimOut(){
        aimLayer.weight = 0;
        PoseLayer.weight = 1;
    }


}
