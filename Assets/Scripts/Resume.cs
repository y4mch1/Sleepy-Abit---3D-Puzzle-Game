using UnityEngine;

public class Resume : MonoBehaviour
{
    public GameObject gameOverObject; 
    public void ResumeGame()
    {
        Time.timeScale = 1f;

        if (gameOverObject != null)
        {
            gameOverObject.SetActive(false);
        }

        Debug.Log("Game resumed");
    }
}
