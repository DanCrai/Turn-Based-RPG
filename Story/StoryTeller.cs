using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryTeller : MonoBehaviour
{
    [SerializeField]
    List<GameObject> acts = new List<GameObject>();
    int contor = 1;
    [SerializeField]
    int splittingAct;
    [SerializeField]
    int chosenAct = -1;
    [SerializeField]
    int specialAct;
    [SerializeField]
    bool atTheStart;
    void Start()
    {
        acts[0].SetActive(true);
        StartCoroutine(Story());
    }

    IEnumerator Story()
    {
        int i = -1;
        if (acts.Count > contor)
        {
            if (contor == splittingAct)
            {
                i = Random.Range(0, 3);
                GeneralManager.SetScenario(i);
                acts[contor + i].SetActive(true);
                Text text = acts[contor].GetComponent<Text>();
                Color color = text.color;
                color.a = 0f;
                text.color = color;
                while (color.a < 1f)
                {
                    yield return new WaitForSeconds(0.01f);
                    color.a += 1f / 255;
                    text.color = color;
                }
            }
            else if (contor == specialAct)
            {
                acts[contor].SetActive(true);
                yield return new WaitUntil(() => chosenAct != -1);
                Debug.Log("CHOSEN ACT: " + chosenAct);
                Debug.Log(contor + chosenAct + 1);
                acts[contor].SetActive(false);
                contor = contor + chosenAct + 1;
                StartCoroutine(Story());
                yield break;
            }
            else
            {
                acts[contor].SetActive(true);
                Text text = acts[contor].GetComponent<Text>();
                Color color = text.color;
                color.a = 0f;
                text.color = color;
                while (color.a < 1f)
                {
                    yield return new WaitForSeconds(0.01f);
                    if (Input.GetKey(KeyCode.Space) && (color.a > 0.1f))
                    {
                        color.a = 1f;
                    }
                    else
                        color.a += 1f / 255;
                    text.color = color;
                }
            }
        }
        else
        {
            acts[0].SetActive(false);
            if(atTheStart)
                GameManager.StartCombat();
        }
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        if (chosenAct != -1)
            GeneralManager.LoadNewScene();
        if (i != -1)
            acts[contor + i].SetActive(false);
        else
            acts[contor].SetActive(false);
        if (contor == splittingAct)
            contor = contor + 3;
        else
            contor++;
        yield return new WaitForEndOfFrame();
        StartCoroutine(Story());
    }

    public void ChooseEnding(int ending)
    {
        int scenario = GeneralManager.GetScenario();
        Debug.Log("SCENARIO: " + scenario);
        switch(ending)
        {
            case (0):
                if (scenario == 0)
                    chosenAct = 0;
                else
                    chosenAct = 1;
                break;
            case (1):
                if (scenario == 1)
                    chosenAct = 2;
                else
                    chosenAct = 3;
                break;
            case (2):
                if (scenario == 2)
                    chosenAct = 4;
                else
                    chosenAct = 5;
                break;
            case (3):
                chosenAct = 6;
                break;
            case (4):
                chosenAct = 7;
                break;
        }
    }
}
