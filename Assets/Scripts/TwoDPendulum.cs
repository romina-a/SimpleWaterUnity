using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TwoDPendulum : MonoBehaviour
{
    [SerializeField]
    GameObject Bob;
    Vector3 gravity = new Vector3(0f, -980f, 0f);
    
    Vector3 pos, acc, vel;

    float l=3f, theta=0f, v_theta=0f;

    void SetBobPos()
    {
        float x = (float) (pos.x + (float) l * Math.Sin((double) theta));
        float y = (float) (pos.y - (float) l * Math.Cos((double) theta));
        float z = pos.z;
        Debug.Log("bob:");
        Debug.Log(x);Debug.Log(y);Debug.Log(z);
        
        Bob.transform.position = new Vector3(x, y, z);
    }
    
    void Start()
    {
        pos = transform.position;
        SetBobPos();
    }

    void Update_Movement_Varibales(float deltaTime)
    {
        Vector3 new_pos = transform.position;
        Vector3 new_vel = (new_pos- pos) / deltaTime;
        acc = (new_vel - vel) / deltaTime;
        vel = new_vel;
        pos = new_pos;
        
        Debug.Log("pivot:");
        Debug.Log(acc);
        Debug.Log(vel);
        Debug.Log(pos);
    }

    void Update_Pendulum_State(float deltaTime)
    {
        float sin = (float) (Math.Sin((double) theta));
        float cos = (float) (Math.Cos((double) theta));
        float a_theta = (-(acc.y - gravity.y) * (float) (Math.Sin((double) theta))
                         - (acc.x * (float) (Math.Cos((double) theta)))) / l - 0.1f * v_theta;
        v_theta = v_theta + a_theta * deltaTime;
        theta = theta + v_theta * deltaTime;
        Debug.Log("state:");
        Debug.Log(v_theta);
        Debug.Log(theta);
    }

    // Update is called once per frame
    void Update()
    {
        Update_Movement_Varibales(Time.deltaTime);
        Update_Pendulum_State(Time.deltaTime);
        SetBobPos();
    }
}
