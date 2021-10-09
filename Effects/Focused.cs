using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Focused : BaseEffect
{
    public Focused(int _value, int _baseDuration)
    {
        effectName = "Focused";
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
        unit.GetUnit().AddBeforeManaConsumedDelegate(TakeEffect);
    }
    public new int TakeEffect(int amount)
    {
        //Instantiate(particleSystem, GameManager.GetCurrentCharacter().transform);
        //particleSystem.Play();
        amount -= Mathf.RoundToInt(amount * (value / 100f));
        base.TakeEffect();
        return amount;
    }
    public override void EndEffect()
    {
        base.EndEffect();
        affectedUnit.GetUnit().RemoveBeforeManaConsumedDelegate(TakeEffect);
    }
}
