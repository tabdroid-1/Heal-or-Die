using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MidGame : MonoBehaviour
{
    [Header("Essentials")]
    [Space]
    private GameObject player;
    private PlayerControls playerControls;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private GameObject deadUI;
    [SerializeField] private GameObject PauseMenu;
    private GameManager gameManager;
    [Space]
    private bool pauseManuEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "Score: " + gameManager.currentHighScore;

        if (playerControls.Player.PauseMenu.triggered && !deadUI.activeInHierarchy)
        {
            if (pauseManuEnabled)
            {
                PauseMenu.SetActive(false);
                pauseManuEnabled = false;
                Time.timeScale = 1f;
            }
            else
            {
                PauseMenu.SetActive(true);
                pauseManuEnabled = true;
                Time.timeScale = 0f;
            }
        }



        if (deadUI.activeInHierarchy)
        {
            PauseMenu.SetActive(false);
        }
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        pauseManuEnabled = false;
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        gameManager.SaveGame();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        gameManager.SaveGame();
    }

}
