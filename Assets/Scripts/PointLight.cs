using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointLight : MonoBehaviour {
    public float Near = 5.0f;
    public float Far = 10.0f;
    public Color LightColor = Color.white;
    // Use the transform's position as light position

    //public GameObject n, f;
    public bool ShowLightRanges = false;
    public bool Light2Toggle = false;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        GetComponent<Renderer>().material.color = LightColor;
        //n.SetActive(ShowLightRanges);
        //f.SetActive(ShowLightRanges);

        Color c = LightColor;
        c.a = 0.2f;
        //n.transform.localPosition = transform.localPosition;
        //n.transform.localScale = new Vector3(2*Near, 2*Near, 2*Near);
        //n.GetComponent<Renderer>().material.color = c;

        c.a = 0.1f;
        //f.transform.localPosition = transform.localPosition;
        //f.transform.localScale = new Vector3(2*Far, 2*Far, 2*Far);
        //f.GetComponent<Renderer>().material.color = c;
        LoadLightToShader();
    }

    virtual public void LoadLightToShader()
    {
        if(Light2Toggle)
        {
            Shader.SetGlobalVector("Light2Position", transform.localPosition);
            Shader.SetGlobalColor("Light2Color", LightColor);
            Shader.SetGlobalFloat("Light2Near", Near);
            Shader.SetGlobalFloat("Light2Far", Far);
        } else
        {
            Shader.SetGlobalVector("LightPosition", transform.localPosition);
            Shader.SetGlobalColor("LightColor", LightColor);
            Shader.SetGlobalFloat("LightNear", Near);
            Shader.SetGlobalFloat("LightFar", Far);
        }
        
    }
}
