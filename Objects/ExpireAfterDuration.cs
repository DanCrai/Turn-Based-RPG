using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpireAfterDuration : MonoBehaviour
{
    CombatStateMachine caster;
    int duration;

    private void Start()
    {
        caster = GameManager.GetCurrentCharacter();
        caster.AddTurnEndDelegate(DecreaseDuration);
    }

    private void OnDisable()
    {
        caster.RemoveTurnEndDelegate(DecreaseDuration);
    }
    void DecreaseDuration()
    {
        duration--;
        if (duration < 0)
        {
            caster.RemoveTurnEndDelegate(DecreaseDuration);
            Destroy(this.gameObject);
        }
    }

    public void SetDuration(int _duration)
    {
        duration = _duration;
    }
}
