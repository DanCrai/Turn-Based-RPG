using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    [SerializeField]
    float baseMovement = 2000;
    float movement = 2000;
    [SerializeField]
    GameObject rangeIndicator;
    NavMeshAgent nma;
    NavMeshPath path;
    NavMeshObstacle nmo;
    bool isMoving = false;
    float distance;
    Animator anim;
    
    private void Start()
    {
        nma = GetComponent<NavMeshAgent>();
        nmo = GetComponent<NavMeshObstacle>();
        nma.enabled = false;
        nmo.enabled = true;
        path = new NavMeshPath();
        anim = GetComponent<Animator>();
        GetComponent<CombatStateMachine>().GetUnit().AddDexIncreasedDelegate(IncreaseMovement);
        //nma.enabled = false;
    }
    public void IncreaseMovement()
    {
        baseMovement += 0.3f;
    }
    public void StartTurn()
    {
        movement = baseMovement;
        nmo.enabled = false;
        //Highlight();
    }
    public void EndTurn()
    {
        movement = 0;
        nmo.enabled = true;
        //RemoveHighlight();
    }
    void Highlight()
    {
        rangeIndicator.SetActive(true);
        rangeIndicator.transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);
        rangeIndicator.GetComponent<Transform>().localScale = new Vector3(2 * movement, 0.01f, 2 * movement);
    }
    void RemoveHighlight()
    {
        rangeIndicator.SetActive(false);
    }
    float PathLength(NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return 0;

        Vector3 previousCorner = path.corners[0];
        float lengthSoFar = 0.0F;
        int i = 1;
        while (i < path.corners.Length)
        {
            Vector3 currentCorner = path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
            i++;
        }
        return lengthSoFar;
    }
    public void MoveToPosition(Vector3 position)
    {
        
        if (distance <= movement)
        {
            isMoving = true;
            if(anim != null)
                anim.SetBool("isMoving", true);
            //RemoveHighlight();
            movement -= distance;
            position.y = 0.5f;
            nmo.enabled = false;
            nma.enabled = true;
            nma.SetDestination(position);
            //nma.enabled = false;

            StartCoroutine(ReachDestination(position));
        }
    }
    IEnumerator ReachDestination(Vector3 position)
    {
        float rotateSpeed = 0.1f;
        //transform.LookAt(position);
        yield return new WaitUntil(() => nma.pathPending == false);
        NavMeshPath curPath = nma.path;
        nma.enabled = false;
        Vector3[] corners = curPath.corners;
        Vector3 lastPosition = transform.position;
        Vector3 nextPosition = corners[0];
        nextPosition.y = 0.5f;
        float startTime = 0f;
        float distance = 0f;
        Quaternion rotationToLookAt = Quaternion.identity;
        Transform from = null;
        var t = 0f;
        var timeToRotate = 0.2f;
        for (int i = 1; i <= corners.Length; i++)
        {
            rotationToLookAt = Quaternion.LookRotation(nextPosition - transform.position);
            rotationToLookAt.x = 0f;
            rotationToLookAt.z = 0f;
            from = transform;
            t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime / timeToRotate;
                transform.rotation = Quaternion.Lerp(from.rotation, rotationToLookAt, t);
                yield return null;
            }
            distance += Vector3.Distance(lastPosition, nextPosition);
            nextPosition.y = 0.5f;
            //float elapsedTime = 0;
            float speed = 3f;
            startTime = Time.time;
            while (transform.position != nextPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            /*while (elapsedTime < 0.5f)
            {
                transform.position = Vector3.Lerp(lastPosition, nextPosition, (elapsedTime / 0.5f));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }*/
            lastPosition = nextPosition;
            if (i != corners.Length)
                nextPosition = corners[i];
            else
                nextPosition = position;
        }
        //transform.position = position;
        startTime = Time.time;
        yield return new WaitUntil(() => (Mathf.Sqrt((transform.position.x - position.x) * (transform.position.x - position.x) + (transform.position.z - position.z) * (transform.position.z - position.z)) <= 0.01f) || Time.time - startTime > 0.2f);
        //Highlight();
        yield return null;
        isMoving = false;
        if (anim != null)
        {
            anim.SetBool("isMoving", false);
        }
    }
    public void ShowPathToPosition(Vector3 position)
    {
        StartCoroutine(ShowPath(position));
    }

    IEnumerator ShowPath(Vector3 position)
    {
        if (isMoving)
            yield break;
        //nma.enabled = true;
        NavMesh.CalculatePath(transform.position, position, NavMesh.AllAreas, path);
        //yield return new WaitUntil(() => nma.pathPending == false);
        //nma.enabled = false;
        Vector3[] corners = path.corners;
        distance = 0f;
        Vector3 lastPosition = transform.position;
        Vector3 nextPosition;
        if (corners.Length == 0)
            nextPosition = position;
        else
            nextPosition = corners[0];
        for (int i = 1; i <= corners.Length; i++)
        {
            distance += Vector3.Distance(lastPosition, nextPosition);
            nextPosition.y = 0.5f;

            lastPosition = nextPosition;
            if (i != corners.Length)
                nextPosition = corners[i];
            else
                nextPosition = position;
        }
        
    }

    public (Vector3, float) GetDistanceToPoint(CombatStateMachine target)
    {
        Vector3 position = target.transform.position;
        UnitMovement um = target.GetComponent<UnitMovement>();
        NavMesh.CalculatePath(transform.position, position, NavMesh.AllAreas, path);
        Vector3[] corners = path.corners;
        distance = 0f;
        Vector3 lastPosition = transform.position;
        Vector3 nextPosition;
        if (corners.Length == 0)
            nextPosition = position;
        else
            nextPosition = corners[0];
        for (int i = 1; i <= corners.Length; i++)
        {
            if ((distance + Vector3.Distance(lastPosition, nextPosition)) <= movement)
            {
                distance += Vector3.Distance(lastPosition, nextPosition);
            }
            else
            {
                Vector3 rez = lastPosition + (nextPosition - lastPosition).normalized * (movement - distance);
                rez.y = 0.5f;
                return (rez, distance + Vector3.Distance(lastPosition, rez));
            }
            nextPosition.y = 0.5f;

            lastPosition = nextPosition;
            if (i != corners.Length)
                nextPosition = corners[i];
            else
                nextPosition = position;
        }
        um.EnableNavMeshObstacle();
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            return (target.transform.position, distance);
        }
        else
        {
            return (transform.position, -1f);
        }
    }
    public float CanReachPoint(Vector3 target)
    {
        Vector3 position = target;
        //UnitMovement um = target.GetComponent<UnitMovement>();
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            Vector3[] corners = path.corners;
            distance = 0f;
            Vector3 lastPosition = transform.position;
            Vector3 nextPosition;
            if (corners.Length == 0)
                nextPosition = position;
            else
                nextPosition = corners[0];
            for (int i = 1; i <= corners.Length; i++)
            {
                distance += Vector3.Distance(lastPosition, nextPosition);
                nextPosition.y = 0.5f;

                lastPosition = nextPosition;
                if (i != corners.Length)
                    nextPosition = corners[i];
                else
                    nextPosition = position;
            }
            return distance;
        }
        else
        {
            return -1f;
        }
    }

    public void RemovePath()
    {
        path.ClearCorners();
    }

    public Vector3[] GetPath()
    {
        return (path.corners);
    }

    public bool GetIsMoving()
    {
        return isMoving;
    }
    public bool GetOutOfMovement()
    {
        return (distance > movement);
    }
    public float GetDistance()
    {
        return distance;
    }
    public float GetMovement()
    {
        return movement;
    }
    public void SetMovement(float value)
    {
        movement = value;
    }
    public void SetBaseMovement(float value)
    {
        baseMovement = value;
    }
    public void DisableNavMeshObstacle()
    {
        nmo.enabled = false;
    }
    public void EnableNavMeshObstacle()
    {
        nmo.enabled = true;
    }
    public NavMeshObstacle GetNMO()
    {
        return nmo;
    }
}
