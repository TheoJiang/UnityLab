using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VexelLighting : MonoBehaviour
{
    private Light[] lights = null;

    
    private void OnEnable()
    {
        lights = GameObject.FindObjectsOfType<Light>().Where((light1, i) =>
        {
            return light1.type == LightType.Point;
        }).ToArray();
    }

    void DivideSpace()
    {
        List<float> dists = new List<float>();
        for (float x = -10; x < 10; x+=0.5f)
        {
            for (float y = -10; y < 10; y+=0.5f)
            {
                for (float z = -10; z < 10; z+=0.5f)
                {
                    var cellPos = new Vector3(x, y, z);
                    for (int i = 0; i < lights.Length; i++)
                    {
                        var dist = (cellPos - lights[i].transform.position).magnitude;
                    }
                }
            }
        }
    }
}
