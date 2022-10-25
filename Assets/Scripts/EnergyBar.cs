using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    private Image healthbar;
    public float currentHealth ;
    private float maxHealth ;
    PlayerStats playerStats;
    void Start()
    {
        healthbar = GetComponent<Image>();
        playerStats = FindObjectOfType<PlayerStats>();
        maxHealth = playerStats.playerMP;
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = playerStats.playerMP;
        healthbar.fillAmount = currentHealth / maxHealth; 
    }
}
