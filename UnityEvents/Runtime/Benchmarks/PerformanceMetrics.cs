using UnityEngine;
using UnityEngine.Profiling;
using System.Collections;

public class PerformanceMetrics : MonoBehaviour
{
    public int durationInSeconds = 10;

    void Start()
    {
        StartCoroutine(CapturePerformanceMetrics(durationInSeconds));
    }

    IEnumerator CapturePerformanceMetrics(int duration)
    {
        float totalCpuUsage = 0;
        float totalMemoryUsage = 0;
        float totalFrameTime = 0;
        float totalUnscaledFrameTime = 0;
        int sampleCount = 0;

        for (int i = 0; i < duration; i++)
        {
            float cpuUsage = GetCpuUsage();
            float memoryUsage = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024); // Convert bytes to MB
            float frameTime = Time.deltaTime * 1000; // Convert to milliseconds
            float unscaledFrameTime = Time.unscaledDeltaTime * 1000; // Convert to milliseconds

            totalCpuUsage += cpuUsage;
            totalMemoryUsage += memoryUsage;
            totalFrameTime += frameTime;
            totalUnscaledFrameTime += unscaledFrameTime;
            sampleCount++;

            yield return new WaitForSeconds(1);
        }

        float averageCpuUsage = totalCpuUsage / sampleCount;
        float averageMemoryUsage = totalMemoryUsage / sampleCount;
        float averageFrameTime = totalFrameTime / sampleCount;
        float averageUnscaledFrameTime = totalUnscaledFrameTime / sampleCount;
        float averageFrameRate = 1000.0f / averageUnscaledFrameTime;

        Debug.LogWarning($"Average CPU Usage: {averageCpuUsage}%");
        Debug.LogWarning($"Average Memory Usage: {averageMemoryUsage} MB");
        Debug.LogWarning($"Average Frame Time: {averageFrameTime} ms");
        Debug.LogWarning($"Average Unscaled Frame Time: {averageUnscaledFrameTime} ms");
        Debug.LogWarning($"Average Frame Rate: {averageFrameRate} FPS");
    }

    float GetCpuUsage()
    {
        float cpuUsage = 0;
        float frameTime = Time.unscaledDeltaTime;
        int cpuCount = SystemInfo.processorCount;

        // Estimate CPU usage per frame
        cpuUsage = (frameTime / (1.0f / cpuCount)) * 100.0f;
        return cpuUsage;
    }
}
