using UnityEngine;
using UnityEngine.UI;

public class SliderConfig : MonoBehaviour
{
    public Slider greenSlider;
    public Slider redSlider;

    void Start()
    {
        SyncSliders();
    }

    void Update()
    {
        SyncSliders();
    }

    void SyncSliders()
    {
        redSlider.minValue = greenSlider.value;
    }
}
