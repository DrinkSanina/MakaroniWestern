using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerCowboy : PlayerController
{
    [Header("Ковбой-машина")]
    public Target target;
    public Weapon weapon;
    public Transform EnemyTarget;


    void Update()
    {
        if(GameStatus.CurrentState == GameState.Duel)
        {
            AimLookAt.position = EnemyTarget.position;
            target.transform.position = EnemyTarget.position;

            Concentrate();

            if (Time.time >= weapon.NTF && concentration >= 70f)
            {
                Fire();
                shots++;
            }
        }
        
    }

    void Fire()
    {
        weapon.NTF = Time.time + 1f / fireRate;
        MuzzleFlash();

        Vector3 randomTarget = target.GetRandomPointAround();

        Vector3 direction = randomTarget - weapon.gunBarrel.position;

        GameObject currentBullet = Instantiate(weapon.bullet, weapon.gunBarrel.position, Quaternion.identity);

        currentBullet.transform.forward = direction.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * weapon.shootForce, ForceMode.Impulse);

        concentration -= weapon.concentrationDecreaseValue;
    }

    void MuzzleFlash()
    {
        weapon.MuzzleFlash.Play();
        int randomSounds = Random.Range(0, weapon.audioClips.Length);
        weapon.shotSound.PlayOneShot(weapon.audioClips[randomSounds]);
    }

    protected override void Concentrate()
    {
        if (concentration < 100f)
        {
            concentration += Time.deltaTime * overTimeConcenctationChangeRate;
        }

        target.ConcentrationBasedRadius(concentration);
    }
}
