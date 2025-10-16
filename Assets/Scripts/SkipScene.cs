using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipScene : MonoBehaviour
{
    public string nextSceneName;
    private void Update()
    {
        // Cek mouse kiri atau spasi
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("NextSceneTrigger: nextSceneName belum diatur di Inspector!");
            }
        }
    }
}
