using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacle : MonoBehaviour
{
    public ObstacleSO objectData;
    public bool canSpawn = false;

    [SerializeField]
    private float spawnDelay = 3f;

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
            StartCoroutine(Spawn());
            canSpawn = false;
        }
    }
    public IEnumerator Spawn()
    {
        Debug.Log($"{spawnDelay} 뒤 스폰 호출");
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(objectData.prefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private bool IsInCameraView()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
}
