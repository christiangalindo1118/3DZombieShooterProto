using System;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
   [Header("Camera to Assign")] 
   public GameObject AimCam;
   public GameObject AimCanvas;
   public GameObject ThirdPersonCam;
   public GameObject ThirdPersonCanvas;

   private void Update()
   {
       if (Input.GetButton("Fire2"))
       {
           ThirdPersonCam.SetActive(false);
           ThirdPersonCanvas.SetActive(false);
           AimCam.SetActive(true);
           AimCanvas.SetActive(true);
       }
       else
       {
           ThirdPersonCam.SetActive(true);
           ThirdPersonCanvas.SetActive(true);
           AimCam.SetActive(false);
           AimCanvas.SetActive(false); 
       }
   }
}
