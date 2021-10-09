using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    int baseSteps = 3;
    int curSteps = 3;
    public delegate void OnMovementEnd();
    OnMovementEnd onMovementEndDelegate;
    public void MovetToPosition(Vector3 position)
    {
        StartCoroutine(MoveTo(position));
    }

    IEnumerator MoveTo(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        direction.Normalize();
        transform.position = transform.position + direction;
        /*while ((transform.position - position).magnitude < 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime);
            yield return null;
        }*/
        yield return new WaitForSeconds(1f);
        onMovementEndDelegate();
    }




    //delegate
    public void AddEndMovementDelegate(OnMovementEnd del)
    {
        onMovementEndDelegate += del;
        
    }
    public void RemoveEndMovementDelegate(OnMovementEnd del)
    {
        onMovementEndDelegate -= del;
        
    }
    void RemoveAllDelegates()
    {
        Debug.Log("removing delegates");
        onMovementEndDelegate = null;
    }

    //getters and setters
    public int GetCurSteps()
    {
        return curSteps;
    }
    public int GetBaseSteps()
    {
        return baseSteps;
    }
}
