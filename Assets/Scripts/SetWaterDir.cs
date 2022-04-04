using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWaterDir : MonoBehaviour
{
    Renderer rend;
    Vector4 dir;// = new Vector4(0,0,0,0);
    Vector4 init_dir;// = new Vector4(0, 0, 0, 0);
    private float init_x, init_z;
    public float limit_x = 10, limit_z = 10;


    // Start is called before the first frame update
    // Awake is called before start
    void Awake()
    {
        rend = GetComponent<Renderer>();
        dir = new Vector4(rend.material.GetFloat("_WaterNormalX"), rend.material.GetFloat("_WaterNormalY"), rend.material.GetFloat("_WaterNormalZ"), 1);
        init_x = dir.x;
        init_z = dir.z;
        init_dir = dir;
    }

    private void clamp_dir ()
    {
        if (limit_x != 0)
            dir.x = Mathf.Clamp(dir.x, -Mathf.Abs(limit_x), Mathf.Abs(limit_x));
        if (limit_z != 0)
            dir.z = Mathf.Clamp(dir.z, -Mathf.Abs(limit_z), Mathf.Abs(limit_z));
    }

    public void SetWaterNormalZ(float inputz)
    {
        dir.z = init_z + inputz;
        clamp_dir();
        Debug.Log("In setwaterdir called with"+ dir.ToString());
        rend.material.SetFloat("_WaterNormalZ", dir.z);
    }

    public void SetWaterNormalX(float inputx)
    {
        dir.x = init_x + inputx;
        clamp_dir();
        Debug.Log("In setwaterdir called with" + dir.ToString());
        rend.material.SetFloat("_WaterNormalX", dir.x);
    }

    public void SetWaterNormalXZ(Vector2 inputxz)
    {
        dir.x = init_x + inputxz.x;
        dir.z = init_z + inputxz.y;
        clamp_dir();
        Debug.Log("In setwaterdir called with" + dir.ToString());
        rend.material.SetFloat("_WaterNormalZ", dir.z);
        rend.material.SetFloat("_WaterNormalX", dir.x);
    }

    public void IncrementWaterNormalZ(float inputz)
    {
        dir.z = dir.z + inputz;
        clamp_dir();
        Debug.Log("In setwaterdir called with" + dir.ToString());
        rend.material.SetFloat("_WaterNormalZ", dir.z);
        rend.material.SetFloat("_WaterNormalX", dir.x);
    }

    public void IncrementWaterNormalX(float inputx)
    {
        dir.x = dir.x + inputx;
        clamp_dir();
        Debug.Log("In setwaterdir called with" + dir.ToString());
        rend.material.SetFloat("_WaterNormalZ", dir.z);
        rend.material.SetFloat("_WaterNormalX", dir.x);
    }

    public void IncrementWaterNormalXZ(Vector2 inputxz)
    {
        dir.x = dir.x + inputxz.x;
        dir.z = dir.z + inputxz.y;
        clamp_dir();
        Debug.Log("In setwaterdir called with" + dir.ToString());
        rend.material.SetFloat("_WaterNormalZ", dir.z);
        rend.material.SetFloat("_WaterNormalX", dir.x);
    }

    public void ResetNormal()
    {
        dir = init_dir;
        Debug.Log("In setwaterdir called with" + dir.ToString());
        rend.material.SetFloat("_WaterNormalZ", dir.z);
        rend.material.SetFloat("_WaterNormalX", dir.x);
    }

}
