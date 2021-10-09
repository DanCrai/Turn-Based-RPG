using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera camera;
    Vector3 cameraOffset;
    Vector3 camTarget;
    static CameraManager CM;
    static Vector3 camSmoothDampV;
    [SerializeField]
    GameObject player;
    GameObject disabled;

    private void Start()
    {
        camera = GetComponent<Camera>();
        Vector3 groundPos = new Vector3(0f, 0f, 0f);  //GetWorldPosAtViewportPoint(0f, 0f);
        cameraOffset = camera.transform.position - groundPos;
        //camTarget = transform.position;
        CM = this;
    }
    private Vector3 GetWorldPosAtViewportPoint(float vx, float vy)
    {
        Ray worldRay = camera.ViewportPointToRay(new Vector3(vx, vy, 0));
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distanceToGround;
        groundPlane.Raycast(worldRay, out distanceToGround);
        return worldRay.GetPoint(distanceToGround);
    }
    private void Update()
    {
        Vector3 cameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, camera.nearClipPlane));
        Vector3 dir = cameraCenter - transform.position;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, dir * 10000, out hit, Mathf.Infinity, layerMask: 4))
        {
            if(hit.collider.tag != "RangeIndicator")
            if (hit.collider.gameObject != disabled && hit.collider.gameObject != player && hit.collider.tag != "Enemy" && hit.collider.tag != "Ally" && hit.collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                if(disabled != null && disabled != hit.collider.gameObject)
                {
                    disabled.GetComponent<MeshRenderer>().enabled = true;
                }
                disabled = hit.collider.gameObject;
                hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            if (disabled != null)
            {
                disabled.GetComponent<MeshRenderer>().enabled = true;
                disabled = null;
            }
        }
        if (Input.GetKey("a"))
        {
            camera.transform.position = new Vector3(camera.transform.position.x - 0.01f, camera.transform.position.y, camera.transform.position.z);
        }
        if (Input.GetKey("d"))
        {
            camera.transform.position = new Vector3(camera.transform.position.x + 0.01f, camera.transform.position.y, camera.transform.position.z);
        }
        if (Input.GetKey("w"))
        {
            camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z + 0.01f);
        }
        if (Input.GetKey("s"))
        {
            camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z - 0.01f);
        }
    }

    public static void CenterCameraOnPosition(Vector3 position)
    {
        //CM.camera.transform.position = position + CM.cameraOffset;
        CM.StopAllCoroutines();
        CM.StartCoroutine(CM.MoveCamera(position + CM.cameraOffset));
    }

    IEnumerator MoveCamera(Vector3 finalPosition)
    {
        float speed = 50f;
        while (camera.transform.position != finalPosition)
        {
            camera.transform.position = Vector3.MoveTowards(transform.position, finalPosition, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }


}
