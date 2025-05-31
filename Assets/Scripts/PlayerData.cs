using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayerData : MonoBehaviourPunCallbacks, IPunObservable
{
    public string playerName;

    public int playerId;

    public float hasEarned;

    public int CorrectAnswers;

    public List<string> questions;
    public List<string> correctAnsweredQuestions = new List<string>();
    public List<string> uncorrectAnsweredQuestions = new List<string>();

    public bool isFinishedTurn = false;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(playerName);
            stream.SendNext(playerId);
            stream.SendNext(hasEarned);
            stream.SendNext(CorrectAnswers);
            stream.SendNext(isFinishedTurn);
            stream.SendNext(questions);
            stream.SendNext(correctAnsweredQuestions);
            stream.SendNext(uncorrectAnsweredQuestions);
        }
        else
        {
            // Network player, receive data
            playerName = (string)stream.ReceiveNext();
            playerId = (int)stream.ReceiveNext();
            hasEarned = (float)stream.ReceiveNext();
            CorrectAnswers = (int)stream.ReceiveNext();
            isFinishedTurn = (bool)stream.ReceiveNext();
            questions = (List<string>)stream.ReceiveNext();
            correctAnsweredQuestions = (List<string>)stream.ReceiveNext();
            uncorrectAnsweredQuestions = (List<string>)stream.ReceiveNext();
        }
    }


}
