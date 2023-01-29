using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewardLine : MonoBehaviour
{
    public float shiftTime = 0.5f;
    public List<float> offsetAfterUnsuckTry = new List<float>();

    private List<FinishReward> finishRewards = new List<FinishReward>();
    private BoxCollider2D boxCollider;

    private Queue<float> offsetAfterUnsuckTryQueue;
    private float currentShiftTime;

    private float targetPositionY;
    private float startPositionY;

    private Rigidbody2D rigidbody;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        offsetAfterUnsuckTryQueue = new Queue<float>(offsetAfterUnsuckTry);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentShiftTime > 0)
        {
            currentShiftTime -= Time.deltaTime;
            var coof = 1 - (currentShiftTime / shiftTime);
            var calculatedY = startPositionY + (targetPositionY - startPositionY) * coof;
            transform.position = new Vector3(transform.position.x, calculatedY, transform.position.z);
            foreach (var finishReward in finishRewards)
            {
                finishReward.transform.position = new Vector3(
                    finishReward.transform.position.x,
                    calculatedY,
                    finishReward.transform.position.z);
            }
        }
    }

    public void SuckLine(Obstacle obstacle)
    {
        if (obstacle.gameObject == gameObject)
        {
            Sucker sucker = null;//SuckerController.Instance.sucker1.isSucked ? SuckerController.Instance.sucker1 : SuckerController.Instance.sucker2;
            var positions = sucker.GetSuckerPositions();
            Physics2D.OverlapCircleAll(positions.left, 0.1f, LayerMask.GetMask("Reward")).ToList()
                .ForEach(x => 
                {
                    if (x.TryGetComponent(out FinishReward finishReward))
                    {
                        finishRewards.Add(finishReward);
                    }
                });
            Physics2D.OverlapCircleAll(positions.right, 0.1f, LayerMask.GetMask("Reward")).ToList()
                .ForEach(x =>
                {
                    if (x.TryGetComponent(out FinishReward finishReward))
                    {
                        finishRewards.Add(finishReward);
                    }
                });

            finishRewards.Distinct();
            var middlePosition = finishRewards.Average(x => x.transform.position.x);
            var size = finishRewards.Sum(x => x.transform.localScale.x);
            boxCollider.size = new Vector2(size, boxCollider.size.y);
            boxCollider.offset = new Vector2(middlePosition, boxCollider.offset.y);
            //SuckerController.Instance.sucker1.ableToUnSuck = false;
            //SuckerController.Instance.sucker2.ableToUnSuck = false;
        }
    }



    public void UnSuckTry()
    {
        if (offsetAfterUnsuckTryQueue.Any())
        {
            currentShiftTime = shiftTime;
            startPositionY = transform.position.y;
            targetPositionY = transform.position.y - offsetAfterUnsuckTryQueue.Dequeue();
        }
        else
        {
            Destroy(transform.GetChild(0).gameObject);
            finishRewards.ForEach(x =>
            {
                x.rigidbody.isKinematic = false;
                rigidbody.AddForce(Vector3.down * 20, ForceMode2D.Force);
                rigidbody.constraints = RigidbodyConstraints2D.None;
                x.ShowChest();
            });
            rigidbody.isKinematic = false;
            //SuckerController.Instance.sucker1.ableToSuck = false;
            //SuckerController.Instance.sucker1.ableToSuck = false;
            StartCoroutine(FinishManager.Instance.ShowResultsRoutine());
        }
    }


}
