using System.Collections;
using System.Collections.Generic;
using BugsnagUnity;
using UnityEngine;

public class BugsnagText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Bugsnag.Notify(new System.InvalidOperationException("Test error"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
