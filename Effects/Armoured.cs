using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Armoured : BaseEffect
{
    public Armoured(int _value, int _baseDuration)
    {
        effectName = "Armoured";
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
                return;
            }
        }
        BaseEffect e = (BaseEffect)this.Clone();
        affectedUnit.GetUnit().AddEffect(e);
        unit.GetUnit().AddBeforeDamageTakenDelegate(((Armoured)e).TakeEffect);
    }
    public new int TakeEffect(int amount)
    {
        base.TakeEffect();
        amount = Mathf.RoundToInt(amount * (100f - value) / 100f);
        return amount;
    }
    public override void EndEffect()
    {
        base.EndEffect();
        affectedUnit.GetUnit().RemoveBeforeDamageTakenDelegate(TakeEffect);
    }
}
