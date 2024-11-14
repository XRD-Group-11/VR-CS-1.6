using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SliderScale : MonoBehaviour
{
    public XRSlider slider;
    public Vector3 minScale = Vector3.zero;
    public Vector3 maxScale = new Vector3(2f, 2f, 2f);
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        Vector3 newScale = Vector3.Lerp(minScale,maxScale,slider.value);
        transform.localScale = newScale; 
    }
}
