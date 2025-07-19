using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ObstacleSO", menuName = "Scriptable Object/ObstacleSO")]
public class ObstacleSO : ScriptableObject
{
    public GameObject prefab;
    public Sprite image;
    public string obstacle_name;
    public string tag;
    public float Factor;
    public int damage;
    public MoveType moveType;
}

public enum MoveType
{
    forward = 1,
    fall = 2,
    curve = 3,
}
