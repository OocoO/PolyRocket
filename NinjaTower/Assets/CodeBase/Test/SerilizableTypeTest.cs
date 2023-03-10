using System.Collections;
using System.Collections.Generic;
using Carotaa.Code;
using UnityEngine;

public class SerializableTypeTest : MonoBehaviour
{
    public SerializableType type1;
    // Start is called before the first frame update
    void Start()
    {
        type1.Value = typeof(GameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
