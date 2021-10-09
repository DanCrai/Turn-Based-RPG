using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAi : TurnAction
{
    BaseAttack chosenAttack;
    bool hasChosenAttack = false;
    CombatStateMachine chosenTarget;
    bool hasChosenTarget = false;
    TurnHandler th;
    [SerializeField]
    GameObject rangeIndicator;
    TargetsInRange targetsInRange;
    AbilitySelectButton currentAbilityButton;

    public delegate void BeforeDealingDamageDelegate();
    event BeforeDealingDamageDelegate beforeDealingDamageDelegate;

    GameObject copyObject;
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex > 2 && scene.buildIndex < 10)
        {
            rangeIndicator = GameObject.FindGameObjectWithTag("RangeIndicator");
            targetsInRange = rangeIndicator.GetComponent<TargetsInRange>();
        }
    }
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (GameManager.IsInstantiated())
        {
            rangeIndicator = GameObject.FindGameObjectWithTag("RangeIndicator");
            targetsInRange = rangeIndicator.GetComponent<TargetsInRange>();
        }
    }
    public override TurnHandler SelectAction(CombatStateMachine unit)
    {
        CombatStateMachine csm = GameManager.GetRandomUnit(unit);
        th = new TurnHandler(unit.GetUnit().GetAttacks()[0], unit, csm);
        StartCoroutine(WaitForSelection(unit));
        return th;
    }

    public void ButtonSelectAction(BaseAttack _chosenAttack)
    {
        //Debug.Log(_chosenAttack);
        if (currentAbilityButton)
            currentAbilityButton.UnselectAbility();
        hasChosenAttack = true;
        chosenAttack = _chosenAttack;
        th.SetAttack(chosenAttack);
        StartCoroutine(CheckForAbilityCancel());
        rangeIndicator.SetActive(true);
        rangeIndicator.transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
        float rangeValue = chosenAttack.GetRange();
        rangeValue *= 2;
        rangeIndicator.transform.localScale = new Vector3(rangeValue, rangeIndicator.transform.localScale.y, rangeValue);
        targetsInRange.StartCoroutine("OutlineCoroutine",chosenAttack);
        if (_chosenAttack.GetTargeting() == BaseAttack.TargetingSystem.none)
            StartCoroutine("WaitForCast");
        if (_chosenAttack.GetTargeting() == BaseAttack.TargetingSystem.groundPoint)
        {
            ObjectCreationAttack oca = chosenAttack as ObjectCreationAttack;
            if (oca != null)
            {
                //ObjectCreationAttack at = (ObjectCreationAttack)chosenAttack;
                StartCoroutine("WaitForGroundPointObject", oca.GetObjectToSpawn());
            }
            else
            {
                StartCoroutine("WaitForGroundPoint");
            }
            if(chosenAttack.GetIsRanged())
                targetsInRange.ShowAim(chosenAttack, ((GroundAttack)chosenAttack).GetAoeRange());
            
        }
        if (_chosenAttack.GetTargeting() == BaseAttack.TargetingSystem.unit && _chosenAttack.GetIsRanged())
        {
            targetsInRange.ShowAim(chosenAttack);
        }
    }

    public void SelectTarget(CombatStateMachine unit)
    {
        if (chosenAttack.GetTargeting() == BaseAttack.TargetingSystem.groundPoint)
        {
            if((ObjectCreationAttack)chosenAttack == null)
                hasChosenTarget = true;
        }
        else if (chosenAttack.GetTargeting() == BaseAttack.TargetingSystem.unit)
        {
            switch(chosenAttack.GetAffectedTargets())
            {
                case (BaseAttack.AffectedTargets.allies):
                    if (unit.gameObject.tag == "Ally")
                    {
                        hasChosenTarget = true;
                        chosenTarget = unit;
                        th.SetTarget(chosenTarget);
                    }
                    break;
                case (BaseAttack.AffectedTargets.enemies):
                    if (unit.gameObject.tag == "Enemy")
                    {
                        hasChosenTarget = true;
                        chosenTarget = unit;
                        th.SetTarget(chosenTarget);
                    }
                    break;
                case (BaseAttack.AffectedTargets.all):
                    hasChosenTarget = true;
                    chosenTarget = unit;
                    th.SetTarget(chosenTarget);
                    break;
            }
        }
    }

    public void UnselectAttack()
    {
        StopCoroutine("WaitForCast");
        StopCoroutine("WaitForGroundPoint");
        StopCoroutine("WaitForGroundPointObject");
        if (copyObject != null)
            Destroy(copyObject);
        currentAbilityButton.UnselectAbility();
        hasChosenAttack = false;
        th.SetAttack(null);
        rangeIndicator.SetActive(false);
        CombatStateMachine csm = GameManager.GetCurrentCharacter();
        csm.SetCurState(CombatStateMachine.TurnState.SelectingAbility);
        csm.Act();
    }

    IEnumerator WaitForSelection(CombatStateMachine unit)
    {
        yield return new WaitUntil(() => (hasChosenAttack == true && hasChosenTarget == true));
        //yield return new WaitUntil(() => hasChosenTarget == true);
        hasChosenAttack = false;
        hasChosenTarget = false;
        rangeIndicator.SetActive(false);
        if(chosenAttack.GetDamage(unit.GetUnit()) != 0 && chosenAttack.GetAffectedTargets() == BaseAttack.AffectedTargets.enemies)
        {
            beforeDealingDamageDelegate?.Invoke();
        }
        GameManager.SetTurnHandler(th);
        currentAbilityButton.UnselectAbility();
        unit.SetCurState(CombatStateMachine.TurnState.Action);
        GameManager.DisableAbilitiesPanel();
        unit.Act();
        //Debug.Log("finished selection and changes");
    }
    

    IEnumerator WaitForCast()
    {
        yield return new WaitUntil(() => (Input.GetMouseButtonDown(0)));
        List<CombatStateMachine> targets = targetsInRange.GetTargetsInRange();
        foreach(CombatStateMachine target in targets)
        {
            switch(chosenAttack.GetAffectedTargets())
            {
                case (BaseAttack.AffectedTargets.self):
                    th.SetTarget(GameManager.GetCurrentCharacter());
                    hasChosenTarget = true;
                    yield break;
                case (BaseAttack.AffectedTargets.enemies):
                    if (target.gameObject.tag == "Enemy")
                        th.AddTarget(target);
                    break;
                case (BaseAttack.AffectedTargets.allies):
                    if (target.gameObject.tag == "Ally")
                        th.AddTarget(target);
                    break;
                case (BaseAttack.AffectedTargets.all):
                    th.AddTarget(target);
                    break;

            }
        }
        hasChosenTarget = true;

    }
    IEnumerator WaitForGroundPoint()
    {
        yield return new WaitUntil(() => (Input.GetMouseButtonDown(0)));
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            if (Vector3.Distance(hit.point, transform.position) <= chosenAttack.GetRange())
                hasChosenTarget = true;
            else
                StartCoroutine("WaitForGroundPoint");
        }
        else
            StartCoroutine("WaitForGroundPoint");
    }
    IEnumerator WaitForGroundPointObject(GameObject obj)
    {
        Vector3 positionToHit = Vector3.zero;
        Quaternion rotationToHit = Quaternion.identity;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        bool hasHit = false;
        copyObject = Instantiate(obj, Input.mousePosition, Quaternion.identity);
        copyObject.GetComponent<Collider>().isTrigger = true;
        int initialLayer = copyObject.layer;
        copyObject.layer = 2;
        ObjectsInCollider oic = copyObject.GetComponent<ObjectsInCollider>();
        MeshRenderer mr = copyObject.GetComponent<MeshRenderer>();
        Color originalColor = mr.material.color;
        Rigidbody rb = copyObject.GetComponent<Rigidbody>();
        if (rb != null)
            Destroy(rb);
        while (!hasHit)
        {
            float time = Time.time;
            if (oic.GetNumberOfObjectsInsideCollider() > 0)
                mr.material.color = Color.red;
            else
                mr.material.color = originalColor;
            if (Input.GetMouseButtonDown(0))
            {
                if(oic.GetNumberOfObjectsInsideCollider() == 0)
                {
                    Destroy(copyObject);
                    hasHit = true;
                }
            }
            if (Physics.Raycast(ray, out hit))
            {
                positionToHit = hit.point;
                positionToHit.y += copyObject.transform.localScale.y / 2 + 0.1f;
                rotationToHit = Quaternion.LookRotation(hit.point - transform.position);
                rotationToHit *= Quaternion.Euler(0, 90, 0);
                rotationToHit.eulerAngles = new Vector3(0f, rotationToHit.eulerAngles.y, 0f);
                copyObject.transform.position = positionToHit;
                copyObject.transform.rotation = rotationToHit;
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Time.time - time > 0.01f);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        //yield return new WaitUntil(() => (Input.GetMouseButtonDown(0)));
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            if (Vector3.Distance(hit.point, transform.position) <= chosenAttack.GetRange())
                hasChosenTarget = true;
            else
                StartCoroutine("WaitForGroundPoint");
        }
        else
            StartCoroutine("WaitForGroundPoint");
    }
    IEnumerator CheckForAbilityCancel()
    {
        yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Escape) || hasChosenTarget));
        if (hasChosenTarget)
            yield break;
        UnselectAttack();
    }
    public TurnHandler GetTurnHandler()
    {
        return th;
    }
    public bool IsInRange(Transform target)
    {
        if(target.GetComponent<CombatStateMachine>() != null)
            if(targetsInRange.GetTargetsInRange().Contains(target.gameObject.GetComponent<CombatStateMachine>()))
            {
                return true;
            }
        if (Vector3.Distance(target.position, transform.position) <= chosenAttack.GetRange())
            return true;
        else
            return false;
    }

    public void SetCurAbilityButton(AbilitySelectButton ab)
    {
        currentAbilityButton = ab;
    }

    public void AddBeforeDealingDamageDelegate(BeforeDealingDamageDelegate f)
    {
        if (beforeDealingDamageDelegate != null)
            beforeDealingDamageDelegate -= f;
        beforeDealingDamageDelegate += f;
    }
    public void RemoveBeforeDealingDamageDelegate(BeforeDealingDamageDelegate f)
    {
        beforeDealingDamageDelegate -= f;
    }
}
