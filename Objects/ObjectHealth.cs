using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectHealth : MonoBehaviour
{
    [SerializeField]
    int baseHp;
    int curHp;
    [SerializeField]
    GameObject HealthBarPrefab;
    GameObject healthBar;
    Text healthText;
    Slider healthSlider;

    void Start()
    {
        curHp = baseHp;
        healthBar = Instantiate(HealthBarPrefab, GameObject.FindGameObjectWithTag("Canvas").transform);
        healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        healthText = healthBar.GetComponentInChildren<Text>();
        healthSlider = healthBar.GetComponentInChildren<Slider>();
        UpdateHealthBar();
    }

    private void Update()
    {
        healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    public void TakeDamage(int amount)
    {
        curHp -= amount;
        UpdateHealthBar();
        if (curHp <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnDestroy()
    {
        Destroy(healthBar);
    }
    void UpdateHealthBar()
    {
        healthText.text = curHp + "/" + baseHp;
        healthSlider.value = (float)curHp / baseHp;
    }
}
