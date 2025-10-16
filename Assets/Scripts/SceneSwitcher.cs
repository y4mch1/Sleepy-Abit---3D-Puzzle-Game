using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject targetObject; 
    public GameObject fadePanel; 
    public float fadeDuration = 1.5f;

    private bool triggered = false;
    private Image fadeImage;
    private void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.SetActive(false);
            fadeImage = fadePanel.GetComponentInChildren<Image>();
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = 0f;
                fadeImage.color = c;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

      
        if (targetObject != null && other.gameObject != targetObject)
            return;

        triggered = true;
        StartCoroutine(FadeAndSwitch());
    }

    private IEnumerator FadeAndSwitch()
    {
       
        if (fadePanel != null)
            fadePanel.SetActive(true);

   
        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeImage(fadeImage, 0f, 1f, fadeDuration));
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

     
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
      
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }
        else
        {
            Debug.LogWarning("SceneToLoad belum diassign!");
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

 
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
     
                if (fadeImage != null)
                {
                    yield return StartCoroutine(FadeImage(fadeImage, 1f, 0f, fadeDuration));
                }

                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    private IEnumerator FadeImage(Image img, float startAlpha, float endAlpha, float duration)
    {
        float t = 0f;
        Color c = img.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            img.color = c;
            yield return null;
        }
        c.a = endAlpha;
        img.color = c;
    }
}
