using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Essentials")]
    [Space]
    private GameManager gameManager;
    private PlayerControls playerControls;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject howToPlay;


    private void Start()
    {
        Time.timeScale = 1f;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.LoadGame();
    }

    private void Update()
    {
        score.text = "Score: " + gameManager.highestScore;
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


    public void Play(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void Credits()
    {
        mainMenu.SetActive(false);
        credits.SetActive(true);
        howToPlay.SetActive(false);
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        credits.SetActive(false);
        howToPlay.SetActive(false);
    }

    public void HowToPlay()
    {
        mainMenu.SetActive(false);
        credits.SetActive(false);
        howToPlay.SetActive(true);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }
}
