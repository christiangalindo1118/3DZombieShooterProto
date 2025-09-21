using TMPro;
using UnityEngine;

public class AmmoAcount : MonoBehaviour
{
    public TextMeshProUGUI ammunitionText;  // Para Canvas UI
    public TextMeshProUGUI magText;         // Para Canvas UI

    public static AmmoAcount occurrence;

    private void Awake()
    {
        occurrence = this;
    }

    public void UpdateAmmoText(int presentAmmunition)
    {
        if (ammunitionText != null)
        {
            ammunitionText.text = "Ammo: " + presentAmmunition;
        }
    }

    public void UpdateMagText(int presentMag)
    {
        if (magText != null)
        {
            magText.text = "Magazines: " + presentMag;
        }
    }
}