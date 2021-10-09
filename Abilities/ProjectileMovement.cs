using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    Vector3 target;
    [SerializeField]
    GameObject aoe;
    CombatStateMachine caster;
    SphereCollider collider;
    GroundAttack groundAttack = null;
    BaseAttack attack = null;
    float speed = 3f;


    private void Start()
    {
        collider = GetComponent<SphereCollider>();
    }

    public void Set(CombatStateMachine _caster, GroundAttack _attack, Vector3 _target, GameObject _aoe)
    {
        caster = _caster;
        groundAttack = _attack;
        target = _target;
        aoe = _aoe;
        StartCoroutine(GoTowardsPosition());
    }
    public void Set(CombatStateMachine _caster, BaseAttack _attack, Vector3 _target, GameObject _aoe)
    {
        caster = _caster;
        attack = _attack;
        target = _target;
        aoe = _aoe;
        StartCoroutine(GoTowardsPosition());
    }
    IEnumerator GoTowardsPosition()
    {
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("WindWall"))
        {
            ObjectHealth objectHealth = other.gameObject.GetComponent<ObjectHealth>();
            if (objectHealth != null)
            {
                objectHealth.TakeDamage(attack.GetDamage(caster.GetUnit()));
                GameManager.SpellHasEnded();
                Destroy(this.gameObject);
            }
            else
            {
                GameManager.SpellHasEnded();
                Destroy(this.gameObject);
            }
        }
        else if (other.gameObject != caster.gameObject)
            Explode(other.gameObject);   
    }

    void Explode(GameObject target)
    {
        if (groundAttack != null)
        {
            GameObject aoeInstance = Instantiate(aoe, transform.position, Quaternion.identity);
            aoeInstance.GetComponent<Aoe>().Set(caster, groundAttack);
            Destroy(this.gameObject);
        }
        else
        {
            CombatStateMachine targetCsm = target.GetComponent<CombatStateMachine>();
            if(targetCsm != null)
            {
                attack.Cast(caster, targetCsm, true);
                //attack.DealDamage(caster, targetCsm);
            }
            attack.Ready();
            Destroy(this.gameObject);
        }
    }
}
