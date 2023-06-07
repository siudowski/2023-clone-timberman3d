using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Default, Ready, Started, End }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] PlayerController player;
    [SerializeField] TreeController tree;
    [SerializeField] GameState gameState;
    
    public event Action<GameState> OnStateChanged;
    public event Action<int> OnPlayerScoreChanged;
    public int PlayerScore { get; private set; }
    public Timer Timer { get; private set; }
    public float EndGameDelay { get; } = 0.5f;
    public float MaxAudioVolume { get; } = 0.25f;
    public float VolumeTransitionDuration { get; } = 0.5f;

    float startTimerTime = 1f;

    private void Awake()
    {
        Debug.Log("GameManager | initialized");
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        Timer = GetComponent<Timer>();
    }

    private void Start()
    {
        player.OnPlayerAxeSwing += Player_OnPlayerAxeSwing;
    }

    private void OnDestroy()
    {
        player.OnPlayerAxeSwing -= Player_OnPlayerAxeSwing;
    }

    void UpdateGameState(GameState state)
    {
        gameState = state;
        OnStateChanged?.Invoke(state);
        Debug.Log("GameManager | GameState updated: " + state);
    }

    private void Player_OnPlayerAxeSwing(int playerPos)
    {
        switch (gameState)
        {
            case GameState.Default:
                UpdateGameState(GameState.Ready);
                break;
            case GameState.Ready:
                ProcessPlayerSwing(playerPos);
                UpdateGameState(GameState.Started);
                break;
            case GameState.Started:
                ProcessPlayerSwing(playerPos);
                break;
            case GameState.End:
                UpdateGameState(GameState.Default);
                PlayerScore = 0;
                break;
            default:
                break;
        }
    }

    void ProcessPlayerSwing(int playerPos)
    {
        Timer.SetTimer(startTimerTime, EndOfTime);
        if (playerPos == tree.GetBranchPosition())
        {
            UpdateGameState(GameState.End);
            Timer.IsPaused = true;
        }
        else
        {
            Timer.SetTimer(startTimerTime, EndOfTime);
            tree.CutTree();
            PlayTreeSoundEffect();
            PlayerScore++;
            OnPlayerScoreChanged?.Invoke(PlayerScore);
        }
    }

    void EndOfTime()
    {
        UpdateGameState(GameState.End);
        Debug.Log("GameManager | Timer end!");
        Timer.IsPaused = true;
    }

    void PlayTreeSoundEffect()
    {
        AudioSource audioSource = tree.GetComponent<AudioSource>();
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.1f);
        audioSource.PlayOneShot(audioSource.clip);
    }
}