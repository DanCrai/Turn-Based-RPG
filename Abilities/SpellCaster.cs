using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [SerializeField]
    protected GameObject projectile;
    [SerializeField]
    protected GameObject aoe;
    Color projectileColor;
    public virtual void InstantiateProjectile(Vector3 position, Quaternion rotation, CombatStateMachine casterCsm, BaseAttack attack, Vector3 targetPosition)
    {
        GameObject proj = Instantiate(projectile, position, rotation);
        proj.GetComponent<MeshRenderer>().material.color = projectileColor;
        TrailRenderer tr = proj.GetComponentInChildren<TrailRenderer>();
        tr.startColor = projectileColor;
        //some height offset to not aim towards the ground
        targetPosition.y += 0.5f;
        proj.GetComponent<ProjectileMovement>().Set(casterCsm, attack, targetPosition, aoe);
    }
    public virtual void InstantiateProjectile(Vector3 position, Quaternion rotation, CombatStateMachine casterCsm, GroundAttack attack, Vector3 targetPosition)
    {
        GameObject proj = Instantiate(projectile, position, rotation);
        proj.GetComponent<MeshRenderer>().material.color = projectileColor;
        TrailRenderer tr = proj.GetComponentInChildren<TrailRenderer>();
        if(tr != null)
            tr.startColor = projectileColor;
        //some height offset to not aim towards the ground
        //targetPosition.y += 0.5f;
        proj.GetComponent<ProjectileMovement>().Set(casterCsm, attack, targetPosition, aoe);
    }
    public void SetProjectileColor(Color _projectileColor)
    {
        projectileColor = _projectileColor;
    }
}
