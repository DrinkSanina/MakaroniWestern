using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class NetworkCowboy : NetworkBehaviour
{
    [Header("Общие поля")]
    public Transform AimLookAt;
    public PlayerType playerType;

    public float fireRate = 1f;

    [Range(0, 100)]
    [SyncVar]
    public float concentration;

    [SyncVar]
    public float health = 100f;

    [SyncVar]
    public int shots;

    public float overTimeConcenctationChangeRate = 20f;


    [Header("Сетевой игрок")]

    [SyncVar]
    public string playerName;

    public Camera playerCamera;
    public Cinemachine.CinemachineFreeLook cineCamera;
    public GameObject UI;

    public Image healthBar;
    public Image concBar;

    public Weapon weapon;
    public DynamicCrosshair crosshair;

    public float minRandSphereRadius = 0.5f;
    public float maxRandSphereRadius = 4f;

    private float ShootSphereRadius;

    bool dead;
    public override void OnStartAuthority()
    {
        playerCamera.gameObject.SetActive(true);
        cineCamera.gameObject.SetActive(true);
        UI.gameObject.SetActive(true);
    }


    void Update()
    {
        if (!isLocalPlayer || dead) return;

        healthBar.fillAmount = health / 100f;

        if (GameStatus.CurrentState == GameState.Duel)
        {
            if (crosshair != null)
                RayAim();

            float yawCamera = playerCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), 15f * Time.deltaTime);
            Concentrate();
            concBar.fillAmount = concentration / 100f;
            if (Input.GetButton("Fire1") && Time.time >= weapon.NTF)
            {
                Fire();
                Recoil();
            }
        }
    }

    [Client]
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
        crosshair.concentration = concentration;

    }

    [Command]
    void Concentrate()
    {
        if (concentration < 100f)
        {
            concentration += Time.deltaTime * overTimeConcenctationChangeRate;
        }  
    }

    [ClientRpc]
    public void DecreaseHealth(float value)
    {
        health -= value;

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        GetComponent<Animator>().enabled = false;
        Weapon playerWeapon = gameObject.GetComponentInChildren<Weapon>();
        Destroy(playerWeapon.gameObject);
    }


    [Command]
    void DecreaseConcentration()
    {
        concentration -= weapon.concentrationDecreaseValue;
    }

    [Command]
    void Fire()
    {
        MuzzleFlash();
        Vector3 randomTarget = GetPointAroundInCircle(AimLookAt.transform.position, ShootSphereRadius);
        Vector3 direction = randomTarget - weapon.gunBarrel.position;
        GameObject currentBullet = Instantiate(weapon.bullet, weapon.gunBarrel.position, Quaternion.identity);
        currentBullet.transform.forward = direction.normalized;
        NetworkServer.Spawn(currentBullet);
        shots++;
    }

    [Client]
    void Recoil()
    {
        weapon.NTF = Time.time + 1f / fireRate;
        weapon.recoil.GenerateRecoil(playerCamera);
        DecreaseConcentration();
    }

    [ClientRpc]
    void MuzzleFlash()
    {
        weapon.MuzzleFlash.Play();
        int randomSounds = Random.Range(0, weapon.audioClips.Length);
        weapon.shotSound.PlayOneShot(weapon.audioClips[randomSounds]);
    }



    Vector3 GetPointAroundInCircle(Vector3 point, float aroundRadius)
    {
        Vector3 offset = Random.insideUnitCircle * aroundRadius;
        Vector3 pos = point + offset;
        return pos;
    }
}
