using UnityEngine;
using UnityEngine.UI;
using System.Collections;   
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Slider BossEaseHealthbar;
    public Gradient gradient;
    public Image fill;
    private float lerpSpeed = 0.5f;
    private float healthValue;

    private void Update()
    {
        if (BossEaseHealthbar.value != slider.value)
        {
            BossEaseHealthbar.value = Mathf.Lerp(BossEaseHealthbar.value, slider.value, Time.deltaTime * lerpSpeed);
            fill.color = gradient.Evaluate(BossEaseHealthbar.normalizedValue);
        }
    }
    public void SetHealth(int health)
    {
       healthValue = health;
    }
    public void SetMaxHealth(int health)
    {  
        healthValue = health;
        slider.maxValue = health;
        slider.value = health;
        fill.color=gradient.Evaluate(1f);
    }
}
