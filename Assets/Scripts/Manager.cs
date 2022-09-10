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
        if(right)
        Instantiate(enemy, spawnPos.position, Quaternion.identity);
        else Instantiate(enemy, spawnPos1.position, Quaternion.identity);
        right = !right;
    }

}
