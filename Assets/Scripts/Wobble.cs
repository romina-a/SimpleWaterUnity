using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    Renderer rend;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;

    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;

    public Vector4 gravity = new Vector4(0, -1, 0, 0);
    private Vector4 waterNormal;

    [Range(-1, 1)]
    public float baseWobbleAmountX = 0;
    [Range(-1, 1)]
    public float baseWobbleAmountZ = 0;
    [Range(-1, 1)]
    public float constantWobbleX = 0;
    [Range(-1, 1)]
    public float constantWobbleZ = 0;

    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        waterNormal = -gravity;
    }
    private void Update()
    {
        time += Time.deltaTime;
        // decrease wobble over time
        wobbleAmountToAddX =  Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;

        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);


        wobbleAmountX += constantWobbleX - baseWobbleAmountX * (180 - Mathf.Abs(transform.rotation.eulerAngles.x - 180)) * Mathf.Sign(transform.rotation.eulerAngles.x - 180);
        wobbleAmountZ += constantWobbleZ - baseWobbleAmountZ * (180 - Mathf.Abs(transform.rotation.eulerAngles.z - 180)) * Mathf.Sign(transform.rotation.eulerAngles.z - 180);


        // send it to the shader
        //rend.material.SetFloat("_WobbleX", wobbleAmountX + constantWobbleX - baseWobbleAmountX * (180-Mathf.Abs(transform.rotation.eulerAngles.x-180)) * Mathf.Sign(transform.rotation.eulerAngles.x - 180));
        //rend.material.SetFloat("_WobbleZ", wobbleAmountZ + constantWobbleZ - baseWobbleAmountZ * (180-Mathf.Abs(transform.rotation.eulerAngles.z-180)) * Mathf.Sign(transform.rotation.eulerAngles.z - 180));
        //
        rend.material.SetFloat("_WobbleX", wobbleAmountX);
        rend.material.SetFloat("_WobbleZ", wobbleAmountZ);

        // velocity
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;


        // add clamped velocity to wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) /* * MaxWobble */, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) /* * MaxWobble */, -MaxWobble, MaxWobble);

        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }



}