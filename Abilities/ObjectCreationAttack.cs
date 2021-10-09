using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectCreationAttack : GroundAttack
{
    [SerializeField]
    GameObject objectToSpawn;

    ObjectCreationAttack(ObjectCreationAttack ocAttack) : base(ocAttack)
    { }

    public override void Cast(CombatStateMachine casterCsm)
    {
        cooldown = baseCooldown;
        casterCsm.GetUnit().ConsumeMP(attackCost);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            Quaternion rotation = Quaternion.LookRotation(hit.point - casterCsm.transform.position);
            rotation *= Quaternion.Euler(0, 90, 0);
            rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
            Vector3 pos = hit.point;
            pos.y += objectToSpawn.transform.localScale.y / 2;
            GameObject wall = Instantiate(objectToSpawn, pos, rotation);
            ExpireAfterDuration expire = wall.GetComponent<ExpireAfterDuration>();
            if(expire != null)
            {
                expire.SetDuration(effectDuration);
            }
        }
        GameManager.SpellHasEnded();
    }
    public GameObject GetObjectToSpawn()
    {
        return objectToSpawn;
    }
}
