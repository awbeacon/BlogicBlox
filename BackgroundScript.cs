using UnityEngine;
using System.Collections;
using System;

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider[]))]

public class BackgroundScript : MonoBehaviour
{
	public Texture  BackgroundTexture;
	 
 	IntVector2 m_Boundries; 
	int num_tiles;
	public bool DisplayChange = false; 
	int m_Direction  ;
	bool m_Reversed  ;

	MeshFilter MF;
	MeshRenderer MR;
	MeshCollider MC;

	Vector3[] verts;
	int[] tris;
	Vector3[] normals;
	Vector2[] uv;
	 
	public void StartBackground (int _Direction, bool _Reversed)
	{
		m_Direction = _Direction;
		m_Reversed = _Reversed;
		MF = GetComponent<MeshFilter>();
		MR = GetComponent<MeshRenderer>();
		MC = GetComponent<MeshCollider>();
		 
        BuildBackground(100,100);
	}

	public void BuildBackground (int _X, int _Y )//REBUILD MESH WITH NEW BOUNDRIES
	{
		m_Boundries.x = _X;
		m_Boundries.y = _Y;
		SetObjects ();
		BuildMesh();
		MR.sharedMaterials[0].mainTexture = BackgroundTexture;
	}


	private void SetObjects ()
	{		
		num_tiles = (100 * 100);
		
		verts = new Vector3[num_tiles * 4];
		tris = new int[num_tiles * 6];
		normals = new Vector3[num_tiles * 4];
		uv = new Vector2[num_tiles * 4];
	}

	void Update ()
	{
		if (DisplayChange)//DONT CHANGE MESH JUST DISPLAY CHANGE
		{
			DisplayChanges();
			DisplayChange = false;
		}
		 
	}

	private void DisplayChanges ()
	{
		Mesh M = new Mesh();
		M = MF.mesh;
		M.uv = uv;
		M.vertices = verts;
		MF.mesh = M;
	}
	 
	public void SetTile (int x, int y,int Type )//16
	{ 		  
            float RES_tile = 64;
            float RES_full = 512;

           

            int TI = TileIndex(x, y);
            int TL = TI;
            int TR = TI + 1;
            int BL = TI + 2;
            int BR = TI + 3;

            uv[TL] = new Vector2(((Type + 0) * RES_tile) / RES_full, ((m_Direction+1)*RES_tile)/RES_full);
            uv[TR] = new Vector2(((Type + 1) * RES_tile) / RES_full, ((m_Direction + 1) * RES_tile) / RES_full);
            uv[BL] = new Vector2(((Type + 0) * RES_tile) / RES_full, ((m_Direction + 0) * RES_tile) / RES_full);
            uv[BR] = new Vector2(((Type + 1) * RES_tile) / RES_full, ((m_Direction + 0) * RES_tile) / RES_full);
            DisplayChange = true;         
	}
	
  

	private void BuildMesh ()
	{
		int tris_index = 0;

		int x, y;
		for (y = 0; y < 100; y++)
		{
			for (x = 0; x < 100; x++)
			{
				int TL = TileIndex(x, y);
				int TR = TL + 1;
				int BL = TL + 2;
				int BR = TL + 3;
				if (x < m_Boundries.x && y < m_Boundries.y)
				{
					verts[TL] = new Vector3(x + 0, y + 0, 0);
					verts[TR] = new Vector3(x + 1, y + 0, 0);
					verts[BL] = new Vector3(x + 0, y + 1, 0);
					verts[BR] = new Vector3(x + 1, y + 1, 0);
				}
				else
				{
					verts[TL] = new Vector3(  0,   0, 0);
					verts[TR] = new Vector3( 0,0, 0);
					verts[BL] = new Vector3(0,  .0001f, 0);
					verts[BR] = new Vector3(0, .0001f, 0);
				}

				Vector3 NormalDirection = new Vector3(0, 0, -1);// Vector3.up;
				normals[TL] = NormalDirection;
				normals[TR] = NormalDirection;
				normals[BL] = NormalDirection;
				normals[BR] = NormalDirection;
				SetTile(x, y,1); //sets UVs type 1 
				if (!m_Reversed)
				{
					tris[tris_index + 0] = TL;
					tris[tris_index + 1] = BR;
					tris[tris_index + 2] = TR;

					tris[tris_index + 3] = TL;
					tris[tris_index + 4] = BL;
					tris[tris_index + 5] = BR;
				}
				else
				{
					tris[tris_index + 0] = TR;
					tris[tris_index + 1] = BR;
					tris[tris_index + 2] = TL;

					tris[tris_index + 3] = BR;
					tris[tris_index + 4] = BL;
					tris[tris_index + 5] = TL;
				}

				tris_index += 6;
			}
		}
		Mesh M = new Mesh();
		M = MF.mesh;
		M.vertices = verts;
		M.triangles = tris;
		M.normals = normals;
		M.uv = uv;
		MC.sharedMesh = M;
		MF.mesh = M;

	}

	int TileNumber (int x, int y)
	{ 
		return (y * (100)) + x;
	}

	int TileIndex (int x, int y)
	{
		return TileNumber(x, y) * 4;
	}
    

}

