using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagingAreaScript : MonoBehaviour {
	Group StagingGroup;
	public BasicConstructor StagingConstructor;
	public Transform CameraRotationTransform;
	public Transform CameraTransform;
	public Camera StagingCamera;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		SetTransform();
	}

	public void SetTransform ()
	{
		if (StagingGroup!=null)
		{
			//Vector3 currentAngles = CameraRotationTransform.localEulerAngles;
			Vector3 targetAngles = new Vector3(StagingGroup.udRotation	, StagingGroup.lrRotation, 0); 
			CameraRotationTransform.localEulerAngles = targetAngles;
			CameraTransform.localPosition = new Vector3(0, 0, -(StagingGroup.zoom));
			StagingGroup.SetMinMax();
			Vector3 C = StagingGroup.center + new Vector3(.5f, .5f, .5f);
			CameraRotationTransform.localPosition = C;
		}
	}

	public void SetGroup (Group G)
	{
		StagingGroup = G;
		StagingConstructor.currentGroup = G;
	}
}
