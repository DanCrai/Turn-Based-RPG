using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveStat : MonoBehaviour
{
    [SerializeField]
    string stat;
    public void IncreaseStat()
    {
        Unit unit = WorldManager.GetCurrentAlly().GetComponent<CombatStateMachine>().GetUnit();
        unit.IncreaseStat(stat);
        unit.DecreaseAvailablePoints(1);
        AllyDescriptionButton.UpdateDescriptionTexts();
    }
}
