using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject RangedEnemy;
    [SerializeField] Transform spawnPos;
    [SerializeField] Transform spawnPos1;
    bool right = true;

    public void Spawn(){
        int a = Random.Range(2,3);
        int b = Random.Range(0,2);
        if(right){
            for(int i = 0; i < a; i++){
                if(b == 0)
                Instantiate(enemy, spawnPos.position, Quaternion.identity);
                else Instantiate(RangedEnemy, spawnPos.position, Quaternion.identity);
            }
        }
        else{
            for(int i = 0; i < a; i++){
                if(b==0)
                Instantiate(enemy, spawnPos1.position, Quaternion.identity);
                else Instantiate(RangedEnemy, spawnPos1.position, Quaternion.identity);
            }
        }
        right = !right;
    }
}