using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class BaseAttack : ScriptableObject, ICloneable
{
    [field: SerializeField]
    protected string attackName { get; set; }
    [field: SerializeField]
    protected string attackDescription { get; set; }
    [field: SerializeField]
    protected int attackDamage { get; set; }
    [field: SerializeField]
    protected string attackFormula { get; set; }
    [field: SerializeField]
    protected int attackCost { get; set; }
    [SerializeField]
    protected Sprite spellSprite;
    [SerializeField]
    protected Color projectileColor = Color.white;
    [field: SerializeField]
    protected int baseCooldown { get; set; }
    [field: SerializeField]
    protected int cooldown { get; set; }
    [field: SerializeField]
    protected int levelRequired { get; set; }
    [field: SerializeField]
    protected Sprite EffectSprite { get; set; }
    [field: SerializeField]
    protected bool isRanged { get; set; } = false;
    [field: SerializeField]
    protected float range { get; set; }
    [field: SerializeField]
    protected TargetingSystem targeting { get; set; }
    [field: SerializeField]
    protected AffectedTargets affectedTargets { get; set; }
    [field: SerializeField]
    protected bool isSilenced = false;
    [SerializeField]
    protected bool isCastable = true;
    [SerializeField]
    protected Castable becomeCastable;
    [SerializeField]
    protected int becomeCastableThreshold;
    [SerializeField]
    protected string forcedTarget = null;
    [field: SerializeField]
    protected SpellCaster spellCaster { get; set; }
    [SerializeField]
    protected BaseEffect spellEffect = null;
    [SerializeField]
    protected int effectValue;
    [SerializeField]
    protected int effectDuration;
    [SerializeField]
    protected int valueAiModifier = 0;

    protected int ready = 0;

    public object Clone()
    {
        return MemberwiseClone();
        //return this;
    }

    public BaseAttack(string _attackName, int _attackDamage, int _attackCost, float _range, TargetingSystem _targeting, AffectedTargets _affectedTargets, int _baseCooldown, string _attackFormula, bool _isRanged, int _levelRequired)
    {
        attackName = _attackName;
        attackDamage = _attackDamage;
        attackCost = _attackCost;
        range = _range;
        targeting = _targeting;
        affectedTargets = _affectedTargets;
        baseCooldown = _baseCooldown;
        cooldown = 0;
        attackFormula = _attackFormula;
        isRanged = _isRanged;
        levelRequired = _levelRequired;
    }

    public BaseAttack(BaseAttack other)
    {
        attackName = other.attackName;
        attackDamage = other.attackDamage;
        attackCost = other.attackCost;
        range = other.range;
        targeting = other.targeting;
        affectedTargets = other.affectedTargets;
        baseCooldown = other.baseCooldown;
        cooldown = other.cooldown;
        attackFormula = other.attackFormula;
        isRanged = other.isRanged;
        levelRequired = other.levelRequired;
        if (other.spellCaster != null)
            spellCaster = other.spellCaster;
        spellEffect = other.spellEffect;
    }


    //targeting system
    //a spell can either target a speicific unit => unit
    //a point on the ground => groundPoint
    //or nothing and affect self, or all available targets within range => none
    public enum TargetingSystem
    {
        unit,
        groundPoint,
        none
    };
    //the targets that can be affected by the spell
    //if a spell can only affect the caster => self
    //if a spell can only affect friendly units => allies
    //if a spell can only affect enemy units => enemies
    //if a spell can affect any unit => all
    public enum AffectedTargets
    {
        self,
        allies,
        enemies,
        all
    };

    public int GetDamage(Unit caster)
    {
        CultureInfo.CreateSpecificCulture("de-DE");
        int dmg = attackDamage;
        if (attackFormula != "" && attackFormula != null)
        {
            string[] parts = attackFormula.Split(' ');
            float modifier = 0f;
            Dictionary<string, int> stats = caster.GetStats().stats;
            foreach (string part in parts)
            {
                if (modifier < 0.1f)
                {
                    modifier = float.Parse(part, CultureInfo.InvariantCulture);
                }
                else
                {
                    dmg += (int)(modifier * stats[part]);
                    modifier = 0f;
                }
            }
        }
        return dmg;
    }

    public virtual void Cast(CombatStateMachine casterCsm)
    {
        cooldown = baseCooldown;
        casterCsm.GetUnit().ConsumeMP(attackCost);
        GameManager.SpellHasEnded();
    }
    public virtual void Cast(CombatStateMachine casterCsm, List<CombatStateMachine> targetsCsm)
    {
        if (spellCaster != null)
            spellCaster.SetProjectileColor(projectileColor);
        cooldown = baseCooldown;
        Unit caster = casterCsm.GetUnit();
        caster.ConsumeMP(attackCost);
        if (!isRanged)
        {
            foreach (CombatStateMachine target in targetsCsm)
            {
                DealDamage(casterCsm, target);
                if (!object.ReferenceEquals(spellEffect, null))
                {
                    spellEffect.SetValue(effectValue);
                    spellEffect.SetBaseDuration(effectDuration);
                    spellEffect.AddEffect(target);
                }
            }
            GameManager.SpellHasEnded();
        }
        else
        {
            foreach (CombatStateMachine target in targetsCsm)
            {
                CastProjectile(casterCsm, target);
                ready++;
            }
        }
    }
    public virtual void Cast(CombatStateMachine casterCsm, CombatStateMachine targetCsm, bool fromProjectile = false)
    {
        if(spellCaster != null)
            spellCaster.SetProjectileColor(projectileColor);
        cooldown = baseCooldown;
        Unit caster = casterCsm.GetUnit();
        if (!isRanged || fromProjectile)
        {
            if(!fromProjectile)
            {
                caster.ConsumeMP(attackCost);
            }
            DealDamage(casterCsm, targetCsm);
            if(!object.ReferenceEquals(spellEffect, null))
            {
                spellEffect.SetValue(effectValue);
                spellEffect.SetBaseDuration(effectDuration);
                spellEffect.AddEffect(targetCsm);
            }
            if (!fromProjectile)
            {
                GameManager.SpellHasEnded();
            }
        }
        else
        {
            caster.ConsumeMP(attackCost);
            CastProjectile(casterCsm, targetCsm);
            ready = 1;
        }
    }
    public virtual int GetAIValue()
    {
        int val = GetDamage(GameManager.GetCurrentCharacter().GetUnit());
        if(!object.ReferenceEquals(spellEffect, null))
        {
            val += spellEffect.GetAIValue();
        }
        val += valueAiModifier;
        return val;
    }

    public void Ready()
    {
        ready--;
        if (ready == 0)
            GameManager.SpellHasEnded();
    }

    public virtual void DealDamage(CombatStateMachine casterCsm, CombatStateMachine targetCsm)
    {
        if (targetCsm.gameObject.tag == casterCsm.gameObject.tag)
            targetCsm.Heal(GetDamage(casterCsm.GetUnit()));
        else
            targetCsm.GetUnit().TakeDamage(GetDamage(casterCsm.GetUnit()));
    }

    public virtual void CastProjectile(CombatStateMachine casterCsm, CombatStateMachine targetCsm)
    {
        spellCaster.InstantiateProjectile(casterCsm.transform.position, Quaternion.identity, casterCsm, this, targetCsm.transform.position);
    }

    public virtual string GetAttackMessage()
    {
        string message = "";
        message += attackName + "\n";
        message += "Value: " + GetDamage(GameManager.GetCurrentCharacter().GetUnit()) + "\n";
        message += "Mana cost: " + attackCost + "\n";
        message += "Cooldown: " + baseCooldown + "\n";
        message += attackDescription;
        return message;
    }

    public string GetName()
    {
        return attackName;
    }

    public float GetRange()
    {
        return range;
    }

    public string GetForcedTarget()
    {
        return forcedTarget;
    }
    public int GetManaCost()
    {
        return attackCost;
    }
    public TargetingSystem GetTargeting()
    {
        return targeting;
    }
    public AffectedTargets GetAffectedTargets()
    {
        return affectedTargets;
    }
    void ShowCast(Unit caster, Unit target)
    {
        Debug.Log(caster.GetUnitName() + " cast " + attackName + " on " + target.GetUnitName() + " dealing " + attackDamage + " damage!");
    }

    public int GetCooldown()
    {
        return cooldown;
    }
    public int GetBaseCooldown()
    {
        return baseCooldown;
    }

    public bool GetIsRanged()
    {
        return isRanged;
    }
    public void SetIsRanged(bool _isRanged)
    {
        isRanged = _isRanged;
    }
    public void DecreaseCooldown()

    {
        if (cooldown > 0)
            cooldown--;
    }
    public void SetCooldown(int cd)
    {
        cooldown = cd;
    }
    public void SetIsSilenced(bool  _isSilenced)
    {
        isSilenced = _isSilenced;
    }
    public bool CanBeCast(Unit unit)
    {
        return ((!isSilenced) && (cooldown == 0) && (attackCost <= unit.GetCurrentMp()));
    }
    public void SetSpellCaster(SpellCaster _spellCaster)
    {
        spellCaster = _spellCaster;
    }

    public int GetLevelRequired()
    {
        return levelRequired;
    }

    public BaseEffect GetEffect()
    {
        return spellEffect;
    }
    public void SetEffect(BaseEffect effect)
    {
        spellEffect = effect;
    }
    public void SetSpellSprite(Sprite sprite)
    {
        spellSprite = sprite;
    }
    public Sprite GetSpellSprite()
    {
        return spellSprite;
    }

    public enum Castable
    {
        hpUnder,
        hpOver
    }

    public bool GetIsCastable()
    {
        if (isCastable)
            return true;
        switch(becomeCastable)
        {
            case (Castable.hpUnder):
                if (((float)GameManager.GetCurrentCharacter().GetUnit().GetCurrentHp() / GameManager.GetCurrentCharacter().GetUnit().GetBaseHp()) * 100 <= becomeCastableThreshold)
                    isCastable = true;
                break;
        }
        return isCastable;
    }
    /*public virtual void cast_magic(AllyStateMachine caster)
    {

    }
    public virtual void cast_magic(EnemyStateMachine caster)
    {

    }
    public void add_effect(AllyStateMachine caster)
    {
        if (effect.Key == "Poisoned")
        {
            if (caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects.ContainsKey(effect.Key))
            {
                int dur, val;
                if (caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item1 < effect.Value.Item1)
                {
                    dur = effect.Value.Item1;
                }
                else
                {
                    dur = caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item1;
                }
                val = effect.Value.Item2 + caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item2;
                caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key] = (dur, val, caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item3);
            }
            else
            {
                caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects.Add(effect.Key, effect.Value);
            }
        }
        else
        {
            if (target == typeOfTarget.Enemy || target == typeOfTarget.AllEnemies)
            {
                if (caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects.ContainsKey(effect.Key))
                {
                    int dur, val;
                    bool ok = false;
                    if (caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item1 < effect.Value.Item1)
                    {
                        dur = effect.Value.Item1;
                        ok = true;
                    }
                    else
                    {
                        dur = caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item1;
                    }
                    if (caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item2 < effect.Value.Item2)
                    {
                        val = effect.Value.Item2;
                        ok = true;
                    }
                    else
                    {
                        val = caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item2;
                    }
                    if (ok)
                    {
                        caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key] = (dur, val, caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item3);
                    }
                }
                else
                {
                    caster.EnemyToAttack.GetComponent<EnemyStateMachine>().enemy.effects.Add(effect.Key, effect.Value);
                }
            }
            else if (target == typeOfTarget.Ally || target == typeOfTarget.Party)
            {
                if (caster.EnemyToAttack.GetComponent<AllyStateMachine>().hero.effects.ContainsKey(effect.Key))
                {
                    int dur, val;
                    bool ok = false;
                    if (caster.EnemyToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item1 < effect.Value.Item1)
                    {
                        dur = effect.Value.Item1;
                        ok = true;
                    }
                    else
                    {
                        dur = caster.EnemyToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item1;
                    }
                    if (caster.EnemyToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item2 < effect.Value.Item2)
                    {
                        val = effect.Value.Item2;
                        ok = true;
                    }
                    else
                    {
                        val = caster.EnemyToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item2;
                    }
                    if (ok)
                    {
                        caster.EnemyToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key] = (dur, val, caster.EnemyToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item3);
                    }
                }
                else
                {
                    caster.EnemyToAttack.GetComponent<AllyStateMachine>().hero.effects.Add(effect.Key, effect.Value);
                }
            }
        }
    }
    public void add_effect(EnemyStateMachine caster)
    {
        if (effect.Key == "Poisoned")
        {
            if (caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects.ContainsKey(effect.Key))
            {
                int dur, val;
                if (caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item1 < effect.Value.Item1)
                {
                    dur = effect.Value.Item1;
                }
                else
                {
                    dur = caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item1;
                }
                val = effect.Value.Item2 + caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item2;
                caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key] = (dur, val, caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item3);
            }
            else
            {
                caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects.Add(effect.Key, effect.Value);
            }
        }
        else
        {
            if (target == typeOfTarget.Enemy || target == typeOfTarget.AllEnemies)
            {
                if (caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects.ContainsKey(effect.Key))
                {
                    int dur, val;
                    bool ok = false;
                    if (caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item1 < effect.Value.Item1)
                    {
                        dur = effect.Value.Item1;
                        ok = true;
                    }
                    else
                    {
                        dur = caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item1;
                    }
                    if (caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item2 < effect.Value.Item2)
                    {
                        val = effect.Value.Item2;
                        ok = true;
                    }
                    else
                    {
                        val = caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item2;
                    }
                    if (ok)
                    {
                        caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key] = (dur, val, caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects[effect.Key].Item3);
                    }
                }
                else
                {
                    caster.HeroToAttack.GetComponent<AllyStateMachine>().hero.effects.Add(effect.Key, effect.Value);
                }
            }
            else if (target == typeOfTarget.Ally || target == typeOfTarget.Party)
            {
                if (caster.HeroToAttack.GetComponent<EnemyStateMachine>().enemy.effects.ContainsKey(effect.Key))
                {
                    int dur, val;
                    bool ok = false;
                    if (caster.HeroToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item1 < effect.Value.Item1)
                    {
                        dur = effect.Value.Item1;
                        ok = true;
                    }
                    else
                    {
                        dur = caster.HeroToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item1;
                    }
                    if (caster.HeroToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item2 < effect.Value.Item2)
                    {
                        val = effect.Value.Item2;
                        ok = true;
                    }
                    else
                    {
                        val = caster.HeroToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item2;
                    }
                    if (ok)
                    {
                        caster.HeroToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key] = (dur, val, caster.HeroToAttack.GetComponent<EnemyStateMachine>().enemy.effects[effect.Key].Item3);
                    }
                }
                else
                {
                    caster.HeroToAttack.GetComponent<EnemyStateMachine>().enemy.effects.Add(effect.Key, effect.Value);
                }
            }
        }
    }
    public void add_Effect(AllyStateMachine caster, BaseEffect curEffect)
    {
        curEffect.addEffect(caster.EnemyToAttack);
    }

    public void add_Effect(EnemyStateMachine caster, BaseEffect curEffect)
    {
        curEffect.addEffect(caster.HeroToAttack);
    }*/
}
