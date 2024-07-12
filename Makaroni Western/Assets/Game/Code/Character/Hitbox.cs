using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitboxType {Head, Body, Limb, RicoSurface};

public class Hitbox : MonoBehaviour
{
    public HitboxType hitBoxType;
    public PlayerController pc;

    public GameObject impactEffect;
    public GameObject netImpactEffect;


    private void OnCollisionEnter(Collision collision)
    {
        GameObject collider = collision.gameObject;
        if(collider.GetComponent<Bullet>() != null)
        {
            GameObject imp = Instantiate(impactEffect, collider.transform.position, Quaternion.LookRotation(collider.transform.up));
            Destroy(imp, 1f);

            switch(hitBoxType)
            {
                case HitboxType.Head:
                {
                    Destroy(collider.gameObject);
                    pc.health -= 1000;
                    break;
                }
                case HitboxType.Body:
                {
                    Destroy(collider.gameObject);
                    pc.health -= 55;
                    break;
                }
                case HitboxType.Limb:
                {
                    Destroy(collider.gameObject);
                    pc.health -= 30;
                    break;
                }
                case HitboxType.RicoSurface: Ricochet(collider); break;
            }
        }
    }

    void Ricochet(GameObject RicoObject)
    {
        var rb = RicoObject.GetComponent<Rigidbody>();
        if (Random.Range(0, 100) > 65)
        {

        }
        else
        {
            Destroy(RicoObject);
        }
    }
}
