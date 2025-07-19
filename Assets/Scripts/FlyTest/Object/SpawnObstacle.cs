using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacle : MonoBehaviour
{
    public ObstacleSO objectData;
    public bool canSpawn = true; // 추후 false로 설정

    // Update is called once per frame
    void Update()
    {
        if(canSpawn)
        {
            Spawn();
            canSpawn = false;
        }
    }

    public void Spawn()
    {
        Debug.Log("스폰 호출");
        Instantiate(objectData.prefab, transform.position, Quaternion.identity);
    }
}
