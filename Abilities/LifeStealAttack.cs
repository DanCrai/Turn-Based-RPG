using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LifeStealAttack : BaseAttack
{
    [SerializeField]
    float lifestealPercentage;
    LifeStealAttack(LifeStealAttack lsAttack) : base(lsAttack)
    { }
    public override void Cast(CombatStateMachine casterCsm, CombatStateMachine targetCsm, bool fromProjectile = false)
    {
        targetCsm.GetUnit().AddGetDamageTakenDelegate(LifeSteal);
        base.Cast(casterCsm, targetCsm, fromProjectile);
    }

    public void LifeSteal(int amount)
    {
        GameManager.GetCurrentCharacter().GetUnit().Heal(Mathf.RoundToInt(amount * lifestealPercentage / 100));

    }
}
