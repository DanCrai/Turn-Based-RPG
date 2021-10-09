using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Silenced : BaseEffect
{
    public Silenced(int _value, int _baseDuration)
    {
        effectName = "Silenced";
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
        base.TakeEffect();
        List<BaseAttack> attacks = affectedUnit.GetUnit().GetAttacks();
        for(int i = 1; i < attacks.Count; i++)
        {
            attacks[i].SetIsSilenced(true);
        }
    }
    public override void EndEffect()
    {
        base.EndEffect();
        List<BaseAttack> attacks = affectedUnit.GetUnit().GetAttacks();
        for (int i = 1; i < attacks.Count; i++)
        {
            attacks[i].SetIsSilenced(false);
        }
    }
}
