using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance;

    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject gameplayPanelPart1;
    [SerializeField] GameObject gameplayPanelPart2;

    [SerializeField] private TextMeshProUGUI playerNameInputField;
    [SerializeField] private TextMeshProUGUI currentPlayerNameText;
    [SerializeField] TextMeshProUGUI maxQuestionsCount;

    [SerializeField] TextMeshProUGUI timer;

    [SerializeField] PlayerData player;

    [SerializeField] Button StartButton;

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
        if (GameManager.Instance.gameHasStarted)
        {
            UpdateTimerText();
        }
    }

    public void SavePlayerName()
    {
        string currentPlayerName = playerNameInputField.text;

        //create new player and assign the name to it
        GameObject newPlayer = PhotonNetwork.Instantiate(player.name, Vector3.zero, Quaternion.identity);
        PlayerData newPlayerData = newPlayer.GetComponent<PlayerData>();

        newPlayerData.name = currentPlayerName;
        newPlayerData.playerName = currentPlayerName;
        newPlayerData.playerId = GameManager.Instance.players.Count + 1; //assign player id based on the number of players already in the game

        //add player to the players list
        GameManager.Instance.players.Add(newPlayerData);
        GameManager.Instance.currentPlayerPlaying = newPlayerData;

        //show UI

        lobbyPanel.gameObject.SetActive(true);
        currentPlayerNameText.gameObject.SetActive(true);

        UpdateCurrentPlayerUI(); // Update UI to show current player
    }

    public void StartGameBtn()
    {

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

        lobbyPanel.gameObject.SetActive(false);
        gameplayPanelPart1.gameObject.SetActive(false);
        gameplayPanelPart2.gameObject.SetActive(false);
        timer.gameObject.SetActive(false);
        maxQuestionsCount.gameObject.SetActive(false);
        currentPlayerNameText.gameObject.SetActive(false);
    }


}