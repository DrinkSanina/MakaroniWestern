using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LGameController : MonoBehaviour
{
    public GameObject HumanPlayerPrefab;
    public GameObject ComputerPlayerPrefab;

    public SpawnPointMarkup spawnpoint1;
    public SpawnPointMarkup spawnpoint2;

    public UIController uicontroller;

    public Cinemachine.CinemachineVirtualCamera killCamPrefab;

    public GameObject databaseContent;
    public GameObject recordPrefab;

    private List<PlayerController> players;

    private List<GameObject> subs;

    void Start()
    {
        players = new List<PlayerController>();
        subs = new List<GameObject>();
        GameStatus.ChangeState(GameState.Menu);
    }

    public void StartLocal()
    {
        GameObject humanPlayer = Instantiate(HumanPlayerPrefab, spawnpoint1.transform.position, Quaternion.LookRotation(spawnpoint2.transform.position - spawnpoint1.transform.position));
        humanPlayer.name = "Player";
        GameObject compPlayer = Instantiate(ComputerPlayerPrefab, spawnpoint2.transform.position, Quaternion.LookRotation(spawnpoint1.transform.position - spawnpoint2.transform.position));
        compPlayer.name = "Computer";
        compPlayer.GetComponent<ComputerCowboy>().EnemyTarget = humanPlayer.transform.Find("BodyTarget");
        
        players.Add(humanPlayer.GetComponent<PlayerController>());
        players.Add(compPlayer.GetComponent<PlayerController>());

        GameStatus.ChangeState(GameState.Duel);
        GameStatus.GameType = GameType.Local;

        uicontroller.StartGame();
    }

    void Update()
    {
        if (GameStatus.GameType != GameType.Local) return;

        if(GameStatus.CurrentState == GameState.Duel)
        {
            foreach (PlayerController pc in players)
            {
                if (pc.health <= 0)
                {
                    PlayerController alive = Alive(pc);

                    //Действия мертвеца
                    pc.GetComponent<Animator>().enabled = false;
                    Weapon playerWeapon = pc.gameObject.GetComponentInChildren<Weapon>();
                    Destroy(playerWeapon.gameObject);

                    //Действия выжившего
                    alive.GetComponent<Animator>().SetBool("HasWon", true);
                    foreach (MultiAimConstraint mac in alive.GetComponentsInChildren<MultiAimConstraint>())
                    {
                        var data = mac.data.sourceObjects;
                        data.SetWeight(0, 0);
                        mac.data.sourceObjects = data;
                    }
                    alive.GetComponent<PlayerController>().enabled = false;

                    GameOver();
                }
            }
        }
        
    }

    private void GameOver()
    {
        ResultDisplayData rdd = uicontroller.ShowEndgameMenu();

        LocalCowboy lc = null;

        for(int i = 0; i < players.Count;i++)
        {
            if (players[i].GetComponent<LocalCowboy>() != null)
                lc = players[i].GetComponent<LocalCowboy>();

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
            }
                
            rdd.labels[i].text = $"{players[i].name}\nHealth: {health}\nConcentration: {Mathf.Round(players[i].concentration)}\nShots Fired: {players[i].shots}"; 
        }


        lc.cineCamera.gameObject.SetActive(false);
        lc.UI.gameObject.SetActive(false);

        Cinemachine.CinemachineVirtualCamera killcam = Instantiate(killCamPrefab);
        subs.Add(killcam.gameObject);
        killcam.transform.position = GameObject.Find("KillCamPos").transform.position;
        killcam.Follow = GameObject.Find("MapCenter").transform;
        killcam.LookAt = GameObject.Find("MapCenter").transform;

        DatabaseRecord();

        GameStatus.ChangeState(GameState.Final);
    }

    private void DatabaseRecord()
    {
        string status = "";
        if (players[0].health > 0)
            status = "won";
        else
            status = "lost";

        Results battle = new Results(players[0], players[1], status);

        DatabaseController.AddRecord(battle);
    }

    public void MainMenu()
    {
        ClearData();
        uicontroller.ShowMainMenu();
    }

    public void TryAgain()
    {
        ClearData();
        StartLocal();
    }


    private PlayerController Alive(PlayerController dead)
    {
        foreach(PlayerController pc in players)
        {
            if(pc != dead)
            {
                return pc;
            }
        }
        return null;
    }

    public void ClearData()
    {
        foreach(PlayerController pc in players)
        {
            Destroy(pc.gameObject);
        }
        foreach(GameObject go in subs)
        {
            Destroy(go.gameObject);
        }
        subs.Clear();
        players.Clear();
    }

    public void ResetData()
    {
        DatabaseController.Clean();
        CleanRecords();
        ShowRecords();
    }

    public void ShowRecords()
    {
        List<Results> records = DatabaseController.ReadDatabase();

        foreach(Results r in records)
        {
            GameObject UIRecord = Instantiate(recordPrefab, databaseContent.transform);
            RecordDataDisplay rdd = UIRecord.GetComponent<RecordDataDisplay>();

            rdd.id.text = r.match_id.ToString();
            rdd.player_1.text = r.player_1;
            rdd.player_2.text = r.player_2;
            rdd.health_1.text = Mathf.Round(r.remaining_hp_1).ToString();
            rdd.health_2.text = Mathf.Round(r.remaining_hp_2).ToString();
            rdd.conc_1.text = Mathf.Round(r.concentration_1).ToString();
            rdd.conc_2.text = Mathf.Round(r.concentration_2).ToString();
            rdd.shots_1.text = r.shots_number_1.ToString();
            rdd.shots_2.text = r.shots_number_2.ToString();
            rdd.status.text = r.status;

            if (r.status == "lost")
                rdd.status.color = Color.red;
            else
                rdd.status.color = Color.green;
        }
    }

    public void CleanRecords()
    {
        for(int i = 0; i < databaseContent.transform.childCount;i++)
        {
            Destroy(databaseContent.transform.GetChild(i).gameObject);
        }
    }
}
