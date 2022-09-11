using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorEnemy : MonoBehaviour
{

   void OnTriggerStay2D(Collider2D col){
        if(col.CompareTag("Enemy")){
            transform.root.gameObject.GetComponent<EnemyKnight>().canMove = false;
        }
        
   }

   void OnTriggerExit2D(Collider2D col){
    if(col.CompareTag("Enemy"))
        transform.root.gameObject.GetComponent<EnemyKnight>().canMove = true;
   }
}
