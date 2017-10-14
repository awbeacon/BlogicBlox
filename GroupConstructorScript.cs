using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupConstructorScript : MonoBehaviour
{

	public BlockConstructor blockConstructor;
	public Material mat;

	public List<BlockSectionScript> BlockSections = new List<BlockSectionScript>();

	public List<bit> DisplayBits = new List<bit>();//ARRAY BROKEN DOWN BY SECTION CAN BE SUBDIVIDED

	public int CurrentBlockSections = 0;

	//public bool _Rebuild;

	public bool cursor;
	public bool selected;


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

		bSEction.BlockSectionStart(Section_Index, BC, _mat, cursor);//if not cursor select then it has no collider
		bSEction.GO.transform.SetParent(this.transform);

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
		int PartCount = blockConstructor.currentGroup.GetPartCount();
		CreateBlockSections(PartCount + 1);
		for (int part = 0; part < PartCount; part++)
		{
			if (blockConstructor.currentGroup.checkSums.Count > part)
			{
				int checkSum = blockConstructor.currentGroup.checkSums[part];
				if (BlockSections[part].checkSum != checkSum)//SET TO COMPARE CHECKSUM
				{
					BlockSections[part].checkSum = checkSum;
					DisplayBits.Clear();

					var _bits = rawBits2Bits(blockConstructor.currentGroup.GetRawBits(part, cursor, selected));
					foreach (var B in _bits)
					{//this is where we divide visibles 					 
						DisplayBits.Add(B);
					}
					BuildSections(part);
				}

			}
		}
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
