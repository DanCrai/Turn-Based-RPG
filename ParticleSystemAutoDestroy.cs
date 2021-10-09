using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemAutoDestroy : MonoBehaviour
{
    ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject.name);
        ps = gameObject.GetComponent<ParticleSystem>();
        //Debug.Log
    }

    // Update is called once per frame
    void Update()
    {
        if (!ps.IsAlive())
            Destroy(this.gameObject);
    }
}
