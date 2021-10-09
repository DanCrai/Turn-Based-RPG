using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetsInRange : MonoBehaviour
{
    List<CombatStateMachine> targetsInRange = new List<CombatStateMachine>();
    List<TargetSelect> targetsDelegated = new List<TargetSelect>();
    [SerializeField]
    AimLine aimLine;
    private void OnTriggerEnter(Collider other)
    {
        CombatStateMachine csm = other.GetComponent<CombatStateMachine>();
        if (csm != null)
        {
            targetsInRange.Add(csm);
            //csm.ChangeOutline(true);
        }
    }
    private void OnDisable()
    {
        foreach (CombatStateMachine csm in targetsInRange)
            csm.ChangeOutline(false);
        foreach (TargetSelect target in targetsDelegated)
        {
            target.RemoveOnHoverDelegate(OutlineTarget);
            target.RemoveOnHoverExitDelegate(RemoveOutlineTarget);
        }
        if(aimLine != null)
            aimLine.gameObject.SetActive(false);
        targetsInRange.Clear();
    }
    public void OutlineTargets(BaseAttack attack, CombatStateMachine caster)
    {
        switch(attack.GetTargeting())
        {
            case BaseAttack.TargetingSystem.unit:
                switch(attack.GetAffectedTargets())
                {
                    case BaseAttack.AffectedTargets.allies:
                        foreach (CombatStateMachine csm in targetsInRange)
                            if (csm.gameObject.tag == "Ally")
                            {
                                TargetSelect target = csm.GetComponent<TargetSelect>();
                                target.AddOnHoverDelegate(new TargetSelect.OnHoverDelegate(OutlineTarget));
                                target.AddOnHoverExitDelegate(new TargetSelect.OnHoverExitDelegate(RemoveOutlineTarget));
                                targetsDelegated.Add(target);
                            }
                        break;
                    case BaseAttack.AffectedTargets.enemies:
                        foreach (CombatStateMachine csm in targetsInRange)
                            if (csm.gameObject.tag == "Enemy")
                            {
                                TargetSelect target = csm.GetComponent<TargetSelect>();
                                target.AddOnHoverDelegate(new TargetSelect.OnHoverDelegate(OutlineTarget));
                                target.AddOnHoverExitDelegate(new TargetSelect.OnHoverExitDelegate(RemoveOutlineTarget));
                                targetsDelegated.Add(target);
                            }
                        break;
                }
                break;
            case BaseAttack.TargetingSystem.groundPoint:
                break;
            case BaseAttack.TargetingSystem.none:
                switch (attack.GetAffectedTargets())
                {
                    case BaseAttack.AffectedTargets.self:
                        caster.ChangeOutline(true);
                        break;
                    case BaseAttack.AffectedTargets.allies:
                        foreach (CombatStateMachine csm in targetsInRange)
                            if (csm.gameObject.tag == "Ally")
                                csm.ChangeOutline(true);
                        break;
                    case BaseAttack.AffectedTargets.enemies:
                        foreach (CombatStateMachine csm in targetsInRange)
                            if (csm.gameObject.tag == "Enemy")
                                csm.ChangeOutline(true);
                        break;
                    case BaseAttack.AffectedTargets.all:
                        foreach (CombatStateMachine csm in targetsInRange)
                            csm.ChangeOutline(true);
                        break;
                }
                break;
        }
    }

    public void OutlineTarget(TargetSelect target)
    {
        target.GetComponent<CombatStateMachine>().ChangeOutline(true);
    }
    public void RemoveOutlineTarget(TargetSelect target)
    {
        target.GetComponent<CombatStateMachine>().ChangeOutline(false);
    }

    public IEnumerator OutlineCoroutine(BaseAttack chosenAttack)
    {
        yield return new WaitForSeconds(0.1f);
        OutlineTargets(chosenAttack, GameManager.GetCurrentCharacter());
    }

    public List<CombatStateMachine> GetTargetsInRange()
    {
        return targetsInRange;
    }

    public void ShowAim(BaseAttack attack, float aoeRange = 0f)
    {
        aimLine.gameObject.SetActive(true);
        aimLine.Set(GameManager.GetCurrentCharacter().transform, attack, aoeRange);
    }
}
