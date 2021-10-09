using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu]
public class DashAttack : BaseAttack
{
    DashAttack(DashAttack other) : base(other)
    { }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 300; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    public override void Cast(CombatStateMachine casterCsm, CombatStateMachine targetCsm, bool fromProjectile = false)
    {
        if (spellCaster != null)
            spellCaster.SetProjectileColor(projectileColor);
        cooldown = baseCooldown;
        Unit caster = casterCsm.GetUnit();
        if (!isRanged || fromProjectile)
        {
            if (!fromProjectile)
            {
                caster.ConsumeMP(attackCost);
            }
            Vector3 teleportPosition;
            if (RandomPoint(targetCsm.transform.position, 2f, out teleportPosition))
                casterCsm.transform.position = teleportPosition;
            DealDamage(casterCsm, targetCsm);
            if (!object.ReferenceEquals(spellEffect, null))
            {
                spellEffect.SetValue(effectValue);
                spellEffect.SetBaseDuration(effectDuration);
                spellEffect.AddEffect(targetCsm);
            }
            if (!fromProjectile)
            {
                GameManager.SpellHasEnded();
            }
        }
        else
        {
            caster.ConsumeMP(attackCost);
            CastProjectile(casterCsm, targetCsm);
            ready = 1;
        }
    }
}
