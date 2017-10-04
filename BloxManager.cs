using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloxManager : MonoBehaviour
{
	public GameManagerScript GM;
	public BlockConstructor _bConstructor;

	public Vector3 _minBorder = new Vector3(0, 0, 0);
	public Vector3 _maxBorder = new Vector3(0, 0, 0);
	public Vector3 _minVisible = new Vector3(-5, -500, -500);
	public Vector3 _maxVisible = new Vector3(5, 500, 500);
	public	bool _RebuildALL=false;

	
	public GameObject HighlightCamera;
	[Range(0, 50)]
	public float Transparency;

	// Update is called once per frame
	void Update ()
	{

		CheckBoundaries();
		CheckRebuild();
		SetTransparency();
	}
	 
	private void CheckRebuild ()
	{
		if (_RebuildALL)
		{
			_RebuildALL = false;			 
			_bConstructor._Rebuild = true;		
		}
	}

	private void CheckBoundaries ()//CHECK AND SET WORKSHOP BOUNDRIES
	{
		_minBorder = new Vector3(0, 0, 0);
		_maxBorder = new Vector3(0, 0, 0);
		_bConstructor.currentGroup.SetMinMax();
		_minBorder = _bConstructor.currentGroup.min;
		_maxBorder = _bConstructor.currentGroup.max; 
		GM._backgroundController.m_Boundries_set.x = (int)(_maxBorder.x - _minBorder.x) + 10;
		GM._backgroundController.m_Boundries_set.y = (int)(_maxBorder.y - _minBorder.y) + 10;
		GM._backgroundController.m_Boundries_set.z = (int)(_maxBorder.z - _minBorder.z) + 10;
		GM._backgroundController.m_Boundries_set.x += GM._backgroundController.m_Boundries_set.x % 2;
		GM._backgroundController.m_Boundries_set.y += GM._backgroundController.m_Boundries_set.y % 2;
		GM._backgroundController.m_Boundries_set.z += GM._backgroundController.m_Boundries_set.z % 2;
		GM._backgroundController.Center.x = (int)((_maxBorder.x + _minBorder.x) / 2);
		GM._backgroundController.Center.y = (int)((_maxBorder.y + _minBorder.y) / 2);
		GM._backgroundController.Center.z = (int)((_maxBorder.z + _minBorder.z) / 2);
	}

	public void SetTransparency ()
	{
		GM.BloxMatTransparent.SetColor("_Color", new Color(1, 1, 1, Transparency / 100));
	}
}
