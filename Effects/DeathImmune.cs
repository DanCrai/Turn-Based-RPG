using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DeathImmune : BaseEffect
{
    public DeathImmune(int _value, int _baseDuration)
    {
        effectName = "Death Immune";
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
        unit.GetUnit().AddBeforeDeathDelegate(((DeathImmune)e).TakeEffect);
    }
    public new int TakeEffect()
    {
        //Instantiate(particleSystem, GameManager.GetCurrentCharacter().transform);
        //particleSystem.Play();
        base.TakeEffect();
        return 1;
    }
    public override void EndEffect()
    {
        base.EndEffect();
        affectedUnit.GetUnit().RemoveBeforeDeathDelegate(TakeEffect);
    }
}
