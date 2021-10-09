using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeOutline : MonoBehaviour
{
    List<CombatStateMachine> targetsInRange = new List<CombatStateMachine>();

    //meshrenderer problem
    MeshRenderer mr;

    private void OnTriggerEnter(Collider other)
    {
        CombatStateMachine csm = other.GetComponent<CombatStateMachine>();
        if (csm != null)
        {
            targetsInRange.Add(csm);
            OutlineTarget(csm);
            //csm.ChangeOutline(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        CombatStateMachine csm = other.GetComponent<CombatStateMachine>();
        if (csm != null)
        {
            targetsInRange.Remove(csm);
            RemoveOutlineTarget(csm);
            //csm.ChangeOutline(true);
        }
    }

    private void OnDisable()
    {
        foreach (CombatStateMachine csm in targetsInRange)
            csm.ChangeOutline(false);
        targetsInRange.Clear();
    }

    public void OutlineTarget(CombatStateMachine csm)
    {
        csm.ChangeOutline(true);
    }
    public void RemoveOutlineTarget(CombatStateMachine csm)
    {
        csm.ChangeOutline(false);
    }

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();   
    }

    private void Update()
    {
        if (mr.enabled == false)
            mr.enabled = true;
    }
}
