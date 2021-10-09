using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackHolder : MonoBehaviour
{
    [SerializeField]
    BaseAttack attack;
    //[SerializeField]
    BaseEffect effect;
    public BaseAttack GetAttack()
    {
        if (effect != null)
        {
            attack.SetEffect(effect);
        }
        return attack;
    }
}
