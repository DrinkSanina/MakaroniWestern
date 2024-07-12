using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCrosshair : MonoBehaviour
{
    public float shakeRadius = 1f;
    public float secondsToMove = 1f;

    public float minSize = 250f;
    public float maxSize = 700f;
    
    [Range(0, 100)]
    public float concentration;

    private float maxSpeed = 150;
    private float minSpeed = 10;

    public float ConcenctrationBasedSize => Mathf.Lerp(minSize, maxSize, (100-concentration)/100);
    float ConcentrationBasedSpeed => Mathf.Lerp(minSpeed, maxSpeed, (100-concentration) / 100);

    bool movesToPoint;
    RectTransform rectTransform;
    public Vector2 CurrentAnchoredPosition => rectTransform.anchoredPosition;
    public Vector2 CurrentPosition => rectTransform.position;

    private Vector2 initialPosition;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {

        UpdateCrosshairPosition();
        UpdateCrosshairSize();
        UpdateCrosshairRotation();

    }

    void UpdateCrosshairSize()
    {
        rectTransform.sizeDelta = new Vector2(ConcenctrationBasedSize, ConcenctrationBasedSize);
    }

    void UpdateCrosshairRotation()
    {
        rectTransform.Rotate(new Vector3(0f, 0f, ConcentrationBasedSpeed) * Time.deltaTime);
    }

    IEnumerator ChangeCrosshairPositionInTime(Vector2 from, Vector2 to, float duration)
    {
        float timePassed = 0f;
        Vector2 parameterData;

        while (timePassed < duration)
        {
            var factor = timePassed / duration;

            parameterData = Vector2.Lerp(from, to, factor);

            rectTransform.anchoredPosition = parameterData;
            timePassed += Mathf.Min(Time.deltaTime, duration - timePassed);
            yield return null;
        }

        rectTransform.anchoredPosition = to;
        movesToPoint = false;
    }

    void UpdateCrosshairPosition()
    {
        //≈сли не движетс€ - найти точку и заставить двигатьс€
        if (!movesToPoint)
        {
            Vector2 oldPosition = rectTransform.anchoredPosition;
            Vector2 newPosition = GetPointAround(initialPosition, shakeRadius);

            StartCoroutine(ChangeCrosshairPositionInTime(oldPosition, newPosition, secondsToMove));
            movesToPoint = true;
        }
    }

    Vector2 GetPointAround(Vector2 point, float aroundRadius)
    {
        Vector2 offset = Random.insideUnitCircle * aroundRadius;
        Vector2 pos = point + offset;
        return pos;
    }
}
