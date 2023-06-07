using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [SerializeField] int treeSize = 5;
    [SerializeField] Queue<Log> logsQueue = new Queue<Log>();
    [SerializeField] Transform logPrefab;
    [SerializeField] Transform[] branchPrefabs;

    private void Start()
    {
        GameManager.instance.OnStateChanged += GameManager_OnStateChanged;
        for (int i = 0; i < treeSize; i++)
        {
            logsQueue.Enqueue(AddLog(i, true));
        }
        Debug.Log("TreeController | initialized");
    }

    private void GameManager_OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Default:
                GenerateTree();
                break;
            case GameState.Ready:
                break;
            case GameState.Started:
                break;
            case GameState.End:
                break;
            default:
                break;
        }
    }

    public void CutTree()
    {
        RemoveLog();
        foreach (Log log in logsQueue)
        {
            log.PlayAnimation(0);
        }
        logsQueue.Enqueue(AddLog(logsQueue.Count, false));
        Debug.Log("TreeController | tree cut");
    }

    Log AddLog(int index, bool isGameStart)
    {
        GameObject logObj = Instantiate(logPrefab.gameObject, new Vector3(0f, index * 2f + (transform.position.y + 1), 0f), transform.rotation, transform);

        Log log = logObj.GetComponent<Log>();

        if (!isGameStart)
        {
            log.Init(UnityEngine.Random.Range(0, 3));

            if (log.Position != 0)
            {
                GameObject branchObject = Instantiate(branchPrefabs[UnityEngine.Random.Range(0,4)].gameObject, logObj.transform.position, Quaternion.Euler(0f, (log.Position == 1 ? -90f : 90f), 0f), logObj.transform.GetChild(0));
            }
        }
        else
            log.Init(0);

        Debug.Log("TreeController | log generated " + (log.Position != 0 ? "with branch at: " + log.Position : "with no branch"));
        return log;
    }

    void RemoveLog()
    {
        Log logToDestroy = logsQueue.Peek();
        logToDestroy.PlayAnimation(1);
        logsQueue.Dequeue();
        //Destroy(logToDestroy.gameObject);
        Debug.Log("TreeController | log removed");
    }

    void GenerateTree()
    {
        ClearTree();
        for (int i = 0; i < treeSize; i++)
        {
            logsQueue.Enqueue(AddLog(i, true));
        }
        Debug.Log("TreeController | tree generated");
    }

    void ClearTree()
    {
        foreach (Log log in logsQueue)
        {
            Destroy(log.gameObject);
        }
        logsQueue.Clear();
        Debug.Log("TreeController | tree cleared");
    }

    public int GetBranchPosition()
    {
        return logsQueue.Peek().Position;
    }
}