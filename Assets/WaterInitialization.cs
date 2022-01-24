using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterInitialization : MonoBehaviour
{

    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.SetFloat("_XScale", transform.parent.transform.localScale.x);
        rend.material.SetFloat("_YScale", transform.parent.transform.localScale.y);
        rend.material.SetFloat("_ZScale", transform.parent.transform.localScale.z);

        Debug.Log("local x: " + transform.localScale.x.ToString() + "parent x: " + transform.parent.transform.localScale.x.ToString());
    }

}
