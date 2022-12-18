using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public static bool isWorked;

    public Vector2 offset;
    public float smoothSpeed = 0.1f;
    public bool followByPlunger;

    private float maxCameraY;
    private Vector3 target;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        isWorked = true;
    }

    private void Start()
    {
        maxCameraY = FinishManager.Instance.GetHighestPoint().y - Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (followByPlunger)
        {
            var avarageSuckerY = (SuckerController.Instance.sucker1.transform.position.y + SuckerController.Instance.sucker2.transform.position.y) / 2f;
            target = new Vector3(offset.x, avarageSuckerY + offset.y, transform.position.z);
        }

    }

    private void LateUpdate()
    {
        SmoothFollow();
        LimitMovement();
    }

    private void LimitMovement()
    {
        if (transform.position.y > maxCameraY)
        {
            transform.position = new Vector3(transform.position.x, maxCameraY, transform.position.z);
        }
    }

    public void SetTarget(Vector3 position)
    {
        target = new Vector3(offset.x, position.y + offset.y, transform.position.z);
    }

    public void SmoothFollow()
    {
        var smoothFollow = Vector3.Lerp(transform.position,
            target, smoothSpeed);

        transform.position = new Vector3(smoothFollow.x, smoothFollow.y, transform.position.z);
    }
}
