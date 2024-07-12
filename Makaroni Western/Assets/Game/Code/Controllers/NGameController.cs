using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Animations.Rigging;

public class NGameController : NetworkBehaviour
{
    public NetworkManagerLobby netmanager;
    public UIController uicontroller;
    public Cinemachine.CinemachineVirtualCamera killCamPrefab;

    private List<NetworkCowboy> players;

    private List<GameObject> subs;

    [ClientRpc]
    public void StartNetwork(List<NetworkCowboy> _players)
    {
        subs = new List<GameObject>();
        uicontroller.StartGame();

        GameStatus.GameType = GameType.Network;
        GameStatus.ChangeState(GameState.Duel);

        players = _players;
    }

    private void Update()
    {
        if(GameStatus.CurrentState == GameState.Duel && GameStatus.GameType == GameType.Network)
        foreach(NetworkCowboy nc in players)
        {
            if(nc.health <= 0)
                Death(nc);
        }
    }

    private void Death(NetworkCowboy pc)
    {
        GameOverScreen();
    }

    private void GameOverScreen()
    {

        ResultDisplayData rdd = uicontroller.ShowEndgameMenuNet();
        
        for (int i = 0; i < players.Count; i++)
        {
            players[i].cineCamera.gameObject.SetActive(false);
            players[i].playerCamera.gameObject.SetActive(false);
            players[i].UI.gameObject.SetActive(false);

            string health;
            if (players[i].health <= 0)
            {
                rdd.labelStatus[i].text = "Dead";
                rdd.labelStatus[i].color = Color.red;
                rdd.images[i].sprite = rdd.DeadPicture;
                health = "0";
            }
            else
            {
                rdd.labelStatus[i].text = "Alive";
                rdd.labelStatus[i].color = Color.green;
                rdd.images[i].sprite = rdd.AlivePicture;
                health = players[i].health.ToString();

                players[i].GetComponent<Animator>().SetBool("HasWon", true);
            }

            rdd.labels[i].text = $"{players[i].playerName}\nHealth: {health}\nConcentration: {Mathf.Round(players[i].concentration)}\nShots Fired: {players[i].shots}";
        }


        Cinemachine.CinemachineVirtualCamera killcam = Instantiate(killCamPrefab);
        
        uicontroller.observeCam.gameObject.SetActive(true);
        subs.Add(killcam.gameObject);
        killcam.transform.position = GameObject.Find("KillCamPos").transform.position;
        killcam.Follow = GameObject.Find("MapCenter").transform;
        killcam.LookAt = GameObject.Find("MapCenter").transform;

        DatabaseRecord();

        GameStatus.ChangeState(GameState.Final);

        
    }

    [Client]
    private void DatabaseRecord()
    {
        int myIndex;
        for (myIndex = 0; myIndex < players.Count; myIndex++)
        {
            if (players[myIndex].hasAuthority)
                break;
        }

        string status = "";
        if (players[myIndex].health > 0)
            status = "won";
        else
            status = "lost";

        int nextIndex = 0;
        if (myIndex == 0) nextIndex = 1;
        else if (myIndex == 1) nextIndex = 0;
        
        Results battle = new Results(players[myIndex], players[nextIndex], status);

        DatabaseController.AddRecord(battle);


    }

    [Client]
    public void ClearData()
    {
        if (isServer) netmanager.StopHost();
        if (isClient) netmanager.StopClient();

 

        foreach (NetworkCowboy pc in players)
        {
            if(pc != null)
                Destroy(pc.gameObject);
        }
        foreach (GameObject go in subs)
        {
            if(go != null)
                Destroy(go.gameObject);
        }

        subs.Clear();
        players.Clear();

        uicontroller.ShowMainMenu();
    }
}
