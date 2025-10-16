using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneButton : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject fadePanel;
    public float fadeDuration = 1f;
    private Image panelImage;
    void Start()
    {
        if(fadePanel != null)
        {
            panelImage = fadePanel.GetComponent<Image>();
            panelImage.color = new Color(0,0,0,0);
            fadePanel.SetActive(true);
        }
    }

    public void OnButtonPressed()
    {
        if(fadePanel != null)
        {
            StartCoroutine(FadeAndLoadScene());
        }
        else
        {

            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        float t = 0f;
        Color c = panelImage.color;

       
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            panelImage.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }

        panelImage.color = new Color(c.r, c.g, c.b, 1);
        SceneManager.LoadScene(sceneToLoad);
    }
}
