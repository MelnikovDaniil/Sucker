using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Chain))]
public class Trajectory : MonoBehaviour
{
    public Transform trajectoryElementPrefab;
    public float trajectoryLength = 2f;
    public float elementsCount = 5;
    public float elementSpeed = 10f;
    [Range(0, 1f)]
    public float decrisingElementsAfter = 0.8f;

    private float distanceBetweenElements;
    private float currentCompressionTime;
    private float coof;
    private Chain chain;
    private GameObject trajectory;
    private List<Transform> trajectoryElements = new List<Transform>();

    private void Awake()
    {
        chain = GetComponent<Chain>();
        GenerateTrajectory();
        currentCompressionTime = chain.compressionTime;
    }

    private void Update()
    {
        if (Time.timeScale > 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space)
            && (chain.sucker1.isSucked || chain.sucker2.isSucked))
            {
                trajectory.SetActive(true);
                currentCompressionTime = chain.compressionTime;
            }

            if ((Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Space))
                && (chain.sucker1.isSucked || chain.sucker2.isSucked))
            {
                if (currentCompressionTime > 0)
                {
                    currentCompressionTime -= Time.deltaTime;
                    coof = 1f - (currentCompressionTime / chain.compressionTime);
                    distanceBetweenElements = coof * trajectoryLength / elementsCount;
                    for (int i = 1; i < elementsCount; i++)
                    {
                        var previosElementPosition = trajectoryElements[i - 1].localPosition;
                        trajectoryElements[i].localPosition = previosElementPosition + Vector3.up * distanceBetweenElements;
                    }
                }

                var fromSucker = chain.sucker1.isSucked ? chain.sucker1 : chain.sucker2;
                var toSucker = fromSucker == chain.sucker1 ? chain.sucker2 : chain.sucker1;
                trajectory.transform.position = fromSucker.transform.position;
                var direction = toSucker.transform.position - fromSucker.transform.position;
                trajectory.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);


                foreach (var element in trajectoryElements)
                {
                    var currentLenght = coof * trajectoryLength;
                    var decreseAfterLenght = currentLenght * decrisingElementsAfter;
                    var decreseLenght = currentLenght - decreseAfterLenght;
                    element.localPosition += Vector3.up * elementSpeed * Time.deltaTime;
                    element.localPosition = new Vector3(0, element.localPosition.y % currentLenght, 0);
                    if (element.localPosition.y - decreseAfterLenght > 0)
                    {
                        element.localScale = Vector3.one * (1f - (element.localPosition.y - decreseAfterLenght) / decreseLenght);
                    }
                    else
                    {
                        element.localScale = Vector3.one;
                    }
                }

            }


            if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Space))
            {
                trajectory.SetActive(false);
            }
        }
    }

    private void GenerateTrajectory()
    {
        trajectory = new GameObject("Trajectory");
        for (int i = 0; i < elementsCount; i++)
        {
            trajectoryElements.Add(Instantiate(trajectoryElementPrefab, trajectory.transform));
        }
        trajectory.SetActive(false);
    }
}
