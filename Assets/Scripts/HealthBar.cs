using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthbarSlider;
    private RectTransform sliderRect;
    private Vector2 originalSize;

    private void Start()
    {
        sliderRect = healthbarSlider.GetComponent<RectTransform>();
        originalSize = sliderRect.sizeDelta;
    }

    public void GiveFullHealth(float health)
    {
        healthbarSlider.maxValue = health;
        healthbarSlider.value = health;
        
        if (sliderRect != null)
        {
            sliderRect.sizeDelta = originalSize;
        }
    }

    public void SetHealth(float health)
    {
        healthbarSlider.value = health;
        
        if (sliderRect != null)
        {
            sliderRect.sizeDelta = originalSize;
        }
    }
}
