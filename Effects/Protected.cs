using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Protected : BaseEffect
{
    int hpToRecover;
    public Protected(int _value, int _baseDuration)
    {
        effectName = "Protected";
        value = _value;
        baseDuration = _baseDuration;
        duration = baseDuration;
        spriteColor = new Color(0f, 1f, 0f);
    }
    public override void AddEffect(CombatStateMachine unit)
    {
        affectedUnit = unit;
        hpToRecover = affectedUnit.GetUnit().GetCurrentHp();
        List<BaseEffect> effects = affectedUnit.GetUnit().GetEffects();
        foreach (BaseEffect ef in effects)
        {
            if (ef == this)
            {
                if (ef.GetDuration() > this.GetDuration())
                    return;
                else
                {
                    ef.EndEffect();
                    break;
                }
            }
        }
        BaseEffect e = (BaseEffect)this.Clone();
        affectedUnit.GetUnit().AddEffect(e);
        unit.GetUnit().AddBeforeDeathDelegate(((Protected)e).TakeEffect);
        unit.GetUnit().AddBeforeDamageTakenDelegate(((Protected)e).UpdateHpToRestore);
    }
    public int UpdateHpToRestore(int amount)
    {
        hpToRecover = affectedUnit.GetUnit().GetCurrentHp();
        return amount;
    }
    public new int TakeEffect()
    {
        //Instantiate(particleSystem, GameManager.GetCurrentCharacter().transform);
        //particleSystem.Play();
        base.TakeEffect();
        EndEffect();
        return hpToRecover;
    }
    public override void EndEffect()
    {
        base.EndEffect();
        affectedUnit.GetUnit().RemoveBeforeDeathDelegate(TakeEffect);
        affectedUnit.GetUnit().RemoveBeforeDamageTakenDelegate(UpdateHpToRestore);
    }
}
