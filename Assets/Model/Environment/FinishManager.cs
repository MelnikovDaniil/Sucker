using UnityEngine;

public class FinishManager : MonoBehaviour
{
    public static FinishManager Instance;
    public RewardLine rewardLine;
    public Vector3 highestPoint;

    private bool isFinished;

    private void Awake()
    {
        Instance = this;
    }

    private void Finishing()
    {
        isFinished = true;
        SuckerController.Instance.sucker1.OnSuck += rewardLine.SuckLine;
        SuckerController.Instance.sucker2.OnSuck += rewardLine.SuckLine;
        SuckerController.Instance.sucker1.OnUnSuckTry += rewardLine.UnSuckTry;
        SuckerController.Instance.sucker2.OnUnSuckTry += rewardLine.UnSuckTry;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(GetHighestPoint() + Vector3.left * 5, GetHighestPoint() + Vector3.right * 5);

    }
}
