using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int nextLeveIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextLeveIndex);
        }
    }
}
