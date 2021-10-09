using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemAi : TurnAction
{
    [SerializeField]
    CombatStateMachine target;
    public override TurnHandler SelectAction()
    {
        TurnHandler th = new TurnHandler();
        GameManager.SetTurnHandler(th);
        return th;
    }
    public override TurnHandler SelectAction(CombatStateMachine unit)
    {
        if (unit.GetUnit().GetAttacks()[0].GetCooldown() <= 0)
        {
            TurnHandler th = new TurnHandler(unit.GetUnit().GetAttacks()[0], unit, target);
            GameManager.SetTurnHandler(th);
            unit.SetCurState(CombatStateMachine.TurnState.Action);
            unit.Act();
        }
        unit.SetCurState(CombatStateMachine.TurnState.EndTurn);
        unit.Act();
        return null;
    }
}
