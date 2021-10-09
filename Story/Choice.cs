using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice : MonoBehaviour
{
    [SerializeField]
    GameObject choice;
    private void Start()
    {
        choice.SetActive(false);
    }
    public void ShowChoice()
    {
        choice.SetActive(true);
    }
    public void Choice1()
    {
        GeneralManager.MakeChoice(0);
        GeneralManager.Victory();
    }
    public void Choice2()
    {
        GeneralManager.MakeChoice(1);
        GeneralManager.Victory();
    }
    public void Choice3()
    {
        GeneralManager.MakeChoice(2);
        GeneralManager.Victory();
    }
}
