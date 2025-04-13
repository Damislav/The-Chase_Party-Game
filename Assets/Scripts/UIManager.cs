using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;


    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject gameplayPanelPart1;
    [SerializeField] GameObject gameplayPanelPart2;



    [SerializeField] private TextMeshProUGUI playerNameInputField;
    [SerializeField] private TextMeshProUGUI currentPlayerNameText;
    [SerializeField] TextMeshProUGUI maxQuestionsCount;

    [SerializeField] TextMeshProUGUI timer;

    [SerializeField] Player player;

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


    void Update()
    {
        UpdateTimerText();
    }

    public void SavePlayerName()
    {
        string currentPlayerName = playerNameInputField.text;

        //create new player and assign the name to it
        Player newPlayer = Instantiate(player, Vector3.zero, Quaternion.identity);
        newPlayer.playerName = currentPlayerName;
        newPlayer.playerId = GameManager.Instance.players.Count + 1; //assign player id based on the number of players already in the game

        //add player to the players list
        GameManager.Instance.players.Add(newPlayer);
        GameManager.Instance.currentPlayerPlaying = newPlayer;


        mainMenuPanel.gameObject.SetActive(false);
        lobbyPanel.gameObject.SetActive(true);
        UpdateCurrentPlayerUI(); // Update UI to show current player

        currentPlayerNameText.gameObject.SetActive(true);
    }

    public void StartGameBtn()
    {
        mainMenuPanel.gameObject.SetActive(false);
        lobbyPanel.gameObject.SetActive(false);
        gameplayPanelPart1.gameObject.SetActive(true);
        timer.gameObject.SetActive(true);
        maxQuestionsCount.gameObject.SetActive(true);

        maxQuestionsCount.text = GameManager.Instance.questions.Count.ToString(); // Display the number of questions

        GameManager.Instance.gameHasStarted = true;

    }

    private void UpdateCurrentPlayerUI()
    {
        currentPlayerNameText.text = $"It's {GameManager.Instance.currentPlayerPlaying.playerName}'s turn!";
    }

    public void UpdateTimerText()
    {
        timer.SetText(GameManager.Instance.currentTimeToAnswerQuestion.ToString("F0"));
    }


    public void BackToMainMenu()
    {
        mainMenuPanel.gameObject.SetActive(true);
        lobbyPanel.gameObject.SetActive(false);
        gameplayPanelPart1.gameObject.SetActive(false);
        gameplayPanelPart2.gameObject.SetActive(false);
        timer.gameObject.SetActive(false);
        maxQuestionsCount.gameObject.SetActive(false);
        currentPlayerNameText.gameObject.SetActive(false);


    }




}