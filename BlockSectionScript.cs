using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshCollider[]))]
public class BlockSectionScript
{
	public bool Active = false;
	bool Visible;
	BlockConstructor Constructor;
	List<bit> BitList;

	public GameObject GO;
	MeshFilter MF;
	MeshRenderer MR;
	MeshCollider MC;
	Mesh M;

	Vector3[] verts;
	int[] tris;
	Vector3[] normals;
	Vector2[] uv;

	int m_VertCount = 0;
	int m_TrisCount = 0;

	public int checkSum = 0;

	public void BlockSectionStart (int _SectionIndex, BlockConstructor _constructor, Material _bloxMat, bool _visible)
	{
		Visible = _visible;
		GO = new GameObject("Section-" + (_visible ? "Visible" : "Trasparent") + "_Section-" + _SectionIndex);
		GO.tag = _visible ? "Blox" : "Blox_transparent";
		Constructor = _constructor;  
		MF = GO.AddComponent(typeof(MeshFilter)) as MeshFilter;
		MR = GO.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		MC = GO.AddComponent(typeof(MeshCollider)) as MeshCollider;
		ResetSection(); 
		MR.material = _bloxMat;
		if (!Visible)
		{
			MC.enabled = false;
		}
	}

	public void ResetSection ()
	{
		M = new Mesh();
		BitList = new List<bit>();

		MF.mesh = M;
		Active = false;
	}

	public void SetBits (List<bit> _bits)
	{
		BitList = _bits;
		SetObjects();
		BuildMesh();
	}


	private void SetObjects () //change to discover shared walls ********************************
	{
		int vertCount = 0;
		int trisCount = 0;

		foreach (bit B in BitList)
		{
			for (int i = 0; i < 6; i++)
			{ //  *************************************** hide edges *******************
				if (B.Neighbors[i] == null)
				{
					vertCount += 4;
					trisCount += 6;
				}
			}
		}
		verts = new Vector3[vertCount];
		tris = new int[trisCount];
		normals = new Vector3[vertCount]; 
		uv = new Vector2[vertCount]; 
	}
	 
	public void DisplayChanges ()
	{
		if (Active)
		{
			M = MF.mesh;
			M.uv = uv;
			MF.mesh = M;
		}
	}

	public void SetAllSides (bit B)
	{
		for (int i = 0; i < 6; i++)
		{
			if (B.Neighbors[i] == null)
			{
				SetSide(B.Sides[i], B.Type, B.Value);
			}
		}
	}

	public void SetSide (Side S, int Type, int Value)//add type and intensity or something
	{
		float RES_tile = 32;
		float RES_full = 4096;

		int TL = S.VertIndex[0];
		int TR = S.VertIndex[1];
		int BR = S.VertIndex[2];
		int BL = S.VertIndex[3];
		Vector2 OffSet = Common.GetOffset(Type, Value); 
		uv[TL] = new Vector2(((OffSet.x + 0) * RES_tile) / RES_full, ((OffSet.y + 1) * RES_tile) / RES_full);
		uv[TR] = new Vector2(((OffSet.x + 1) * RES_tile) / RES_full, ((OffSet.y + 1) * RES_tile) / RES_full);
		uv[BR] = new Vector2(((OffSet.x + 1) * RES_tile) / RES_full, ((OffSet.y + 0) * RES_tile) / RES_full);
		uv[BL] = new Vector2(((OffSet.x + 0) * RES_tile) / RES_full, ((OffSet.y + 0) * RES_tile) / RES_full);

	}


	private void BuildMesh ()
	{
		m_VertCount = 0;
		m_TrisCount = 0;

		foreach (bit B in BitList)
		{
			for (int i = 0; i < 6; i++)
			{ //  *************************************** hide edges *******************
				if (B.Neighbors[i] == null)
				{
					for (int j = 0; j < 4; j++)
					{
						verts[m_VertCount] = B.Sides[i].verts[j];
						B.Sides[i].VertIndex[j] = m_VertCount;
						normals[m_VertCount] = new Vector3(0, 0, -1);// Vector3.up
						m_VertCount++;
					}
					tris[m_TrisCount + 0] = B.Sides[i].VertIndex[0];
					tris[m_TrisCount + 1] = B.Sides[i].VertIndex[1];
					tris[m_TrisCount + 2] = B.Sides[i].VertIndex[3];
					tris[m_TrisCount + 3] = B.Sides[i].VertIndex[2];
					tris[m_TrisCount + 4] = B.Sides[i].VertIndex[3];
					tris[m_TrisCount + 5] = B.Sides[i].VertIndex[1];

					m_TrisCount += 6; 
					SetSide(B.Sides[i], B.Type, B.Value); //sets UVs type 1 
				}
			}
		}
		M = new Mesh();

		M = MF.mesh;
		M.vertices = verts;
		M.triangles = tris;
		M.normals = normals;
		M.uv = uv;
		MC.sharedMesh = M;
		MF.mesh = M; 
	}


}


