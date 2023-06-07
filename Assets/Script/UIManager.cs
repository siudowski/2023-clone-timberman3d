using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    int playerScore;
    AudioSource UIaudioSource;

    VisualElement titleLayout;
    VisualElement gameLayout;
    VisualElement helperLayout;
    VisualElement endLayout;

    VisualElement root;
    Label scoreLabel;
    ProgressBar timerBar;

    bool isMuted;
    Button muteButton;

    [SerializeField] Sprite sprite0;
    [SerializeField] Sprite sprite1;
    [SerializeField] AudioSource musicSource;

    private void Awake()
    {
        Debug.Log("UIManager | initialized");
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        scoreLabel = root.Q<Label>("GameLabelScore");
        timerBar = root.Q<ProgressBar>("GameTimerBar");
        muteButton = root.Q<Button>("MuteButton");
        

        titleLayout = root.Q<VisualElement>("TitleLayout");
        gameLayout = root.Q<VisualElement>("GameLayout");
        helperLayout = root.Q<VisualElement>("HelperLayout");
        endLayout = root.Q<VisualElement>("EndLayout");

        UIaudioSource = GetComponent<AudioSource>();

        InitializeUIValues();

        muteButton.clicked += MuteButton_clicked;
        GameManager.instance.OnStateChanged += GameManager_OnStateChanged;
        GameManager.instance.OnPlayerScoreChanged += Instance_OnPlayerScoreChanged;
    }

    private void MuteButton_clicked()
    {
        isMuted = !isMuted;
        var bg = muteButton.style.backgroundImage;
        bg.value = Background.FromSprite(isMuted ? sprite0 : sprite1);
        muteButton.style.backgroundImage = bg;

        musicSource.mute = isMuted;
    }

    private void OnDestroy()
    {
        GameManager.instance.OnStateChanged -= GameManager_OnStateChanged;
        muteButton.clicked -= MuteButton_clicked;
    }

    private void Update()
    {
        timerBar.lowValue = GameManager.instance.Timer.TimerRemaining / GameManager.instance.Timer.TimerSet;
    }

    private void Instance_OnPlayerScoreChanged(int newScore)
    {
        playerScore = newScore;
        scoreLabel.text = newScore.ToString();
    }
    
    private void GameManager_OnStateChanged(GameState state)
    {
        Debug.Log("UIManager | game state change registered");
        switch (state)
        {
            case GameState.Default:
                UIaudioSource.PlayOneShot(UIaudioSource.clip);
                endLayout.style.display = DisplayStyle.None;
                titleLayout.style.display = DisplayStyle.Flex;
                break;
            case GameState.Ready:
                titleLayout.style.display = DisplayStyle.None;
                helperLayout.style.display = DisplayStyle.Flex;
                break;
            case GameState.Started:
                helperLayout.style.display = DisplayStyle.None;
                gameLayout.style.display = DisplayStyle.Flex;
                break;
            case GameState.End:
                UIaudioSource.PlayOneShot(UIaudioSource.clip);
                root.Q<Label>("EndGameLabel").text = "Game over!\nyour score was: " + playerScore;
                gameLayout.style.display = DisplayStyle.None;
                endLayout.style.display = DisplayStyle.Flex;
                break;
            default:
                break;
        }
        
    }

    void InitializeUIValues()
    {
        gameLayout.style.display = DisplayStyle.None;
        endLayout.style.display = DisplayStyle.None;
        helperLayout.style.display = DisplayStyle.None;

        titleLayout.style.display = DisplayStyle.Flex;
        titleLayout.style.opacity = 1;
    }
}