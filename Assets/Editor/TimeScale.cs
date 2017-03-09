///
///  @file TimeScale.cs
///  @brief 機能説明
///
///  Copyright (c) 2016- R-FORCE ENTERTAINMENT inc. All rights reserved.
///  @author 所有者名
///
using UnityEngine;
using UnityEditor;

/// <summary>
/// 実行速度の変更
/// </summary>
public class TimeScaler : UnityEditor.Editor
{
    [MenuItem("Tools/TimeScale/x1")]
    public static void TimeScale1()
    {
        Time.timeScale = 1f;
    }

    [MenuItem("Tools/TimeScale/x0.5")]
    public static void TimeScale05()
    {
        Time.timeScale = 0.5f;
    }

    [MenuItem("Tools/TimeScale/x0.1")]
    public static void TimeScale01()
    {
        Time.timeScale = 0.1f;
    }
}
