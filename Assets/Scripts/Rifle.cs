using UnityEngine;

public class Rifle : MonoBehaviour
{
   [Header("Rifle Things")] 
   public Camera cam;

   public float giveDamageOf = 10f;
   public float shootingRange = 100f;

   [Header("Rifle Effects")] 
   public ParticleSystem muzzleSpark;

   public GameObject WoodEffect;

   private void Update()
   {
      
      if (Input.GetButtonDown("Fire1"))
      {
         Shoot();
      }
      
       
   }

   private void Shoot()
   {
      muzzleSpark.Play();
      RaycastHit hitInfo;

      if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, shootingRange))
      {
         Debug.Log(hitInfo.transform.name);
         
         ObjectToHit objectToHit = hitInfo.transform.GetComponent<ObjectToHit>();

         if (objectToHit != null)
         {
            objectToHit.ObjectHitDamage(giveDamageOf);
            GameObject WoodGo = Instantiate(WoodEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(WoodGo, 1f);
         }
      }
   }  
}
