using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseAi : TurnAction
{
    void DrawCircle(Vector3 center, float radius, Color color)
    {
        Vector3 prevPos = center + new Vector3(radius, 0, 0);
        for (int i = 0; i < 30; i++)
        {
            float angle = (float)(i + 1) / 30.0f * Mathf.PI * 2.0f;
            Vector3 newPos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Debug.DrawLine(prevPos, newPos, color, 10f);
            prevPos = newPos;
        }
    }

    #region Welzl's algorithm
    const float INF = 1e18f;

    struct Circle
    {
        public Vector3 C;
        public float R;

        public Circle(Vector3 c, float r)
        {
            C = c;
            R = r;
        }
    };

    bool is_inside(Circle c, Vector3 p)
    {
        return (Vector3.Distance(c.C, p) <= c.R);
    }

    Vector3 get_circle_center(float bx, float by,
                        float cx, float cy)
    {
    float B = bx * bx + by * by;
    float C = cx * cx + cy * cy;
    float D = bx * cy - by * cx;
    return new Vector3((cy * B - by * C) / (2 * D), 0.5f, (bx * C - cx * B) / (2 * D));
    }

    Circle circle_from(Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 I = get_circle_center(B.x - A.x, B.z - A.z, C.x - A.x, C.z - A.z);
        I.x += A.x;
        I.z += A.z;
        return new Circle(I, Vector3.Distance(I, A));
    }


    Circle circle_from(Vector3 A, Vector3 B)
    {
        Vector3 C = new Vector3((A.x + B.x) / 2.0f, 0.5f, (A.z + B.z) / 2.0f);
        return new Circle(C, Vector3.Distance(A, B) / 2.0f);
    }

    bool is_valid_circle(Circle c, List<Vector3> P)
    {
        foreach(Vector3 p in P)
            if (!is_inside(c, p))
                return false;
        return true;
    }

    Circle min_circle_trivial(List<Vector3> P)
    {
        //if (!(P.Count <= 3))
        //    return new Circle(Vector3.zero,0);
        if (P.Count == 0)
        {
            return new Circle(new Vector3(0, 0.5f, 0), 0);
        }
        else if (P.Count == 1)
        {
            return new Circle(P[0], 0);
        }
        else if (P.Count == 2)
        {
            return circle_from(P[0], P[1]);
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = i + 1; j < 3; j++)
            {
                Circle c = circle_from(P[i], P[j]);
                if (is_valid_circle(c, P))
                    return c;
            }
        }
        return circle_from(P[0], P[1], P[2]);
    }

    Circle welzl_helper(List<Vector3> P, List<Vector3> R, int n)
    {
        if (n == 0 || R.Count == 3)
        {
            return min_circle_trivial(R);
        }

        // Pick a random point randomly
        int idx = Random.Range(0, n-1);
        Vector3 p = P[idx];

        Vector3 aux = P[idx];
        P[idx] = P[n - 1];
        P[n - 1] = aux;

        List<Vector3> RCopy = new List<Vector3>(R);

        Circle d = welzl_helper(P, RCopy, n - 1);

        if (is_inside(d, p))
        {
            return d;
        }

        R.Add(p);

        return welzl_helper(P, R, n - 1);
    }
    public void Shuffle(List<Vector3> list, System.Random rng)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Vector3 value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    Circle welzl(List<Vector3> P)
    {
        List<Vector3> P_copy =  new List<Vector3>(P);
        System.Random rng = new System.Random();
        Shuffle(P_copy, rng);
        return welzl_helper(P_copy, new List<Vector3>(), P_copy.Count);
    }
#endregion
public enum TacticalMode
    {
        survival,
        defensive,
        normal,
        agressive,
        reckless
    };

    class PossibleTarget
    {
        List<CombatStateMachine> possibleTarget;
        float abilityValueOnTarget;
        float remainingMovement;
        Vector3 positionToMoveTo;
        public PossibleTarget()
        {
            possibleTarget = null;
            abilityValueOnTarget = 0;
            remainingMovement = 0;
            positionToMoveTo = Vector3.zero;
        }
        public PossibleTarget(CombatStateMachine _possibleTarget, float _abilityValueOnTarget, float _remainingMovement, Vector3 _positionToMoveTo)
        {
            possibleTarget = new List<CombatStateMachine>();
            possibleTarget.Add(_possibleTarget);
            abilityValueOnTarget = _abilityValueOnTarget;
            remainingMovement = _remainingMovement;
            positionToMoveTo = _positionToMoveTo;
        }
        public PossibleTarget(List<CombatStateMachine> _possibleTarget, float _abilityValueOnTarget, float _remainingMovement, Vector3 _positionToMoveTo)
        {
            possibleTarget = _possibleTarget;
            abilityValueOnTarget = _abilityValueOnTarget;
            remainingMovement = _remainingMovement;
            positionToMoveTo = _positionToMoveTo;
        }
        public List<CombatStateMachine> GetTarget()
        {
            return possibleTarget;
        }
        public float GetValue()
        {
            return abilityValueOnTarget;
        }
        public float GetMovement()
        {
            return remainingMovement;
        }
        public Vector3 GetPosition()
        {
            return positionToMoveTo;
        }
        public override string ToString()
        {
            return possibleTarget[0].gameObject.name + " " + positionToMoveTo;
        }
    }


    class PossibleTargets
    {
        List<PossibleTarget> possibleTargets;

        public PossibleTargets()
        {
            possibleTargets = new List<PossibleTarget>();
        }
        public void AddTarget(CombatStateMachine target, float abilityValue, float remMovement, Vector3 position)
        {
            PossibleTarget pt = new PossibleTarget(target, abilityValue, remMovement, position);
            possibleTargets.Add(pt);
        }
        public void AddTarget(List<CombatStateMachine> targets, float abilityValue, float remMovement, Vector3 position)
        {
            PossibleTarget pt = new PossibleTarget(targets, abilityValue, remMovement, position);
            possibleTargets.Add(pt);
        }

        public void AddTarget(PossibleTarget pt)
        {
            possibleTargets.Add(pt);
        }
        /*public void RemoveTarget(CombatStateMachine target)
        {
            foreach(PossibleTarget pt in possibleTargets)
            {
                if(pt.GetTarget() == target)
                {
                    possibleTargets.Remove(pt);
                    return;
                }
            }
        }*/
        public void RemoveTarget(int index)
        {
            possibleTargets.RemoveAt(index);
        }

        private static int ComparePossibleTargetsByLength(PossibleTarget x, PossibleTarget y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    int retval = y.GetValue().CompareTo(x.GetValue());

                    return retval;
                }
            }
        }
        public void SortByValue()
        {
            possibleTargets.Sort(ComparePossibleTargetsByLength);
        }
        public PossibleTarget GetFirstPossibleTarget()
        {
            if (possibleTargets.Count > 0)
                return possibleTargets[0];
            else
                return null;
        }
    }

    [SerializeField]
    GameObject rangeIndicator;

    bool hasFinishedCurrentCoroutine = true;

    List<CombatStateMachine> friendlyTargets;
    List<CombatStateMachine> hostileTargets;
    [SerializeField]
    TacticalMode tactic = TacticalMode.normal;

    Dictionary<BaseAttack, PossibleTargets> attackStats = new Dictionary<BaseAttack, PossibleTargets>();

    (List<CombatStateMachine>,float,Circle) ReturnValueOfGroup(CombatStateMachine caster, BaseAttack attack, List<CombatStateMachine> targets)
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (CombatStateMachine csm in targets)
        {
            positions.Add(csm.transform.position);
        }

        Circle attackPosition = welzl(positions);
        UnitMovement um = caster.GetComponent<UnitMovement>();
        float dis = um.CanReachPoint(attackPosition.C);
        if (Vector3.Distance(attackPosition.C, transform.position) < attack.GetRange())
        {
            attackPosition.C = transform.position; float abilityValue = 0f;
            foreach (CombatStateMachine csm in targets)
            {
                abilityValue += GetAbilityValue(caster, csm, attack);
            }
            //Debug.Log("returning!");
            return (targets, abilityValue, attackPosition);
        }
        else if (attackPosition.R <= attack.GetRange() && (dis >= 0) && (dis <= um.GetMovement()))// + (attack.GetRange() - attackPosition.R))))
        {
            //Debug.Log("=================================");
            //Debug.Log(um.CanReachPoint(attackPosition.C));
            //Debug.Log(um.GetMovement());
            float abilityValue = 0f;
            foreach (CombatStateMachine csm in targets)
            {
                abilityValue += GetAbilityValue(caster, csm, attack);
            }
            //Debug.Log("returning!");
            return (targets, abilityValue,attackPosition);
        }
        else
        {
            List<CombatStateMachine>[] reducedTargets = new List<CombatStateMachine>[targets.Count];
            for(int i = 0; i < targets.Count; i ++)
            {
                List<CombatStateMachine> posTargets = new List<CombatStateMachine>(targets);
                posTargets.RemoveAt(i);
                reducedTargets[i] = posTargets;
            }
            float maxx = 0f;
            List<CombatStateMachine> rez = new List<CombatStateMachine>();
            Circle c = new Circle();
            foreach(List<CombatStateMachine> l in reducedTargets)
            {
                (List<CombatStateMachine>, float, Circle) val = ReturnValueOfGroup(caster, attack, l);
                if (val.Item2 > maxx)
                {
                    rez = l;
                    maxx = val.Item2;
                    c = val.Item3;
                }
            }
            return (rez, maxx, c);
        }
    }

    IEnumerator SetAbilityValues(CombatStateMachine caster, BaseAttack attack)
    {
        yield return new WaitForSeconds(1f);
        UnitMovement um = caster.GetComponent<UnitMovement>();
        PossibleTargets pt = new PossibleTargets();
        switch(attack.GetTargeting())
        {
            case BaseAttack.TargetingSystem.unit:
                if(attack.GetAffectedTargets() == BaseAttack.AffectedTargets.allies)
                {
                    string forcedTarget = attack.GetForcedTarget();
                    foreach (CombatStateMachine csm in friendlyTargets)
                    {
                        if(forcedTarget == "" || csm.GetUnit().GetUnitName() == forcedTarget)
                        if (csm != caster)
                        {
                            csm.GetComponent<UnitMovement>().DisableNavMeshObstacle();
                            yield return new WaitUntil(() => (csm.GetComponent<UnitMovement>().GetNMO().enabled == false));
                            //yield return new WaitForSeconds(0.01f);
                            float distance = 0f;
                            Vector3 pos;
                            (pos, distance) = um.GetDistanceToPoint(csm);
                            if (pos == csm.transform.position)
                            {
                                float abilityValue = GetAbilityValue(caster, csm, attack);
                                if (abilityValue > 0)
                                {
                                    Vector3[] path = um.GetPath();
                                    Vector3 distancePosition;
                                    if (path.Length > 1)
                                        distancePosition = path[path.Length - 2];
                                    else
                                        distancePosition = transform.position;
                                    if (Vector3.Distance(distancePosition, pos) > attack.GetRange())
                                    {
                                        Vector3 rez = pos + (distancePosition - pos).normalized * attack.GetRange();
                                        pt.AddTarget(csm, abilityValue, distance - Vector3.Distance(rez, pos), rez);
                                    }
                                    else
                                    {
                                        pt.AddTarget(csm, abilityValue, distance - Vector3.Distance(distancePosition, pos), distancePosition);
                                    }
                                }
                            }
                            else
                            {
                                RaycastHit hit = new RaycastHit();
                                Vector3 direction = (csm.transform.position - pos).normalized;
                                if (Physics.Raycast(pos, direction, out hit, attack.GetRange()))
                                {
                                    if (hit.collider == csm.GetComponent<Collider>())
                                    {
                                        float abilityValue = GetAbilityValue(caster, csm, attack);
                                        if(abilityValue > 0 )
                                            pt.AddTarget(csm, abilityValue, 0f, pos);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (CombatStateMachine csm in hostileTargets)
                    {
                        csm.GetComponent<UnitMovement>().DisableNavMeshObstacle();
                        yield return new WaitUntil(() => (csm.GetComponent<UnitMovement>().GetNMO().enabled == false));
                        //yield return new WaitForSeconds(0.01f);
                        float distance = 0f;
                        Vector3 pos;
                        (pos, distance) = um.GetDistanceToPoint(csm);
                        if (pos == csm.transform.position)
                        {
                            float abilityValue = GetAbilityValue(caster, csm, attack);
                            if (abilityValue > 0)
                            {
                                Vector3[] path = um.GetPath();
                                Vector3 distancePosition;
                                if (path.Length > 1)
                                {
                                    distancePosition = path[path.Length - 2];
                                }
                                else
                                {
                                    distancePosition = transform.position;
                                }
                                if (Vector3.Distance(distancePosition, pos) > attack.GetRange())
                                {
                                    Vector3 rez = pos + (distancePosition - pos).normalized * attack.GetRange();
                                    pt.AddTarget(csm, abilityValue, distance - Vector3.Distance(rez, pos), rez);
                                }
                                else
                                {
                                    pt.AddTarget(csm, abilityValue, distance - Vector3.Distance(distancePosition, pos), distancePosition);
                                }
                            }
                        }
                        else
                        {
                            RaycastHit hit = new RaycastHit();
                            Vector3 direction = (csm.transform.position - pos).normalized;
                            //Debug.LogWarning(direction);
                            //Debug.Log(pos + direction * attack.GetRange());
                            Debug.DrawRay(pos, direction * attack.GetRange(), Color.red, 10f);
                            if (Physics.Raycast(pos, direction, out hit, attack.GetRange()))
                            {
                                Debug.Log(hit.collider);
                                Debug.Log(csm.GetComponent<Collider>());
                                if (hit.collider == csm.GetComponent<Collider>())
                                {
                                    float abilityValue = GetAbilityValue(caster, csm, attack);
                                    if (abilityValue > 0)
                                        pt.AddTarget(csm, abilityValue, 0f, pos);
                                }
                            }
                        }
                    }
                    
                }
                break;
            case BaseAttack.TargetingSystem.none:
                switch(attack.GetAffectedTargets())
                {
                    case BaseAttack.AffectedTargets.self:
                        yield return new WaitForSeconds(0.1f);
                        CombatStateMachine csm = GetComponent<CombatStateMachine>();
                        float abilityVal = GetAbilityValue(csm, csm, attack);
                        pt.AddTarget(csm, abilityVal, um.GetMovement(), transform.position);
                        break;
                    case BaseAttack.AffectedTargets.allies:
                        yield return new WaitForSeconds(0.1f);
                        List<CombatStateMachine> friendlyTargetsCopy = new List<CombatStateMachine>(friendlyTargets);
                        List<Vector3> positions = new List<Vector3>();
                        List<CombatStateMachine> targets;
                        float abilityValue;
                        Circle c;
                        (targets, abilityValue, c) = ReturnValueOfGroup(caster, attack, friendlyTargetsCopy);
                        pt.AddTarget(targets, abilityValue, 0f, c.C);
                        break;
                    case BaseAttack.AffectedTargets.enemies:
                        yield return new WaitForSeconds(0.1f);
                        List<CombatStateMachine> hostileTargetsCopy = new List<CombatStateMachine>(hostileTargets);
                        List<Vector3> positions2 = new List<Vector3>();
                        List<CombatStateMachine> targets2;
                        float abilityValue2;
                        Circle c2;
                        (targets2, abilityValue2, c2) = ReturnValueOfGroup(caster, attack, hostileTargetsCopy);
                        pt.AddTarget(targets2, abilityValue2, 0f, c2.C);
                        break;
                    case BaseAttack.AffectedTargets.all:
                        yield return new WaitForSeconds(0.1f);
                        List<CombatStateMachine> friendly = new List<CombatStateMachine>(friendlyTargets);
                        List<CombatStateMachine> hostile = new List<CombatStateMachine>(hostileTargets);
                        List<CombatStateMachine> targetsCopy = new List<CombatStateMachine>();
                        targetsCopy.AddRange(friendly);
                        targetsCopy.AddRange(hostile);
                        List<Vector3> positions3 = new List<Vector3>();
                        List<CombatStateMachine> targets3;
                        float abilityValue3;
                        Circle c3;
                        (targets3, abilityValue3, c3) = ReturnValueOfGroup(caster, attack, targetsCopy);
                        pt.AddTarget(targets3, abilityValue3, 0f, c3.C);
                        break;
                }
                break;
        }
        pt.SortByValue();
        if (pt.GetFirstPossibleTarget() != null)
            attackStats[attack] = pt;
        else
            attackStats[attack] = null;
        yield return new WaitForSeconds(0.1f);
        hasFinishedCurrentCoroutine = true;
    }

    float GetAbilityValue(CombatStateMachine caster, CombatStateMachine target, BaseAttack attack)
    {
        float value = 0;
        if(target.tag == "Enemy")
        {
            //this means the ai should try to heal
            float percentageOfHp = (float)target.GetUnit().GetCurrentHp() / target.GetUnit().GetBaseHp();
            int flatHp = target.GetUnit().GetCurrentHp();
            int missingHp = target.GetUnit().GetBaseHp() - target.GetUnit().GetCurrentHp();
            value = attack.GetAIValue();
            if (percentageOfHp < 0.7f)
            {
                value *= Mathf.Min((value / missingHp) * 10, 10);
            }
            if (missingHp == 0 && object.ReferenceEquals(attack.GetEffect(), null))
            {
                value = 0;
            }
            value *= (1.1f - percentageOfHp) * 2;
            switch (tactic)
            {
                case TacticalMode.survival:
                    value *= 5;
                    break;
                case TacticalMode.defensive:
                    value *= 2;
                    break;
                case TacticalMode.normal:
                    value *= 1;
                    break;
                case TacticalMode.agressive:
                    value *= 0.5f;
                    break;
                case TacticalMode.reckless:
                    value *= 0.2f;
                    break;
            }
        }
        else
        {
            //this means that the ai should try to damage
            float percentageOfHp = (float)target.GetUnit().GetCurrentHp() / target.GetUnit().GetBaseHp();
            int flatHp = target.GetUnit().GetCurrentHp();
            value = attack.GetAIValue();
            value *= Mathf.Min((value / flatHp) * 10, 10);
            //value *= (1.1f - percentageOfHp);
            switch (tactic)
            {
                case TacticalMode.survival:
                    value *= 0.2f;
                    break;
                case TacticalMode.defensive:
                    value *= 0.5f;
                    break;
                case TacticalMode.normal:
                    value *= 1;
                    break;
                case TacticalMode.agressive:
                    value *= 2;
                    break;
                case TacticalMode.reckless:
                    value *= 5;
                    break;
            }
        }
        //Debug.Log(attack.GetName() + ": " + value);
        return value;
    }

    Vector3 GetClosestEnemyPosition(List<CombatStateMachine> enemies)
    {
        float maxDistance = 999f;
        Vector3 enemyPos = Vector3.zero;
        foreach (CombatStateMachine enemy in enemies)
        {
            float dist = Vector3.Distance(enemy.transform.position, transform.position);
            if (dist < maxDistance)
            {
                maxDistance = dist;
                enemyPos = enemy.transform.position;
            }
        }
        return enemyPos;
    }
    public override TurnHandler SelectAction(CombatStateMachine unit)
    {
        friendlyTargets = GameManager.GetUnitsWithTag("Enemy");
        hostileTargets = GameManager.GetUnitsWithTag("Ally");
        float distance = GetComponent<UnitMovement>().CanReachPoint(new Vector3(0.6f, 0.5f, -0.1f));
        StartCoroutine(ConstructTurn(unit));
        return null;
    }
    IEnumerator ConstructTurn(CombatStateMachine unit)
    {
        //temporary
        /*List<Vector3> friendlyPositions = new List<Vector3>();
        foreach (CombatStateMachine csm2 in friendlyTargets)
            friendlyPositions.Add(csm2.transform.position);
        Circle circle = welzl(friendlyPositions);
        rangeIndicator.SetActive(true);
        rangeIndicator.transform.position = circle.C;
        rangeIndicator.transform.localScale = new Vector3(2 * circle.R, 0.01f, 2 * circle.R);
        yield return new WaitForSeconds(2f);*/
        //end temporary

        unit.SetCurState(CombatStateMachine.TurnState.SelectingTarget);
        unit.Act();
        CombatStateMachine csm = GameManager.GetRandomUnit(unit);
        TurnHandler th = new TurnHandler(unit.GetUnit().GetAttacks()[0], unit, csm);
        yield return new WaitForSeconds(1f);
        BaseAttack chosenAttack = null;
        PossibleTarget possibleTarget = new PossibleTarget();
        foreach (BaseAttack attack in unit.GetUnit().GetAttacks())
        {
            //Debug.LogWarning(attack.GetName() + ": " + attack.GetCooldown());
            if (attack.CanBeCast(unit.GetUnit()) && attack.GetIsCastable())
            {
                StartCoroutine(SetAbilityValues(unit, attack));
                hasFinishedCurrentCoroutine = false;
                yield return new WaitUntil(() => hasFinishedCurrentCoroutine == true);
                if (attackStats.ContainsKey(attack))
                {
                    if (attackStats[attack] != null)
                    {
                        if (attackStats[attack].GetFirstPossibleTarget().GetValue() > possibleTarget.GetValue())
                        {
                            chosenAttack = attack;
                            possibleTarget = attackStats[attack].GetFirstPossibleTarget();
                        }
                    }
                }
            }
        }
        if (possibleTarget.GetValue() <= 0)
        {
            Debug.Log("No good action!");
            UnitMovement um = GetComponent<UnitMovement>();
            Vector3 enemyPos = GetClosestEnemyPosition(hostileTargets);
            enemyPos = transform.position + um.GetMovement() * (enemyPos - transform.position).normalized;
            Debug.Log("Position to walk to: " + enemyPos);
            if (possibleTarget.GetPosition() != transform.position)
                um.MoveToPosition(enemyPos);
            yield return new WaitUntil(() => um.GetIsMoving() == false);
            unit.SetCurState(CombatStateMachine.TurnState.EndTurn);
            unit.Act();
        }
        else
        {
            UnitMovement um = GetComponent<UnitMovement>();
            if(possibleTarget.GetPosition() != transform.position)
                um.MoveToPosition(possibleTarget.GetPosition());
            yield return new WaitUntil(() => um.GetIsMoving() == false);
            th.SetAttack(chosenAttack);
            if (possibleTarget.GetTarget().Count == 1)
            {
                th.SetTarget(possibleTarget.GetTarget()[0]);
            }
            else
            {
                th.AddTargets(possibleTarget.GetTarget());
            }
            GameManager.SetTurnHandler(th);
            unit.SetCurState(CombatStateMachine.TurnState.Action);
            unit.Act();
            yield return new WaitUntil(() => unit.GetCurrentState() == CombatStateMachine.TurnState.EndTurn);
            unit.Act();
            yield break;
        }
    }
    
}
