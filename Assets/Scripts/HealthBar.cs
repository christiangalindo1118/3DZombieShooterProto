using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthbarSlider;
    private RectTransform sliderRect;
    private Vector2 originalSize;

    private void Start()
    {
        // Guardar el tamaño original del slider
        sliderRect = healthbarSlider.GetComponent<RectTransform>();
        originalSize = sliderRect.sizeDelta;
    }

    public void GiveFullHealth(float health)
    {
        healthbarSlider.maxValue = health;
        healthbarSlider.value = health;
        
        // Forzar el tamaño original después de cambiar maxValue
        if (sliderRect != null)
        {
            sliderRect.sizeDelta = originalSize;
        }
    }

    public void SetHealth(float health)
    {
        healthbarSlider.value = health;
        
        // Mantener el tamaño fijo
        if (sliderRect != null)
        {
            sliderRect.sizeDelta = originalSize;
        }
    }
}
