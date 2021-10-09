using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    BaseAttack spell;
    public void SetSpell(BaseAttack _spell)
    {
        spell = _spell;
        GetComponentInChildren<Text>().text = spell.GetName();
    }
    public BaseAttack GetSpell()
    {
        return spell;
    }
    public void OnClickSpellButton()
    {
        if (!object.ReferenceEquals(spell, null))
        {
            WorldManager.GetCurrentAlly().GetComponent<CombatStateMachine>().GetUnit().AddAttack(spell);
            AllyButton.SpellButtons();
        }
        else
            Debug.LogWarning("No spell added to the button!");
    }
}
