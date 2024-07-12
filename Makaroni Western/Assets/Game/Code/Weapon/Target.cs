using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Target : MonoBehaviour
{
    public float minSphereRadius;
    public float maxSphereRadius;

    float randomSphereRadius;

    Ray ray;
    RaycastHit hitinfo;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minSphereRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxSphereRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, randomSphereRadius);
    }

    public void ConcentrationBasedRadius(float concentrationPercent)
    {
        randomSphereRadius = Mathf.Lerp(minSphereRadius, maxSphereRadius, (100 - concentrationPercent) / 100);
    }

    public void UpdateAim(Vector3 crosshairPosition, Camera cam)
    {
        ray = cam.ScreenPointToRay(new Vector3(crosshairPosition.x, crosshairPosition.y, 20));

        if (Physics.Raycast(ray, out hitinfo))
        {
            transform.position = ray.GetPoint(20);
        }
        else
        {
            transform.position = ray.GetPoint(20);
        }
    }

    public Vector3 GetRandomPointAround()
    {
        return GetPointAroundInCircle(transform.position, randomSphereRadius);
        
        /*
        Vector3 point = GetPointAroundInSphere(transform.position, randomSphereRadius);
        ray.origin = barrel;
        ray.direction = point;


        if (Physics.Raycast(ray, out hitinfo))
        {
            return hitinfo.point;
        }
        else
        {
            return ray.GetPoint(70);
        }
        */
    }

    Vector3 GetPointAround(Vector3 point, float aroundRadius)
    {
        Vector3 offset = Random.insideUnitSphere * aroundRadius;
        Vector3 pos = point + offset;
        return pos;
    }

    Vector3 GetPointAroundInCircle(Vector3 point, float aroundRadius)
    {
        Vector3 offset = Random.insideUnitCircle * aroundRadius;
        Vector3 pos = point + offset;
        return pos;
    }

    Vector3 GetPointAroundInSphere(Vector3 point, float aroundRadius)
    {
        Vector3 offset = Random.insideUnitSphere * aroundRadius;
        Vector3 pos = point + offset;
        return pos;
    }
}
