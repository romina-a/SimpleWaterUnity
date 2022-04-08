using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleImproved : MonoBehaviour
{
    Renderer rend;

    [SerializeField]
    Vector3 gravity = new Vector3(0, -10, 0);
    [SerializeField]
    float waterHeight = 0; //todo: apply changes to container position based on height
    [SerializeField]
    float density = 1f;
    [SerializeField, Range(0.01f, 100)]
    float acceleration_coef = 1;
    [SerializeField, Range(0.01f, 100)]
    float angular_acceleration_coef = 1;

    [SerializeField, Range(-1, 1)]
    float baseWobbleAmount = 0;
    [SerializeField, Range(-1, 1)]
    public float constantWobbleX = 0;
    [SerializeField, Range(-1, 1)]
    public float constantWobbleZ = 0;

    [SerializeField]
    float MaxWobble = 0.03f;
    [SerializeField]
    float WobbleSpeed = 1f;
    [SerializeField]
    float Recovery = 1f;

    [SerializeField]
    float pulse;
    float time = 0.5f;

    Vector3 waterNormal;

    float xWobble = 0, xWobbleV = 0;
    float zWobble = 0, zWobbleV = 0;

    float theta = 0, thetaV = 0;
    float phi = 0, phiV = 0;

    Vector3 acceleration, velocity, pos;
    Vector3 angular_acceleration, angular_velocity, rot;

    float elapsed_time_pos, elapsed_time_rot, delta_time;

    private void InitMovementVariables()
    {
        pos = transform.position;
        rot = transform.rotation.eulerAngles;
        velocity = Vector3.zero;
        angular_velocity = Vector3.zero;
        acceleration = Vector3.zero;
        angular_acceleration = Vector3.zero;
    }

    private void UpdateMovementVariables(float deltaTime)
    {
        Vector3 newPos = transform.position;
        Vector3 newV = (newPos - pos) / deltaTime;
        acceleration = (newV - velocity) / deltaTime;
        velocity = newV;
        pos = newPos;
        if (acceleration == Vector3.zero)
        {
            elapsed_time_pos += deltaTime;
        }
        else
        {
            elapsed_time_pos = 0;
        }
        Vector3 newRot = transform.rotation.eulerAngles;
        Vector3 newAV = (newRot - rot) / deltaTime;
        angular_acceleration = (newAV - angular_velocity) / deltaTime;
        angular_velocity = newAV;
        rot = newRot;
        if (angular_acceleration == Vector3.zero)
        {
            elapsed_time_rot += deltaTime;
        }
        else
        {
            elapsed_time_rot = 0;
        }
    }

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        waterNormal = -gravity / Vector3.Magnitude(gravity);
        InitMovementVariables();
    }
    private void Update1()
    {
        UpdateMovementVariables(Time.deltaTime);

        time += Time.deltaTime;
        // decrease previous wobble over time
        xWobble = Mathf.Lerp(xWobble, 0, Time.deltaTime * (Recovery));
        zWobble = Mathf.Lerp(zWobble, 0, Time.deltaTime * (Recovery));
        xWobbleV = Mathf.Lerp(xWobbleV, 0, Time.deltaTime * (Recovery));
        zWobbleV = Mathf.Lerp(zWobbleV, 0, Time.deltaTime * (Recovery));

        // add acceleration to wobble speed
        //xWobbleV += -acceleration.x + angular_acceleration.z * 0.2f;
        //zWobbleV += -acceleration.z + angular_acceleration.x * 0.2f;
        xWobbleV += Mathf.Clamp((-acceleration.x + (angular_velocity.z * 0.2f)) /* * MaxWobble */, -MaxWobble, MaxWobble);
        zWobbleV += Mathf.Clamp((-acceleration.z + (angular_velocity.x * 0.2f)) /* * MaxWobble */, -MaxWobble, MaxWobble);

        // add wobble speed to wobble
        xWobble += xWobbleV * Time.deltaTime;
        zWobble += zWobbleV * Time.deltaTime;

        // make a sine wave of the decreasing wobble
        pulse = 2 * Mathf.PI * WobbleSpeed;

        float wobbleAmountX = xWobble * Mathf.Sin(pulse * time);
        float wobbleAmountZ = zWobble * Mathf.Sin(pulse * time);

        wobbleAmountX += constantWobbleX - baseWobbleAmount * (180 - Mathf.Abs(transform.rotation.eulerAngles.x - 180)) * Mathf.Sign(transform.rotation.eulerAngles.x - 180);
        wobbleAmountZ += constantWobbleZ - baseWobbleAmount * (180 - Mathf.Abs(transform.rotation.eulerAngles.z - 180)) * Mathf.Sign(transform.rotation.eulerAngles.z - 180);

        //wobbleAmountX = Mathf.Clamp(wobbleAmountX, -MaxWobble, MaxWobble);
        //wobbleAmountZ = Mathf.Clamp(wobbleAmountZ, -MaxWobble, MaxWobble);

        waterNormal = new Vector3(wobbleAmountX, 1, wobbleAmountZ);

        float[] WaterNormalarr = new float[4] { waterNormal.x, waterNormal.y, waterNormal.z, 0};


        // send it to the shader
        rend.material.SetVector("_WaterNormal", waterNormal);
        Debug.DrawLine(transform.position, transform.position + waterNormal);

    }

    Vector3 get_normal()
    {
        Vector3 n = new Vector3(Mathf.Cos(phi), 0, Mathf.Sin(phi));
        n = Vector3.RotateTowards(n, new Vector3(0, 1, 0), Mathf.PI/2-theta, 0);
        Debug.Log("n: " + n);
        Debug.Log("here theta: " + theta);
        //n= Vector3.RotateTowards(gravity)
        return n;
    }

    private void Update2()
    {
        thetaV = Mathf.Lerp(thetaV, 0, Time.deltaTime);
        theta = Mathf.Lerp(theta, 0, Time.deltaTime);

        Debug.Log("thetaV: "+thetaV);
        thetaV -= 1000000 *Vector3.Magnitude(Vector3.Cross(waterNormal, -gravity))* Time.deltaTime; //gravity pushes normal towards itself
        Debug.Log("thetaV: "+thetaV);

        thetaV += 1000000 * Vector3.Magnitude(Vector3.Cross(acceleration, -waterNormal)) * Time.deltaTime;
        Debug.Log("thetaV: "+thetaV);

        theta += thetaV * Time.deltaTime;
        
        Debug.Log("theta: "+theta);

        //phiV = Vector3.Cross(new Vector3(acceleration.x, 0, acceleration.y))

        waterNormal = get_normal();

        // send it to the shader
        rend.material.SetVector("_WaterNormal", waterNormal);
        Debug.DrawLine(transform.position, transform.position + waterNormal*10);

    }

    private void Update()
    {
        Update1();
    }

}