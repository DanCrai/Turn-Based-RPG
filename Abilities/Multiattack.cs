using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Multiattack : BaseAttack
{
    Multiattack(Multiattack other) : base(other)
    { }

    public override void Cast(CombatStateMachine casterCsm, CombatStateMachine targetCsm, bool fromProjectile = false)
    {
        casterCsm.SetCurState(CombatStateMachine.TurnState.SelectingAbility);
        base.Cast(casterCsm, targetCsm, fromProjectile);
        GameManager.ResetAbilityPanel();
    }
}
