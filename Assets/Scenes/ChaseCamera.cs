///
///  @file ChaseCamera.cs
///  @brief 機能説明
///
///  Copyright (c) 2016- R-FORCE ENTERTAINMENT inc. All rights reserved.
///  @author 所有者名
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour
{

    public GameObject target;

    private float length;

    private float height;

    // Use this for initialization
    void Start ()
    {
        length = Vector3.Distance(transform.position, target.transform.position);
        height = target.transform.position.y;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = new Vector3(target.transform.position.x, height, target.transform.position.z+length);
    }
}
