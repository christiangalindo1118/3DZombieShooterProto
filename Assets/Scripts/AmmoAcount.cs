using TMPro;
using UnityEngine;

public class AmmoAcount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammunitionText;
    [SerializeField] private TextMeshProUGUI magText;

    public static AmmoAcount occurrence { get; private set; }

    private void Awake()
    {
        if (occurrence != null && occurrence != this)
        {
            Destroy(gameObject);
            return;
        }
        occurrence = this;
    }

    public void UpdateAmmoText(int presentAmmunition)
    {
        if (ammunitionText != null)
        {
            ammunitionText.text = $"Ammo: {presentAmmunition}";
        }
    }

    public void UpdateMagText(int presentMag)
    {
        if (magText != null)
        {
            magText.text = $"Magazines: {presentMag}";
        }
    }
}