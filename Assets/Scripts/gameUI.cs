using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameUI : MonoBehaviour
{
    public bool isStarted = false;
    public bool m_isDead;
    public bool m_isFinished;
    [SerializeField] private GameObject m_startButton, m_settingsButton, m_pauseButton;
    [SerializeField] private GameObject m_deadPanel;
    [SerializeField] private GameObject m_settingsPanel;
    [SerializeField] private GameObject m_stopPanel;
    [SerializeField] private GameObject m_finishPanel;
    [SerializeField] private GameObject m_countDown;
    //[SerializeField] private GameObject musicon, musicoff, vibrateon, vibrateoff;
    public Slider energySlider;
    public float currentBarLevel;
    [SerializeField] private PlayableDirector m_beginning, m_ending;
    [SerializeField] private float timeline1Duration, timeline2Duration;

    void Start()
    {
        m_pauseButton.SetActive(false);
        energySlider.gameObject.SetActive(false);
        //LoadNewLevel();
        Time.timeScale = 1;
    }
    void Update()
    {
        if (m_isDead)
        {
            m_startButton.SetActive(false);
            m_stopPanel.SetActive(false);
            m_pauseButton.SetActive(false);
            Invoke("TimeStop", 0.2f);
        }
        if (m_isFinished)
        {
            m_ending.Play();
            m_pauseButton.SetActive(false);
            Invoke("FinishLevel", timeline2Duration);
        }
        if(m_stopPanel.activeSelf)
        {
            m_pauseButton.SetActive(false);
        }
        BarProgress(currentBarLevel);
    }
    public void NextLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        int nextLevel = scene.buildIndex + 1;
        PlayerPrefs.SetInt("saveLevel", nextLevel);
        PlayerPrefs.Save();
        SceneManager.LoadScene(nextLevel);
    }
    void LoadNewLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (PlayerPrefs.GetInt("Level1Occured") == 1) // Checks whether level1 is played
        {
            if (scene.buildIndex != PlayerPrefs.GetInt("saveLevel"))
            {
                SceneManager.LoadScene(PlayerPrefs.GetInt("saveLevel"));
            }
        }
        else // For Level1 exception
        {
            PlayerPrefs.SetInt("Level1Occured", 1);
            SceneManager.LoadScene(1);
        }
    }
    public void StopGame()
    {
        m_stopPanel.SetActive(true);
        m_pauseButton.SetActive(false);
        isStarted = false;
    }
    public void GameStart() //tap to play butonu
    {
        m_startButton.SetActive(false);
        m_settingsButton.SetActive(false);
        m_pauseButton.SetActive(true);
        m_settingsPanel.SetActive(false);
        energySlider.gameObject.SetActive(true);
        m_beginning.Play();
        Invoke("StartTheGame", timeline1Duration);
    }
    public void Settings() //settings giriş butonu
    {
        m_settingsPanel.SetActive(true);
        m_settingsButton.SetActive(false);
    }
    public void ContinueButton()
    {
        Invoke("GameIsStartingAgain", 3f);
        m_countDown.SetActive(true);
        m_stopPanel.SetActive(false);
        isStarted = true;
    }
    void GameIsStartingAgain()
    {
        m_countDown.SetActive(false);
    }
    public void ExitButton()//çıkış butonu
    {
        Application.Quit();
    }
    public void TryAgain()//tekrar deneyin butonu
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SettingsBack()
    {
        m_settingsPanel.SetActive(false);
        m_settingsButton.SetActive(true);
    }
    /*public void MusicOn()
    {
        musicoff.SetActive(true);
        musicon.SetActive(false);
    }
    public void MusicOff()
    {
        musicoff.SetActive(false);
        musicon.SetActive(true);
    }
    public void VibrateOn()
    {
        vibrateoff.SetActive(true);
        vibrateon.SetActive(false);
    }
    public void VibrateOff()
    {
        vibrateoff.SetActive(false);
        vibrateon.SetActive(true);
    }*/
    void TimeStop()
    {
        m_deadPanel.SetActive(true);
    }
    void BarProgress(float a)
    {
        energySlider.value = a;
    }
    void StartTheGame()
    {
        isStarted = true;
        //GameObject.FindWithTag("Player").GetComponent<Movement>().firstTouch = false;
    }
    void FinishLevel()
    {
        m_finishPanel.SetActive(true);
    }
}
