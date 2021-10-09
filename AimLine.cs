using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimLine : MonoBehaviour
{
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 20;
    LineRenderer lineRenderer;
    Gradient regularGradient;
    Gradient outOfMovementGradient;
    Vector3 caster = Vector3.zero;
    Vector3[] path;
    BaseAttack attack;
    [SerializeField]
    Text pathInterruptedText;
    [SerializeField]
    GameObject aoeRangeIndicator;
    float aoeRange = 0f;


    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = lengthOfLineRenderer;

        path = new Vector3[lengthOfLineRenderer];

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        regularGradient = new Gradient();
        regularGradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.yellow, 1.0f) },
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

    void Update()
    {
        if (caster != Vector3.zero)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            Vector3 targetPosition;
            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;
                if(Vector3.Distance(hit.point, caster) >= attack.GetRange())
                {
                    lineRenderer.colorGradient = outOfMovementGradient;
                }
                else
                {
                    lineRenderer.colorGradient = regularGradient;
                }
                Vector3 direction = targetPosition - caster;;
                if (Physics.Raycast(caster, direction, out hit, Mathf.Infinity, 3067))
                {
                    if (Vector3.Distance((hit.point - targetPosition), Vector3.zero) > 0.1f)
                    {
                        if (!hit.collider.bounds.Contains(targetPosition))
                        {
                            for (int i = 0; i < lengthOfLineRenderer; i++)
                            {
                                path[i] = caster * ((float)i / (lengthOfLineRenderer - 1)) + hit.point * ((float)(lengthOfLineRenderer - i - 1) / (lengthOfLineRenderer - 1));
                            }
                            PathInterrupted(Input.mousePosition);
                        }
                        else
                        {
                            for (int i = 0; i < lengthOfLineRenderer; i++)
                            {
                                path[i] = caster * ((float)i / (lengthOfLineRenderer - 1)) + targetPosition * ((float)(lengthOfLineRenderer - i - 1) / (lengthOfLineRenderer - 1));
                            }
                            pathInterruptedText.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < lengthOfLineRenderer; i++)
                        {
                            path[i] = caster * ((float)i / (lengthOfLineRenderer - 1)) + hit.point * ((float)(lengthOfLineRenderer - i - 1) / (lengthOfLineRenderer - 1));
                        }
                        pathInterruptedText.gameObject.SetActive(false);
                    }
                    aoeRangeIndicator.transform.position = hit.point;
                }
                //aoeRangeIndicator.transform.position = path[0];
            }

            for (int i = 0; i < lengthOfLineRenderer; i++)
            {
                lineRenderer.SetPosition(i, path[i]);
            }
            
        }
    }

    public void Set(Transform _caster, BaseAttack _attack, float _aoeRange = 0f)
    {
        caster = _caster.position;
        attack = _attack;
        aoeRange = _aoeRange;
        if(aoeRange > 0.01f)
        {
            aoeRangeIndicator.SetActive(true);
            aoeRangeIndicator.transform.localScale = Vector3.one * aoeRange * 2;
        }
    }

    void PathInterrupted(Vector3 interruptionPoint)
    {
        pathInterruptedText.gameObject.SetActive(true);
        pathInterruptedText.transform.position = interruptionPoint;
    }

    private void OnDisable()
    {
        pathInterruptedText.gameObject.SetActive(false);
        aoeRangeIndicator.SetActive(false);
    }
}
