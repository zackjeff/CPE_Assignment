using System.Collections;
using UnityEngine;

// Slides a UI element in from the right and stops exactly where you placed it in the editor.
// Put this on each button (it must be a UI object, so it has a RectTransform).
public class SlideIn : MonoBehaviour
{
    [Tooltip("How far off to the right the button starts, in canvas units.")]
    public float startOffset = 2500f;

    [Tooltip("How long the slide takes, in seconds.")]
    public float duration = 0.6f;

    [Tooltip("Wait this long before sliding. Use a bigger delay on each button to stagger them.")]
    public float delay = 0f;

    IEnumerator Start()
    {
        RectTransform rt = (RectTransform)transform;

        // The spot you placed it in the editor is the final, locked position.
        Vector2 endPos = rt.anchoredPosition;
        Vector2 startPos = endPos + new Vector2(startOffset, 0f);

        // Wait off-screen for its turn.
        rt.anchoredPosition = startPos;
        yield return new WaitForSecondsRealtime(delay);

        // Glide from right to left, slowing down as it settles.
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
        {
            float eased = 1f - (1f - t) * (1f - t);
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);
            yield return null;
        }

        // Snap to the exact final position so it stays locked there.
        rt.anchoredPosition = endPos;
    }
}