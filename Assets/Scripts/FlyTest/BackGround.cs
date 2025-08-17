using UnityEngine;
using System.Collections.Generic;

public class InfiniteBackground : MonoBehaviour
{
    [Header("배경 설정")]
    public Transform cameraTransform;
    public List<Transform> backgrounds;
    public float speed = 1f;

    private float bgWidth;

    private void Start()
    {
        bgWidth = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.x;

        for (int i = 1; i < backgrounds.Count; i++)
        {
            backgrounds[i].position = new Vector3(backgrounds[i - 1].position.x + bgWidth, backgrounds[i - 1].position.y, backgrounds[i - 1].position.z);
        }
    }

    private void Update()
    {
        foreach (var bg in backgrounds)
        {
            bg.position += Vector3.left * speed * Time.deltaTime;
        }

        Transform leftMost = backgrounds[0];
        Transform rightMost = backgrounds[backgrounds.Count - 1];

        if (cameraTransform.position.x - leftMost.position.x > bgWidth)
        {
            leftMost.position = new Vector3(rightMost.position.x + bgWidth, leftMost.position.y, leftMost.position.z);

            backgrounds.RemoveAt(0);
            backgrounds.Add(leftMost);
        }
    }
}