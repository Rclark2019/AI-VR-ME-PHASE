using System.Collections;
using System.Collections.Generic;  
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text toastText;          
    [SerializeField] private float fadeDuration = 0.3f;

    // Simple struct to hold a toast request
    private struct ToastRequest
    {
        public string Message;
        public float Duration;

        public ToastRequest(string message, float duration)
        {
            Message = message;
            Duration = duration;
        }
    }

    // ✅ generic Queue<T>, not System.Collections.Queue
    private readonly Queue<ToastRequest> _queue = new Queue<ToastRequest>();
    private Coroutine _currentRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void Show(string message, float duration = 2f)
    {
        if (canvasGroup == null || toastText == null)
        {
            Debug.LogWarning("ToastManager is missing UI references.");
            return;
        }

        _queue.Enqueue(new ToastRequest(message, duration));

        if (_currentRoutine == null)
        {
            _currentRoutine = StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        while (_queue.Count > 0)
        {
            ToastRequest req = _queue.Dequeue();

            toastText.text = req.Message;

            // Fade in
            yield return FadeCanvasGroup(canvasGroup, 0f, 1f, fadeDuration);

            // Stay visible
            yield return new WaitForSeconds(req.Duration);

            // Fade out
            yield return FadeCanvasGroup(canvasGroup, 1f, 0f, fadeDuration);
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        _currentRoutine = null;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float time)
    {
        cg.interactable = true;
        cg.blocksRaycasts = true;

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / time);
            cg.alpha = Mathf.Lerp(from, to, lerp);
            yield return null;
        }

        cg.alpha = to;

        if (Mathf.Approximately(to, 0f))
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }
}
