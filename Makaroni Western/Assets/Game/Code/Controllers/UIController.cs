using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIController : MonoBehaviour
{
    public Camera observeCam;

    public Cinemachine.CinemachineVirtualCamera SettingsCam, MainMenuCam, ModeMenuCam, LobbyCam;
    public GameObject MainUI;

    public GameObject startChord;
    public GameObject soundTrack;
    public GameObject endTrack;
    public List<TextMeshProUGUI> controls;
    public List<string> russian;
    public List<string> english;
    public Sprite[] flags;
    public Image langBtnImg;

    private Cinemachine.CinemachineVirtualCamera CurrentCam;
    public string lang;
    private int langIndex = 1;
    private string[] langArray = { "ru", "en" };

    private void Start()
    {
        soundTrack.GetComponent<AudioSource>().Play();
        if (Application.systemLanguage == SystemLanguage.Russian)
            Localize("ru");
        else Localize("en");
    }
    public void ShowMainMenu()
    {
        GameStatus.ChangeState(GameState.Menu);
        ResetMenu();
        MainUI.gameObject.SetActive(true);
        MainUI.transform.GetChild(0).gameObject.SetActive(true);
        observeCam.gameObject.SetActive(true);
        MainMenuCam.gameObject.SetActive(true);
        CurrentCam = MainMenuCam;
        soundTrack.GetComponent<AudioSource>().Play();
        endTrack.GetComponent<AudioSource>().Stop();
    }

    public ResultDisplayData ShowEndgameMenu()
    {
        MainUI.SetActive(true);
        ResetMenu();
        GameObject localendgame = MainUI.transform.Find("LocalEndgame").gameObject;
        localendgame.gameObject.SetActive(true);
        endTrack.GetComponent<AudioSource>().Play();
        return localendgame.GetComponent<ResultDisplayData>();
    }

    public ResultDisplayData ShowEndgameMenuNet()
    {
        MainUI.SetActive(true);
        ResetMenu();
        GameObject localendgame = MainUI.transform.Find("NetworkEndgame").gameObject;
        localendgame.gameObject.SetActive(true);
        return localendgame.GetComponent<ResultDisplayData>();
    }

    public void ResetMenu()
    {
        for (int i = 1; i < MainUI.transform.childCount; i++)
        {
            GameObject go = MainUI.transform.GetChild(i).gameObject;
            go.SetActive(false);
        }
    }
    public void EnableCam()
    {
        observeCam.gameObject.SetActive(true);
    }
    public void PlayPressed()
    {
        ModeMenuCam.gameObject.SetActive(true);
        CurrentCam = ModeMenuCam;
        MainMenuCam.gameObject.SetActive(false);
    }
    public void ExitPressed()
    {
        Application.Quit();
    }
    public void SettingsPressed()
    {
        SettingsCam.gameObject.SetActive(true);
        CurrentCam = SettingsCam;
        MainMenuCam.gameObject.SetActive(false);
    }
    public void BackPressed()
    {
        CurrentCam.gameObject.SetActive(false);
        MainMenuCam.gameObject.SetActive(true);
        CurrentCam = MainMenuCam;
    }
    public void StartGame()
    {
        observeCam.gameObject.SetActive(false);
        MainUI.SetActive(false);
        CurrentCam.gameObject.SetActive(false);
        soundTrack.GetComponent<AudioSource>().Stop();
        startChord.GetComponent<AudioSource>().Play();
        endTrack.GetComponent<AudioSource>().Stop();
    }
    public void LobbyPressed()
    {
        CurrentCam.gameObject.SetActive(false);
        LobbyCam.gameObject.SetActive(true);
        CurrentCam = LobbyCam;
    }
    public void LobbyBackPressed()
    {
        CurrentCam.gameObject.SetActive(false);
        ModeMenuCam.gameObject.SetActive(true);
        CurrentCam = ModeMenuCam;
    }
    public void SwitchBtn()
    {
        if (langIndex != langArray.Length) langIndex++;
        else langIndex = 1;
        lang = langArray[langIndex-1];
        Localize(lang);
        langBtnImg.sprite = flags[langIndex - 1];
    }
    private void Localize(string newlang) 
    {
        if (newlang == "ru")
        {
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].text = russian[i];
            }
        }
        else
        {
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].text = english[i];
            }
        }
    }
}