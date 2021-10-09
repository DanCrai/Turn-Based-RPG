using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TargetSelect : MonoBehaviour
{
    CombatStateMachine unit;
    [SerializeField]
    GameObject selector;
    public delegate void OnHoverDelegate(TargetSelect target);
    event OnHoverDelegate onHoverDelegate;
    public delegate void OnHoverExitDelegate(TargetSelect target);
    event OnHoverExitDelegate onHoverExitDelegate;


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex > 2 && scene.buildIndex < 10)
        {
            selector = GameObject.FindGameObjectWithTag("Selector");
        }
    }
    private void Start()
    {
        unit = GetComponent<CombatStateMachine>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        if(GameManager.IsInstantiated())
            selector = GameObject.FindGameObjectWithTag("Selector");
    }

    private void OnMouseEnter()
    {
        if (GameManager.IsInstantiated())
        {
            selector.SetActive(true);

            // Final position of marker above GO in world space
            Vector3 offsetPos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);

            // Calculate *screen* position (note, not a canvas/recttransform position)
            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);
            RectTransform canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();

            // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);

            // Set
            selector.transform.localPosition = canvasPos;
            onHoverDelegate?.Invoke(this);
        }
    }

    private void OnMouseExit()
    {
        if (GameManager.IsInstantiated())
        {
            selector.SetActive(false);
            onHoverExitDelegate?.Invoke(this);
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.IsInstantiated())
        {
            CombatStateMachine csm = GameManager.GetCurrentCharacter();
            if (csm.GetCurrentState() == CombatStateMachine.TurnState.SelectingTarget && csm.GetComponent<PlayerAi>().IsInRange(transform))
            {
                csm.GetComponent<PlayerAi>().SelectTarget(unit);
            }
        }
    }
    public void AddOnHoverDelegate(OnHoverDelegate f)
    {
        onHoverDelegate += f;
    }
    public void RemoveOnHoverDelegate(OnHoverDelegate f)
    {
        onHoverDelegate -= f;
    }
    public void AddOnHoverExitDelegate(OnHoverExitDelegate f)
    {
        onHoverExitDelegate += f;
    }
    public void RemoveOnHoverExitDelegate(OnHoverExitDelegate f)
    {
        onHoverExitDelegate -= f;
    }

}
