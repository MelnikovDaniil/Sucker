using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Chain))]
public class Plunger : MonoBehaviour
{
    public Sucker sucker1;
    public Sucker sucker2;
    public float plungerStrength;

    private Chain _chain;
    private float currentPlungerStrength;
    private float plungerBlockStrength;
    private Block connectedBlock;

    private void Awake()
    {
        _chain = GetComponent<Chain>();
        _chain.sucker1 = sucker1;
        _chain.sucker2 = sucker2;
        SetupSucker(sucker1);
        SetupSucker(sucker2);
    }

    private void Start()
    {
        currentPlungerStrength = plungerStrength;
    }

    private void Update()
    {
        if ((Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1)) 
            && connectedBlock != null)
        {
            connectedBlock.rigidbody.isKinematic = false;
        }
    }

    private void SetupSucker(Sucker sucker)
    {
        sucker.OnSuckBlock += ConnectBlock;
    }

    private void ConnectBlock(Block block)
    {
        connectedBlock = block;
        plungerBlockStrength = currentPlungerStrength + block.strength;
        block.SetResistance(currentPlungerStrength);
        block.OnCollision += PlungerCollision;
    }

    private void PlungerCollision(Block collisionBlock)
    {
        if (plungerBlockStrength >= collisionBlock.strength)
        {
            plungerBlockStrength -= collisionBlock.strength;
            collisionBlock.Break();
        }
        else
        {
            collisionBlock.strength -= plungerBlockStrength;
            currentPlungerStrength /= 2f;
            connectedBlock.Break();
        }
        // Try to break collision block
        // if break then decrease general strenth and destroy block
        // else devide strenth two spring strenth and destroy connected block
    }
}
