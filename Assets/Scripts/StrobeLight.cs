using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrobeLight : PointLight
{
    public Color OffColor = Color.black;
    private bool flash = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(flash)
        {
            if(Far < 1)
            {
                flash = !flash;
            }
            Far -= 10f * Time.deltaTime;
        } else
        {
            if(Far > 10)
            {
                flash = !flash;
            }
            Far += 10f * Time.deltaTime;
        }
        GetComponent<Renderer>().material.color = LightColor;

        Color c = LightColor;
        c.a = 0.2f;

        c.a = 0.1f;
        LoadLightToShader();
    }

    override public void LoadLightToShader()
    {
        if (Light2Toggle)
        {
            Shader.SetGlobalVector("Light2Position", transform.localPosition);
            Shader.SetGlobalColor("Light2Color", LightColor);

            Shader.SetGlobalFloat("Light2Near", Near);
            Shader.SetGlobalFloat("Light2Far", Far);
        }
        else
        {
            Shader.SetGlobalVector("LightPosition", transform.localPosition);
            
            Shader.SetGlobalFloat("LightNear", Near);
            Shader.SetGlobalFloat("LightFar", Far);
        }

    }
}
