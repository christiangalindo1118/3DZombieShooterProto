using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToHit : MonoBehaviour
{
    public float ObjectHealt = 30f;

    public void ObjectHitDamage(float amount)
    {
        ObjectHealt -= amount;
        if (ObjectHealt <= 0f)
        {
            Die();
        }

        void Die()
        {
            Destroy(gameObject);    
        }
}
}
