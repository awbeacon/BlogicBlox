using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class BackgroundController : MonoBehaviour
{
	public float Speed = .5f;
	//public BackgroundScript[] Backgrounds;
	public Transform[] Backgrounds;
	Vector3[] BackroundLocations;
	public Vector3 Center;
	public Vector3 m_Boundries;
	//public Transform[] FarWalls;
	public Vector3 m_Boundries_set;

	public VisibleMaskScript[] Masks;
	public GameManagerScript GM;

	// Use this for initialization
	void Start ()
	{
		BackroundLocations = new Vector3[6];
		for (int i = 0; i < 6; i++)
		{
			BackroundLocations[i] = Backgrounds[i].localPosition;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		//CheckChanges();
		CheckBoundries();
		SetBoundries();
	}

	private void SetBoundries ()
	{
		this.transform.position = Center;
		for (int i = 0; i < 6; i++)
		{
			Backgrounds[i].localPosition = BackroundLocations[i];
		}

	}

	private void CheckBoundries ()
	{
		float Offset = .01f;
		m_Boundries = Vector3.Lerp(m_Boundries_set, m_Boundries, Speed);
		float _Z = m_Boundries.z/2;
		float _X = m_Boundries.x/2;
		float _Y = m_Boundries.y/2;
		BackroundLocations[0].z = -(_Z  ) - Offset;
		BackroundLocations[1].z = (_Z ) + Offset;
		BackroundLocations[2].x = -(_X ) - Offset;
		BackroundLocations[3].x = (_X  ) + Offset;
		BackroundLocations[4].y = -(_Y) - Offset;
		BackroundLocations[5].y = (_Y) + Offset;

		Vector3 _min = GM._bloxManager._minVisible;
		Vector3 _max = GM._bloxManager._maxVisible;
		 Masks[0]._LR = new Vector2(-_max.x, -_min.x);
		 Masks[0]._DU = new Vector2(_min.y, _max.y);
		 Masks[0].Depth =new Vector3(0,0, -(_Z - (Offset * 2)) + Center.z); 

	    Masks[1]._LR = new Vector2(_min.x, _max.x);
		Masks[1]._DU = new Vector2(_min.y, _max.y);
		Masks[1].Depth = new Vector3(0, 0, (_Z - (Offset * 2)) + Center.z);

		Masks[2]._LR = new Vector2(_min.z, _max.z);
		Masks[2]._DU = new Vector2(_min.y, _max.y);
		Masks[2].Depth = new Vector3(  -(_X - (Offset * 2)) + Center.x, 0, 0);

		Masks[3]._LR = new Vector2(-_max.z, -_min.z);		
		Masks[3]._DU = new Vector2(_min.y, _max.y);
		Masks[3].Depth =new Vector3(  (_X - (Offset * 2)) + Center.x,0,0);


		Masks[4]._LR = new Vector2(_min.x, _max.x);
		Masks[4]._DU = new Vector2(_min.z, _max.z);
		Masks[4].Depth = new Vector3(0,-(_Y - (Offset * 2)) + Center.y, 0);

		Masks[5]._LR = new Vector2(-_max.x, -_min.x);
		Masks[5]._DU = new Vector2(_min.z, _max.z);
		Masks[5].Depth = new Vector3(0, (_Y - (Offset * 2)) + Center.y, 0);

	}


}
