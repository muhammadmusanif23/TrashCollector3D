using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int totalTrash = 15;
    private int collectedTrash = 0;

    public float timeLeft = 90f;
    private bool gameActive = true;

    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject winPanel;
    public GameObject losePanel;

    public AudioClip pickupClip;
    public AudioClip winClip;
    public AudioClip loseClip;
    private AudioSource audioSource;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RestartGame();

        if (!gameActive) return;

        timeLeft -= Time.deltaTime;

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timeLeft);

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            LoseGame();
        }
    }

    public void CollectTrash()
    {
        collectedTrash++;

        if (audioSource != null && pickupClip != null)
            audioSource.PlayOneShot(pickupClip);

        if (scoreText != null)
            scoreText.text = "Collected: " + collectedTrash + " / " + totalTrash;

        if (collectedTrash >= totalTrash)
            WinGame();
    }

    public void OnTrashCollected()
    {
        CollectTrash();
    }

    void WinGame()
    {
        gameActive = false;
        winPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (audioSource != null && winClip != null)
        {
            audioSource.PlayOneShot(winClip);
            Invoke("StopTime", winClip.length);
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    void LoseGame()
    {
        gameActive = false;
        losePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (audioSource != null && loseClip != null)
        {
            audioSource.PlayOneShot(loseClip);
            Invoke("StopTime", loseClip.length);
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    void StopTime()
    {
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}