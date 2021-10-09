using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyButton : MonoBehaviour
{
    GameObject ally;
    static Transform spellPanel;
    static List<AllyButton> alliesButtons = new List<AllyButton>();
    int count;
    private void Awake()
    {
        count = transform.GetSiblingIndex();
        if (alliesButtons.Count == 4)
            alliesButtons.Clear();
        alliesButtons.Add(this);
        if(spellPanel == null)
            spellPanel = transform.parent.parent;
    }
    private void OnEnable()
    {
        ally = GameObject.FindGameObjectWithTag("Party").transform.GetChild(count).gameObject;
        GetComponentInChildren<Text>().text = ally.GetComponent<CombatStateMachine>().GetUnit().GetUnitName();
        if (count == 0)
        {
            WorldManager.SetCurrentAlly(ally);
            OnClickAllyButton();
        }
    }
    
    static public void SpellButtons()
    {
        GameObject ally = WorldManager.GetCurrentAlly();
        Unit unit = ally.GetComponent<CombatStateMachine>().GetUnit();
        BaseClass cl = (unit.GetClass());
        List<BaseAttack> attacks = new List<BaseAttack>(cl.GetAvailableSpells());
        attacks.Sort((BaseAttack a1, BaseAttack a2) => a1.GetLevelRequired() - a2.GetLevelRequired());
        for (int i = 1; i < spellPanel.childCount; i++)
        {
            Transform spellSpacer = spellPanel.GetChild(i);
            SpellButton[] sb = spellSpacer.GetComponentsInChildren<SpellButton>();
            sb[0].SetSpell(attacks[(i - 1) * 2]);
            sb[1].SetSpell(attacks[(i - 1) * 2 + 1]);
            Button[] buttons = spellSpacer.GetComponentsInChildren<Button>();
            buttons[0].image.sprite = attacks[(i - 1) * 2].GetSpellSprite();
            buttons[1].image.sprite = attacks[(i - 1) * 2 + 1].GetSpellSprite();
            if (unit.GetLevel() >= attacks[(i - 1) * 2].GetLevelRequired())
            {
                if (unit.GetAttacks().Contains(attacks[(i - 1) * 2]))
                {
                    if (unit.GetAttacks().Contains(attacks[(i - 1) * 2 + 1]))
                    {

                        buttons[0].interactable = true;
                        buttons[1].interactable = true;
                        buttons[0].GetComponent<Image>().color = Color.yellow;
                        buttons[1].GetComponent<Image>().color = Color.yellow;
                    }
                    else
                    {
                        buttons[0].interactable = true;
                        buttons[1].interactable = false;
                        buttons[0].GetComponent<Image>().color = Color.yellow;
                        buttons[1].GetComponent<Image>().color = Color.white;
                    }
                }
                else if (unit.GetAttacks().Contains(attacks[(i - 1) * 2 + 1]))
                {
                    buttons[1].interactable = true;
                    buttons[0].interactable = false;
                    buttons[1].GetComponent<Image>().color = Color.yellow;
                    buttons[0].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    buttons[0].interactable = true;
                    buttons[1].interactable = true;
                    buttons[0].GetComponent<Image>().color = Color.white;
                    buttons[1].GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                foreach (Button b in spellSpacer.GetComponentsInChildren<Button>())
                {
                    b.interactable = false;
                }
            }
        }
    }

    public void OnClickAllyButton()
    {
        Image im = GetComponent<Image>();
        im.color = Color.red;
        foreach(AllyButton ab in alliesButtons)
        {
            if(ab != this)
                ab.GetComponent<Image>().color = Color.white;
        }
        WorldManager.SetCurrentAlly(ally);
        Unit unit = ally.GetComponent<CombatStateMachine>().GetUnit();
        BaseClass cl = (unit.GetClass());
        List<BaseAttack> attacks = new List<BaseAttack>(cl.GetAvailableSpells());
        attacks.Sort((BaseAttack a1, BaseAttack a2) => a1.GetLevelRequired() - a2.GetLevelRequired());
        for(int i = 1; i < spellPanel.childCount; i++)
        {
            Transform spellSpacer = spellPanel.GetChild(i);
            SpellButton[] sb = spellSpacer.GetComponentsInChildren<SpellButton>();
            sb[0].SetSpell(attacks[(i - 1) * 2]);
            sb[1].SetSpell(attacks[(i - 1) * 2 + 1]);
            Button[] buttons = spellSpacer.GetComponentsInChildren<Button>();
            buttons[0].image.sprite = attacks[(i - 1) * 2].GetSpellSprite();
            buttons[1].image.sprite = attacks[(i - 1) * 2 + 1].GetSpellSprite();
            if (unit.GetLevel() >= attacks[(i - 1) * 2].GetLevelRequired())
            {
                if(unit.GetAttacks().Contains(attacks[(i-1)*2]))
                {
                    if (unit.GetAttacks().Contains(attacks[(i - 1) * 2 + 1]))
                    {
                        
                        buttons[0].interactable = true;
                        buttons[1].interactable = true;
                        buttons[0].GetComponent<Image>().color = Color.yellow;
                        buttons[1].GetComponent<Image>().color = Color.yellow;
                    }
                    else
                    {
                        buttons[0].interactable = true;
                        buttons[1].interactable = false;
                        buttons[0].GetComponent<Image>().color = Color.yellow;
                        buttons[1].GetComponent<Image>().color = Color.white;
                    }
                }
                else if(unit.GetAttacks().Contains(attacks[(i-1)*2+1]))
                {
                    buttons[1].interactable = true;
                    buttons[0].interactable = false;
                    buttons[1].GetComponent<Image>().color = Color.yellow;
                    buttons[0].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    buttons[0].interactable = true;
                    buttons[1].interactable = true;
                    buttons[0].GetComponent<Image>().color = Color.white;
                    buttons[1].GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                foreach (Button b in spellSpacer.GetComponentsInChildren<Button>())
                {
                    b.interactable = false;
                }
            }
        }
    }
}
