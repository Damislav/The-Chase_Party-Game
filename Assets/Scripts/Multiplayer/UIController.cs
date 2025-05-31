using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class UIController : MonoBehaviour
{
  public static UIController instance;



  public GameObject leaderboard;
  public LeaderboardPlayer leaderboardPlayerDisplay;

  public GameObject endScreen;

  public TMP_Text timerText;

  void Awake()
  {
    instance = this;
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (Cursor.lockState != CursorLockMode.None)
      {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
      }
    }
  }

  public void ReturnToMainMenu()
  {
    PhotonNetwork.AutomaticallySyncScene = false;
    PhotonNetwork.LeaveRoom();
  }

  public void QuitGame()
  {
    Application.Quit();
  }
}
