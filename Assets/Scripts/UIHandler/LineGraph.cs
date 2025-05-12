using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class LineGraph : MonoBehaviour
{
    public GameObject emptyGraphPrefab;

	public WMG_Axis_Graph graph;

	public WMG_Series series1;

    public List<string> series1Data = new List<string> {"21.08.,10","20.08.,6","19.08.,8","18.08.,2","17.08.,4"};

    public int AxisLabelSize = 25;

	// Use this for initialization
	void Start () {
        series1Data = new List<string> {"21.08.,10","20.08.,6","19.08.,8","18.08.,2","17.08.,4"};

        // SetGraph(series1Data, "Answers", 0, 5, 0, 10);
	}

    void Awake()
    {

    }

    public void Update()
    {
        
    }

    public void OnValidate()
    {

    }

    public void SetGraph(List<String> dataList, string seriesName, int xMin, int xMax, int yMin, int yMax)
    {
        GameObject graphGO = emptyGraphPrefab;
		graphGO.transform.SetParent(this.transform, false);
		graph = graphGO.GetComponent<WMG_Axis_Graph>();

		series1 = graph.addSeries();

        graphGO.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta;
        graph.xAxis.AxisLabelSize = AxisLabelSize;
        graph.yAxis.AxisLabelSize = AxisLabelSize;
        
        graph.xAxis.AxisMinValue = xMin;
        graph.yAxis.AxisMinValue = yMin;
        graph.xAxis.AxisMaxValue = xMax;
        graph.yAxis.AxisMaxValue = yMax;

        List<string> groups = new List<string>();
        List<Vector2> data = new List<Vector2>();
        for (int i = 0; i < dataList.Count; i++) {
            string[] row = dataList[i].Split(',');
            groups.Add(row[0]);
            if (!string.IsNullOrEmpty(row[1])) {
                float y = float.Parse(row[1]);
                data.Add(new Vector2(i+1, y));
            }
        }

        graph.groups.SetList(groups);
        graph.useGroups = true;

        graph.xAxis.LabelType = WMG_Axis.labelTypes.groups;
        graph.xAxis.AxisNumTicks = groups.Count;

        series1.seriesName = seriesName;

        series1.UseXDistBetweenToSpace = true;

        series1.pointValues.SetList(data);
    }

}
