using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyDescriptionButton : MonoBehaviour
{
    GameObject ally;
    static Transform statsPanel;
    static List<AllyDescriptionButton> alliesButtons = new List<AllyDescriptionButton>();
    int count;
    private void Awake()
    {
        count = transform.GetSiblingIndex();
        if (alliesButtons.Count == 4)
            alliesButtons.Clear();
        alliesButtons.Add(this);
        if (statsPanel == null)
            statsPanel = transform.parent.parent;
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
    public void OnClickAllyButton()
    {
        Image im = GetComponent<Image>();
        im.color = Color.red;
        foreach (AllyDescriptionButton ab in alliesButtons)
        {
            if (ab != this)
                ab.GetComponent<Image>().color = Color.white;
        }
        WorldManager.SetCurrentAlly(ally);
        Unit unit = ally.GetComponent<CombatStateMachine>().GetUnit();

        Text[] texts = statsPanel.GetComponentsInChildren<Text>();
        texts[4].text = unit.GetUnitName();
        texts[5].text = "Level: " + unit.GetLevel().ToString();
        texts[6].text = "HP: " + unit.GetCurrentHp() + "/" + unit.GetBaseHp();
        texts[7].text = "MP: " + unit.GetCurrentMp() + "/" + unit.GetBaseMp();
        int availablePoints = unit.GetAvailablePoints();
        texts[8].text = "Available stat points: " + availablePoints;
        Unit.Stats stats = unit.GetStats();
        texts[9].text = "Strength: " + stats.stats["STR"];
        texts[10].text = "Agility: " + stats.stats["AGI"];
        texts[11].text = "Constitution: " + stats.stats["CONST"];
        texts[12].text = "Intelligence: " + stats.stats["INT"];
        Button[] buttons = statsPanel.GetComponentsInChildren<Button>();
        if(availablePoints <= 0)
        {
            for(int i = 4; i < 8; i++)
            {
                buttons[i].interactable = false;
            }
        }
        else
        {
            for (int i = 4; i < 8; i++)
            {
                buttons[i].interactable = true;
            }
        }

        /*for (int i = 1; i < statsPanel.childCount; i++)
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
        }*/
    }
    static public void UpdateDescriptionTexts()
    {
        Unit unit = WorldManager.GetCurrentAlly().GetComponent<CombatStateMachine>().GetUnit();
        Text[] texts = statsPanel.GetComponentsInChildren<Text>();
        texts[4].text = unit.GetUnitName();
        texts[5].text = "Level: " + unit.GetLevel().ToString();
        texts[6].text = "HP: " + unit.GetCurrentHp() + "/" + unit.GetBaseHp();
        texts[7].text = "MP: " + unit.GetCurrentMp() + "/" + unit.GetBaseMp();
        int availablePoints = unit.GetAvailablePoints();
        texts[8].text = "Available stat points: " + availablePoints;
        Unit.Stats stats = unit.GetStats();
        texts[9].text = "Strength: " + stats.stats["STR"];
        texts[10].text = "Agility: " + stats.stats["AGI"];
        texts[11].text = "Constitution: " + stats.stats["CONST"];
        texts[12].text = "Intelligence: " + stats.stats["INT"];
        Button[] buttons = statsPanel.GetComponentsInChildren<Button>();
        if (availablePoints <= 0)
        {
            for (int i = 4; i < 8; i++)
            {
                buttons[i].interactable = false;
            }
        }
        else
        {
            for (int i = 4; i < 8; i++)
            {
                buttons[i].interactable = true;
            }
        }
    }
}
