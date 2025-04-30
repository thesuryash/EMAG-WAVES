using UnityEngine;
using XCharts.Runtime;

public class LineChartExample : MonoBehaviour
{
    public LineChart lineChart; // Assign your LineChart component in the Inspector

    private void Start()
    {
        // Initialize the chart with two series: one for Flux and one for Current 
        lineChart.AddSerie<Line>("Flux Data");    // Add a series named "Flux Data"
        lineChart.AddSerie<Line>("Current Data"); // Add a series named "Current Data"

        // Add initial data points for time = 0
        lineChart.AddXAxisData("Time 0");
        lineChart.AddData(0, FaradayLawVariables.Flux);    // Flux at time 0
        lineChart.AddData(1, FaradayLawVariables.Current); // Current at time 0
    }

    private void Update()
    {
        // Fetch current time and format it for the X-axis label
        string timeLabel = $"Time {Time.time:F1}";

        // Fetch the current values from FaradayLawVariables
        float fluxValue = Mathf.Round(FaradayLawVariables.Flux);       // Replace this with actual logic if needed
        float currentValue = Mathf.Round(FaradayLawVariables.Current); // Replace this with actual logic if needed

        // Add new data to the chart
        lineChart.AddXAxisData(timeLabel); // Add the time label on the X-axis
        lineChart.AddData(0, FaradayLawVariables.Flux);    // Add Flux to the first series
        lineChart.AddData(1, FaradayLawVariables.Current); // Add Current to the second series
    }
}