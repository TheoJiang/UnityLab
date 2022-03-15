using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class VexelLighting : MonoBehaviour
{
    private Light[] lights = null;

    public Material mat;
    public GameObject go;
    private void OnEnable()
    {
        lights = GameObject.FindObjectsOfType<Light>().Where((light1, i) =>
        {
            return light1.type == LightType.Point && light1.gameObject.activeSelf;
        }).ToArray();

        mat = go.GetComponent<Renderer>().sharedMaterial;
        DivideSpace();
    }
    List<Color> colors;
    List<Vector4> pos;
    
    public void DivideSpace()
    {
        List<float> cofs = new List<float>();
        pos = new List<Vector4>();
        colors = new List<Color>();
        for (float x = -5; x < 5; x+=1f)
        {
            for (float y = -5; y < 5; y+=1f)
            {
                for (float z = -5; z < 5; z+=1f)
                {
                    int xID = (int)(x * 2 + 5);
                    int yID = (int)(y * 2 + 5);
                    int zID = (int)(z * 2 + 5);
                    var cellPos = new Vector3(x, y, z);
                    pos.Add(cellPos);
                    float totalCellToLightdist = 0;

                    List<Light> tempLights = new List<Light>();
                    List<float> cellToLightRate = new List<float>();
                    // Bounds bounds = new Bounds();
                     // bounds.Encapsulate(cellPos);
                     List<float> cellToLightdistList = new List<float>();

                    for (int i = 0; i < lights.Length; i++)
                    {
                        var cellToLightdist = (cellPos - lights[i].transform.position).magnitude;
                        if (cellToLightdist <= lights[i].range)
                        {
                            totalCellToLightdist += cellToLightdist;
                            cellToLightdistList.Add(cellToLightdist);
                            tempLights.Add(lights[i]);
                        }
                    }
                    
                    foreach (var cellToLightdist in cellToLightdistList)
                    {
                        cellToLightRate.Add(cellToLightdist / totalCellToLightdist);
                    }

                    Color lightColor = Color.black;
                    for (int i = 0; i < tempLights.Count; i++)
                    {
                        // var dist = (cellPos - tempLights[i].transform.position).magnitude;
                        
                        if (cellToLightdistList[i] <= lights[i].range)
                        {
                            var rate = cellToLightRate[i];
                            if (rate>=1)
                            {
                                
                            }
                            // var atte = 1/(cellToLightdistList[i]*cellToLightdistList[i] + tempLights[i].range*tempLights[i].range/2.0f);
                            lightColor += (float)(Math.Pow(0.005, (cellToLightdistList[i] / tempLights[i].range) )) * tempLights[i].color; // * rate
                            // lightColor += (1 - (cellToLightdistList[i] / tempLights[i].range)) * tempLights[i].color * rate;
                            lightColor.a = 1;
                        }
 
                    }
                    colors.Add(lightColor);
                }
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < pos.Count; i++)
        {
            Gizmos.color = colors[i];
            Gizmos.DrawWireCube(pos[i], Vector3.one);
        }
        // Gizmos.DrawCube(new Vector3(0,0,0), Vector3.one);
    }

    private void Update()
    {
        SendMatProps();
    }

    void SendMatProps()
    {
        mat.SetColorArray("lights", colors);
        mat.SetVectorArray("Pos", pos);
    }
}
