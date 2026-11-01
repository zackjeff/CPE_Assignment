using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIBobbing : MonoBehaviour
{
    [Header("Bobbing Settings")]
    [Tooltip("How far up and down the image moves (in pixels).")]
    public float amplitude = 15f;

    [Tooltip("How fast the image bobs.")]
    public float speed = 2f;

    private RectTransform rectTransform;
    private Vector2 startPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Save the starting UI position
        startPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // Calculate the new Y position using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;

        // Apply the updated anchored position
        rectTransform.anchoredPosition = new Vector2(startPosition.x, newY);
    }
}