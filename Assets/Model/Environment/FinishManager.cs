using System;
using System.Collections;
using UnityEngine;

public class FinishManager : MonoBehaviour
{
    public static FinishManager Instance;
    public RewardLine rewardLine;
    public Vector3 highestPoint;
    public float resultsAfterTime;

    private bool isFinished;
    private Coroutine resultsRoutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Finishing()
    {
        isFinished = true;
        GameManager.Instance.DisableDeathCollider();
        SuckerController.Instance.sucker1.OnSuck += rewardLine.SuckLine;
        SuckerController.Instance.sucker2.OnSuck += rewardLine.SuckLine;
        SuckerController.Instance.sucker1.OnSuck += ResultAfterReward;
        SuckerController.Instance.sucker2.OnSuck += ResultAfterReward;
        SuckerController.Instance.sucker1.OnUnSuckTry += rewardLine.UnSuckTry;
        SuckerController.Instance.sucker2.OnUnSuckTry += rewardLine.UnSuckTry;
        resultsRoutine = StartCoroutine(ShowResultsRoutine());
    }

    private void ResultAfterReward(Obstacle obj)
    {
        if (resultsRoutine != null && obj.gameObject == rewardLine.gameObject)
        {
            StopCoroutine(resultsRoutine);
            resultsRoutine = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isFinished && other.gameObject.layer == LayerMask.NameToLayer("Sucker"))
        {
            Finishing();
        }
    }

    public Vector3 GetHighestPoint()
    {
        return transform.position + highestPoint;
    }

    public IEnumerator ShowResultsRoutine()
    {
        yield return new WaitForSeconds(resultsAfterTime);
        UIManager.Instance.ShowFinishPanel();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(GetHighestPoint() + Vector3.left * 5, GetHighestPoint() + Vector3.right * 5);

    }
}
