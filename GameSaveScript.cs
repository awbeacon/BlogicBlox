using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class GameSaveScript   {

	//maybe save group & metadata separately so the metadata can be loaded with 
	//picture, details, name, and file location for displaying

	public static Group LoadTestGroup ()
	{
		Group newgroup = new Group();
		newgroup.AddBit(new RawBit(new Point(0, 0, 0), 1, 4));
		newgroup.AddBit(new RawBit(new Point(1, 0, 0), 0, 4));
		newgroup.AddBit(new RawBit(new Point(2, 0, 0), 2, 4));
		newgroup.AddBit(new RawBit(new Point(3, 0, 0), 3, 4));
		newgroup.AddBit(new RawBit(new Point(3, 1, 0), 0, 4));
		newgroup.AddBit(new RawBit(new Point(3, -1, 0), 0, 4));
		newgroup.AddBit(new RawBit(new Point(3, 0, 1), 0, 4));
		newgroup.AddBit(new RawBit(new Point(3, 0, -1), 0, 4)); 
		Group G = new Group();
		G.addGroup(new Point(0, 0, 0),newgroup);
		G.addGroup(new Point(0, 1, 0), newgroup);
		return G;
	}

	public static bool SaveGroup (Group G)
	{
		return SaveGroup(G, G.GetFileName());
	}

	public static void SaveTexture (String fileName, RenderTexture rt)
	{
		Texture2D texture2D = new Texture2D(rt.width,rt.height, TextureFormat.RGB24, false);
		RenderTexture.active = rt;
		texture2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
		string filePath = Application.streamingAssetsPath + "\\" + fileName + ".png";
		System.IO.File.WriteAllBytes(filePath, texture2D.EncodeToPNG());		
    } 

	public static bool SaveGroup (Group G, String fileName)
	{
		GroupSave GS = new GroupSave(G);
		try
		{
			string filePath = Application.streamingAssetsPath + "\\" + fileName;
			//Debug.Log("save: " + filePath);
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(filePath);
		 
			bf.Serialize(file, GS);
			file.Close();
		}
		catch (Exception e)
		{
			Debug.Log("save fail" + e);
			return false;
		}
		return true;
    }


	public static Group  LoadGroup (String FileName) // MOVE TO RESOURCE FOLDER IN PRODUCITON ********************
	{
		GroupSave GS;
        BinaryFormatter bf;
		FileStream file;
		string filePath = Application.streamingAssetsPath + "\\"+ FileName;
		//Debug.Log("load: " + filePath);
		if (File.Exists(filePath))
		{
			bf = new BinaryFormatter();
			try
			{
				file = File.Open(filePath, FileMode.Open);
				GS = (GroupSave)bf.Deserialize(file);
				file.Close();
			}
			catch (Exception e)
			{
				Debug.Log(e);
				return null;
			}
		}
		else
		{ Debug.Log("nope");
			return null;

		}		 
		return new Group(GS);
	}

	internal static Texture2D LoadTexture (string FileName)
	{
		Texture2D T = new Texture2D(512,512);
		string filePath = Application.streamingAssetsPath + "\\" + FileName + ".png"; 
			byte[] fileData;
		//Debug.Log(filePath);
		if (File.Exists(filePath))
			{			 
				fileData = File.ReadAllBytes(filePath);				 
				T.LoadImage(fileData); //..this will auto-resize the texture dimensions.
			}  
		return T;
	}
}

[Serializable]
public class GroupSave
{
	//public bool hasChanges = true;
	public List<RawBitSave> rawBitSaves;
	public String Name;
	public String Description;
	internal float udRotation;
	internal float lrRotation;
	internal float zoom;	 
	public GroupSave (Group G)
	{
		Name = G.Name;
		Description = G.Description;
		rawBitSaves = new List<RawBitSave>();
		udRotation = G.udRotation;
		lrRotation = G.lrRotation;
		zoom = G.zoom;
        foreach (RawBit rb in G.GetRawBits())
		{
			rawBitSaves.Add(new RawBitSave(rb));
		}
	}
}

[Serializable]
public class RawBitSave
{
	public Point point;
	public int type;
	public int value; 
	public RawBitSave(RawBit rb)
	{
		point = rb.point;
		type = rb.type;
		value = rb.value;  
	}
}



