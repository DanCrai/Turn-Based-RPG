using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAction : MonoBehaviour
{
    public virtual TurnHandler SelectAction()
    {
        TurnHandler th = new TurnHandler();
        GameManager.SetTurnHandler(th);
        return th;
    }
    public virtual TurnHandler SelectAction(CombatStateMachine unit)
    {
        TurnHandler th = new TurnHandler(unit.GetUnit().GetAttacks()[0], unit, GameManager.GetRandomUnit(unit));
        GameManager.SetTurnHandler(th);
        unit.SetCurState(CombatStateMachine.TurnState.Action);
        return th;
    }
}
