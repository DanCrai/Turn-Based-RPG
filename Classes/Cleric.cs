using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleric : BaseClass
{
    [SerializeField]
    SpellCaster SmiteSpellCaster;
    [SerializeField]
    SpellCaster groundSpellCaster;
    [SerializeField]
    Sprite burningSprite;
    [SerializeField]
    Sprite poisonedSprite;
    /*public override void SetAvailableSpells()
    {
        availableSpells = new List<BaseAttack>();
        BaseAttack attack = new BaseAttack("Smite", 10, 10, 30f, BaseAttack.TargetingSystem.unit, BaseAttack.AffectedTargets.enemies, 1, "", true, 1);
        attack.SetSpellCaster(SmiteSpellCaster);
        BaseEffect ef = new Poisoned(2, 2);
        ef.SetEffectSprite(poisonedSprite);
        attack.SetEffect(ef);
        availableSpells.Add(attack);
        attack = new BaseAttack("Smite2", 10, 10, 3f, BaseAttack.TargetingSystem.unit, BaseAttack.AffectedTargets.enemies, 1, "", false, 1);
        attack.SetSpellCaster(SmiteSpellCaster);
        ef = new Burning(3, 3);
        ef.SetEffectSprite(burningSprite);
        attack.SetEffect(ef);
        availableSpells.Add(attack);
        GroundAttack attack2 = new GroundAttack("ground", 10, 10, 3f, BaseAttack.TargetingSystem.groundPoint, BaseAttack.AffectedTargets.enemies, 10, "", 2f, 1);
        attack2.SetSpellCaster(groundSpellCaster);
        availableSpells.Add(attack2);
        attack = new BaseAttack("Healing touch", 10, 10, 2f, BaseAttack.TargetingSystem.unit, BaseAttack.AffectedTargets.allies, 2, "", false, 1);
        attack.SetSpellCaster(SmiteSpellCaster);
        availableSpells.Add(attack);
        attack = new BaseAttack("Smite2", 50, 30, 4f, BaseAttack.TargetingSystem.unit, BaseAttack.AffectedTargets.enemies, 2, "", true, 2);
        attack.SetSpellCaster(SmiteSpellCaster);
        availableSpells.Add(attack);
        attack = new BaseAttack("Healing touch2", 50, 25, 2.5f, BaseAttack.TargetingSystem.unit, BaseAttack.AffectedTargets.allies, 3, "", false, 2);
        attack.SetSpellCaster(SmiteSpellCaster);
        availableSpells.Add(attack);
        attack = new BaseAttack("Smite3", 150, 50, 5f, BaseAttack.TargetingSystem.unit, BaseAttack.AffectedTargets.enemies, 3, "", true, 3);
        attack.SetSpellCaster(SmiteSpellCaster);
        availableSpells.Add(attack);
        attack = new BaseAttack("Healing touch3", 120, 35, 3f, BaseAttack.TargetingSystem.unit, BaseAttack.AffectedTargets.allies, 4, "", false, 3);
        attack.SetSpellCaster(SmiteSpellCaster);
        availableSpells.Add(attack);

    }*/
}
