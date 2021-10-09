using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathRenderer : MonoBehaviour
{
    // Creates a line renderer that follows a Sin() function
    // and animates it.

    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    static public int lengthOfLineRenderer = 50;
    static LineRenderer lineRenderer;
    static Gradient regularGradient;
    static Gradient outOfMovementGradient;
    //Vector3[] oldPath;
    [SerializeField]
    GameObject positionMarker;
    [SerializeField]
    Text costText;
    static PathRenderer PR;

    void Start()
    {
        PR = this;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = lengthOfLineRenderer;

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        regularGradient = new Gradient();
        regularGradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        outOfMovementGradient = new Gradient();
        outOfMovementGradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = regularGradient;
        lineRenderer.material.mainTextureScale = new Vector2(1f / lineRenderer.startWidth, 1.0f);
    }

    //void Update()
    static public void Draw()
    {
        var t = Time.time;
        CombatStateMachine csm = GameManager.GetCurrentCharacter();
        if (csm && csm.tag != "Enemy")
        {
            PR.positionMarker.SetActive(true);
            UnitMovement um = csm.GetComponent<UnitMovement>();
            if (!um || (um.GetIsMoving()) || csm.GetIsActing())
            {
                lineRenderer.enabled = false;
                PR.positionMarker.SetActive(false);
                PR.costText.gameObject.SetActive(false);
                return;
            }
            lineRenderer.enabled = true;
            if (um.GetOutOfMovement())
                lineRenderer.colorGradient = outOfMovementGradient;
            else
                lineRenderer.colorGradient = regularGradient;
            Vector3[] path = um.GetPath();

            if (path.Length > 0)
            {
                for (int i = 0; i < lengthOfLineRenderer; i++)
                {
                    lineRenderer.SetPosition(i, path[(int)i / ((int)(lengthOfLineRenderer / path.Length) + 1)]);
                }
                PR.positionMarker.SetActive(true);
                PR.positionMarker.transform.position = new Vector3(path[path.Length - 1].x, 0.01f, path[path.Length - 1].z);
                PR.costText.gameObject.SetActive(true);
                PR.costText.transform.position = Input.mousePosition;
                PR.costText.text = um.GetDistance().ToString();
                if (um.GetOutOfMovement())
                    PR.costText.color = Color.red;
                else
                    PR.costText.color = Color.white;
            }
            /*else
                for (int i = 0; i < lengthOfLineRenderer; i++)
                {
                    lineRenderer.enabled = false;
                    costText.gameObject.SetActive(false);
                    positionMarker.SetActive(false);
                }*/
            /*if (oldPath != null)
                if ((path != oldPath) && (Mathf.Abs(path.Length - oldPath.Length) >= 2))
                {
                    //Debug.Log("-------------------------");
                    //printareVector(oldPath);
                    //printareVector(path);
                }
            oldPath = path;*/
        }
        else
        {
            lineRenderer.enabled = false;
            PR.positionMarker.SetActive(false);
            PR.costText.gameObject.SetActive(false);
        }
    }

    static public void Erase()
    {
        lineRenderer.enabled = false;
        PR.positionMarker.SetActive(false);
        PR.costText.gameObject.SetActive(false);
    }
    void printareVector(Vector3[] p)
    {
        string mes = "";
        foreach(Vector3 vect in p)
        {
            mes += vect.ToString() + ", ";
        }
        Debug.Log(mes);
    }
}
