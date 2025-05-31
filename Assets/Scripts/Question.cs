using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quiz Multiplayer Game/Question")]
public class Question : ScriptableObject
{
    public string questionText;

    public string correctAnswer;

    public List<string> possibleAnswers = new List<string>();

    public int questionId;

    public bool isAnswered = false;

    public Sprite imageSprite;


}