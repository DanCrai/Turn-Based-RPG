using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThrough : MonoBehaviour
{
    Material mat;
    Camera camera;
    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mat = GetComponent<MeshRenderer>().materials[0];
    }
   /*private void OnWillRenderObject()
    {
        Vector3 CameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, camera.nearClipPlane));
        //if (Physics.Raycast(CameraCenter, transform.forward, 100))
        //    Debug.Log("Ou yeah!");
        //GetComponent<MeshRenderer>().enabled = false;
    }
    private void OnBecameInvisible()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }*/
}
