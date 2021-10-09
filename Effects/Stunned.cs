using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Stunned : BaseEffect
{
    public Stunned(int _value, int _baseDuration)
    {
        effectName = "Stunned";
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
        unit.AddTurnStartDelegate(((Stunned)e).TakeEffect);
    }
    public override void TakeEffect()
    {
        GameManager.GetCurrentCharacter().SetCurState(CombatStateMachine.TurnState.EndTurn);
    }
    public override void EndEffect()
    {
        base.EndEffect();
        affectedUnit.RemoveTurnStartDelegate(TakeEffect);
    }
}
