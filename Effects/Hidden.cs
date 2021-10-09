using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Hidden : BaseEffect
{
    public Hidden(int _value, int _baseDuration)
    {
        effectName = "Hidden";
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
        base.TakeEffect();
        Renderer mr = affectedUnit.GetComponentInChildren<Renderer>();
        Color col = mr.material.color;
        mr.material.color = new Color(col.r, col.g, col.b, 0.3f);
        PlayerAi player = affectedUnit.GetComponent<PlayerAi>();
        if(player != null)
        {
            player.AddBeforeDealingDamageDelegate(EndEffect);
        }
        GameManager.RemoveTargetableUnit(affectedUnit);
    }

    public override void EndEffect()
    {
        base.EndEffect();
        PlayerAi player = affectedUnit.GetComponent<PlayerAi>();
        if (player != null)
        {
            player.RemoveBeforeDealingDamageDelegate(EndEffect);
        }
        Renderer mr = affectedUnit.GetComponentInChildren<Renderer>();
        Color col = mr.material.color;
        mr.material.color = new Color(col.r, col.g, col.b, 1f);
        GameManager.AddTargetableUnit(affectedUnit);
    }
}
