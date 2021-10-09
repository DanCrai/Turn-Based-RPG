using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHandler
{
    BaseAttack attack;
    CombatStateMachine target;
    List<CombatStateMachine> targets = new List<CombatStateMachine>();
    CombatStateMachine caster;

    public TurnHandler()
    {

    }
    public TurnHandler(BaseAttack _attack, CombatStateMachine _caster, CombatStateMachine _target)
    {
        attack = _attack;
        caster = _caster;
        target = _target;
    }
    public TurnHandler(TurnHandler other)
    {
        attack = other.attack;
        caster = other.caster;
        target = other.target;
    }
    public void ExecuteAction()
    {
        Debug.Log(caster.gameObject.name + " is attacking: " + target.gameObject.name + " using " + attack.GetName());
        if (targets.Count > 0)
        {
            attack.Cast(caster, targets);
            targets.Clear();
        }
        else
        {
            if (attack.GetTargeting() == BaseAttack.TargetingSystem.groundPoint)
                attack.Cast(caster);
            else
            {
                attack.Cast(caster, target);
            }
        }
        
    }
    //getters and setters
    public void SetAttack(BaseAttack _attack)
    {
        attack = _attack;
    }
    public void SetTarget(CombatStateMachine _target)
    {
        target = _target;
    }
    public void SetCaster(CombatStateMachine _caster)
    {
        caster = _caster;
    }
    public void AddTarget(CombatStateMachine _target)
    {
        targets.Add(_target);
    }
    public void AddTargets(List<CombatStateMachine> _targets)
    {
        targets.AddRange(_targets);
    }
    public void RemoveTarget(CombatStateMachine _target)
    {
        targets.Remove(_target);
    }
    public BaseAttack GetChosenAttack()
    {
        return attack;
    }

    public CombatStateMachine GetChosenTarget()
    {
        return target;
    }
}
