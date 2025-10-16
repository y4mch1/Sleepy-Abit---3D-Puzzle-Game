using UnityEngine;

public class PauseManager : MonoBehaviour
{
      public GameObject gameOverObject; 
    private bool isPaused = false;
    private void Start()
    {
        Time.timeScale = 1f;  
        isPaused = false;

        if (gameOverObject != null)
            gameOverObject.SetActive(false);
    }

    public void TogglePause()
    {
        if (!isPaused)
        {
            // Pause
            Time.timeScale = 0f;
            isPaused = true;

            if (gameOverObject != null)
                gameOverObject.SetActive(true);
        }
        else
        {
            // Resume
            Time.timeScale = 1f;
            isPaused = false;

            if (gameOverObject != null)
                gameOverObject.SetActive(false);
        }
    }
}
