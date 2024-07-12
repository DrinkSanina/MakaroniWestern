using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointMarkup : MonoBehaviour
{
    public Vector3 size = new Vector3(0.6f, 1.5f, 0.56f);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(new Vector3(transform.position.x,transform.position.y + size.y/2,transform.position.z), size);
    }
}
