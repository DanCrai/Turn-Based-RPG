using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Epidemic : BaseAttack
{
    Epidemic(Epidemic attack) : base(attack)
    { }
    public override void Cast(CombatStateMachine casterCsm, List<CombatStateMachine> targets)
    {
        CombatStateMachine mostInfectedUnit = null;
        int maxx = 0;
        foreach (CombatStateMachine csm in targets)
        {
            int contor = 0;
            foreach(BaseEffect ef in csm.GetUnit().GetEffects())
            {
                if (ef.GetEffectName() == "Poisoned")
                    contor++;
            }
            if(contor > maxx)
            {
                maxx = contor;
                mostInfectedUnit = csm;
            }    
        }
        List<BaseEffect> poisons = new List<BaseEffect>();
        foreach(BaseEffect ef in mostInfectedUnit.GetUnit().GetEffects())
        {
            if (ef.GetEffectName() == "Poisoned")
                poisons.Add(ef);
        }
        foreach (CombatStateMachine csm in targets)
        {
            foreach (BaseEffect ef in poisons)
            {
                BaseEffect efCopy = (BaseEffect)ef.Clone();
                efCopy.AddEffect(csm);
            }
        }
        base.Cast(casterCsm, targets);
    }
}
