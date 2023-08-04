using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    [Range(0.1f, 5f)]
    public float WobbleAnimationTime = 0.5f;
    float currentWobbleAnimationTime;

    [Range(0.1f, 5f)]
    public float BobbleAnimationTime = 0.5f;
    float currentBobbleAnimationTime;

    //Wobble    
    public float WobbleIntesity = 0.5f;

    float currentTargetIntensity;

    Quaternion targetAngle;
    Quaternion originalAngle;

    // Bobble
    public float BobbleIntensity = 0.5f;

    float currentTargetBobbleIntensity;

    Vector3 rootPosition;
    Vector3 targetPosition;
    Vector3 originalPosition;
    void Start()
    {
        originalAngle = transform.rotation;
        targetAngle = Quaternion.Euler(Vector3.forward * WobbleIntesity);
        currentTargetIntensity = WobbleIntesity;

        rootPosition = transform.position;
        originalPosition = transform.position;
        targetPosition = transform.position + Vector3.up * currentTargetBobbleIntensity;
        currentTargetBobbleIntensity = BobbleIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        var bobbleRation = currentBobbleAnimationTime / BobbleAnimationTime;
        var wobbleRatio = currentWobbleAnimationTime / WobbleAnimationTime;

        transform.position = Vector3.Lerp(originalPosition, targetPosition, bobbleRation);
        transform.rotation = Quaternion.Lerp(originalAngle, targetAngle, wobbleRatio);

        currentWobbleAnimationTime += Time.deltaTime;
        currentBobbleAnimationTime += Time.deltaTime;

        if (currentWobbleAnimationTime > WobbleAnimationTime)
            ChangeWobbleTarget();
        if (currentBobbleAnimationTime > BobbleAnimationTime)
            ChangeBobbleTarget();
    }

    void ChangeBobbleTarget()
    {
        currentBobbleAnimationTime = 0f;

        originalPosition = transform.position;
        currentTargetBobbleIntensity = -currentTargetBobbleIntensity;
        targetPosition = rootPosition + Vector3.up * currentTargetBobbleIntensity;
    }
    void ChangeWobbleTarget()
    {
        currentWobbleAnimationTime = 0f;
        
        originalAngle = transform.rotation;
        currentTargetIntensity = -currentTargetIntensity;
        targetAngle =  Quaternion.Euler(Vector3.forward * currentTargetIntensity);


    }
}
