using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ExecuteAttack : BaseAttack
{
    [SerializeField]
    float percentageIncreaseValue;
    ExecuteAttack(ExecuteAttack other) : base(other)
    { }

    public override void DealDamage(CombatStateMachine casterCsm, CombatStateMachine targetCsm)
    {
        if (targetCsm.gameObject.tag == casterCsm.gameObject.tag)
            targetCsm.Heal(GetDamage(casterCsm.GetUnit()));
        else
        {
            targetCsm.GetUnit().TakeDamage(Mathf.RoundToInt(GetDamage(casterCsm.GetUnit()) * ((targetCsm.GetUnit().GetBaseHp() - targetCsm.GetUnit().GetCurrentHp())/(float)targetCsm.GetUnit().GetBaseHp() * percentageIncreaseValue + 1)));
        }
    }
}
