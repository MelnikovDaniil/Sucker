using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Plunger))]
public class Chain : MonoBehaviour
{
    public ObiRod obiRod;

    [Space]
    [Range(0, 1f)]
    public float compressionLimit = 0.2f;
    public float compressionTime = 1.5f;
    public float decompressionTime = 0.1f;
    public float maxElementAngle = 5f;
    public float maxElementDistance = 1f;

    [Space]
    public float springLenth = 4;
    public float springPower = 300;

    [Space]
    public ChainElement chaintElementPrefab;
    public float elementHeight = 0.5f;
    public float elementWidth = 1;

    [NonSerialized]
    public Sucker sucker1;
    [NonSerialized]
    public Sucker sucker2;

    private SoundGroupComponent soundGroupComponent;

    private List<ChainElement> chainElements = new List<ChainElement>();
    private List<(Transform transform, Vector3 startEulerAngles)> rotationElements = new List<(Transform, Vector3)>();
    private List<(Transform transform, Vector3 startEulerAngles)> currentRotationElements;
    private float currentElementAngle;
    private float compressionSpeed;
    private float compressionProgress;
    private float currentDecompressionTime;

    private float currentElementRotation;
    private float nextElementRotation;

    private Vector3 currentElementPosition;
    private Vector3 nextElementPosition;
    private Vector3 nextElementDirection;
    private float nextElementDistance;

    private SMSound currentSpringSound;

    private void Awake()
    {
        soundGroupComponent = GetComponent<SoundGroupComponent>();
    }

    private void Start()
    {
        compressionProgress = 1;
        compressionSpeed = (compressionProgress - compressionLimit) / compressionTime;
        currentDecompressionTime = decompressionTime;
        currentElementAngle = maxElementAngle;
        GenerateChain();
        PinRodToChain();
        rotationElements.Add((sucker1.transform, sucker1.transform.localEulerAngles));
        rotationElements.AddRange(
            chainElements.Select(x => (x.transform, x.transform.localEulerAngles)));
        rotationElements.Add((sucker2.transform, sucker2.transform.localEulerAngles));
        //Time.timeScale = 0;
    }

    private void PinRodToChain()
    {
        var fullChainElement = new List<Transform>();
        fullChainElement.Add(sucker1.transform.GetChild(0));
        fullChainElement.AddRange(chainElements.Select(x => x.transform));
        fullChainElement.Add(sucker2.transform.GetChild(0));
        fullChainElement.Reverse();
        var pinCount = fullChainElement.Count;
        var obiRodPoints = obiRod.elements;

        for (int i = 0; i < pinCount; i++)
        {
            var particleAttachment = obiRod.gameObject.AddComponent<ObiParticleAttachment>();
            var chainElementIndex = i / (pinCount - 1f) * obiRodPoints.Count;
            particleAttachment.attachmentType = ObiParticleAttachment.AttachmentType.Static;
            particleAttachment.target = fullChainElement[i].transform;
            particleAttachment.particleGroup = obiRod.blueprint.groups[Mathf.RoundToInt(chainElementIndex)];

        }

        var additionalParticleAttachment1 = obiRod.gameObject.AddComponent<ObiParticleAttachment>();
        additionalParticleAttachment1.attachmentType = ObiParticleAttachment.AttachmentType.Static;
        additionalParticleAttachment1.target = fullChainElement[0].transform;
        additionalParticleAttachment1.particleGroup = obiRod.blueprint.groups[1];

        var additionalParticleAttachment2 = obiRod.gameObject.AddComponent<ObiParticleAttachment>();
        additionalParticleAttachment2.attachmentType = ObiParticleAttachment.AttachmentType.Static;
        additionalParticleAttachment2.target = fullChainElement[pinCount - 1].transform;
        additionalParticleAttachment2.particleGroup = obiRod.blueprint.groups[obiRodPoints.Count - 2];
    }

    private void FixedUpdate()
    {
        if (Time.timeScale > 0)
        {
            if (compressionProgress <= 1)
            {
                currentElementAngle = (compressionProgress - compressionLimit) / (1 - compressionLimit) * maxElementAngle;
                chainElements.ForEach(x =>
                {
                    x.capsuleCollider.size = new Vector2(x.capsuleCollider.size.x, compressionProgress * elementHeight);
                    if (x.springJoint != null)
                    {
                        x.springJoint.anchor = x.springJoint.anchor.normalized * x.capsuleCollider.size.y / 2f;
                        x.springJoint.connectedAnchor = x.springJoint.connectedAnchor.normalized * x.capsuleCollider.size.y / 2f;
                    }

                    if (x.suckerJoint != null)
                    {
                        x.suckerJoint.anchor = x.suckerJoint.anchor.normalized * x.capsuleCollider.size.y / 2f;
                    }
                });

                if (compressionProgress == 1)
                {
                    compressionProgress += 0.1f;
                }
            }
        }

        LimitRotation();
        LimitDistance();
    }

    public void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }

        if (Time.timeScale > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                var randomClip = soundGroupComponent.soundGroups.FirstOrDefault(x => x.groupName == "Spring")
                    .audioClips.GetRandom();
                currentSpringSound = SoundManager.PlaySound(randomClip)
                    .SetVolume(0.4f);
            }

            if (compressionProgress > compressionLimit && (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0)))
            {
                compressionProgress = Mathf.Clamp(compressionProgress, compressionLimit, 1.00f);
                compressionProgress -= compressionSpeed * Time.deltaTime;
                compressionProgress = Mathf.Clamp(compressionProgress, compressionLimit, 1.00f);
            }

            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0))
            {
                currentSpringSound?.Stop();
                var randomClip = soundGroupComponent.soundGroups.FirstOrDefault(x => x.groupName == "Boing")
                    .audioClips.GetRandom();
                SoundManager.PlaySound(randomClip).SetVolume(0.3f);
                currentDecompressionTime = (compressionProgress - compressionLimit) / (1 - compressionLimit) * decompressionTime;
            }

            if (currentDecompressionTime <= decompressionTime)
            {
                currentDecompressionTime += Time.deltaTime;
                compressionProgress = Mathf.Clamp(currentDecompressionTime / decompressionTime, compressionLimit, 1.00f);
            }
            //LimitPosition();
        }
    }

    private void GenerateChain()
    {
        var elementsAmount = (int) (springLenth / elementHeight);
        var y = springLenth / 2;
        var startY = y - sucker1.springConnectionPosition.y;
        for (int i = 0; i < elementsAmount; i++)
        {
            var newElement = Instantiate(chaintElementPrefab, transform);
            y -= elementHeight / 2f;
            newElement.transform.localPosition = new Vector3(0, y, 0);
            y -= elementHeight / 2f;
            var rigidbody = newElement.gameObject.AddComponent<Rigidbody2D>();
            var spring = newElement.gameObject.AddComponent<SpringJoint2D>();
            var collider = newElement.gameObject.AddComponent<CapsuleCollider2D>();
            var obiCollider = newElement.gameObject.AddComponent<ObiCollider2D>();

            SetupSpring(spring);

            rigidbody.angularDrag = 180;
            rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            rigidbody.gravityScale = sucker1.rigidbody.gravityScale;

            collider.direction = CapsuleDirection2D.Horizontal; //x-axis orientation
            collider.size = new Vector2(elementWidth, elementHeight);

            obiCollider.Filter = 0;

            newElement.capsuleCollider = collider;
            newElement.springJoint = spring;
            chainElements.Add(newElement);

            if (i == 0)
                continue;

            var previosSpring = chainElements[i - 1].springJoint;
            previosSpring.connectedBody = rigidbody;
            previosSpring.connectedAnchor = Vector3.up * elementHeight / 2f;
        }

        sucker1.transform.localPosition = new Vector3(0, startY, 0);
        sucker1.transform.localRotation = Quaternion.identity;
        var suckerJoint = chainElements[0].gameObject.AddComponent<SpringJoint2D>();

        SetupSpring(suckerJoint);
        suckerJoint.anchor = Vector3.up * elementHeight / 2f;
        suckerJoint.connectedBody = sucker1.rigidbody;
        suckerJoint.connectedAnchor = sucker1.springConnectionPosition;

        chainElements[0].suckerJoint = suckerJoint;


        sucker2.transform.localPosition = new Vector3(0, -startY, 0);
        sucker2.transform.localRotation = Quaternion.Euler(0, 0, 180);
        var suckerJoint2 = chainElements[elementsAmount - 1].springJoint;
        chainElements[elementsAmount - 1].springJoint = null;
        chainElements[elementsAmount - 1].suckerJoint = suckerJoint2;

        SetupSpring(suckerJoint2);
        suckerJoint2.anchor = Vector3.down * elementHeight / 2f;
        suckerJoint2.connectedBody = sucker2.rigidbody;
        suckerJoint2.connectedAnchor = sucker2.springConnectionPosition;
    }

    private void SetupSpring(SpringJoint2D spring)
    {
        spring.enableCollision = true;
        spring.autoConfigureDistance = false;
        spring.autoConfigureConnectedAnchor = false;
        spring.anchor = Vector3.down * elementHeight / 2f;
        spring.dampingRatio = 1;
        spring.distance = 0.025f;
        spring.frequency = springPower;
    }

    private void LimitPosition()
    {
        foreach (var element in rotationElements)
        {
            element.transform.position = new Vector3(element.transform.position.x, element.transform.position.y, 0);
        }
    }

    private void LimitDistance()
    {
        currentRotationElements = new List<(Transform, Vector3)>(rotationElements);
        if (sucker2.isSucked)
        {
            currentRotationElements.Reverse();
        }
        for (int i = 0; i < currentRotationElements.Count - 1; i++)
        {
            var distanceBonus = 0f;
            if (i == 0 || i == currentRotationElements.Count - 2)
            {
                distanceBonus = 1.2f;
            }
            currentElementPosition = currentRotationElements[i].transform.position;
            nextElementPosition = currentRotationElements[i + 1].transform.position;
            nextElementDirection = nextElementPosition - currentElementPosition;
            nextElementDistance = Mathf.Clamp(nextElementDirection.magnitude, 0, maxElementDistance * compressionProgress + distanceBonus);

            currentRotationElements[i + 1].transform.position = currentElementPosition + nextElementDirection.normalized * nextElementDistance;
        }
    }

    private void LimitRotation()
    {
        if (sucker1.isSucked || sucker2.isSucked)
        {
            currentRotationElements = new List<(Transform, Vector3)>(rotationElements);
            if (sucker2.isSucked)
            {
                currentRotationElements.Reverse();
            }

            for (int i = 0; i < currentRotationElements.Count - 1; i++)
            {
                currentElementRotation = currentRotationElements[i].transform.localEulerAngles.z - currentRotationElements[i].startEulerAngles.z;
                nextElementRotation = currentRotationElements[i + 1].transform.localEulerAngles.z - currentRotationElements[i + 1].startEulerAngles.z;
                nextElementRotation = ClampAngle(
                    nextElementRotation, 
                    currentElementRotation - currentElementAngle,
                    currentElementRotation + currentElementAngle);
                currentRotationElements[i + 1].transform.localRotation = 
                    Quaternion.Euler(
                        currentRotationElements[i + 1].startEulerAngles.x,
                        currentRotationElements[i + 1].startEulerAngles.y,
                        nextElementRotation + currentRotationElements[i + 1].startEulerAngles.z);
            }
        }
    }

    private float ClampAngle(float current, float min, float max)
    {
        float dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
        float hdtAngle = dtAngle * 0.5f;
        float midAngle = min + hdtAngle;

        float offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
        if (offset > 0)
            current = Mathf.MoveTowardsAngle(current, midAngle, offset);
        return current;
    }

}
