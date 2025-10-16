using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro
using UnityEngine.UI;

public class DropInSequence : MonoBehaviour
{
    public List<Transform> objectsToDrop = new List<Transform>();
    public float dropHeight = 10f;
    public float dropSpeed = 5f;
    public float delayBetweenDrops = 0.5f;
    public bool autoStart = true;
    public TMP_Text timerText;
    public AudioClip dropAudioClip;
    [Range(0f, 1f)]
    public float dropVolume = 1f;
    public float timerDuration = 300f;
    public GameObject gameOverPanel;

    private AudioSource audioSource;
    private List<Vector3> targetPositions = new List<Vector3>();
    private bool hasDropped = false;
    private bool isGameOver = false;
    private Coroutine dropCoroutine;
    private Coroutine timerCoroutine;

    void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        targetPositions.Clear();

        foreach (Transform obj in objectsToDrop)
        {
            if (obj == null) continue;

            targetPositions.Add(obj.position);
            obj.position += Vector3.up * dropHeight;
            obj.gameObject.SetActive(false);
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (autoStart)
            StartDropSequence();
    }

    public void StartDropSequence()
    {
        if (!hasDropped)
        {
            dropCoroutine = StartCoroutine(DropSequence());
        }
    }

    IEnumerator DropSequence()
    {
        hasDropped = true;

        for (int i = 0; i < objectsToDrop.Count; i++)
        {
            Transform obj = objectsToDrop[i];
            if (obj == null) continue;

            obj.gameObject.SetActive(true);

            if (dropAudioClip != null)
                audioSource.PlayOneShot(dropAudioClip, dropVolume);

            MonoBehaviour[] otherScripts = obj.GetComponents<MonoBehaviour>();
            foreach (var s in otherScripts)
            {
                if (s != this) s.enabled = false;
            }

            Vector3 startPos = obj.position;
            Vector3 endPos = targetPositions[i];

            float t = 0f;
            while (t < 1f)
            {
                if (isGameOver) yield break; 
                t += Time.deltaTime * dropSpeed;
                obj.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            obj.position = endPos;

            foreach (var s in otherScripts)
            {
                if (s != this) s.enabled = true;
            }

            yield return new WaitForSeconds(delayBetweenDrops);
        }

        timerCoroutine = StartCoroutine(StartTimer(timerDuration));
    }

    IEnumerator StartTimer(float duration)
    {
        float remaining = duration;

        while (remaining > 0f)
        {
            if (isGameOver) yield break; 
            remaining -= Time.deltaTime;

            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(remaining / 60f);
                int seconds = Mathf.FloorToInt(remaining % 60f);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            yield return null;
        }

        TriggerGameOver();
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (dropCoroutine != null) StopCoroutine(dropCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("Game Over! Semua dihentikan dan game dipause.");
    }

}
