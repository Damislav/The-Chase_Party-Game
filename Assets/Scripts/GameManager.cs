using System;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int GameRound;

    public int currentPlayerIndex = 0;// Track the current player's turn
    public List<Player> players;// List of players

    public Player currentPlayerPlaying;

    public int currentQuestionIndex = 0;

    public List<string> questions;
    public List<string> correctAnswers;

    public string currentQuestion;
    public string currentCorrectAnswer;


    public float currentTimeToAnswerQuestion;
    public float maxTimeToAnswerQuestions = 60;

    public bool gameHasStarted = false;
    private bool gameInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (gameHasStarted && !gameInitialized)
        {
            InitializeGame();
            gameInitialized = true;
        }

        if (gameHasStarted)
        {
            HandleTimer();
        }
    }

    private void HandleTimer()
    {
        currentTimeToAnswerQuestion -= Time.deltaTime;


        if (currentTimeToAnswerQuestion <= 0)
        {
            // Time is up, switch to next player
            EndRound();
        }
    }

    public void LoadNextQuestion()
    {
        // If we've completed all questions for the current player, switch to the next player
        if (GameManager.Instance.currentQuestionIndex >= GameManager.Instance.questions.Count)
        {
            EndPlayerTurn();
        }
        else
        {
            // Otherwise, load the next question
            QuestionManager.Instance.UpdateQuestion();
            NextQuestion();
        }
    }

    private void InitializeGame()
    {

        currentPlayerPlaying = players[currentPlayerIndex];
        currentQuestionIndex = 0; // Reset to the first question
        currentTimeToAnswerQuestion = maxTimeToAnswerQuestions;

        currentQuestion = questions[currentQuestionIndex];
        currentCorrectAnswer = correctAnswers[currentQuestionIndex];

        QuestionManager.Instance.UpdateQuestion();

    }

    private void NextQuestion()
    {
        currentQuestionIndex++;

        //get next question for that player
        if (currentQuestionIndex < questions.Count)
        {
            currentQuestion = questions[currentQuestionIndex];
            currentCorrectAnswer = correctAnswers[currentQuestionIndex];
            QuestionManager.Instance.UpdateQuestion();
        }
        else
        {
            Debug.Log("Player has answered all the questions.");
            EndPlayerTurn(); // End the player's turn after all questions are answered
        }

    }

    private void EndPlayerTurn()
    {

        Debug.Log($"Player {currentPlayerPlaying.playerName} has finished their turn!");
        // Check if we need to switch players
        if (currentPlayerIndex < players.Count - 1)// More players to go
        {
            currentPlayerIndex++;//move to the next player
            currentPlayerPlaying = players[currentPlayerIndex];
            InitializeGame(); // Re-initialize game for the new player
        }
        else
        {
            Debug.Log("All players have completed their turns!");
            gameHasStarted = false; // End the game or handle next steps
        }
    }

    private void EndRound()
    {
        Debug.Log($"Round ended. Moving to the next player.");

        currentPlayerIndex++;

        if (currentPlayerIndex < players.Count - 1)
        {
            currentPlayerPlaying = players[currentPlayerIndex];
            InitializeGame();
        }
        else
        {
            Debug.Log("All players have played.");
            gameHasStarted = false;
            gameInitialized = false;
            // Handle end of game logic here
        }

    }
}
