using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnGroundClick : MonoBehaviour
{
    float characterRadius = 0.7f;
    MeshCollider collider;
    int groundLayer = 8;
    float cameraPaddingN = 30f;
    float cameraPaddingS = -30f;
    float cameraPaddingE = 30f;
    float cameraPaddingW = -30f;
    [SerializeField]
    GameObject playerModel;

    Vector3 lastMousePosition;

    private void Start()
    {
        collider = GetComponent<MeshCollider>();
        float paddingValue = playerModel.transform.localScale.x / 1.0f;
        cameraPaddingN = paddingValue;
        cameraPaddingS = paddingValue;
        cameraPaddingE = paddingValue;
        cameraPaddingW = paddingValue;
    }
    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
            CombatStateMachine csm = GameManager.GetCurrentCharacter();
        if (csm == null)
            return;
        if (csm.tag == "Enemy")
            return;
        if (csm.GetIsActing())
            return;
        //Debug.Log(GameManager.GetCurrentCharacter() + " clicked the ground!");
        UnitMovement um = csm.GetComponent<UnitMovement>();
        if (um.GetIsMoving())
            return;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        RaycastHit hit2 = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            float diagonalDistance = Mathf.Sqrt(cameraPaddingN * cameraPaddingN / 2);
            Vector3 posN = hit.point;
            posN.x += cameraPaddingN;
            Vector3 posS = hit.point;
            posS.x -= cameraPaddingN;
            Vector3 posE = hit.point;
            posE.z += cameraPaddingN;
            Vector3 posW = hit.point;
            posW.z -= cameraPaddingN;
            Vector3 posNE = hit.point;
            posNE.x += diagonalDistance;
            posNE.z += diagonalDistance;
            Vector3 posNW = hit.point;
            posNW.x += diagonalDistance;
            posNW.z -= diagonalDistance;
            Vector3 posSE = hit.point;
            posSE.x -= diagonalDistance;
            posSE.z += diagonalDistance;
            Vector3 posSW = hit.point;
            posSW.x -= diagonalDistance;
            posSW.z -= diagonalDistance;

            posN.y += 10;
            posS.y += 10;
            posE.y += 10;
            posW.y += 10;
            posNE.y += 10;
            posNW.y += 10;
            posSE.y += 10;
            posSW.y += 10;

            if (Physics.Raycast(posN, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
                if (Physics.Raycast(posS, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
                    if (Physics.Raycast(posE, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
                        if (Physics.Raycast(posW, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
                            if (Physics.Raycast(posNE, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
                                if (Physics.Raycast(posNW, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
                                    if (Physics.Raycast(posSE, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
                                        if (Physics.Raycast(posSW, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
                                        {
                                            um.MoveToPosition(hit.point);
                                            return;
                                        }
        }
    }
    private void OnMouseOver()
    {   
        CombatStateMachine csm = GameManager.GetCurrentCharacter();
        if (csm == null)
            return;
        if (csm.tag == "Enemy")
        {
            PathRenderer.Erase();
            return;
        }
        if (csm.GetIsActing())
        {
            PathRenderer.Erase();
            return;
        }
        UnitMovement um = csm.GetComponent<UnitMovement>();
        if (!um)
            return;
        if (um.GetIsMoving())
        {
            lastMousePosition = Input.mousePosition;
            PathRenderer.Erase();
            return;
        }
        if (lastMousePosition == Input.mousePosition)
            return;
        else
            lastMousePosition = Input.mousePosition;
        PathRenderer.Draw();
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        RaycastHit hit2 = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            float diagonalDistance = Mathf.Sqrt(cameraPaddingN * cameraPaddingN / 2);
            Vector3 posN = hit.point;
            posN.x += cameraPaddingN;
            Vector3 posS = hit.point;
            posS.x -= cameraPaddingN;
            Vector3 posE = hit.point;
            posE.z += cameraPaddingN;
            Vector3 posW = hit.point;
            posW.z -= cameraPaddingN;
            Vector3 posNE = hit.point;
            posNE.x += diagonalDistance;
            posNE.z += diagonalDistance;
            Vector3 posNW = hit.point;
            posNW.x += diagonalDistance;
            posNW.z -= diagonalDistance;
            Vector3 posSE = hit.point;
            posSE.x -= diagonalDistance;
            posSE.z += diagonalDistance;
            Vector3 posSW = hit.point;
            posSW.x -= diagonalDistance;
            posSW.z -= diagonalDistance;

            posN.y += 2;
            posS.y += 2;
            posE.y += 2;
            posW.y += 2;
            posNE.y += 2;
            posNW.y += 2;
            posSE.y += 2;
            posSW.y += 2;

            if (Physics.Raycast(posN, Vector3.down, out hit2, Mathf.Infinity, (~0)) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
            if (Physics.Raycast(posS, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
            if (Physics.Raycast(posE, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
            if (Physics.Raycast(posW, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
            if (Physics.Raycast(posNE, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
            if (Physics.Raycast(posNW, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
            if (Physics.Raycast(posSE, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
            if (Physics.Raycast(posSW, Vector3.down, out hit2) && (hit2.collider.gameObject.layer == groundLayer || hit2.collider.gameObject == csm.gameObject))
                                        {
                                            Debug.DrawRay(posN, Vector3.down * 10, Color.green);
                                            Debug.DrawRay(posS, Vector3.down * 10, Color.green);
                                            Debug.DrawRay(posE, Vector3.down * 10, Color.green);
                                            Debug.DrawRay(posW, Vector3.down * 10, Color.green);
                                            Debug.DrawRay(posNE, Vector3.down * 10, Color.green);
                                            Debug.DrawRay(posNW, Vector3.down * 10, Color.green);
                                            Debug.DrawRay(posSE, Vector3.down * 10, Color.green);
                                            Debug.DrawRay(posSW, Vector3.down * 10, Color.green);
                                            um.ShowPathToPosition(hit.point);
                                            return;
                                        }
        }
        um.RemovePath();
    }

    private void OnMouseExit()
    {
        PathRenderer.Erase();
        CombatStateMachine csm = GameManager.GetCurrentCharacter();
        if (csm == null)
            return;
        UnitMovement um = csm.GetComponent<UnitMovement>();
        if (!um)
            return;
        um.RemovePath();
    }
}
