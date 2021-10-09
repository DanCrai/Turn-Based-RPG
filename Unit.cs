using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Unit
{
    [SerializeField]
    string unitName;

    [SerializeField]
    int level = 1;
    [SerializeField]
    int baseHP;
    [SerializeField]
    int curHP;
    [SerializeField]
    int baseMP;
    [SerializeField]
    int curMP;

    [SerializeField]
    int str;
    [SerializeField]
    int dex;
    [SerializeField]
    int con;
    [SerializeField]
    int intel;

    Stats stats;
    [SerializeField]
    int Initiative;
    [SerializeField]
    int availablePoints = 0;

    List<BaseEffect> Effects = new List<BaseEffect>();
    List<BaseAttack> attacks;
    [SerializeField]
    List<AttackHolder> knownAttacks;

    BaseClass unitClass;

    public GameObject LastAttackedUnit;
    public GameObject LastAttackerUnit;
    //public BaseAttack LastSpellTaken;



    public delegate void OnDeathDelegate();
    event OnDeathDelegate deathDelegate;

    public delegate void OnDamageTakenDelegate();
    event OnDamageTakenDelegate damageTakenDelegate;

    public delegate void OnHealTakenDelegate();
    event OnHealTakenDelegate healTakenDelegate;

    public delegate void OnManaChangedDelegate();
    event OnManaChangedDelegate manaChangedDelegate;

    public delegate void OnEffectAddedDelegate();
    event OnEffectAddedDelegate effectAddedDelegate;

    public delegate void OnEffectRemovedDelegate(BaseEffect effect);
    event OnEffectRemovedDelegate effectRemovedDelegate;

    public delegate int BeforeDamageTakenDelegate(int damage);
    event BeforeDamageTakenDelegate beforeDamageTakenDelegate;

    public delegate int BeforeManaConsumedDelegate(int amount);
    event BeforeManaConsumedDelegate beforeManaConsumedDelegate;

    public delegate int BeforeDeathDelegate();
    event BeforeDeathDelegate beforeDeathDelegate;

    public delegate void GetDamageTakenDelegate(int damage);
    event GetDamageTakenDelegate getDamageTakenDelegate;

    public delegate void DexIncreasedDelegate();
    event DexIncreasedDelegate dexIncreasedDelegate;


    public Unit()
    {
        unitName = "Nameless";
        curHP = 1;
        baseHP = 1;
        curMP = 1;
        baseMP = 1;
        stats = new Stats(1, 1, 1, 1);
        attacks = new List<BaseAttack>();
        availablePoints = 0;
    }

    public Unit(string _unitName, int _baseHP, int _baseMP, int _STR, int _AGI, int _CONST, int _INT)
    {
        unitName = _unitName;
        baseHP = _baseHP;
        curHP = baseHP;
        baseMP = _baseMP;
        curMP = baseMP;
        stats = new Stats(_STR, _AGI, _CONST, _INT);
        attacks = new List<BaseAttack>();
        availablePoints = 0;
    }

    public Unit(Unit other)
    {
        unitName = other.unitName;
        baseHP = other.baseHP;
        curHP = other.curHP;
        baseMP = other.baseMP;
        curMP = other.curMP;
        stats = new Stats(other.str, other.dex, other.con, other.intel);
        Initiative = other.Initiative;
        availablePoints = other.availablePoints;
        attacks = other.attacks;
        level = other.level;
    }

    public override string ToString()
    {
        return unitName + "\n" + curHP + "/" + baseHP + " HP\n" + curMP + "/" + baseMP + " MP\n" +  stats.ToString();
    }

    public void DoSetup()
    {
        stats = new Stats(str, dex, con, intel);
        if(knownAttacks.Count > 0)
            foreach(AttackHolder attack in knownAttacks)
            {
                AddAttack((BaseAttack)(attack.GetAttack().Clone()));
            }
    }
    public void AddEffect(BaseEffect effect)
    {
        //BaseEffect ef = (BaseEffect)effect.Clone();
        Effects.Add(effect);
        effectAddedDelegate?.Invoke();
    }
    public void RemoveEffect(BaseEffect effect)
    {
        Effects.Remove(effect);
        effectRemovedDelegate?.Invoke(effect);
    }
    public List<BaseEffect> GetEffects()
    {
        return Effects;
    }

    //getters and setters

    public string GetUnitName()
    {
        return unitName;
    }
    public void SetUnitName(string _unitName)
    {
        unitName = _unitName;
    }
    public int GetCurrentHp()
    {
        return curHP;
    }
    public int GetBaseHp()
    {
        return baseHP;
    }
    public void SetBaseHp(int newVal)
    {
        baseHP = newVal;
    }

    public int GetCurrentMp()
    {
        return curMP;
    }
    public int GetBaseMp()
    {
        return baseMP;
    }
    public int GetInitiative()
    {
        return Initiative;
    }
    public void SetBaseMp(int newVal)
    {
        baseMP = newVal;
    }
    public int GetLevel()
    {
        return level;
    }
    public Stats GetStats()
    {
        return stats;
    }
    public void SetStat(string stat, int value)
    {
        stats.stats[stat] = value;
    }
    public void IncreaseStat(string stat)
    {
        stats.stats[stat] += 1;
        if(stat == "STR")
        {
            str++;
        }
        else if (stat == "AGI")
        {
            Initiative++;
            dex++;
            dexIncreasedDelegate?.Invoke();
        }
        else if (stat == "CONST")
        {
            unitClass.UpdateHealth(this);
            con++;
        }
        else if (stat == "INT")
        {
            unitClass.UpdateMana(this);
            intel ++;
        }
    }
    public int GetAvailablePoints()
    {
        return availablePoints;
    }
    public void IncreaseAvailablePoints(int amount)
    {
        availablePoints += amount;
    }
    public void DecreaseAvailablePoints(int amount)
    {
        availablePoints -= amount;
    }
    public BaseClass GetClass()
    {
        return unitClass;
    }
    public void AddAttack(BaseAttack baseAttack)
    {
        foreach (BaseAttack at in attacks)
            if (at.GetName() == baseAttack.GetName())
            {
                Debug.LogWarning("Attack " + at.GetName() + " is already known!");
                return;
            }
        attacks.Add(baseAttack);
    }
    public List<BaseAttack> GetAttacks()
    {
        return attacks;
    }

    //combat changes
    public bool ConsumeMP(int amount)
    {
        if(amount <= curMP)
        {
            if (beforeManaConsumedDelegate != null)
                amount = beforeManaConsumedDelegate.Invoke(amount);
            curMP -= amount;
            manaChangedDelegate();
            return true;
        }
        return false;
    }
    public void RestoreMP(int amount)
    {
        curMP = Mathf.Min(curMP + amount, baseMP);
        manaChangedDelegate();
    }
    public void TakeDamage(int amount)
    {
        if (beforeDamageTakenDelegate != null)
            amount = beforeDamageTakenDelegate.Invoke(amount);
        getDamageTakenDelegate?.Invoke(amount);
        curHP -= amount;
        if (curHP <= 0)
        {
            curHP = 0;
            Die();
        }
        damageTakenDelegate?.Invoke();
    }

    public void Heal(int amount)
    {
        curHP += amount;
        if (curHP > baseHP)
            curHP = baseHP;
        healTakenDelegate?.Invoke();
    }
    public void Die()
    {
        int newHp = 0;
        if (beforeDeathDelegate != null)
            newHp = beforeDeathDelegate();
        if(newHp > 0)
        {
            curHP = newHp;
            healTakenDelegate();
            return;
        }
        GameManager.Defeat(this);
        deathDelegate();
    }
    public void AddDeathDelegate(OnDeathDelegate f)
    {
        if (deathDelegate != null)
            deathDelegate -= f;
        deathDelegate += f;
    }
    public void AddDamageTakenDelegate(OnDamageTakenDelegate f)
    {
        if (damageTakenDelegate != null)
            damageTakenDelegate -= f;
        damageTakenDelegate += f;
    }
    public void AddHealTakenDelegate(OnHealTakenDelegate f)
    {
        if (healTakenDelegate != null)
            healTakenDelegate -= f;
        healTakenDelegate += f;
    }

    public void AddManaChangedDelegate(OnManaChangedDelegate f)
    {
        if (manaChangedDelegate != null)
            manaChangedDelegate -= f;
        manaChangedDelegate += f;
    }

    public void AddEffectAddedDelegate(OnEffectAddedDelegate f)
    {
        if (effectAddedDelegate != null)
            effectAddedDelegate -= f;
        effectAddedDelegate += f;
    }
    public void AddEffectRemoveddDelegate(OnEffectRemovedDelegate f)
    {
        if (effectRemovedDelegate != null)
            effectRemovedDelegate -= f;
        effectRemovedDelegate += f;
    }
    public void AddBeforeDamageTakenDelegate(BeforeDamageTakenDelegate f)
    {
        if (beforeDamageTakenDelegate != null)
            beforeDamageTakenDelegate -= f;
        beforeDamageTakenDelegate += f;
    }
    public void RemoveBeforeDamageTakenDelegate(BeforeDamageTakenDelegate f)
    {
        beforeDamageTakenDelegate -= f;
    }
    public void AddBeforeManaConsumedDelegate(BeforeManaConsumedDelegate f)
    {
        if (beforeManaConsumedDelegate != null)
            beforeManaConsumedDelegate -= f;
        beforeManaConsumedDelegate += f;
    }
    public void RemoveBeforeManaConsumedDelegate(BeforeManaConsumedDelegate f)
    {
        beforeManaConsumedDelegate -= f;
    }
    public void AddBeforeDeathDelegate(BeforeDeathDelegate f)
    {
        if (beforeDeathDelegate != null)
            beforeDeathDelegate -= f;
        beforeDeathDelegate += f;
    }
    public void RemoveBeforeDeathDelegate(BeforeDeathDelegate f)
    {
        beforeDeathDelegate -= f;
    }
    public void AddGetDamageTakenDelegate(GetDamageTakenDelegate f)
    {
        if (getDamageTakenDelegate != null)
            getDamageTakenDelegate -= f;
        getDamageTakenDelegate += f;
    }
    public void RemoveGetDamageTakenDelegate(GetDamageTakenDelegate f)
    {
        getDamageTakenDelegate -= f;
    }
    public void AddDexIncreasedDelegate(DexIncreasedDelegate f)
    {
        if (dexIncreasedDelegate != null)
            dexIncreasedDelegate -= f;
        dexIncreasedDelegate += f;
    }
    public void RemoveDexIncreasedDelegate(DexIncreasedDelegate f)
    {
        dexIncreasedDelegate -= f;
    }



    //stats structure
    [System.Serializable]
    public struct Stats
    {
        public Dictionary<string, int> stats;
        public Stats(int _str, int _agi, int _const, int _int)
        {
            stats = new Dictionary<string, int>();
            stats["STR"] = _str;
            stats["AGI"] = _agi;
            stats["CONST"] = _const;
            stats["INT"] = _int;
        }
        public override string ToString()
        {
            return "STR: " + stats["STR"] + "\nAGI: " + stats["AGI"] + "\nCONST: " + stats["CONST"] + "\nINT: " + stats["INT"];
        }
    };

    public void SetClassFromSave(BaseClass chosenClass)
    {
        unitClass = chosenClass;
    }
    public void SetClass(BaseClass chosenClass)
    {
        attacks = new List<BaseAttack>();
        unitClass = chosenClass;
        stats.stats["STR"] = unitClass.GetStr();
        stats.stats["AGI"] = unitClass.GetAgi();
        stats.stats["CONST"] = unitClass.GetConst();
        stats.stats["INT"] = unitClass.GetInt();
        str = stats.stats["STR"];
        dex = stats.stats["AGI"];
        con = stats.stats["CONST"];
        intel = stats.stats["INT"];
        Initiative = stats.stats["AGI"];
        List<BaseAttack> availableSpells = new List<BaseAttack>(chosenClass.GetAvailableSpells());
        foreach(BaseAttack attack in availableSpells)
        {
            if (attack.GetLevelRequired() == 1)
            {
                /*GroundAttack groundAttack = attack as GroundAttack;
                if (groundAttack != null)
                {
                    GroundAttack copyAttack = new GroundAttack(groundAttack);
                    attacks.Add(copyAttack);
                }
                else
                {
                    BaseAttack copyAttack = new BaseAttack(attack);
                    attacks.Add(copyAttack);
                }*/
                BaseAttack at = (BaseAttack)attack.Clone();
                attacks.Add(at);
            }
        }
        chosenClass.UpdateHealth(this);
        curHP = baseHP;
        chosenClass.UpdateMana(this);
        curMP = baseMP;
    }

    public void EndTurn()
    {
        foreach(BaseAttack attack in attacks)
        {
            attack.DecreaseCooldown();
        }
        for(int i = Effects.Count - 1; i >= 0; i--)
        {
            Effects[i].EndTurnEffect();
        }
    }

    public void LevelUp()
    {
        unitClass.LevelUp(this);
    }
    public void IncreaseLevel()
    {
        level++;
    }
    public void Restore()
    {
        curHP = baseHP;
        curMP = baseMP;
        for(int i = Effects.Count - 1; i >= 0; i--)
        {
            Effects[i].EndEffect();
        }
        foreach(BaseAttack at in attacks)
        {
            at.SetCooldown(0);
        }
    }
}
