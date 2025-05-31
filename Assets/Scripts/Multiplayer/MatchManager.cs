using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Collections;

// Main class that handles the multiplayer match lifecycle and player stat syncing
public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;
    private List<LeaderboardPlayer> lboardPlayers = new List<LeaderboardPlayer>();

    // Called before Start
    void Awake()
    {
        instance = this;
    }

    // Custom event codes for Photon communication
    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStat,
        NextMatch,
        TimerSync
    }

    // List of players in the match
    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private int index; // Local player index

    // Game state management
    public enum GameState
    {
        Waiting,
        Playing,
        Ending
    }

    public int killsToWin = 3;

    public GameState state = GameState.Waiting;
    public float waitAfterEnding = 5f;
    public bool perpetual;
    public float matchLength = 180f;

    private float currentMatchTime;
    private float sendTimer;

    // Initialize game state and send player info
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0); // Return to main menu if disconnected
        }
        else
        {
            NewPlayersSend(PhotonNetwork.NickName);
            state = GameState.Playing;
            SetupTimer();

            if (!PhotonNetwork.IsMasterClient)
            {
                UIController.instance.timerText.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        // Toggle leaderboard on Tab key press
        if (Input.GetKeyDown(KeyCode.Tab) && state != GameState.Ending)
        {
            UIController.instance.leaderboard.SetActive(!UIController.instance.leaderboard.activeInHierarchy);
            if (UIController.instance.leaderboard.activeInHierarchy)
                ShowLeaderboard();
        }

        // Master client updates match timer and sends sync event
        if (PhotonNetwork.IsMasterClient && currentMatchTime > 0f && state == GameState.Playing)
        {
            currentMatchTime -= Time.deltaTime;
            if (currentMatchTime <= 0f)
            {
                currentMatchTime = 0;
                state = GameState.Ending;
                ListPlayersSend();
                StateCheck();
            }

            UpdateTimerDisplay();

            sendTimer -= Time.deltaTime;
            if (sendTimer <= 0)
            {
                sendTimer += 1;
                TimerSend();
            }
        }
    }

    // Handle incoming Photon events
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            switch (theEvent)
            {
                case EventCodes.NewPlayer:
                    NewPlayerRecieve(data);
                    break;
                case EventCodes.ListPlayers:
                    ListPlayersRecieve(data);
                    break;
                case EventCodes.UpdateStat:
                    UpdateStatsRecieve(data);
                    break;
                case EventCodes.NextMatch:
                    NextMatchRecieve();
                    break;
                case EventCodes.TimerSync:
                    TimerRecieve(data);
                    break;
            }
        }
    }

    public override void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    public override void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    // Sends a new player's data to the master client
    public void NewPlayersSend(string username)
    {
        object[] package = new object[] { username, PhotonNetwork.LocalPlayer.ActorNumber, 0, 0 };
        PhotonNetwork.RaiseEvent((byte)EventCodes.NewPlayer, package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true });
    }

    // Receives new player data and updates list
    public void NewPlayerRecieve(object[] dataReceived)
    {
        PlayerInfo player = new PlayerInfo((string)dataReceived[0], (int)dataReceived[1], (int)dataReceived[2], (int)dataReceived[3]);
        allPlayers.Add(player);
        ListPlayersSend(); // Sync full list to all clients
    }

    // Sends current player list and game state
    public void ListPlayersSend()
    {
        object[] package = new object[allPlayers.Count + 1];
        package[0] = state;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            package[i + 1] = new object[] { allPlayers[i].name, allPlayers[i].actor, allPlayers[i].kills, allPlayers[i].deaths };
        }

        PhotonNetwork.RaiseEvent((byte)EventCodes.ListPlayers, package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true });
    }

    // Receives and reconstructs the player list
    public void ListPlayersRecieve(object[] dataReceived)
    {
        allPlayers.Clear();
        state = (GameState)dataReceived[0];

        for (int i = 1; i < dataReceived.Length; i++)
        {
            object[] piece = (object[])dataReceived[i];
            PlayerInfo player = new PlayerInfo((string)piece[0], (int)piece[1], (int)piece[2], (int)piece[3]);
            allPlayers.Add(player);

            if (PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
            {
                index = i - 1;
            }
        }

        StateCheck();
    }

    // Sends a stat update (kills or deaths)
    public void UpdateStatsSend(int actorSending, int statToUpdate, int amountToChange)
    {
        PhotonNetwork.RaiseEvent((byte)EventCodes.UpdateStat, new object[] { actorSending, statToUpdate, amountToChange },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true });
    }

    // Applies received stat changes
    public void UpdateStatsRecieve(object[] dataReceived)
    {
        int actor = (int)dataReceived[0];
        int statType = (int)dataReceived[1];
        int amount = (int)dataReceived[2];

        foreach (var player in allPlayers)
        {
            if (player.actor == actor)
            {
                if (statType == 0) player.kills += amount;
                else player.deaths += amount;

                if (UIController.instance.leaderboard.activeInHierarchy) ShowLeaderboard();
                if (allPlayers.IndexOf(player) == index) UpdateStatsDisplay();
                break;
            }
        }

        ScoreCheck();
    }

    // Updates the player's UI stats
    public void UpdateStatsDisplay()
    {
        if (allPlayers.Count > index)
        {
            // UIController.instance.killsText.text = "Kills: " + allPlayers[index].kills;
            // UIController.instance.deathsText.text = "Deaths: " + allPlayers[index].deaths;
        }
        else
        {
            // UIController.instance.killsText.text = "Kills: 0";
            // UIController.instance.deathsText.text = "Deaths: 0";
        }
    }

    // Shows and updates the leaderboard UI
    void ShowLeaderboard()
    {
        UIController.instance.leaderboard.SetActive(true);
        foreach (var lp in lboardPlayers) Destroy(lp.gameObject);
        lboardPlayers.Clear();

        UIController.instance.leaderboardPlayerDisplay.gameObject.SetActive(false);

        List<PlayerInfo> sorted = SortPlayers(allPlayers);
        foreach (PlayerInfo player in sorted)
        {
            var newDisplay = Instantiate(UIController.instance.leaderboardPlayerDisplay,
                                         UIController.instance.leaderboardPlayerDisplay.transform.parent);
            newDisplay.SetDetails(player.name, player.kills, player.deaths);
            newDisplay.gameObject.SetActive(true);
            lboardPlayers.Add(newDisplay);
        }
    }

    // Sort players by kills
    private List<PlayerInfo> SortPlayers(List<PlayerInfo> players)
    {
        List<PlayerInfo> sorted = new List<PlayerInfo>();
        while (sorted.Count < players.Count)
        {
            int highest = -1;
            PlayerInfo selectedPlayer = players[0];
            foreach (var player in players)
            {
                if (!sorted.Contains(player) && player.kills > highest)
                {
                    selectedPlayer = player;
                    highest = player.kills;
                }
            }
            sorted.Add(selectedPlayer);
        }
        return sorted;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0); // Return to main menu
    }

    // Checks for a winner
    void ScoreCheck()
    {
        foreach (var player in allPlayers)
        {
            if (player.kills >= killsToWin && killsToWin > 0)
            {
                if (PhotonNetwork.IsMasterClient && state != GameState.Ending)
                {
                    state = GameState.Ending;
                    ListPlayersSend();
                }
                break;
            }
        }
    }

    // Called when player list or state is synced
    void StateCheck()
    {
        if (state == GameState.Ending) EndGame();
    }

    // Handles ending the game round
    void EndGame()
    {
        state = GameState.Ending;
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.DestroyAll();

        UIController.instance.endScreen.SetActive(true);
        ShowLeaderboard();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;



        StartCoroutine(EndCo());
    }

    // Wait and either restart or leave the room
    private IEnumerator EndCo()
    {
        yield return new WaitForSeconds(waitAfterEnding);

        if (!perpetual)
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (!Launcher.instance.changeMapBetweenRounds)
                {
                    NextSendMatch();
                }
                else
                {
                    int newLevel = Random.Range(0, Launcher.instance.allMaps.Length);
                    if (Launcher.instance.allMaps[newLevel] == SceneManager.GetActiveScene().name)
                        NextSendMatch();
                    else
                        PhotonNetwork.LoadLevel(Launcher.instance.allMaps[newLevel]);
                }
            }
        }
    }

    // Sends request to start a new match
    public void NextSendMatch()
    {
        PhotonNetwork.RaiseEvent((byte)EventCodes.NextMatch, null,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true });
    }

    // Resets and starts a new match
    public void NextMatchRecieve()
    {
        state = GameState.Playing;
        UIController.instance.endScreen.SetActive(false);
        UIController.instance.leaderboard.SetActive(false);

        foreach (var player in allPlayers)
        {
            player.kills = 0;
            player.deaths = 0;
        }

        UpdateStatsDisplay();
        PlayerSpawner.instance.SpawnPlayer();
        SetupTimer();
    }

    // Initializes match timer
    public void SetupTimer()
    {
        if (matchLength > 0)
        {
            currentMatchTime = matchLength;
            UpdateTimerDisplay();
        }
    }

    // Updates timer UI
    public void UpdateTimerDisplay()
    {
        var timeToDisplay = System.TimeSpan.FromSeconds(currentMatchTime);
        UIController.instance.timerText.text = timeToDisplay.Minutes.ToString("00") + " " + timeToDisplay.Seconds.ToString("00");
    }

    // Sends timer sync to all clients
    public void TimerSend()
    {
        object[] package = new object[] { (int)currentMatchTime, state };
        PhotonNetwork.RaiseEvent((byte)EventCodes.TimerSync, package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true });
    }

    // Receives synced timer from master client
    public void TimerRecieve(object[] dataReceieved)
    {
        currentMatchTime = (int)dataReceieved[0];
        state = (GameState)dataReceieved[1];
        UpdateTimerDisplay();
        UIController.instance.timerText.gameObject.SetActive(true);
    }
}

// Player data structure
[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor, kills, deaths;

    public PlayerInfo(string _name, int _actor, int _kills, int _deaths)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;
    }
}
