using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Poisoned : BaseEffect
{
    public Poisoned(int _value, int _baseDuration)
    {
        effectName = "Poisoned";
        value = _value;
        baseDuration = _baseDuration;
        duration = baseDuration;
        spriteColor = new Color(0.4f, 1f, 0.05f);
    }
    public override void AddEffect(CombatStateMachine unit)
    {
        affectedUnit = unit;
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
