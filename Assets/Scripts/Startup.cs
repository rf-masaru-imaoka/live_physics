///
///  @file Startup.cs
///  @brief 機能説明
///
///  Copyright (c) 2016- R-FORCE ENTERTAINMENT inc. All rights reserved.
///  @author 所有者名
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

class MyOptions
{
    public static readonly MyOptions Instance = new MyOptions();

    public void SomeAction() { /* ... */ }
    public float SomeProperty { get; set; }
}

public class MyOptions2
{
    public static readonly MyOptions2 Instance = new MyOptions2();

    private float test;

    public enum DebugMenuMain
    {
        InGame,
        OutGame,
        System
    }

    private DebugMenuMain debugMenuMain;

    [Category("Main")]
    public DebugMenuMain Main
    {
        get { return debugMenuMain; }
        set
        {
            //OnValueChanged("TestEnum", value);
            debugMenuMain = value;
        }
    }

    [Category("Test2")]
    public float TestFloat2
    {
        get { return test; }
        set
        {
            test = value;
        }
    }

}


public class Startup : MonoBehaviour {

	// Use this for initialization
	void Start () {

}
	
	// Update is called once per frame
	void Update () {
		
	}

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        SRDebug.Instance.AddOptionContainer(new SROptions2());
        SRDebug.Instance.AddOptionContainer(new SROptions3());
    }



}
