using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacle : MonoBehaviour
{
    public ObstacleSO objectData;
    public bool canSpawn = false;


    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!canSpawn && IsInCameraView())
        {
            canSpawn = true;
        }

        if (canSpawn)
        {
            Spawn();
            Destroy(gameObject);
            canSpawn = false;
        }
    }
    public void Spawn()
    {
        Debug.Log("스폰 호출");
        Instantiate(objectData.prefab, transform.position, Quaternion.identity);
    }
    private bool IsInCameraView()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
}
