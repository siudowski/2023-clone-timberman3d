using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float playerPosOffset = 1f;
    int playerPos; //1 = left, 2 = right
    bool isInputFrozen;
    bool endGameWait;

    [SerializeField] Animator animator;
    [SerializeField] Transform characterTransform;

    Vector3 v1 = new Vector3(1, 1, 1);
    Vector3 v2 = new Vector3(-1, 1, 1);

    public event Action<int> OnPlayerAxeSwing;

    private void Start()
    {
        playerPos = 1;

        transform.position = new Vector3(-playerPosOffset, transform.position.y, transform.position.z);
        characterTransform.localScale = v1;

        GameManager.instance.OnStateChanged += GameManager_OnStateChanged;

        isInputFrozen = true;

        Debug.Log("PlayerController | initialized");
    }

    private void OnDestroy()
    {
        GameManager.instance.OnStateChanged -= GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Default:
                animator.SetTrigger("OnGameReset");
                break;
            case GameState.Ready:
                isInputFrozen = false;
                animator.SetTrigger("OnGameStart");
                break;
            case GameState.Started:
                break;
            case GameState.End:
                isInputFrozen = true;
                endGameWait = true;
                StartCoroutine(EndGameWait());
                animator.SetTrigger("OnGameEnd");
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void OnMove(int movePos)
    {
        playerPos = movePos;

        if (movePos == 1)
        {
            transform.position = new Vector3(-playerPosOffset, transform.position.y, transform.position.z);
            characterTransform.localScale = v1;
            Debug.Log("PlayerController | moved to the left");
        }
        else
        {
            transform.position = new Vector3(playerPosOffset, transform.position.y, transform.position.z);
            characterTransform.localScale = v2;
            Debug.Log("PlayerController | moved to the right");
        }
        SwingAxe();
    }

    void HandleInput()
    {
        if (!endGameWait)
        {
            if (!isInputFrozen)
            {
                if (Input.GetKeyDown(KeyCode.A))
                    OnMove(1);
                if (Input.GetKeyDown(KeyCode.D))
                    OnMove(2);
            }
            else
            {
                if (Input.anyKeyDown)
                    SwingAxe();
            }
        }
    }

    void SwingAxe()
    {
        OnPlayerAxeSwing?.Invoke(playerPos);

        if (!isInputFrozen)
            animator.SetTrigger("OnAxeSwing");

        Debug.Log("PlayerController | Axe swinged");
    }

    IEnumerator EndGameWait()
    {
        yield return new WaitForSeconds(GameManager.instance.EndGameDelay);
        endGameWait = false;
        yield break;
    }
}