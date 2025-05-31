using UnityEngine;
using TMPro;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;
    [SerializeField] public TextMeshProUGUI questionNumberText;
    [SerializeField] public TextMeshProUGUI questionText;
    [SerializeField] public TextMeshProUGUI playerAnswerInput;
    [SerializeField] public string showAnswerText;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void UpdateQuestion()
    {
        // Set question and answer
        questionText.text = GameManager.Instance.currentQuestion;
        showAnswerText = GameManager.Instance.currentCorrectAnswer;

        // Update the question number UI
        int questionNumberIndex = GameManager.Instance.currentQuestionIndex + 1;

        questionNumberText.text = questionNumberIndex.ToString();

    }

    public void HandleQuestionButton()
    {
        //get player
        PlayerData currentPlayer = GameManager.Instance.currentPlayerPlaying.GetComponent<PlayerData>();

        // Get and clean the player's answer and correct answer
        string playerAnswer = playerAnswerInput.text.Trim().ToLower();
        string correctAnswer = showAnswerText.Trim().ToLower();

        // Remove invisible characters (like zero-width space) from both strings
        playerAnswer = Regex.Replace(playerAnswer, @"[\u200B\u00A0\u202F\u200C\u200D]", "");
        correctAnswer = Regex.Replace(correctAnswer, @"[\u200B\u00A0\u202F\u200C\u200D]", "");

        currentPlayer.questions.Add(GameManager.Instance.currentQuestion);

        if (currentPlayer != null)
        {
            if (playerAnswer == correctAnswer)
            {

                Debug.Log($"Correct answer {GameManager.Instance.currentPlayerPlaying.playerName}");

                var player = GameManager.Instance.currentPlayerPlaying;
                player.CorrectAnswers++;
                player.hasEarned += 1000;

                currentPlayer.correctAnsweredQuestions.Add(GameManager.Instance.currentCorrectAnswer);
            }
            else
            {
                Debug.Log($"im triggered");
                Debug.Log($"Wrong answer {GameManager.Instance.currentPlayerPlaying.playerName}");
                GameManager.Instance.currentPlayerPlaying.uncorrectAnsweredQuestions.Add(playerAnswer);
            }
        }

        GameManager.Instance.LoadNextQuestion();

    }


}