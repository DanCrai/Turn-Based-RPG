using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aoe : MonoBehaviour
{
    SphereCollider collider;
    List<CombatStateMachine> targets;
    GroundAttack attack;
    CombatStateMachine caster;
    ParticleSystem particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<SphereCollider>();
        targets = new List<CombatStateMachine>();
        particleSystem = GetComponent<ParticleSystem>();
        StartCoroutine("Explode");
    }
    public void Set(CombatStateMachine _caster, GroundAttack _attack)
    {
        caster = _caster;
        attack = _attack;
        transform.localScale *= attack.GetAoeRange() / 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        CombatStateMachine csm = other.GetComponent<CombatStateMachine>();
        if (csm != null)
            targets.Add(csm);
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.1f);
        switch (attack.GetAffectedTargets())
        {
            case BaseAttack.AffectedTargets.all:
                foreach (CombatStateMachine csm in targets)
                {
                    attack.Cast(caster, csm, true);
                }
                break;
            case BaseAttack.AffectedTargets.allies:
                foreach (CombatStateMachine csm in targets)
                {
                    if(csm.gameObject.tag == caster.gameObject.tag)
                        attack.Cast(caster, csm, true);
                }
                break;
            case BaseAttack.AffectedTargets.enemies:
                foreach (CombatStateMachine csm in targets)
                {
                    if (csm.gameObject.tag != caster.gameObject.tag)
                        attack.Cast(caster, csm, true);
                }
                break;
        }
        particleSystem.Play();
        yield return new WaitForSeconds(particleSystem.main.duration);
        GameManager.SpellHasEnded();
        Destroy(this.gameObject);
    }
}
