using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WeaponRecoil : MonoBehaviour
{
    [HideInInspector]
    public CinemachineFreeLook playerCamera;

    public Camera cam;

    [HideInInspector]
    public CinemachineImpulseSource cameraShake;

    public float verticalRecoil = 2f;
    public float duration = 0.1f;

    float time;

    public void Awake()
    {
        cameraShake = GetComponent<CinemachineImpulseSource>();
    }

    public void GenerateRecoil(Camera cam)
    {
        time = duration;

        cameraShake.GenerateImpulse(cam.transform.forward);
    }

    public void GenerateRecoil()
    {
        time = duration;

        cameraShake.GenerateImpulse(cam.transform.forward);
    }

    void Update()
    {
        if(time > 0)
        {
            playerCamera.m_YAxis.Value -= ((verticalRecoil/1000) * Time.deltaTime) / duration;
            time -= Time.deltaTime;
        }

    }
}
