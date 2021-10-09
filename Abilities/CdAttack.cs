using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CdAttack : BaseAttack
{
    CdAttack(CdAttack cdAttack) : base(cdAttack)
    { }
    public override void Cast(CombatStateMachine casterCsm, CombatStateMachine targetCsm, bool fromProjectile = false)
    {
        foreach(BaseAttack attack in casterCsm.GetUnit().GetAttacks())
        {
            attack.DecreaseCooldown();
        }
        base.Cast(casterCsm, targetCsm, fromProjectile);
    }
}
