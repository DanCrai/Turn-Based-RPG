using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSelector : MonoBehaviour
{
    void Start()
    {
        GameObject.FindGameObjectWithTag("Party").transform.GetChild(transform.GetSiblingIndex()).position = transform.position;
    }
}
