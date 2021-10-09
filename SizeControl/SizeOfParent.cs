using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeOfParent : MonoBehaviour
{
    RectTransform parent;
    RectTransform rec;
    [SerializeField]
    float percentageVertical;
    [SerializeField]
    float percentageHorizontal;
    [SerializeField]
    bool isSquare;
    [SerializeField]
    [Tooltip("Should the dimensions be calculated based on the vertical size? If not horizontal size will be considered instead")]
    bool isSquareOnVertical;
    float lastTime = 0f;
    void Start()
    {
        parent = transform.parent.GetComponent<RectTransform>();
        rec = GetComponent<RectTransform>();
        float width = parent.rect.width * percentageHorizontal / 100;
        if (percentageHorizontal < 0.1f)
        {
            width = rec.sizeDelta.x;
        }
        float height = parent.rect.height * percentageVertical / 100;
        if(percentageVertical < 0.1f)
        {
            height = rec.sizeDelta.y;
        }
        if(isSquare)
        {
            if(isSquareOnVertical)
            {
                width = height;
            }
            else
            {
                height = width;
            }
        }
        rec.sizeDelta = new Vector2(width, height);
    }

    private void Update()
    {
        if (Time.time - lastTime > 0.1f)
        {
            float width = parent.rect.width * percentageHorizontal / 100;
            if (percentageHorizontal < 0.1f)
            {
                width = rec.sizeDelta.x;
            }
            float height = parent.rect.height * percentageVertical / 100;
            if (percentageVertical < 0.1f)
            {
                height = rec.sizeDelta.y;
            }
            if (isSquare)
            {
                if (isSquareOnVertical)
                {
                    width = height;
                }
                else
                {
                    height = width;
                }
            }
            rec.sizeDelta = new Vector2(width, height);
            lastTime = Time.time;
        }
    }
}
