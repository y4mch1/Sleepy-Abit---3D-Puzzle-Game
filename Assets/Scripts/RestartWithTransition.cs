using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RestartWithTransition : MonoBehaviour
{
    public Transform transitionObject; // Assign di Inspector
    public Vector3 startOffset = new Vector3(0, 10f, 0);
    public float slideDuration = 1f;
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float delayBeforeRestart = 0.3f;
    public bool freezeSceneDuringTransition = true;
    private Vector3 targetPosition;
    private bool isRestarting = false;

    void Start()
    {
        if (transitionObject != null)
        {
            targetPosition = transitionObject.position;
            transitionObject.position = targetPosition + startOffset;
        }
    }

    public void RestartGame()
    {
        if (!isRestarting)
            StartCoroutine(RestartSequence());
    }

    private IEnumerator RestartSequence()
    {
        isRestarting = true;

       
        MonoBehaviour[] allScripts = null;
        if (freezeSceneDuringTransition)
        {
            allScripts = FindObjectsOfType<MonoBehaviour>();
            foreach (var script in allScripts)
            {
             
                if (script != this)
                    script.enabled = false;
            }
        }

        if (transitionObject != null)
        {
            Vector3 startPos = targetPosition + startOffset;
            Vector3 endPos = targetPosition;
            float elapsed = 0f;

            transitionObject.gameObject.SetActive(true);

            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;
                float t = slideCurve.Evaluate(elapsed / slideDuration);
                transitionObject.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            transitionObject.position = endPos;
        }

     
        yield return new WaitForSeconds(delayBeforeRestart);
        if (freezeSceneDuringTransition && allScripts != null)
        {
            foreach (var script in allScripts)
            {
                if (script != null)
                    script.enabled = true;
            }
        }
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
