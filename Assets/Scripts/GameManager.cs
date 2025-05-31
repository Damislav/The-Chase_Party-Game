using System;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int GameRound;

    public int currentPlayerIndex = 0;// Track the current player's turn
    // public int playersCount = 0;// Track the number of players
    public List<PlayerData> players;// List of players

    public PlayerData currentPlayerPlaying;

    public int currentQuestionIndex = 0;

    public List<string> questions;
    public List<string> correctAnswers;

    public string currentQuestion;
    public string currentCorrectAnswer;

    public float currentTimeToAnswerQuestion;
    public float maxTimeToAnswerQuestions = 60;

    public bool gameHasStarted = false;
    public bool gameInitialized = false;

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
        //game begins
        if (gameHasStarted && !gameInitialized)
        {
            InitializeGame();
            gameInitialized = true;
        }

        //start timer only when game has started and game is initialized
        if (gameHasStarted && gameInitialized)
        {
            HandleTimer();
        }
    }

    public void InitializeGame()
    {
        currentPlayerPlaying = players[currentPlayerIndex];
        currentQuestionIndex = 0; // Reset to the first question
        currentTimeToAnswerQuestion = maxTimeToAnswerQuestions;

        //set current question and correct answer
        currentQuestion = questions[currentQuestionIndex];
        currentCorrectAnswer = correctAnswers[currentQuestionIndex];

        QuestionManager.Instance.UpdateQuestion();

    }

    private void HandleTimer()
    {
        currentTimeToAnswerQuestion -= Time.deltaTime;

        if (currentTimeToAnswerQuestion <= 0)
        {
            // Time is up, switch to next player
            EndPlayerTurn();
        }
    }

    public void LoadNextQuestion()
    {
        // Move to the next question
        currentQuestionIndex++;

        // Check if we've completed all questions
        if (currentQuestionIndex >= questions.Count)
        {
            EndPlayerTurn();
            return;
        }

        // Set current question and correct answer
        currentQuestion = questions[currentQuestionIndex];
        currentCorrectAnswer = correctAnswers[currentQuestionIndex];

        // Update UI
        QuestionManager.Instance.UpdateQuestion();

    }

    private void EndPlayerTurn()
    {

        Debug.Log($"Player {currentPlayerPlaying.playerName} has finished their turn!");

        // Check if we need to switch players
        /*      if (currentPlayerIndex < players.Count - 1)// More players to go
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
      */

        UIManager.Instance.BackToMainMenu();
        currentTimeToAnswerQuestion = 0; // Reset the timer for the next player
        currentQuestionIndex = 0; // Reset question index for the next player
        gameHasStarted = false;
        gameInitialized = false;
    }

    // private void EndRound()
    // {
    //     Debug.Log($"Round ended. Moving to the next player.");

    //     currentPlayerIndex++;
    //     UIManager.Instance.BackToMainMenu();



    //     if (currentPlayerIndex < players.Count - 1)
    //     {

    //         currentPlayerPlaying = players[currentPlayerIndex];
    //         InitializeGame();
    //     }
    //     else
    //     {
    //         Debug.Log("All players have played.");
    //         gameHasStarted = false;
    //         gameInitialized = false;
    //         // Handle end of game logic here
    //     }

    // }
}
