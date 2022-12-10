using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckerController : MonoBehaviour
{
    public float minSpringPower = 0.2f;
    public float maxSpringPower = 10f;
    [Space]
    public float springPowerReduceRate = 0.5f;
    public float connectionOffset = 0.25f;

    public Sucker sucker1;
    public Sucker sucker2;

    private List<SpringJoint> springs = new List<SpringJoint>();
    private float currentSpringPower;
    // Start is called before the first frame update
    void Start()
    {
        GenerateSpring();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            currentSpringPower -= springPowerReduceRate * Time.deltaTime;
            currentSpringPower = Mathf.Clamp(currentSpringPower, minSpringPower, maxSpringPower);
            springs.ForEach(x => x.minDistance = currentSpringPower);
            springs.ForEach(x => x.maxDistance = currentSpringPower);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            currentSpringPower = maxSpringPower;
            springs.ForEach(x => x.minDistance = currentSpringPower);
            springs.ForEach(x => x.maxDistance = currentSpringPower);
        }
    }

    private void GenerateSpring()
    {
        CreateSpring(Vector3.right * connectionOffset);
        CreateSpring(Vector3.left * connectionOffset);
        CreateSpring(Vector3.back * connectionOffset);
        CreateSpring(Vector3.forward * connectionOffset);
    }

    private void CreateSpring(Vector3 offset)
    {
        var spring1 = sucker1.gameObject.AddComponent<SpringJoint>();
        spring1.connectedBody = sucker2.rigidbody;
        spring1.autoConfigureConnectedAnchor = false;
        spring1.anchor = sucker1.springConnectionPosition + offset;
        spring1.connectedAnchor = sucker2.springConnectionPosition + offset;
        spring1.spring = 100;
        //spring1.enableCollision = true;
        spring1.minDistance = maxSpringPower;
        spring1.maxDistance = maxSpringPower;
        spring1.enableCollision = true;
        springs.Add(spring1);
    }
}
