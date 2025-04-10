using UnityEngine;
using TMPro;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;
    [SerializeField] public TextMeshProUGUI questionNumberText;
    [SerializeField] public TextMeshProUGUI questionText;
    [SerializeField] public TextMeshProUGUI playerAnswerInput;
    [SerializeField] public string showAnswerText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateQuestion()
    {
        if (GameManager.Instance.currentQuestionIndex >= GameManager.Instance.questions.Count)
        {
            Debug.Log("No more questions!");
            // Handle end-of-questions logic here, maybe trigger end game or wrap around
            return;
        }
        // Set question and answer
        questionText.text = GameManager.Instance.currentQuestion;
        showAnswerText = GameManager.Instance.currentCorrectAnswer;

        // Update the question number UI
        int questionNumberIndex = GameManager.Instance.currentQuestionIndex + 1;
        questionNumberText.text = questionNumberIndex.ToString();

    }


    public void HandleQuestionButton()
    {
        // Get and clean the player's answer and correct answer
        string playerAnswer = playerAnswerInput.text.Trim().ToLower();
        string correctAnswer = showAnswerText.Trim().ToLower();

        // Remove invisible characters (like zero-width space) from both strings
        playerAnswer = Regex.Replace(playerAnswer, @"[\u200B\u00A0\u202F\u200C\u200D]", "");
        correctAnswer = Regex.Replace(correctAnswer, @"[\u200B\u00A0\u202F\u200C\u200D]", "");

        if (playerAnswer == correctAnswer)
        {

            Debug.Log($"Correct answer {GameManager.Instance.currentPlayerPlaying.playerName}");

            // Correct answer logic
            var player = GameManager.Instance.currentPlayerPlaying;
            player.CorrectAnswers++;
            player.hasEarned += 1000;

            // After answering, move to the next question (handled by GameManager)
            GameManager.Instance.LoadNextQuestion();
            UpdateQuestion();
        }
        else
        {
            Debug.Log($"Wrong answer {GameManager.Instance.currentPlayerPlaying.playerName}");
            //     //Wrong answer
            GameManager.Instance.LoadNextQuestion();
            UpdateQuestion(); ;


        }
    }


}