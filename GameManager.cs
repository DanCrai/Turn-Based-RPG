using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static bool inCombat = true;
    List<CombatStateMachine> unitsInCombat;
    List<CombatStateMachine> targetableUnits;
    static GameManager GM;
    TurnHandler curTurnHandler;
    CombatStateMachine currentCharacter;
    [SerializeField]
    GameObject AbilityPanel;
    [SerializeField]
    GameObject AbilityButton;
    [SerializeField]
    int levelScore;
    [SerializeField]
    int scoreDeductionPerTurn;
    bool isStarting = true;

    void Awake()
    {
        unitsInCombat = new List<CombatStateMachine>();
        targetableUnits = new List<CombatStateMachine>();
        GM = this;
    }
    private void Start()
    {
        if(GetComponent<StoryTeller>() == null)
            StartCoroutine("cor");
        isStarting = true;
    }

    static public void StartCombat()
    {
        GM.isStarting = true;
        GM.StartCoroutine(GM.cor());
    }

    private static int SortByInitiative(CombatStateMachine x, CombatStateMachine y)
    {
        return (y.GetUnit().GetInitiative() - x.GetUnit().GetInitiative());
    }

    IEnumerator cor()
    {
        yield return new WaitForSeconds(1f);
        if (isStarting)
        {
            unitsInCombat.Sort(SortByInitiative);
            isStarting = false;
        }
        Fight();
        //StartCoroutine("cor");
    }

    void Fight()
    {
        bool defeat = true;
        bool victory = true;
        foreach(CombatStateMachine character in unitsInCombat)
        {
            if (character.gameObject.tag == "Ally")
                defeat = false;
            if (character.gameObject.tag == "Enemy")
                victory = false;
        }
        if(defeat)
        {
            GeneralManager.Defeat();
        }
        else if(victory)
        {
            GeneralManager.AddScore(levelScore);
            if(SceneManager.GetActiveScene().buildIndex > 6)
            {
                if(GeneralManager.GetScore() > GeneralManager.GetHighScore())
                    PlayFabLogin.SetStats();
            }
            Choice choice = GetComponent<Choice>();
            if (choice != null)
                choice.ShowChoice();
            else
                GeneralManager.Victory();
        }
        else
        {
            UnitMovement um;
            curTurnHandler = new TurnHandler();
            if (currentCharacter)
            {
                um = currentCharacter.GetComponent<UnitMovement>();
                if(um)
                    currentCharacter.GetComponent<UnitMovement>().EndTurn();
            }
            currentCharacter = unitsInCombat[0];
            CameraManager.CenterCameraOnPosition(currentCharacter.transform.position);
            if(currentCharacter.GetComponent<PlayerAi>() != null)
                CreateAbilityPanel(currentCharacter);
            um = currentCharacter.GetComponent<UnitMovement>();
            if(um)
                currentCharacter.GetComponent<UnitMovement>().StartTurn();
            currentCharacter.SetCurState(CombatStateMachine.TurnState.StartTurn);
            currentCharacter.Act();
            unitsInCombat.RemoveAt(0);
            unitsInCombat.Add(currentCharacter);
        }
        levelScore -= scoreDeductionPerTurn;
    }

    static public void ResetAbilityPanel()
    {
        GM.CreateAbilityPanel(GetCurrentCharacter());
    }

    void CreateAbilityPanel(CombatStateMachine csm)
    {
        AbilityPanel.SetActive(true);
        int abilityNumber = 0;
        Unit unit = csm.GetUnit();
        List<BaseAttack> abilities = unit.GetAttacks();
        int numberOfAbilities = abilities.Count;
        foreach (Transform child in AbilityPanel.transform)
        {
            if (abilityNumber < numberOfAbilities)
            {
                child.gameObject.SetActive(true);
                AbilitySelectButton asb = child.gameObject.GetComponent<AbilitySelectButton>();
                asb.enabled = true;
                asb.SetAbilitySelectButton(unit.GetAttacks()[abilityNumber], currentCharacter);
                Button button = child.gameObject.GetComponent<Button>();
                if (unit.GetAttacks()[abilityNumber].CanBeCast(currentCharacter.GetUnit()))
                    button.interactable = true;
                else
                    button.interactable = false;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { asb.SelectAbility(); });
                abilityNumber++;
            }
            else if (abilityNumber == numberOfAbilities)
            {
                child.gameObject.SetActive(true);
                AbilitySelectButton asb = child.gameObject.GetComponent<AbilitySelectButton>();
                asb.enabled = false;
                Button button = child.gameObject.GetComponent<Button>();
                button.interactable = true;
                button.GetComponent<Image>().sprite = null;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate { EndTurnForCharacter(); });
                button.GetComponentInChildren<Text>().text = "End turn";
                abilityNumber++;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    void DestroyAbilityPanel()
    {
        /*foreach (Transform child in AbilityPanel.transform)
        {
            child.gameObject.SetActive(true);
        }*/
        AbilityPanel.SetActive(false);
    }

    public static void DisableAbilitiesPanel()
    {
        int abilityNumber = 0;
        Unit unit = GM.currentCharacter.GetUnit();
        List<BaseAttack> abilities = unit.GetAttacks();
        int numberOfAbilities = abilities.Count;
        foreach (Transform child in GM.AbilityPanel.transform)
        {
            if (abilityNumber < numberOfAbilities)
            {
                child.gameObject.GetComponent<Button>().interactable = false;
                abilityNumber++;
            }
            else
            {
                break;
            }
        }
    }

    static public void Defeat(Unit unit)
    {
        inCombat = false;
    }
    static public void AddUnit(CombatStateMachine csm)
    {
        GM.unitsInCombat.Add(csm);
        GM.targetableUnits.Add(csm);
    }
    static public void RemoveUnit(CombatStateMachine csm)
    {
        GM.unitsInCombat.Remove(csm);
        GM.targetableUnits.Remove(csm);
    }
    static public void AddTargetableUnit(CombatStateMachine csm)
    {
        GM.targetableUnits.Add(csm);
    }
    static public void RemoveTargetableUnit(CombatStateMachine csm)
    {
        GM.targetableUnits.Remove(csm);
    }

    static public CombatStateMachine GetRandomUnit(CombatStateMachine csm)
    {
        while(true)
        {
            int rand = UnityEngine.Random.Range(0, GM.unitsInCombat.Count);
            if (GM.unitsInCombat[rand] != csm)
                return GM.unitsInCombat[rand];
        }
    }
    static public CombatStateMachine GetRandomUnit(Unit unit)
    {
        while (true)
        {
            int rand = UnityEngine.Random.Range(0, GM.unitsInCombat.Count);
            if (GM.unitsInCombat[rand].GetUnit() != unit)
                return GM.unitsInCombat[rand];
        }
    }

    static public List<CombatStateMachine> GetUnitsWithTag(String tag)
    {
        List<CombatStateMachine> result = new List<CombatStateMachine>();
        foreach(CombatStateMachine csm in GM.targetableUnits)
        {
            if (csm.tag == tag)
                result.Add(csm);
        }
        return result;
    }
    static public TurnHandler GetTurnHandler()
    {
        return GM.curTurnHandler;
    }
    static public void SetTurnHandler(TurnHandler newTurnHandler)
    {
        GM.curTurnHandler = newTurnHandler;
    }
    static public void ExecuteAction()
    {
        GM.curTurnHandler.ExecuteAction();
    }
    static public void EndTurn()
    {
        GM.DestroyAbilityPanel();
        GM.StartCoroutine("cor");
    }
    static public CombatStateMachine GetCurrentCharacter()
    {
        return GM.currentCharacter;
    }

    static public void SpellHasEnded()
    {
        GetCurrentCharacter().Act();
    }

    static void EndTurnForCharacter()
    {
        GetCurrentCharacter().SetCurState(CombatStateMachine.TurnState.EndTurn);
        GetCurrentCharacter().Act();
    }

    static public bool IsInstantiated()
    {
        if (GM != null)
            return true;
        else
            return false;
    }
}
