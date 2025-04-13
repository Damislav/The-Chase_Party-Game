using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public string playerName;

    public int playerId;

    public float hasEarned;

    public int CorrectAnswers;

    public List<string> questions;
    public List<string> correctAnsweredQuestions = new List<string>();
    public List<string> uncorrectAnsweredQuestions = new List<string>();

    public bool isFinishedTurn = false;


}
