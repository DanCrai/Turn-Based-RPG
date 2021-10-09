using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Regenerating : BaseEffect
{
    public Regenerating(int _value, int _baseDuration)
    {
        effectName = "Regenerating";
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
        unit.AddTurnStartDelegate(e.TakeEffect);
    }
    public override void TakeEffect()
    {
        base.TakeEffect();
        affectedUnit.GetUnit().Heal(value);
    }
    public override void EndEffect()
    {
        base.EndEffect();
        affectedUnit.RemoveTurnStartDelegate(TakeEffect);
    }
}
