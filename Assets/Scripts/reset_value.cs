using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class reset_value : MonoBehaviour
{
    Slider I;
    // Start is called before the first frame update
    void Start()
    {
        I = GetComponent<Slider>();
    }
    
    public void reset()
    {
        I.value = 0;
    }
}
