using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Transform spawnPos;

    public void Spawn(){
        Instantiate(enemy, spawnPos.position, Quaternion.identity);
    }
}
