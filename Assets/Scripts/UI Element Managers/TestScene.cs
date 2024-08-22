using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    [SerializeField] private GameObject template;
    // Start is called before the first frame update
    void Start()
    {
        GameObject templater = Instantiate(template, new Vector3(0, -400, 0), Quaternion.identity);
        templater.transform.SetParent(this.transform, false);

        templater.GetComponent<BarConfigVariable>().SetNumberOfBars(5);
        templater.GetComponent<BarConfigVariable>().SetValue(new List<float>{100.0f, 20.0f, 50.0f, 40.0f, 10.0f});
        templater.GetComponent<BarConfigVariable>().SetColor(new List<Color>{Color.black, Color.green, Color.red, Color.blue, Color.yellow});
        templater.GetComponent<BarConfigVariable>().SetScale(BarConfigVariable.Modes.direct);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
