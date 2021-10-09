using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class GroundAttack : BaseAttack
{
    [field : SerializeField]
    float aoeRange { get; set; }


    public GroundAttack(string _attackName, int _attackDamage, int _attackCost, float _range, TargetingSystem _targeting, AffectedTargets _affectedTargets, int _baseCooldown, string _attackFormula, float _aoeRange, int _levelRequired) : base(_attackName, _attackDamage, _attackCost, _range, _targeting, _affectedTargets, _baseCooldown, _attackFormula, true, _levelRequired)
    {
        aoeRange = _aoeRange;
    }

    public GroundAttack(BaseAttack attack, float _aoeRange) : base(attack)
    {
        aoeRange = _aoeRange;
    }
    public GroundAttack(GroundAttack other) : base(other)
    {
        aoeRange = other.aoeRange;
    }
    public override void Cast(CombatStateMachine casterCsm)
    {
        cooldown = baseCooldown;
        casterCsm.GetUnit().ConsumeMP(attackCost);
        Vector3 targetPosition;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(ray, out hit))
        {
            targetPosition = hit.point;
            spellCaster.InstantiateProjectile(casterCsm.transform.position, Quaternion.identity, casterCsm, this, targetPosition);
        }
    }

    public override void DealDamage(CombatStateMachine casterCsm, CombatStateMachine targetCsm)
    {
        targetCsm.GetUnit().TakeDamage(GetDamage(casterCsm.GetUnit()));
    }
    public float GetAoeRange()
    {
        return aoeRange;
    }
}
