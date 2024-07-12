using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bullet;
    public float shootForce = 100f;
    public float concentrationDecreaseValue = 15f;
    public Transform gunBarrel;

    public ParticleSystem MuzzleFlash;
    public AudioSource shotSound;
    public AudioClip[] audioClips;

    private float nextTimeToFire = 0f;

    public float NTF
    {
        get
        {
            return nextTimeToFire;
        }
        set
        {
            nextTimeToFire = value;
        }
    }

    public Cinemachine.CinemachineFreeLook playerCamera;

    [HideInInspector]
    public WeaponRecoil recoil;

    private void Start()
    {
        recoil = GetComponent<WeaponRecoil>();
        shotSound = GetComponent<AudioSource>();

        if(playerCamera != null)
            recoil.playerCamera = playerCamera;
    }

}
