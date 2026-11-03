using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Stores the choices so any script (or the next scene) can read them
public static class GameSettings
{
    public static int Rounds = 10;
    public static float PlayerSpeed = 1f;
    public static float TimePerRound = 120f; // seconds
}

public class GameModeMenu : MonoBehaviour
{
    [Serializable]
    public class ModeColumn
    {
        public RectTransform title;
        public Button[] buttons;
        public float[] values;              // value for each button (same order)
        [HideInInspector] public int selected = -1;
    }

    [Header("Columns")]
    public ModeColumn rounds;   // values: 10, 20
    public ModeColumn speed;    // values: 1, 1.5
    public ModeColumn time;     // values: 120, 300

    [Header("Animation")]
    public RectTransform canvasRect;
    public float dropDuration = 0.6f;
    public float riseDuration = 0.45f;
    public float stagger = 0.08f;
    public float delayBeforeExit = 0.4f;

    [Header("Selection Look")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(1f, 0.8f, 0.2f);
    public float selectedScale = 1.12f;

    [Header("Called after menu leaves the screen")]
    public UnityEvent onMenuClosed;

    readonly Dictionary<RectTransform, Vector2> lockedPos = new();
    readonly List<(RectTransform rt, float delay)> items = new();
    ModeColumn[] columns;
    bool ready, exiting;

    float OffscreenY => canvasRect.rect.height + 300f;

    void Start()
    {
        columns = new[] { rounds, speed, time };

        for (int c = 0; c < columns.Length; c++)
        {
            var col = columns[c];
            Register(col.title, c * stagger);

            for (int i = 0; i < col.buttons.Length; i++)
            {
                var btn = col.buttons[i];
                Register((RectTransform)btn.transform, c * stagger + (i + 1) * stagger * 2f);

                int index = i; var column = col;
                btn.onClick.AddListener(() => Select(column, index));
            }
        }

        StartCoroutine(DropIn());
    }

    void Register(RectTransform rt, float delay)
    {
        lockedPos[rt] = rt.anchoredPosition;                       // remember editor position
        rt.anchoredPosition = lockedPos[rt] + Vector2.up * OffscreenY; // hide above screen
        items.Add((rt, delay));
    }

    // ---------- Intro: fall from top ----------
    IEnumerator DropIn()
    {
        float longest = 0f;
        foreach (var (rt, delay) in items)
        {
            Vector2 target = lockedPos[rt];
            StartCoroutine(Move(rt, target + Vector2.up * OffscreenY, target, dropDuration, delay, EaseOutBack));
            longest = Mathf.Max(longest, delay + dropDuration);
        }
        yield return new WaitForSecondsRealtime(longest);
        ready = true;
    }

    // ---------- Selection ----------
    void Select(ModeColumn col, int index)
    {
        if (!ready || exiting) return;

        col.selected = index;
        for (int i = 0; i < col.buttons.Length; i++)
        {
            bool on = i == index;
            col.buttons[i].image.color = on ? selectedColor : normalColor;
            col.buttons[i].transform.localScale = Vector3.one * (on ? selectedScale : 1f);
        }

        foreach (var c in columns)
            if (c.selected < 0) return;   // still waiting for another column

        StartCoroutine(ExitUp());
    }

    // ---------- Exit: fly up offscreen ----------
    IEnumerator ExitUp()
    {
        exiting = true;

        GameSettings.Rounds       = (int)rounds.values[rounds.selected];
        GameSettings.PlayerSpeed  = speed.values[speed.selected];
        GameSettings.TimePerRound = time.values[time.selected];

        yield return new WaitForSecondsRealtime(delayBeforeExit);

        float longest = 0f;
        for (int i = items.Count - 1; i >= 0; i--)        // reverse order = bottom goes first
        {
            var rt = items[i].rt;
            float delay = (items.Count - 1 - i) * stagger * 0.5f;
            Vector2 from = rt.anchoredPosition;
            StartCoroutine(Move(rt, from, from + Vector2.up * OffscreenY, riseDuration, delay, EaseInBack));
            longest = Mathf.Max(longest, delay + riseDuration);
        }

        yield return new WaitForSecondsRealtime(longest);
        onMenuClosed?.Invoke();
    }

    // ---------- Tween helper ----------
    IEnumerator Move(RectTransform rt, Vector2 from, Vector2 to, float dur, float delay, Func<float, float> ease)
    {
        rt.anchoredPosition = from;
        if (delay > 0) yield return new WaitForSecondsRealtime(delay);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / dur;
            rt.anchoredPosition = Vector2.LerpUnclamped(from, to, ease(Mathf.Clamp01(t)));
            yield return null;
        }
        rt.anchoredPosition = to;
    }

    static float EaseOutBack(float x) { const float c1 = 1.70158f, c3 = c1 + 1f; return 1f + c3 * Mathf.Pow(x - 1f, 3) + c1 * Mathf.Pow(x - 1f, 2); }
    static float EaseInBack(float x)  { const float c1 = 1.70158f, c3 = c1 + 1f; return c3 * x * x * x - c1 * x * x; }
}