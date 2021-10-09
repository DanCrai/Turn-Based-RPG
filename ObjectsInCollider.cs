using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsInCollider : MonoBehaviour
{
    [SerializeField]
    List<GameObject> insideColliderObjects = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        insideColliderObjects.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        insideColliderObjects.Remove(other.gameObject);
    }
    public int GetNumberOfObjectsInsideCollider()
    {
        return insideColliderObjects.Count;
    }
}
