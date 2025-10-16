using UnityEngine;
using UnityEngine.UI;

public class FallTrigger : MonoBehaviour
{
    public GameObject gameOverObject;   
    public Image fadePanel;             
    public float fadeSpeed = 5f;        
    public AudioClip gameOverSound;     
    private bool isGameOver = false;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isGameOver)
        {
            isGameOver = true;
            if (gameOverObject != null)
                gameOverObject.SetActive(true);
            if (fadePanel != null)
                fadePanel.gameObject.SetActive(true);
            if (gameOverSound != null)
            {
                audioSource.clip = gameOverSound;
                audioSource.Play();
            }
            GetComponent<Collider>().enabled = false;
        }
    }

    private void Update()
    {
        if (isGameOver && fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = Mathf.MoveTowards(c.a, 1f, fadeSpeed * Time.deltaTime); 
            fadePanel.color = c;
        }
    }
}
