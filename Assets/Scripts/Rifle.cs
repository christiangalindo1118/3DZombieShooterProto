using System;
using UnityEngine;
using System.Collections;

public class Rifle : MonoBehaviour
{
   [Header("Rifle Things")] public Camera cam;

   public float giveDamageOf = 10f;
   public float shootingRange = 100f;
   public float fireCharge = 15f;
   private float nextTimeToShoot = 0;
   public Animator animator;
   public PlayerScript player;
   public Transform hand;

   [Header("Rifle Amunition and shooting")]
   private int maxinumAmmunition = 32;

   public int mag = 10;
   private int presentAmmunition;
   public float reloadingTime = 1.3f;
   private bool setReloading = false;



   [Header("Rifle Effects")] 
   public ParticleSystem muzzleSpark;

   public GameObject WoodEffect;
   public GameObject goreEffect;

   private void Awake()
   {
      transform.SetParent(hand);
      presentAmmunition = maxinumAmmunition;
   }

   private void Update()
   {

      if (setReloading)
         return;

      if (presentAmmunition <= 0)
      {
         StartCoroutine(Reload());
         return;
      }

      if (Input.GetButton("Fire1") && Time.time >= nextTimeToShoot)
      {
         animator.SetBool("Fire", true);
         animator.SetBool("Idle", true);

         nextTimeToShoot = Time.time + 1f / fireCharge;
         Shoot();
      }
      else if (Input.GetButton("Fire1") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
      {
         animator.SetBool("Idle", false);
         animator.SetBool("FireWalk", true);
      }
      else if (Input.GetButton("Fire2") && Input.GetButton("Fire1"))
      {
         animator.SetBool("Idle", false);
         animator.SetBool("IdleAim", true);
         animator.SetBool("FireWalk", true);
         animator.SetBool("Walk", true);
         animator.SetBool("Reloading", false);
      }
      else
      {
         animator.SetBool("Fire", false);
         animator.SetBool("Idle", true);
         animator.SetBool("FireWalk", false); 
      }


}

   private void Shoot()
   {
      
      //check for mag
      if (mag == 0)
      {
         //show ammo out text
         return;
      }
      
      presentAmmunition--;

      if (presentAmmunition == 0)
      {
         mag--;
      }
      
      //updating the UI
      
      muzzleSpark.Play();
      RaycastHit hitInfo;

      if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, shootingRange))
      {
         Debug.Log(hitInfo.transform.name);
         
         ObjectToHit objectToHit = hitInfo.transform.GetComponent<ObjectToHit>();
         
         Zombie1 zombie1 = hitInfo.transform.GetComponent<Zombie1>();
         Zombie2 zombie2 = hitInfo.transform.GetComponent<Zombie2>();
         
         if (objectToHit != null)
         {
            objectToHit.ObjectHitDamage(giveDamageOf);
            GameObject WoodGo = Instantiate(WoodEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(WoodGo, 1f);
         }
         else if (zombie1 != null)
         {
            zombie1.zombieHitDamage(giveDamageOf);
            GameObject goreEffectGo = Instantiate(goreEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(goreEffectGo, 1f); 
         }
         else if (zombie2 != null)
         {
            zombie2.zombieHitDamage(giveDamageOf);
            GameObject goreEffectGo = Instantiate(goreEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(goreEffectGo, 1f); 
         }
      }
   } 
   
   IEnumerator Reload()
   {
      player.PlayerSpeed = 0f;
      player.PlayerSprint = 0f;
      setReloading = true;
      Debug.Log("reloading");
      animator.SetBool("Reloading", true);
      //play reload sound
      yield return new WaitForSeconds(reloadingTime);
      animator.SetBool("Reloading", false);
      presentAmmunition = maxinumAmmunition;
      player.PlayerSpeed = 1.9f;
      player.PlayerSprint = 3;
      setReloading = false;

   }
}
