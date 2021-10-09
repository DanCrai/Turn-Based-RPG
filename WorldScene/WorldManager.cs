using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    [SerializeField]
    GameObject spellPanel;
    [SerializeField]
    GameObject statsPanel;
    [SerializeField]
    GameObject optionsPanel;
    static GameObject currentAlly;

    static public void SetCurrentAlly(GameObject ally)
    {
        currentAlly = ally;
    }
    static public GameObject GetCurrentAlly()
    {
        return currentAlly;
    }
    public void OpenSpellPanel()
    {
        spellPanel.SetActive(true);
    }
    public void CloseSpellPanel()
    {
        spellPanel.SetActive(false);
    }
    public void OpenStatsPanel()
    {
        statsPanel.SetActive(true);
    }
    public void CloseStatsPanel()
    {
        statsPanel.SetActive(false);
    }
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(2 + GeneralManager.GetLevelCounter());
    }
    public void OpenOptionsPanel()
    {
        optionsPanel.SetActive(true);
    }
    public void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
    }
}
