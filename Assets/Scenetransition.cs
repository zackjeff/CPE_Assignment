using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Black panel that drops down, swaps the scene while the screen is covered, then lifts away.
// Setup: its own Canvas (Sort Order 100) > a black Image > put this script on the Image.
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Tooltip("Seconds for the drop, and again for the lift.")]
    public float duration = 0.6f;

    [Tooltip("How long the screen stays fully black before the scene swaps.")]
    public float holdTime = 0.2f;

    public bool IsBusy { get; private set; }

    RectTransform Rt { get { return (RectTransform)transform; } }

    // Runs in the editor when you add the script: parks the panel above the screen.
    void Reset() { SetY(1f); }

    void Awake()
    {
        // If we come back to a scene that also contains this canvas, keep only the first one.
        if (Instance != null) { Destroy(transform.root.gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);   // survives the scene change
        SetY(1f);
    }

    public void GoTo(string sceneName)
    {
        if (!IsBusy) StartCoroutine(Run(sceneName));
    }

    IEnumerator Run(string sceneName)
    {
        IsBusy = true;
        yield return StartCoroutine(Slide(1f, 0f));                // drop down and cover the screen
        yield return new WaitForSecondsRealtime(holdTime);
        yield return SceneManager.LoadSceneAsync(sceneName);       // swap scenes while covered
        yield return StartCoroutine(Slide(0f, 1f));                // lift away to reveal the new scene
        IsBusy = false;
    }

    IEnumerator Slide(float from, float to)
    {
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
        {
            SetY(Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t)));
            yield return null;
        }
        SetY(to);
    }

    // y = 1: parked just above the screen. y = 0: covering the screen.
    // Uses screen fractions (anchors), so it works at any resolution.
    void SetY(float y)
    {
        Rt.anchorMin = new Vector2(0f, y);
        Rt.anchorMax = new Vector2(1f, y + 1f);
        Rt.offsetMin = Vector2.zero;
        Rt.offsetMax = Vector2.zero;
    }
}