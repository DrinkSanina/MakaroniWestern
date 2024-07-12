using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkProjectile : NetworkBehaviour
{
    public float destroyAfter = 3f;
    public float force = 100f;

    private Rigidbody rigidBody;
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddForce(transform.forward * force);
    }

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }


    [ServerCallback]
    void OnCollisionEnter(Collision co)
    {
        GameObject collider = co.gameObject;
        Hitbox hb = collider.GetComponent<Hitbox>();

        if (hb != null)
        {
            GameObject imp = Instantiate(hb.netImpactEffect, gameObject.transform.position, Quaternion.LookRotation(gameObject.transform.up));
            NetworkServer.Spawn(imp);
            Destroy(imp, 1f);

            switch (hb.hitBoxType)
            {
                case HitboxType.RicoSurface:
                {
                    if (Random.Range(0, 100) > 65)
                    {
                        DestroySelf();
                    }
                    break;
                }
                case HitboxType.Head:
                {
                    DestroySelf();
                    collider.GetComponentInParent<NetworkCowboy>().DecreaseHealth(1000f);
                    break;
                }
                case HitboxType.Body:
                {
                    DestroySelf();
                    collider.GetComponentInParent<NetworkCowboy>().DecreaseHealth(55f);
                    break;
                }
                case HitboxType.Limb:
                {
                    DestroySelf();
                    collider.GetComponentInParent<NetworkCowboy>().DecreaseHealth(30f);
                    break;
                }
                
            }
        }

        NetworkServer.Destroy(gameObject);
    }



}
