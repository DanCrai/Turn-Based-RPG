using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Burning : BaseEffect
{
    public Burning(int _value, int _baseDuration)
    {
        effectName = "Burning";
        value = _value;
        baseDuration = _baseDuration;
        duration = baseDuration;
        spriteColor = new Color(1f, 0.55f, 0f);
    }
    public override void AddEffect(CombatStateMachine unit)
    {
        affectedUnit = unit;
        List<BaseEffect> effects = affectedUnit.GetUnit().GetEffects();
        foreach(BaseEffect ef in effects)
        {
            if (ef == this)
            {
                Burning newEf = new Burning(Mathf.Max(ef.GetValue(), value), Mathf.Max(ef.GetDuration(), duration));
                newEf.SetEffectSprite(ef.GetEffectSprite());
                ef.EndEffect();
                newEf.AddEffect(unit);
                return;
            }
        }
        BaseEffect e = (BaseEffect)this.Clone();
        affectedUnit.GetUnit().AddEffect(e);
        unit.AddTurnEndDelegate(e.TakeEffect);
    }
    public override void TakeEffect()
    {
        base.TakeEffect();
        affectedUnit.GetUnit().TakeDamage(value);
    }
    public override void EndEffect()
    {
        base.EndEffect();
        affectedUnit.RemoveTurnEndDelegate(TakeEffect);
    }
}
