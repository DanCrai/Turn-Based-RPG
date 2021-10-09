using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Backstab : BaseAttack
{
    Backstab(Backstab other) : base(other)
    { }


    public override void DealDamage(CombatStateMachine casterCsm, CombatStateMachine targetCsm)
    {
        if (targetCsm.gameObject.tag == casterCsm.gameObject.tag)
            targetCsm.Heal(GetDamage(casterCsm.GetUnit()));
        else
        {
            Vector3 delta = casterCsm.transform.position - targetCsm.transform.position;
            float angle = Vector3.Angle(targetCsm.transform.forward, delta);
            float backstabbiness = Mathf.Abs(angle) / 180f;
            if(backstabbiness < 0.8f)
                targetCsm.GetUnit().TakeDamage(GetDamage(casterCsm.GetUnit()));
            else
                targetCsm.GetUnit().TakeDamage(2 * GetDamage(casterCsm.GetUnit()));
        }
    }
}
