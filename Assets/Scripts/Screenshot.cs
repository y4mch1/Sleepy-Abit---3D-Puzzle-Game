using System;
using System.IO;
using UnityEngine;
using TMPro;
using System.Collections;

public class ScreenshotWithPanelTMP : MonoBehaviour
{
    [Header("Screenshot Settings")]
    public string filePrefix = "screenshot";
    public bool useSubfolder = true;

    [Header("Panel Settings")]
    public RectTransform panel; 
    public TMP_Text panelText; // <-- TextMeshPro
    public float slideDistance = 200f;
    public float slideDuration = 0.6f;
    public float fadeDuration = 0.5f;

    private CanvasGroup panelGroup;
    private Vector2 startPos;

    private void Start()
    {
        if (panel != null)
        {
            startPos = panel.anchoredPosition;

            panelGroup = panel.GetComponent<CanvasGroup>();
            if (panelGroup == null)
            {
                panelGroup = panel.gameObject.AddComponent<CanvasGroup>();
            }

            panelGroup.alpha = 0f;
        }
    }

    public void CaptureNow()
    {
        StartCoroutine(CaptureCoroutine());
    }

    private IEnumerator CaptureCoroutine()
    {
        yield return new WaitForEndOfFrame();

        int w = Screen.width;
        int h = Screen.height;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string folder = Application.persistentDataPath;
        if (useSubfolder)
        {
            folder = Path.Combine(folder, "Screenshots");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        }

        string filename = $"{filePrefix}_{timestamp}.png";
        string fullPath = Path.Combine(folder, filename);

        File.WriteAllBytes(fullPath, bytes);
        Debug.Log($"Screenshot saved -> {fullPath}");

        // Update TMP text
        if (panelText != null)
        {
            panelText.text = $"SCREENSHOT SAVED:\nLOCATION:\n{fullPath}";
        }

        if (panel != null)
            yield return StartCoroutine(PanelAnimation());
    }

    private IEnumerator PanelAnimation()
    {
        panelGroup.alpha = 1f;
        Vector2 targetPos = startPos - new Vector2(0, slideDistance);

        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.deltaTime;
            panel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t / slideDuration);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            panelGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        panel.anchoredPosition = startPos;
        panelGroup.alpha = 0f;
    }
}
