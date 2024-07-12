using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalCowboy : PlayerController
{
    [Header("Локальный игрок")]
    public Camera playerCamera;
    public Cinemachine.CinemachineFreeLook cineCamera;

    public Weapon weapon;
    public DynamicCrosshair crosshair;


    public float minRandSphereRadius = 0.5f;
    public float maxRandSphereRadius = 4f;

    private float ShootSphereRadius;

    public GameObject UI;
    public Image healthBar;
    public Image concBar;

    void Update()
    {
        if (GameStatus.CurrentState == GameState.Duel)
        {
            RayAim();

            float yawCamera = playerCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), 15f * Time.deltaTime);
            Concentrate();

            if (Input.GetButton("Fire1") && Time.time >= weapon.NTF)
            {
                Fire();
                shots++;
            }

            concBar.fillAmount = concentration / 100f;
        }
        healthBar.fillAmount = health / 100f;
    }

    private void RayAim()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(crosshair.CurrentPosition.x, crosshair.CurrentPosition.y, 0));
        RaycastHit hitinfo;

        if (Physics.Raycast(ray, out hitinfo))
        {
            AimLookAt.transform.position = hitinfo.point;
        }
        else
        {
            AimLookAt.transform.position = ray.GetPoint(20);
        }

        ShootSphereRadius = Mathf.Lerp(minRandSphereRadius, maxRandSphereRadius, (100 - concentration) / 100);

    }

    protected override void Concentrate()
    {
        if (concentration < 100f)
        {
            concentration += Time.deltaTime * overTimeConcenctationChangeRate;
        }

        crosshair.concentration = concentration;
    }

    void Fire()
    {
        weapon.NTF = Time.time + 1f / fireRate;
        MuzzleFlash();

        int randomSounds = Random.Range(0, weapon.audioClips.Length);
        weapon.shotSound.PlayOneShot(weapon.audioClips[randomSounds]);

        Vector3 randomTarget = GetPointAroundInCircle(AimLookAt.transform.position, ShootSphereRadius);

        Vector3 direction = randomTarget - weapon.gunBarrel.position;

        GameObject currentBullet = Instantiate(weapon.bullet, weapon.gunBarrel.position, Quaternion.identity);
        currentBullet.transform.forward = direction.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * weapon.shootForce, ForceMode.Impulse);

        concentration -= weapon.concentrationDecreaseValue;
        weapon.recoil.GenerateRecoil(playerCamera);
    }

    Vector3 GetPointAroundInCircle(Vector3 point, float aroundRadius)
    {
        Vector3 offset = Random.insideUnitCircle * aroundRadius;
        Vector3 pos = point + offset;
        return pos;
    }

    void MuzzleFlash()
    {
        weapon.MuzzleFlash.Play();
    }
}
