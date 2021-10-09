using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Enraged : BaseEffect
{
    public Enraged(int _value, int _baseDuration)
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
        TakeEffect();
    }
    public override void TakeEffect()
    {
        //Instantiate(particleSystem, GameManager.GetCurrentCharacter().transform);
        //particleSystem.Play();
        Unit.Stats stats = affectedUnit.GetUnit().GetStats();
        affectedUnit.GetUnit().SetStat("STR", Mathf.RoundToInt((float)stats.stats["STR"] * value));
        base.TakeEffect();
    }
    public override void EndEffect()
    {
        base.EndEffect();
        Unit.Stats stats = affectedUnit.GetUnit().GetStats();
        affectedUnit.GetUnit().SetStat("STR", Mathf.RoundToInt((float)stats.stats["STR"] / value));
    }
}
