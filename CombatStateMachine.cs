using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CombatStateMachine : MonoBehaviour
{

    [SerializeField]
    Unit unit;
    TurnAction ta;
    //[SerializeField]
    Outline outline;
    [SerializeField]
    ParticleSystem healingEffect;
    bool isActing = false;
    //Movement movement;
    public enum TurnState
    {
        AddToCombat,
        Waiting,
        StartTurn,
        SelectingAbility,
        SelectingTarget,
        Action,
        MovementAfter,
        Dead,
        EndTurn
    }
    TurnState curState;

    public delegate void OnTurnStartDelegate();
    event OnTurnStartDelegate turnStartDelegate;

    public delegate void OnTurnEndDelegate();
    event OnTurnEndDelegate turnEndDelegate;

    //test start

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex > 2 && scene.buildIndex < 10)
        {
            StartCoroutine(AddToCombat());
        }
    }

    IEnumerator AddToCombat()
    {
        yield return new WaitForEndOfFrame();
        curState = TurnState.AddToCombat;
        Act();
    }

    private void Start()
    {
        if(gameObject.tag == "Ally")
            SceneManager.sceneLoaded += OnSceneLoaded;
        outline = GetComponent<Outline>();
        outline.enabled = false;
        ta = GetComponent<TurnAction>();
        healingEffect = Instantiate(healingEffect, transform);
        /*movement = GetComponent<Movement>();
        if (movement == null)
            Debug.LogError("No movement detected on object " + gameObject.name);*/
        if (ta == null)
            Debug.LogError("No turn action detected on object " + gameObject.name);
        //unit = new Unit(gameObject.name, 500, 300, 10, 10, 10, 10);
        //unit.TakeDamage(250);
        unit.DoSetup();
        unit.AddDeathDelegate(new Unit.OnDeathDelegate(SetCurStateToDead));
        if(GameManager.IsInstantiated())
        {
            StartCoroutine(AddToCombat());
        }
    }

    public void Act()
    {
        //Debug.Log("Acting, state: " + curState.ToString());
        switch (curState)
        {
            case (TurnState.AddToCombat):
                GameManager.AddUnit(this);
                curState = TurnState.Waiting;
                break;
            case (TurnState.Waiting):
                //GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 1f, 1f);
                break;
            case (TurnState.StartTurn):
                //movement.AddEndMovementDelegate(SetCurStateToAction);
                //movement.MovetToPosition(GameManager.GetTurnHandler().GetBeforePos());
                curState = TurnState.SelectingAbility;
                turnStartDelegate?.Invoke();
                Act();
                break;
            case (TurnState.SelectingAbility):
                isActing = false;
                //GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f, 1f);
                TurnHandler th = ta.SelectAction(this);
                //curState = TurnState.Waiting;
                //Act();
                break;
            case (TurnState.SelectingTarget):
                isActing = true;
                break;
            case (TurnState.Action):
                //movement.RemoveEndMovementDelegate(SetCurStateToAction);
                Animator anim = GetComponent<Animator>();
                if (anim != null)
                {
                    BaseAttack at = GameManager.GetTurnHandler().GetChosenAttack();
                    if (at.GetAffectedTargets() == BaseAttack.AffectedTargets.enemies && at.GetTargeting() == BaseAttack.TargetingSystem.unit)
                        anim.SetTrigger("attack");
                    else
                    {
                        anim.SetTrigger("magic");
                    }

                }
                curState = TurnState.MovementAfter;
                GameManager.ExecuteAction();
                /*if (movement.GetCurSteps() != 0)
                    curState = TurnState.MovementAfter;
                else
                {
                    curState = TurnState.EndTurn;
                }*/
                break;
            case(TurnState.MovementAfter):
                isActing = false;
                //movement.AddEndMovementDelegate(SetCurStateToEndTurn);
                //movement.MovetToPosition(GameManager.GetTurnHandler().GetAfterPos());
                curState = TurnState.EndTurn;
                //Act();
                break;
            case (TurnState.EndTurn):
                //movement.RemoveEndMovementDelegate(SetCurStateToEndTurn);
                turnEndDelegate?.Invoke();
                unit.EndTurn();
                curState = TurnState.Waiting;
                GameManager.EndTurn();
                Act();
                break;
            case (TurnState.Dead):
                //GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f, 1f);
                transform.position = new Vector3(200, 200, 200);
                GameManager.RemoveUnit(this);
                //this.enabled = false;
                break;
        }
    }
    

    //getters and setters
    public void SetCurState(TurnState newState = TurnState.Dead)
    {
        curState = newState;
    }
    void SetCurStateToEndTurn()
    {
        curState = TurnState.EndTurn;
        Act();
    }
    void SetCurStateToAction()
    {
        curState = TurnState.Action;
        Act();
    }
    void SetCurStateToDead()
    {
        curState = TurnState.Dead;
        Act();
    }
    public Unit GetUnit()
    {
        return unit;
    }
    public void SetUnit(Unit _unit)
    {
        unit = _unit;
    }
    public TurnState GetCurrentState()
    {
        return curState;
    }

    public void ChangeOutline(bool on)
    {
        if (on)
            outline.enabled = true;
        else
            outline.enabled = false;
    }
    public bool GetIsActing()
    {
        return isActing;
    }

    public void Heal(int amount)
    {
        unit.Heal(amount);
        healingEffect.Play();
    }

    public void AddTurnStartDelegate(OnTurnStartDelegate f)
    {
        turnStartDelegate += f;
    }

    public void AddTurnEndDelegate(OnTurnEndDelegate f)
    {
        turnEndDelegate += f;
    }

    public void RemoveTurnStartDelegate(OnTurnStartDelegate f)
    {
        turnStartDelegate -= f;
    }
    public void RemoveTurnEndDelegate(OnTurnEndDelegate f)
    {
        turnEndDelegate -= f;
    }
    /*public Movement GetMovement()
    {
        return movement;
    }*/
}
