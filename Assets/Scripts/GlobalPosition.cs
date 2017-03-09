///
///  @file GlobalPosition.cs
///  @brief 機能説明
///
///  Copyright (c) 2016- R-FORCE ENTERTAINMENT inc. All rights reserved.
///  @author 所有者名
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPosition : MonoBehaviour {

    public Vector3 GlobalPos;



	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        GlobalPos = gameObject.transform.position;
	}
}
