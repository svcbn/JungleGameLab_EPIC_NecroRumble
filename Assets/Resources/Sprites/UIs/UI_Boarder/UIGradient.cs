using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGradient : MonoBehaviour
{
    public Material gradientMat;
    public Color leftCol;
    public Color centerCol;
    public Color rightCol;

    void Start()
    {
        leftCol.a = 0;
        gradientMat.SetColor("_Color", leftCol);    
        gradientMat.SetColor("_Color2", centerCol);    
        rightCol.a = 0;
        gradientMat.SetColor("_Color3", rightCol);    
    }
}