using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    [Header("Общие поля")]
    public Transform AimLookAt;
    public PlayerType playerType;

    public float fireRate = 1f;

    [Range(0, 100)]
    public float concentration;
    public float overTimeConcenctationChangeRate = 20f;
    public float health = 100f;


    public int shots;
    protected abstract void Concentrate();
}
