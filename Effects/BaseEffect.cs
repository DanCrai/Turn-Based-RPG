using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class BaseEffect : ScriptableObject, ICloneable
{
    [SerializeField]
    protected string effectName;
    [SerializeField]
    protected int value;
    [SerializeField]
    protected int baseDuration;
    [SerializeField]
    protected int duration;
    [SerializeField]
    protected Color spriteColor;
    [SerializeField]
    protected Sprite effectSprite;
    [SerializeField]
    protected bool positive = true;
    [SerializeField]
    protected CombatStateMachine affectedUnit;
    [SerializeField]
    protected ParticleSystem particleSystem;
    [SerializeField]
    protected int AiValue;

    public BaseEffect()
    {

    }
    public BaseEffect(string _effectName, int _value, int _baseDuration, Color _effectColor)
    {
        effectName = _effectName;
        value = _value;
        baseDuration = _baseDuration;
        duration = baseDuration;
        spriteColor = _effectColor;

    }

    public static bool operator ==(BaseEffect a, BaseEffect b)
    {
        if (object.ReferenceEquals(a, null))
        {
            return object.ReferenceEquals(b, null);
        }
        if(object.ReferenceEquals(b, null))
        {
            return object.ReferenceEquals(b, null);
        }
        return (a.GetEffectName() == b.GetEffectName());
    }
    public static bool operator !=(BaseEffect a, BaseEffect b)
    {
        if (object.ReferenceEquals(a, null))
        {
            return !object.ReferenceEquals(b, null);
        }
        if (object.ReferenceEquals(b, null))
        {
            return !object.ReferenceEquals(b, null);
        }
        return (a.GetEffectName() != b.GetEffectName());
    }

    public virtual void AddEffect(CombatStateMachine unit)
    {
        affectedUnit = unit;
        affectedUnit.GetUnit().AddEffect((BaseEffect)this.Clone());
    }

    public virtual void TakeEffect()
    {
        //the hero takes damage, the character skips his turn, etc.
    }
    public virtual int TakeEffect(int val)
    {
        return 0;
    }
    public virtual void EndTurnEffect()
    {
        duration--;
        if(duration <= 0)
        {
            EndEffect();
        }

    }
    public virtual void EndEffect()
    {
        affectedUnit.GetUnit().RemoveEffect(this);
    }
    public object Clone()
    {
        return MemberwiseClone();
        //return this;
    }
    
    public string GetEffectName()
    {
        return effectName;
    }
    public int GetValue()
    {
        return value;
    }
    public int GetDuration()
    {
        return duration;
    }
    public Sprite GetEffectSprite()
    {
        return effectSprite;
    }
    public void SetEffectSprite(Sprite sprite)
    {
        effectSprite = sprite;
    }
    public Color GetSpriteColor()
    {
        return spriteColor;
    }
    public void SetValue(int amount)
    {
        value = amount;
    }
    public void SetBaseDuration(int amount)
    {
        baseDuration = amount;
        duration = amount;
    }
    public int GetAIValue()
    {
        return AiValue;
    }
}