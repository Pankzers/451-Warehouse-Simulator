using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLight : MonoBehaviour {
    public PointLight ALight;
    public PointLight BLight;

	void OnPreRender()
    {
        ALight.LoadLightToShader();
        BLight.LoadLightToShader();
    }
}
