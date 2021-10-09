using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ReEnergized : BaseEffect
{
    int damage = 0;
    public ReEnergized(int _value, int _baseDuration)
    {
        effectName = "Re-Energized";
        value = _value;
        baseDuration = _baseDuration;
        duration = baseDuration;
        spriteColor = new Color(1f, 0.55f, 0f);
    }
    public override void AddEffect(CombatStateMachine unit)
    {
        affectedUnit = unit;
        damage = Mathf.RoundToInt((value / 100f) * unit.GetUnit().GetBaseMp());
        List<BaseEffect> effects = affectedUnit.GetUnit().GetEffects();
        foreach (BaseEffect ef in effects)
        {
            if (ef == this)
            {
                if (ef.GetDuration() > GetDuration())
                {
                    return;
                }
                else
                {
                    break;
                }
            }
        }
        BaseEffect e = (BaseEffect)this.Clone();
        affectedUnit.GetUnit().AddEffect(e);
        unit.AddTurnEndDelegate(e.TakeEffect);
    }
    public override void TakeEffect()
    {
        base.TakeEffect();
        affectedUnit.GetUnit().RestoreMP(damage);
    }
    public override void EndEffect()
    {
        base.EndEffect();
        affectedUnit.RemoveTurnEndDelegate(TakeEffect);
    }
}
