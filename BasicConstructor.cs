using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicConstructor : MonoBehaviour { 
	public Material mat;

	public List<BlockSectionScript> BlockSections = new List<BlockSectionScript>();

	public List<bit> DisplayBits = new List<bit>();//ARRAY BROKEN DOWN BY SECTION CAN BE SUBDIVIDED

	public int CurrentBlockSections = 0;

	public bool _Rebuild;
	public Group currentGroup;
	void Start ()
	{
		// currentGroup = GameSaveScript.LoadTestGroup();

	}

	public void SetAllColliders (bool _enabled)
	{
		foreach (BlockSectionScript blockSection in BlockSections)
		{
			blockSection.SetCollider(_enabled);
		}
	}


	void CreateBlockSections (int Section)
	{
		for (int i = BlockSections.Count; i < Section; i++)
		{
			BlockSections.Add(BuildBlockSection(i));
		}
	}

	void Update ()
	{
		ReBuildBlox();
	}
	
	private BlockSectionScript BuildBlockSection (int Section_Index)
	{
		Material _mat = mat;
		BlockSectionScript bSEction = new BlockSectionScript();
		BlockConstructor BC = this.gameObject.GetComponent<BlockConstructor>();

		bSEction.BlockSectionStart(Section_Index, BC, _mat, false);
		bSEction.GO.transform.SetParent(this.transform);
		bSEction.GO.transform.position = this.transform.position;

		bSEction.GO.layer = this.gameObject.layer;


		return bSEction;
	}

	// Update is called once per frame


	private List<bit> rawBits2Bits (List<RawBit> rbs)//also add part # ?
	{
		List<bit> newBits = new List<bit>();

		foreach (RawBit rb in rbs)
		{
			newBits.Add(new bit(rb));//.point.x, rb.point.y, rb.point.z, rb.type, rb.value
		}
		return newBits;
	}

	private void ReBuildBlox () //Change to use only CURRENTGROUP - USE FUNCTIONS IN GROUPS/PARTS TO SORT
								//CREATE BLOCKS OF 1000 BITS BY PART AND NAME THEM PART_1.1 PART 1.2 ETC
	{
		int PartCount = currentGroup.GetPartCount(); 
		currentGroup.SetCheckSums();
		CreateBlockSections(PartCount + 1);

		for (int part = 0; part < PartCount; part++)
		{
			int checkSum = currentGroup.checkSums[part];
			if (BlockSections[part].checkSum != checkSum||_Rebuild)//SET TO COMPARE CHECKSUM
			{
				//Debug.Log(checkSum + " rebuilding part " + part + " cursor object");
				BlockSections[part].checkSum = checkSum;
				DisplayBits.Clear();
				var _bits = rawBits2Bits(currentGroup.GetRawBits());
				foreach (var B in _bits)
				{//this is where we divide visibles 					 
					DisplayBits.Add(B);
				}
				BuildSections(part);
			}
		}
		_Rebuild = false;
	}

	private void BuildSections (int sectionNumber)
	{
		BlockSections[sectionNumber].ResetSection();
		if (DisplayBits.Count > 0)
		{
			BlockSections[sectionNumber].SetBits(DisplayBits);
			BlockSections[sectionNumber].Active = true;
		}
		DisplayBits.Clear();
	}


	public void Dispose ()
	{
		DisplayBits.Clear();
	}


}
