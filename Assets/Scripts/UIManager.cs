using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject gameplayPanelPart1;
    [SerializeField] GameObject gameplayPanelPart2;


    [SerializeField] private TextMeshProUGUI playerNameInputField;
    [SerializeField] private TextMeshProUGUI currentPlayerNameText;

    [SerializeField] TextMeshProUGUI timer;

    [SerializeField] Player player;

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
    }

    public void StartGameBtn()
    {
        mainMenuPanel.gameObject.SetActive(false);
        lobbyPanel.gameObject.SetActive(false);
        gameplayPanelPart1.gameObject.SetActive(true);
        currentPlayerNameText.gameObject.SetActive(true);
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

    void Update()
    {
        UpdateTimerText();
    }




}