using System.Collections;
using UnityEngine;

// Moves a UI element in from off-screen and locks it exactly where you placed it in the editor.
// Start Offset (0, 1500) = drops in from the top. Start Offset (2500, 0) = slides in from the right.
// Put this on any UI object (it needs a RectTransform). Everything inside it moves along with it.
public class SlideIn : MonoBehaviour
{
    [Tooltip("Where it starts, relative to its final spot, in canvas units. (0, 1500) drops from the top; (2500, 0) slides from the right.")]
    public Vector2 startOffset = new Vector2(2500f, 0f);

    [Tooltip("How long the move takes, in seconds. A bounce looks better with about 1.")]
    public float duration = 0.6f;

    [Tooltip("Wait this long before moving. Use a bigger delay on each item to stagger them.")]
    public float delay = 0f;

    [Tooltip("Bounce a little when it lands.")]
    public bool bounce = false;

    IEnumerator Start()
    {
        RectTransform rt = (RectTransform)transform;

        // The spot you placed it in the editor is the final, locked position.
        Vector2 endPos = rt.anchoredPosition;
        Vector2 startPos = endPos + startOffset;

        // Wait off-screen for its turn.
        rt.anchoredPosition = startPos;
        yield return new WaitForSecondsRealtime(delay);

        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
        {
            float eased = bounce ? EaseOutBounce(t) : 1f - (1f - t) * (1f - t);
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);
            yield return null;
        }

        // Snap to the exact final position so it stays locked there.
        rt.anchoredPosition = endPos;
    }

    // Falls with gravity, then bounces a few times before settling.
    static float EaseOutBounce(float t)
    {
        const float n = 7.5625f;
        const float d = 2.75f;

        if (t < 1f / d) return n * t * t;
        if (t < 2f / d) { t -= 1.5f / d; return n * t * t + 0.75f; }
        if (t < 2.5f / d) { t -= 2.25f / d; return n * t * t + 0.9375f; }
        t -= 2.625f / d;
        return n * t * t + 0.984375f;
    }
}