using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilitySelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //the attack the button is responsible for
    BaseAttack attack;
    //the character who has the ability
    CombatStateMachine caster;
    //the button this is attached to
    Image buttonImage;
    GameObject abilityDescription;
    Text abilityDescriptionText;

    public AbilitySelectButton(BaseAttack _attack, CombatStateMachine _caster)
    {
        attack = _attack;
        caster = _caster;
    }
    public void SetAbilitySelectButton(BaseAttack _attack, CombatStateMachine _caster)
    {
        attack = _attack;
        caster = _caster;
        //Debug.Log(attack.GetSpellSprite());
        if (buttonImage != null)
        {
            int cooldown = attack.GetCooldown();
            int baseCooldown = attack.GetBaseCooldown();
            buttonImage.sprite = attack.GetSpellSprite();
            buttonImage.fillAmount = (float)(baseCooldown - cooldown) / baseCooldown;
            if (cooldown > 0)
                buttonImage.GetComponentInChildren<Text>().text = cooldown.ToString();
            else
                buttonImage.GetComponentInChildren<Text>().text = "";
        }
    }
    private void Awake()
    {
        abilityDescription = GameObject.FindGameObjectWithTag("AbilityDescription");
        abilityDescriptionText = abilityDescription.GetComponentInChildren<Text>();
        buttonImage = GetComponent<Image>();
    }
    private void Start()
    {
        //later replace with the caster and the attack being given from a constructor
        caster = GameManager.GetCurrentCharacter();
        abilityDescription.SetActive(false);
        int cooldown = attack.GetCooldown();
        int baseCooldown = attack.GetBaseCooldown();
        buttonImage.fillAmount = (float)(baseCooldown - cooldown) / baseCooldown;

        //attack = new BaseAttack("1", 1, 1, 3f, BaseAttack.TargetingSystem.unit, BaseAttack.AffectedTargets.enemies);
    }

    public void SelectAbility()
    {
        if (caster.GetComponent<UnitMovement>().GetIsMoving())
            return;
        if (caster.GetIsActing())
            return;
        caster.SetCurState(CombatStateMachine.TurnState.SelectingTarget);
        caster.Act();
        PlayerAi pAi = caster.GetComponent<PlayerAi>();
        pAi.ButtonSelectAction(attack);
        pAi.SetCurAbilityButton(this);
        buttonImage.color = Color.red;
    }

    public void UnselectAbility()
    {
        buttonImage.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (abilityDescription.activeInHierarchy)
            return;
        abilityDescription.SetActive(true);
        abilityDescription.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 50f, Input.mousePosition.z);
        abilityDescriptionText.text = attack.GetAttackMessage();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        abilityDescription.SetActive(false);
    }
}
