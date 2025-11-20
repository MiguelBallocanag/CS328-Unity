using UnityEngine;
using UnityEngine.UI;
using System.Collections;   
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar UI")]
    public Slider slider;
    public Slider BossEaseHealthbar;
    
    [Header("Visual Settings")]
    public Gradient gradient;
    public Image fill;
    
    [Header("Animation")]
    public float lerpSpeed = 0.5f;

    private void Update()
    {
        // Smoothly animate health bar changes
        if (BossEaseHealthbar != null && slider != null && BossEaseHealthbar.value != slider.value)
        {
            BossEaseHealthbar.value = Mathf.Lerp(BossEaseHealthbar.value, slider.value, Time.deltaTime * lerpSpeed);
            
            if (fill != null && gradient != null)
            {
                fill.color = gradient.Evaluate(BossEaseHealthbar.normalizedValue);
            }
        }
    }
    
    // FIXED: Now actually updates the slider value
    public void SetHealth(int health)
    {
        if (slider != null)
        {
            slider.value = health;
        }
    }
    
    public void SetMaxHealth(int health)
    {
        if (slider != null)
        {
            slider.maxValue = health;
            slider.value = health;
        }
        
        if (BossEaseHealthbar != null)
        {
            BossEaseHealthbar.maxValue = health;
            BossEaseHealthbar.value = health;
        }
        
        if (fill != null && gradient != null)
        {
            fill.color = gradient.Evaluate(1f);
        }
    }
}