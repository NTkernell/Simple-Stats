using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using System;

[BepInPlugin("com.ntkernel.gorillatag.SimpleStats", "SimpleStats", "1.0.0")]
public class SimpleStats : BaseUnityPlugin
{
    private Canvas statsCanvas;
    private Text statsText;
    private GUIStyle style;
    private bool showStats = true;
    private static readonly Process currentProcess = Process.GetCurrentProcess();
    private float gameStartTime;
   

    private void Awake()
    {
        style = new GUIStyle
        {
            fontSize = 22,
            normal = { textColor = Color.white }
        };

        gameStartTime = Time.time;
        InitializeUI();

        Harmony.CreateAndPatchAll(typeof(SimpleStats));
    }

    private void InitializeUI()
    {
        GameObject canvasObject = new GameObject("StatsCanvas");
        statsCanvas = canvasObject.AddComponent<Canvas>();
        statsCanvas.renderMode = RenderMode.WorldSpace;
        statsCanvas.worldCamera = Camera.main;
        statsCanvas.planeDistance = 0.5f;

        GameObject textObject = new GameObject("StatsText");
        textObject.transform.SetParent(statsCanvas.transform);
        statsText = textObject.AddComponent<Text>();
        statsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        statsText.fontSize = 24;
        statsText.color = Color.white;
        statsText.alignment = TextAnchor.UpperLeft;
        statsText.rectTransform.sizeDelta = new Vector2(300, 150);
        statsText.rectTransform.anchoredPosition = new Vector2(0, 0);

        RectTransform canvasRect = statsCanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(300, 150);

        statsCanvas.enabled = showStats;
    }

    private void Update()
    {
        if (showStats)
        {
            UpdateStats();
        }
    }

    private void UpdateStats()
    {
        float fps = 1.0f / Time.unscaledDeltaTime;
        float frameTime = Time.unscaledDeltaTime * 1000;
        string gpuName = SystemInfo.graphicsDeviceName;
        string cpuName = GetCpuName();
        string runtime = GetRuntime();

        if (statsText != null)
        {
            statsText.text = $"CPU: {cpuName}\nGPU: {gpuName}\nFPS: {fps:0}\nFrametime: {frameTime:0.0} ms\nPlaying for: {runtime}";
        }
    }

    private string GetCpuName()
    {
        try
        {
            string cpuName = System.Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
            if (!string.IsNullOrEmpty(cpuName))
            {
                return cpuName;
            }

            return "Sorry! an error occurred"; //awwhhh error :(
        }
        catch
        {
            return "Sorry! an error occurred"; //another error whaaaattttt
        }
    }

    private string GetRuntime()
    {
        float elapsedTime = Time.time - gameStartTime;
        int hours = (int)(elapsedTime / 3600);
        int minutes = (int)((elapsedTime % 3600) / 60);
        int seconds = (int)(elapsedTime % 60);

        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    private void OnGUI()
    {
        if (showStats)
        {
            float fps = 1.0f / Time.unscaledDeltaTime;
            float frameTime = Time.unscaledDeltaTime * 1000;
            string gpuName = SystemInfo.graphicsDeviceName;
            string cpuName = GetCpuName();
            string runtime = GetRuntime();

            GUI.Label(new Rect(10, 10, 400, 200), $"CPU: {cpuName}\nGPU: {gpuName}\nFPS: {fps:0}\nFrametime: {frameTime:0.0} ms\nPlaying for: {runtime}", style);
        }
    }
}
