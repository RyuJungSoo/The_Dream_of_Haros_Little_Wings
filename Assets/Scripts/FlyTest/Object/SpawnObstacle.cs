using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacle : MonoBehaviour
{
    public ObstacleSO objectData;
    public bool canSpawn = true; // ���� false�� ����

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
        Debug.Log("���� ȣ��");
        Instantiate(objectData.prefab, transform.position, Quaternion.identity);
    }
}
