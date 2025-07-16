using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [Tooltip("대사 치는 캐릭터 이름")]
    public string name;

    [Tooltip("대사 내용")]
    public string[] contexts;

    [Tooltip("Sprite_ID")]
    public string[] Sprite_ID;
}

[System.Serializable]
public class DialogueEvent
{

    public string name; // 이벤트 이름

    public Vector2 line; // x 줄부터 y 줄까지 대사를 가져옴
    public Dialogue[] dialogues; // 한 사람만 대사를 치는게 아니기 때문에 배열로 관리

}