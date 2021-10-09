using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TimeWarp : BaseAttack
{
    TimeWarp(TimeWarp attack) : base(attack)
    { }
    public override void Cast(CombatStateMachine casterCsm, CombatStateMachine targetCsm, bool fromProjectile = false)
    {
        foreach(BaseAttack at in casterCsm.GetUnit().GetAttacks())
        {
            at.SetCooldown(0);
        }
        base.Cast(casterCsm, targetCsm, fromProjectile);
    }

    public override int GetAIValue()
    {
        int value = 0;
        CombatStateMachine casterCsm = GameManager.GetCurrentCharacter();
        foreach (BaseAttack at in casterCsm.GetUnit().GetAttacks())
        {
            value += at.GetCooldown() * 20;
        }
        return value;
    }
}
