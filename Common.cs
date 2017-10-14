using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Common
{ 
	public enum GameMode
	{
		PlayModeChangeName, Workshop
	} 
	public enum GameState
	{
		Select, Place, Delete, Open, Save, Info, Play, View,
		Drag
	}
	 
	public enum CursorBinDetailsState
	{
		 Detail, Save, Open, New , Download, Upload
	}

	public static int DirectionZ = 0;
	public static int DirectionX = 1;
	public static int DirectionY = 2;

	public static Vector2 GetOffset (int type, int value)
	{
		Vector2 Return = new Vector2(127, 127);//connector
		if (type > -1)
		{
			if (type < 32)//basic bits with 256 types
			{
				Return.x = value % 128;
				Return.y = (type * 2) + ((value > 127) ? 1 : 0);
			}
		}

		return Return;
	}

	public static Point GetOrigin (Vector3 rawHitPoint, Vector3 rawCameraPoint )
	{
		Vector3 hpFloor = new Vector3(Mathf.Floor(rawHitPoint.x + .001f), Mathf.Floor(rawHitPoint.y + .001f), Mathf.Floor(rawHitPoint.z + .001f));//.001 cuz raycast sometimes registers at .999999999 instead of 1
		Vector3 Diff = rawHitPoint - hpFloor;

		bool X_middle = (Diff.x > .01f && Diff.x < .99f);
		bool Y_middle = (Diff.y > .01f && Diff.y < .99f);
		bool Z_middle = (Diff.z > .01f && Diff.z < .99f);
		bool X_side = Y_middle && Z_middle;
		bool Y_side = !X_side && X_middle && Z_middle;
		bool Z_side = !Y_side && X_middle && Y_middle;
		bool isSide = (X_side || Y_side || Z_side); 
		Vector3 OffSet = new Vector3(0, 0, 0);
		if (X_side && rawCameraPoint.x > hpFloor.x)
		{
			OffSet.x = -1;
		}
		else if (Y_side && rawCameraPoint.y > hpFloor.y)
		{
			OffSet.y = -1;
		}
		else if (Z_side && rawCameraPoint.z > hpFloor.z)
		{
			OffSet.z = -1;
		}
		return new Point(hpFloor + OffSet);
	}
	public static Point GetDesination (Vector3 rawHitPoint, Vector3 rawCameraPoint)
	{
		Vector3 hpFloor = new Vector3(Mathf.Floor(rawHitPoint.x + .001f), Mathf.Floor(rawHitPoint.y + .001f), Mathf.Floor(rawHitPoint.z + .001f));//.001 cuz raycast sometimes registers at .999999999 instead of 1
		Vector3 Diff = rawHitPoint - hpFloor;

		bool X_middle = (Diff.x > .01f && Diff.x < .99f);
		bool Y_middle = (Diff.y > .01f && Diff.y < .99f);
		bool Z_middle = (Diff.z > .01f && Diff.z < .99f);
		bool X_side = Y_middle && Z_middle;
		bool Y_side = !X_side && X_middle && Z_middle;
		bool Z_side = !Y_side && X_middle && Y_middle;
		bool isSide = (X_side || Y_side || Z_side);
		 
		Vector3 OffSet = new Vector3(0, 0, 0);
		if (X_side && rawCameraPoint.x < hpFloor.x)
		{
			OffSet.x = -1;
		}
		else if (Y_side && rawCameraPoint.y < hpFloor.y)
		{
			OffSet.y = -1;
		}
		else if (Z_side && rawCameraPoint.z < hpFloor.z)
		{
			OffSet.z = -1;
		}
		return new Point(hpFloor + OffSet);
	}



}

public static class BitTypes
{
	public static string[] names = new string[3] {
		"Insulator",//0
		"Battery",//1
        "Wire: Copper"//2
	};

	public static string GetName (int _n)
	{
		if (_n < names.Length)
		{
			return names[_n];
		}
		return "Type: " + (_n+1);
	}
}


//**************** GROUP ********************  
public class Group
{
	//public bool hasChanges = true;
	List<RawBit> rawBits;
	
	public String Name;
	public String Description;
	public String Creator;
	public List<int> checkSums = new List<int>(); 
	public Vector3 min = new Vector3(0, 0, 0); 
	public Vector3 max = new Vector3(0, 0, 0); 
	public Vector3 center = new Vector3(0, 0, 0);

	public float udRotation = 0;
	public float lrRotation = 0;
	public float zoom = 2;
	 

	public String GetFileName ()
	{
		string _fileName = Name.Replace(' ', '_');
		return _fileName;
	}
    public Group ()
	{
		initializeGroup();
	}

	public Group (RawBit rb)
	{
		initializeGroup();
		AddBit(rb);//base part
	}

	public Group (List<RawBit> rbs)
	{
		initializeGroup();
		foreach (RawBit rb in rbs)
		{
			AddBit(rb);//base part
		}
	}

	public Group (GroupSave gS)
	{
		initializeGroup();
		Name = gS.Name;
		Description = gS.Description;
		udRotation = gS.udRotation;
		lrRotation = gS.lrRotation;
		zoom = gS.zoom;
		foreach (RawBitSave rbs in gS.rawBitSaves)
		{
			AddBit(new RawBit(rbs.point, rbs.type, rbs.value));
		}
	}

	public Group (Group g)
	{
		initializeGroup();
		Name = g.Name;
		Description = g.Description;
		udRotation = g.udRotation;
		lrRotation = g.lrRotation;
		zoom = g.zoom;
		foreach (RawBit rbs in g.rawBits)
		{
			AddBit(new RawBit(rbs.point, rbs.type, rbs.value));
		}
	}

	private void initializeGroup ()
	{
		rawBits = new List<RawBit>();
		//hasChanges = true;
	}

	public void AddBit (RawBit rb)
	{
		//hasChanges = true;
		if (!BitExists(rb.point))
		{
			rawBits.Add(rb);
			CheckNeighbors(rb);
		}
	}

	internal void SetDefaultViews ()
	{
		SetMinMax();
		float diffX =Mathf.Abs( max.x - min.x);
		float diffY = Mathf.Abs(max.y - min.y);
		float diffZ = Mathf.Abs(max.z - min.z);
		float maxDiff = Mathf.Max(diffX, diffY, diffZ);
		zoom = maxDiff;
		udRotation = 45;
		lrRotation = 45;
	}

	private void CheckNeighbors (RawBit rb)
	{
		Point P = rb.point;
		rb.Neighbors[0] = GetRawbitAtPoint(new Point(P.x + 1, P.y, P.z));
		rb.Neighbors[1] = GetRawbitAtPoint(new Point(P.x - 1, P.y, P.z));
		rb.Neighbors[2] = GetRawbitAtPoint(new Point(P.x, P.y + 1, P.z));
		rb.Neighbors[3] = GetRawbitAtPoint(new Point(P.x, P.y - 1, P.z));
		rb.Neighbors[4] = GetRawbitAtPoint(new Point(P.x, P.y, P.z + 1));
		rb.Neighbors[5] = GetRawbitAtPoint(new Point(P.x, P.y, P.z - 1));
		if (rb.Neighbors[0] != null)
		{
			rb.Neighbors[0].Neighbors[1] = rb;
		}
		if (rb.Neighbors[1] != null)
		{
			rb.Neighbors[1].Neighbors[0] = rb;
		}
		if (rb.Neighbors[2] != null)
		{
			rb.Neighbors[2].Neighbors[3] = rb;
		}
		if (rb.Neighbors[3] != null)
		{
			rb.Neighbors[3].Neighbors[2] = rb;
		}
		if (rb.Neighbors[4] != null)
		{
			rb.Neighbors[4].Neighbors[5] = rb;
		}
		if (rb.Neighbors[5] != null)
		{
			rb.Neighbors[5].Neighbors[4] = rb;
		}
	}

	public void AddBits (List<RawBit> rbs)
	{
		//hasChanges = true;
		foreach (RawBit rb in rbs)
		{
			AddBit(rb);
		}
	}

	public void addGroup (Point destination, Group group)
	{
		//hasChanges = true;
		AddBits(group.GetRawBits(destination));
	}



	public RawBit GetRawbitAtPoint (Point point)
	{
		foreach (RawBit rb in rawBits)
		{
			if (rb.point.Equals(point))
			{
				return rb;
			}
		}

		return null;
	}

	//simple way to check 99.99% if part has any changes without having to keep track of any changes
	public int GetCheckSum (int partNumber)
	{
		int checkSum = 0;
		for (int i = 0; i < 1000; i++)
		{
			int rIndex = i + (1000 * partNumber);
			if (rIndex < rawBits.Count)
			{
				RawBit rb = rawBits[rIndex];
				checkSum +=
					(rb.point.x * (i + 1)) +
					(rb.point.x * (i + 1) * 10) +
					(rb.point.x * (i + 1) * 100) +
					(rb.type * (i + 1) * 1000) +
					(rb.value * (i + 1) * 10000) +
					(rb.Cursor ? 5 * (i + 1) : 55 * (i + 1)) +
					(rb.Selected ? 9 * (i + 1) : 99 * (i + 1));
			}
		}
		return checkSum;
	}

	public void SetCheckSums ()
	{
		checkSums.Clear();
		for (int i = 0; i < GetPartCount(); i++)
		{
			checkSums.Add(GetCheckSum(i));
		}
	}
	public int GetBitZeroValue ()
	{
		return rawBits[0].value;
	}
	public void SetBitZeroValue (int ADD)
	{
		rawBits[0].value = Mathf.Clamp((rawBits[0].value+ ADD),0,255);
	}

	public void SetMinMax ()
	{
		min = new Vector3(0, 0, 0);
		max = new Vector3(0, 0, 0);
		center = new Vector3(0, 0, 0);
		foreach (RawBit rb in rawBits)
		{
			min.x = (rb.point.x < min.x ? rb.point.x : min.x);
			min.y = (rb.point.y < min.y ? rb.point.y : min.y);
			min.z = (rb.point.z < min.z ? rb.point.z : min.z);
			max.x = (rb.point.x > max.x ? rb.point.x : max.x);
			max.y = (rb.point.y > max.y ? rb.point.y : max.y);
			max.z = (rb.point.z > max.z ? rb.point.z : max.z);
		}
		center.x = (min.x + max.x) / 2;
		center.y = (min.y + max.y) / 2;
		center.z = (min.z + max.z) / 2;
	}

	public int GetCount ()
	{
		return rawBits.Count;
	}

	public int GetPartCount ()
	{
		int partCount = (int)Math.Ceiling((float)GetCount() / 1000);
		return partCount;
	}

	public List<RawBit> GetRawBits ()//offsets for placement location
	{
		return rawBits;
	}

	public List<RawBit> GetRawBits (int partNumber, bool cursor, bool selected)
	{
		List<RawBit> returnRawBits = new List<RawBit>();
		for (int i = 0; i < 1000; i++)
		{
			int rIndex = i + (1000 * partNumber);
			if (rIndex < rawBits.Count)
			{
				if (rawBits[rIndex].Cursor == cursor && rawBits[rIndex].Selected == selected)
				{
					returnRawBits.Add(rawBits[rIndex]);
				}
			}
		}
		return returnRawBits;
	}
	public List<RawBit> GetRawBits (bool cursor, bool selected)
	{
		List<RawBit> returnRawBits = new List<RawBit>();
		foreach (RawBit rb in rawBits)
		{
			if (rb.Cursor == cursor && rb.Selected == selected)
			{
				returnRawBits.Add(rb);
			}

		}
		return returnRawBits;
	}


	public List<RawBit> GetRawBits (Point offset)//offsets for placement location
	{
		List<RawBit> rbs = new List<RawBit>();
		foreach (RawBit rb in rawBits)
		{

			Point offsetPoint = new Point(rb.point, offset);

			rbs.Add(new RawBit(offsetPoint, rb.type, rb.value));
		}
		return rbs;
	}


	internal void DeleteNotConnectedToCenter ()
	{

		rawBits.RemoveAll(rb => rb.ConnectedToCenter == false);
	}


	internal void DeleteSelected ()
	{

		int bitsDeleted = rawBits.RemoveAll(rb => rb.Selected == true && !(rb.point.Equals(new Point(0, 0, 0))));
		if (bitsDeleted > 0)
		{
			RemoveOrphans();
		}

	}

	public void rotateGroup (int _X, int _Y, int _Z)
	{
		foreach (RawBit rb in rawBits)
		{
			Point p = rb.point;
			if (_Z == 1)
			{
				rb.point.x = -p.y;
				rb.point.y = p.x;
			}
			if (_Z == -1)
			{
				rb.point.x = p.y;
				rb.point.y = -p.x;
			}
			if (_X == 1)
			{
				rb.point.z = -p.y;
				rb.point.y = p.z;
			}
			if (_X == -1)
			{
				rb.point.z= p.y;
				rb.point.y = -p.z;
			}
			if (_Y == 1)
			{
				rb.point.z = -p.x;
				rb.point.x = p.z;
			}
			if (_Y == -1)
			{
				rb.point.z = p.x;
				rb.point.x = -p.z;
			}
		}
	}

	public void OptimizeGroup (bool Full)//needs full optimization
	{
	}
	public void ReCenter ()//sets bit zero as 0,0,0
	{
		if (rawBits.Count > 0)
		{
			Point Origin = rawBits[0].point;
			Point Offset = new Point(-Origin.x, -Origin.y, -Origin.z);
			List<RawBit> NewRbs = GetRawBits(Offset);
			rawBits = NewRbs;
		}
	}

	public void ChangeCenter (Point point)
	{

		if (rawBits.Count > 0)
		{
			RawBit currentCenter = rawBits[0];
			int _swapInedx = 0;
			for (int i = 0; i < rawBits.Count; i++)
			{
				if (rawBits[i].point.Equals(point))
				{
					_swapInedx = i;
				}
			}
			rawBits[0] = rawBits[_swapInedx];
			rawBits[_swapInedx] = currentCenter;

		}
		ReCenter();
	}


	internal void Clear ()
	{
		//hasChanges = true;
		rawBits.Clear();
	}

	private void RemoveOrphans ()
	{
		foreach (RawBit rb in rawBits)
		{
			rb.ConnectedToCenter = false;
		}
		SetConnected(rawBits[0]);//recursive
		DeleteNotConnectedToCenter();
	}

	private void SetConnected (RawBit rb)
	{
		if (!rb.ConnectedToCenter)
		{
			rb.ConnectedToCenter = true;
			for (int i = 0; i < 6; i++)
			{
				if (rb.Neighbors[i] != null)
				{
					SetConnected(rb.Neighbors[i]);
				}
			}
		}
	}

	/*private int GetRawbitIndexAtPoint (Point point)
	{
		for (int i = 0; i < rawBits.Count; i++)
		{
			if (rawBits[i].point.Equals(point))
			{
				return (rawBits[i].ConnectedToCenter) ? -1 : i;
			}
		}
		return -1;
	}*/

	public bool BitExists (Point point)
	{
		foreach (RawBit rb in rawBits)
		{
			if (rb.point.Equals(point))
			{
				return true;
			}
		}
		return false;
	}

	public void SetSelected (Point point, bool selected)
	{
		//	hasChanges = true;
		foreach (RawBit rb in rawBits)
		{
			if (rb.point.Equals(point))
			{
				rb.Selected = selected;
			}
		}
	}

	public void SetCursor (Point point, bool cursor)
	{
		//hasChanges = true;
		foreach (RawBit rb in rawBits)
		{
			if (rb.point.Equals(point) && !rb.Selected)
			{
				rb.Cursor = cursor;
			}
		}
	}

	public void ClearCursor ()
	{
		//hasChanges = true;
		foreach (RawBit rb in rawBits)
		{
			rb.Cursor = false;
		}
	}

	public void ClearSelected ()
	{
		//hasChanges = true;
		foreach (RawBit rb in rawBits)
		{
			rb.Selected = false;
		}
	}
	public void ChangeCursorToSelected ()
	{
		//hasChanges = true;
		foreach (RawBit rb in rawBits)
		{
			if (rb.Cursor == true)
			{
				rb.Selected = true;
				rb.Cursor = false;
			}
		}
	}

	public void SetLR (float LR)//noramalize number 0 to 1 mapped to 0-380 )wiggle room)
	{
		float lrNormalized = Mathf.Clamp01(LR);
		lrRotation = 380 * lrNormalized; 
	}
	public void SetUD (float UD)//noramalize number 0 to 1 mapped to -100-100 )wiggle room)
	{
		float udNormalized = (2 * Mathf.Clamp01(UD)) - 1;
		udRotation = 75 * udNormalized;
		 
	}
	public void SetZoom (float Z)
	{
		 zoom = Mathf.Clamp(Z, 1.4f, 100);
	}
	
} 

public class RawBit
{
	public Point point;
	public int type;
	public int value;
	public bool ConnectedToCenter;
	public bool Selected;
	public bool Cursor; 
	public RawBit[] Neighbors;
	public RawBit (Point Location, int _type, int _value)
	{
		point = Location;
		type = _type;
		value = _value;
		ConnectedToCenter = true;
		Selected = false;
		Cursor = false;
		Neighbors = new RawBit[6];
	}
	
}


public class bit
{
	public bit[] Neighbors;
	public int Type;
	public int Value;
	public int[] ChargeIn;
	public int NextOut;

	public int X;
	public int Y;
	public int Z;
	//public bool[] SidesActive;//

	public Side[] Sides;
	public int VertCount;
	public int TrisCount;
	//public int BlockSection;

	public bit (int _x, int _y, int _z, int _type, int _value)
	{
		Type = _type;
		Value = _value;
		Vector3[] Corners;
		X = Mathf.Clamp(_x, -5000, 5000);
		Y = Mathf.Clamp(_y, -5000, 5000);
		Z = Mathf.Clamp(_z, -5000, 5000);
		Corners = new Vector3[8];
		Sides = new Side[6];
		Neighbors = new bit[6];
		VertCount = 0;
		TrisCount = 0;
		Corners[0] = new Vector3(X + 0, Y + 0, Z + 0);
		Corners[1] = new Vector3(X + 1, Y + 0, Z + 0);
		Corners[2] = new Vector3(X + 1, Y + 0, Z + 1);
		Corners[3] = new Vector3(X + 0, Y + 0, Z + 1);
		Corners[4] = new Vector3(X + 0, Y + 1, Z + 0);
		Corners[5] = new Vector3(X + 1, Y + 1, Z + 0);
		Corners[6] = new Vector3(X + 1, Y + 1, Z + 1);
		Corners[7] = new Vector3(X + 0, Y + 1, Z + 1);
		for (int i = 0; i < 6; i++)//remove in prod
		{
			Sides[i] = new Side();
		}
		Sides[0].verts[0] = Corners[4];
		Sides[0].verts[1] = Corners[5];
		Sides[0].verts[2] = Corners[1];
		Sides[0].verts[3] = Corners[0];

		Sides[1].verts[0] = Corners[6];
		Sides[1].verts[1] = Corners[7];
		Sides[1].verts[2] = Corners[3];
		Sides[1].verts[3] = Corners[2];

		Sides[2].verts[0] = Corners[7];
		Sides[2].verts[1] = Corners[4];
		Sides[2].verts[2] = Corners[0];
		Sides[2].verts[3] = Corners[3];

		Sides[3].verts[0] = Corners[5];
		Sides[3].verts[1] = Corners[6];
		Sides[3].verts[2] = Corners[2];
		Sides[3].verts[3] = Corners[1];

		Sides[4].verts[0] = Corners[2];
		Sides[4].verts[1] = Corners[3];
		Sides[4].verts[2] = Corners[0];
		Sides[4].verts[3] = Corners[1];


		Sides[5].verts[0] = Corners[7];
		Sides[5].verts[1] = Corners[6];
		Sides[5].verts[2] = Corners[5];
		Sides[5].verts[3] = Corners[4];

		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				Sides[i].VertIndex[j] = VertCount;
				VertCount++;
			}
			Sides[i].tris[0] = Sides[i].VertIndex[0];
			Sides[i].tris[1] = Sides[i].VertIndex[1];
			Sides[i].tris[2] = Sides[i].VertIndex[3];
			Sides[i].tris[3] = Sides[i].VertIndex[1];
			Sides[i].tris[4] = Sides[i].VertIndex[2];
			Sides[i].tris[5] = Sides[i].VertIndex[3];
			TrisCount += 6;
		}
	}
	public bit (RawBit rb) : this(rb.point.x, rb.point.y, rb.point.z, rb.type, rb.value)
	{

	}
}

public class Side
{
	public Vector3[] verts;
	public int[] tris;
	public int[] VertIndex;
	public Side ()
	{
		verts = new Vector3[4];
		tris = new int[6];
		VertIndex = new int[4];
	}
}

[Serializable]
public struct Point
{
	public int x;
	public int y;
	public int z;
	public Point (int _x, int _y, int _z)
	{
		x = _x;
		y = _y;
		z = _z;
	}
	public Point (Vector3 v3)
	{
		x = (int)v3.x;
		y = (int)v3.y;
		z = (int)v3.z;

	}
	public Point (Point p1, Point p2)//add 2 points
	{
		x = p1.x + p2.x;
		y = p1.y + p2.y;
		z = p1.z + p2.z;

	}
	public bool Equals (Point other)
	{

		if (other.x == x && other.y == y && other.z == z)
		{
			return true;
		}
		return false;
	}

	internal Vector3 ToVector ()
	{
		return new Vector3(x, y, z);
	}
	//could add rotate, move, get distance from 2nd point etc
}

public struct IntVector2
{
	public int x; public int y;
}
 
