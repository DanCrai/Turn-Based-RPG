using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Rooted : BaseEffect
{
    public Rooted(int _value, int _baseDuration)
    {
        effectName = "Rooted";
        value = _value;
        baseDuration = _baseDuration;
        duration = baseDuration;
        spriteColor = new Color(0.4f, 1f, 0.05f);
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
        unit.AddTurnStartDelegate(e.TakeEffect);
    }
    public override void TakeEffect()
    {
        base.TakeEffect();
        affectedUnit.GetComponent<UnitMovement>().SetMovement(0);
    }
    public override void EndEffect()
    {
        base.EndEffect();
        affectedUnit.RemoveTurnStartDelegate(TakeEffect);
    }
}
