using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralManager : MonoBehaviour
{
    static GeneralManager GM;
    int levelCounter = 1;
    int scenario = -1;
    int choice = -1;
    int score = 0;
    int highscore = 0;

    private void Start()
    {
        if (GM == null)
            GM = this;
        PlayFabLogin.GetStats();
    }
    static public void Defeat()
    {
        CombatStateMachine[] units = GameObject.FindGameObjectWithTag("Party").GetComponentsInChildren<CombatStateMachine>();
        foreach (CombatStateMachine unit in units)
        {
            unit.GetUnit().Restore();
        }
        SceneManager.LoadScene(2);
    }
    static public void Victory()
    {
        GM.levelCounter++;
        CombatStateMachine[] units = GameObject.FindGameObjectWithTag("Party").GetComponentsInChildren<CombatStateMachine>();
        foreach(CombatStateMachine unit in units)
        {
            unit.GetUnit().LevelUp();
            unit.GetUnit().Restore();
        }
        SaveManager.Save();
        SceneManager.LoadScene(2);
    }
    public void LoadPartyCreation()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadParty()
    {
        SaveManager.Load();
    }
    public void LoadSampleLevel()
    {
        Destroy(GameObject.FindGameObjectWithTag("Party"));
        SceneManager.LoadScene(11);
    }

    static public void LoadNewScene()
    {
        SceneManager.LoadScene(10);
    }
    static public void FinishedLoading()
    {
        SceneManager.LoadScene(2);
    }
    static public int GetLevelCounter()
    {
        return GM.levelCounter;
    }
    static public void SetLevelCounter(int lvl)
    {
        GM.levelCounter = lvl;
    }
    static public int GetScore()
    {
        return GM.score;
    }
    static public void SetScore(int _score)
    {
        GM.score = _score;
    }
    static public void AddScore(int amount)
    {
        GM.score += amount;
    }
    static public int GetHighScore()
    {
        return GM.highscore;
    }
    static public void SetHighScore(int _score)
    {
        GM.highscore = _score;
    }
    static public void SetScenario(int _scenario)
    {
        GM.scenario = _scenario;
    }
    static public int GetScenario()
    {
        return GM.scenario;
    }
    static public void MakeChoice(int _choice)
    {
        GM.choice = _choice;
        GM.levelCounter += GM.choice;
    }

    static public void ChangeFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Debug.Log(Screen.fullScreen);
    }
}
