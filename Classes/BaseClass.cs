using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseClass : MonoBehaviour
{
    [SerializeField]
    protected string className;
    [SerializeField]
    protected List<AttackHolder> availableSpells;
    [SerializeField]
    protected int availableStatPointsPerLevel;
    protected int availableStatPoint;
    [SerializeField]
    protected int hpPerLevel;
    [SerializeField]
    protected int mpPerLevel;
    [SerializeField]
    protected int str;
    [SerializeField]
    protected int agi;
    [SerializeField]
    protected int con;
    [SerializeField]
    protected int intel;
    [SerializeField]
    protected float baseMovement;
    [SerializeField]
    GameObject model;

    private void Start()
    {
        //SetAvailableSpells();
    }
    public virtual void SetAvailableSpells()
    {

    }
    public void LevelUp(Unit unit)
    {
        unit.IncreaseLevel();
        UpdateHealth(unit);
        UpdateMana(unit);
        unit.IncreaseAvailablePoints(availableStatPointsPerLevel);
    }
    public void SelectClass(CombatStateMachine unit)
    {
        //for (int i = unit.transform.childCount - 1; i >= 0; i--)
        //    Destroy(unit.transform.GetChild(i).gameObject);
        if(unit.transform.GetChild(unit.transform.childCount - 1).GetComponent<Animator>() != null)
        {
            Destroy(unit.transform.GetChild(unit.transform.childCount - 1).gameObject);
        }
        GameObject modelCopy = Instantiate(model, unit.transform);
        Animator anim = modelCopy.GetComponent<Animator>();
        Animator currentAnim = unit.GetComponent<Animator>();
        currentAnim.runtimeAnimatorController = anim.runtimeAnimatorController;
        currentAnim.avatar = anim.avatar;
        anim.enabled = false;
        unit.GetComponent<UnitMovement>().SetBaseMovement(baseMovement + unit.GetUnit().GetStats().stats["AGI"] * 0.3f);
    }
    public void UpdateHealth(Unit unit)
    {
        int level = unit.GetLevel();
        Dictionary<string, int> stats = unit.GetStats().stats;
        unit.SetBaseHp(hpPerLevel * (2 + level) + (int)(stats["CONST"] * hpPerLevel * level / 10));
    }
    public void UpdateMana(Unit unit)
    {
        int level = unit.GetLevel();
        Dictionary<string, int> stats = unit.GetStats().stats;
        unit.SetBaseMp(mpPerLevel * (2 + level) + (int)(stats["INT"] * mpPerLevel * level / 10));
    }

    public List<BaseAttack> GetAvailableSpells()
    {
        List<BaseAttack> copy = new List<BaseAttack>();
        foreach(AttackHolder attack in availableSpells)
        {
            copy.Add((BaseAttack)(attack.GetAttack().Clone()));
        }
        return copy;
    }

    public int GetStr()
    {
        return str;
    }
    public int GetAgi()
    {
        return agi;
    }
    public int GetConst()
    {
        return con;
    }
    public int GetInt()
    {
        return intel;
    }

    public string GetClassName()
    {
        return className;
    }
}
