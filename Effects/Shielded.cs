using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Shielded : BaseEffect
{
    public Shielded(int _value, int _baseDuration)
    {
        effectName = "Shielded";
        value = _value;
        baseDuration = _baseDuration;
        duration = baseDuration;
        spriteColor = new Color(0f, 1f, 0f);
    }
    public override void AddEffect(CombatStateMachine unit)
    {
        affectedUnit = unit;
        List<BaseEffect> effects = affectedUnit.GetUnit().GetEffects();
        foreach (BaseEffect ef in effects)
        {
            if (ef == this)
            {
                if (ef.GetValue() > this.GetValue())
                    return;
                else
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
        }
        BaseEffect e = (BaseEffect)this.Clone();
        affectedUnit.GetUnit().AddEffect(e);
        unit.GetUnit().AddBeforeDamageTakenDelegate(e.TakeEffect);
    }
    public override int TakeEffect(int amount)
    {
        //Instantiate(particleSystem, GameManager.GetCurrentCharacter().transform);
        //particleSystem.Play();
        base.TakeEffect();
        if (value > amount)
        {
            value -= amount;
            return 0;
        }
        else
        {
            amount -= value;
            EndEffect();
            return amount;
        }
    }
    public override void EndEffect()
    {
        base.EndEffect();
        affectedUnit.GetUnit().RemoveBeforeDamageTakenDelegate(TakeEffect);
    }
}
