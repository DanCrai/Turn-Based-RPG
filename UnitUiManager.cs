using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UnitUiManager : MonoBehaviour
{

    [SerializeField]
    GameObject UnitPanelPrefab;
    GameObject UnitPanel;
    [SerializeField]
    GameObject EffectImage;
    Text nameText;
    GameObject hpPanel;
    GameObject mpPanel;
    GameObject effectsPanel;
    Text hpText;
    Text mpText;
    Slider hpSlider;
    Slider mpSlider;
    CombatStateMachine csm;
    // Start is called before the first frame update
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex > 2 && scene.buildIndex < 10)
        {
            Transform unitsPanel;
            if(gameObject.tag == "Ally")
            {
                unitsPanel = (GameObject.FindGameObjectWithTag("AlliesPanel")).transform;
            }
            else
            {
                unitsPanel = (GameObject.FindGameObjectWithTag("EnemiesPanel")).transform;
            }
            UnitPanel = Instantiate(UnitPanelPrefab, unitsPanel);
            nameText = UnitPanel.transform.GetChild(0).GetComponent<Text>();
            hpText = UnitPanel.transform.GetChild(1).GetComponentInChildren<Text>();
            mpText = UnitPanel.transform.GetChild(2).GetComponentInChildren<Text>();
            hpSlider = UnitPanel.transform.GetChild(1).GetComponentInChildren<Slider>();
            mpSlider = UnitPanel.transform.GetChild(2).GetComponentInChildren<Slider>();
            effectsPanel = UnitPanel.transform.GetChild(3).gameObject;
            StartCoroutine(SetCombatStateMachine());
        }
    }
    private void Start()
    {
        csm = gameObject.GetComponent<CombatStateMachine>();
        if (csm == null)
            Debug.LogWarning("Combat state machine not found!");
        if(gameObject.tag == "Ally")
            SceneManager.sceneLoaded += OnSceneLoaded;
        if (GameManager.IsInstantiated())
        {
            Transform unitsPanel;
            if (gameObject.tag == "Ally")
            {
                unitsPanel = (GameObject.FindGameObjectWithTag("AlliesPanel")).transform;
            }
            else
            {
                unitsPanel = (GameObject.FindGameObjectWithTag("EnemiesPanel")).transform;
            }
            UnitPanel = Instantiate(UnitPanelPrefab, unitsPanel);
            nameText = UnitPanel.transform.GetChild(0).GetComponent<Text>();
            hpText = UnitPanel.transform.GetChild(1).GetComponentInChildren<Text>();
            mpText = UnitPanel.transform.GetChild(2).GetComponentInChildren<Text>();
            hpSlider = UnitPanel.transform.GetChild(1).GetComponentInChildren<Slider>();
            mpSlider = UnitPanel.transform.GetChild(2).GetComponentInChildren<Slider>();
            effectsPanel = UnitPanel.transform.GetChild(3).gameObject;
            StartCoroutine(SetCombatStateMachine());
        }
    }

    IEnumerator SetCombatStateMachine()
    {
        yield return new WaitUntil(() => csm.GetUnit() != null);
        csm.GetUnit().AddDamageTakenDelegate(UpdateHpPanel);
        csm.GetUnit().AddHealTakenDelegate(UpdateHpPanel);
        csm.GetUnit().AddManaChangedDelegate(UpdateMpPanel);
        csm.GetUnit().AddEffectAddedDelegate(UpdateAddEffect);
        csm.GetUnit().AddEffectRemoveddDelegate(UpdateRemoveEffect);
        TargetSelect ts = csm.GetComponent<TargetSelect>();
        ts.AddOnHoverDelegate(HighlightPanel);
        ts.AddOnHoverExitDelegate(UnhighlightPanel);
        nameText.text = csm.GetUnit().GetUnitName();
        UpdateAll();
        /*Vector3 offsetPos = new Vector3(csm.transform.position.x, csm.transform.position.y + 1.5f, csm.transform.position.z);

        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);
        RectTransform canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();

        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);

        hpPanel.transform.localPosition = canvasPos;*/
    }

    void HighlightPanel(TargetSelect ts)
    {
        UnitPanel.GetComponent<Image>().color = Color.green;
    }
    void UnhighlightPanel(TargetSelect ts)
    {
        UnitPanel.GetComponent<Image>().color = Color.black;
    }

    void UpdateAddEffect()
    {
        BaseEffect effect = csm.GetUnit().GetEffects()[csm.GetUnit().GetEffects().Count - 1];
        GameObject effectSprite = Instantiate(EffectImage, effectsPanel.transform);
        effectSprite.GetComponent<Image>().sprite = effect.GetEffectSprite();
        effectSprite.GetComponent<Image>().color = effect.GetSpriteColor();
    }

    void UpdateRemoveEffect(BaseEffect effect)
    {
        for(int i = 0; i < effectsPanel.transform.childCount; i++)
        {
            if(effectsPanel.transform.GetChild(i).GetComponent<Image>().sprite == effect.GetEffectSprite())
            {
                Destroy(effectsPanel.transform.GetChild(i).gameObject);
                return;
            }    
        }
    }

    void UpdateHpPanel()
    {
        int curHp = csm.GetUnit().GetCurrentHp();
        int baseHp = csm.GetUnit().GetBaseHp();
        hpText.text = curHp.ToString() + "/" + baseHp.ToString();
        hpSlider.value = (float)curHp / baseHp;
    }

    void UpdateMpPanel()
    {
        int curMp = csm.GetUnit().GetCurrentMp();
        int baseMp = csm.GetUnit().GetBaseMp();
        mpText.text = curMp.ToString() + "/" + baseMp.ToString();
        mpSlider.value = (float)curMp / baseMp;
    }

    void UpdateAll()
    {
        UpdateHpPanel();
        UpdateMpPanel();
    }
}
