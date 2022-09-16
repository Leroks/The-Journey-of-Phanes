using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Transform spawnPos;
    [SerializeField] Transform spawnPos1;
    bool right = true;

    public void Spawn(){
        int a = Random.Range(2,3);
        if(right){
            for(int i = 0; i < a; i++){
                Instantiate(enemy, spawnPos.position, Quaternion.identity);
            }
        }
        else{
            for(int i = 0; i < a; i++){
                Instantiate(enemy, spawnPos1.position, Quaternion.identity);
            }
        }
        right = !right;
    }
}