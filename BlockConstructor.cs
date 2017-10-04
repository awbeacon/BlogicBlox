using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockConstructor : MonoBehaviour, IDisposable
{
	public GameManagerScript GM;
	public GameObject _ConnectorBase;

	public Group currentGroup;

	public List<BlockSectionScript> BlockSectionsVisible = new List<BlockSectionScript>();
	public List<BlockSectionScript> BlockSectionsTransparent = new List<BlockSectionScript>();

	public List<bit> DisplayBitsVisible = new List<bit>(); 
	public List<bit> DisplayBitsTransparent = new List<bit>(); 

	public int CurrentBlockSectionVisible = 0;
	public int CurrentBlockSectionTransparent = 0;
    public bool _Rebuild;


	// Use this for initialization
	void Start ()
	{
		ResetGroup();		 
	}

	void CreateBlockSections (int Section) //might need to dynamically delete these as well
	{
		int CurrentCount = BlockSectionsVisible.Count;
		if (CurrentCount<Section)
		{
			for (int i = CurrentCount; i < Section; i++)
			{
				BlockSectionsVisible.Add(BuildBlockSection(i, true));
				BlockSectionsTransparent.Add(BuildBlockSection(i, false));
			}
		} 
      
	}

	void Update ()
	{
		currentGroup.SetCheckSums();
		 
			ReBuildBlox()
    }

	
	public void ResetGroup ()
	{
		RawBit rb = new RawBit(new Point(0, 0, 0), 4, 0); //DEFAULT INSULATOR;		 
		currentGroup = new Group(rb);		 
	}

	public void ResetGroup (Group G)
	{
		currentGroup = G;		 
	}

	//CHANGE TO CHECK NEIGHBORS ON SUB LISTS OF PARTS (VISIBLE / NOT-VISIBLE)
	private void CheckNeighbors ()
	{
		foreach (bit b in DisplayBitsVisible)
		{
			CheckNeighborVisible(b);
		}
		foreach (bit b in DisplayBitsTransparent)
		{
			CheckNeighborTransparent(b);
		}
	}

	private bit GetBitVisible (int _x, int _y, int _z)
	{
		return DisplayBitsVisible.Find(b => (b.X == _x && b.Y == _y && b.Z == _z));
	}

	private void CheckNeighborVisible (bit b)
	{
		bit[] testbit = new bit[6];
		testbit[0] = GetBitVisible(b.X + 0, b.Y + 0, b.Z - 1);
		testbit[1] = GetBitVisible(b.X + 0, b.Y + 0, b.Z + 1);
		testbit[2] = GetBitVisible(b.X - 1, b.Y + 0, b.Z + 0);
		testbit[3] = GetBitVisible(b.X + 1, b.Y + 0, b.Z + 0);
		testbit[4] = GetBitVisible(b.X + 0, b.Y - 1, b.Z + 0);
		testbit[5] = GetBitVisible(b.X + 0, b.Y + 1, b.Z + 0);
		for (int i = 0; i < 6; i++)
		{
			if (testbit[i] != null)
			{
				b.Neighbors[i] = testbit[i];
				int iFlip = Flip(i);
				testbit[i].Neighbors[iFlip] = b;
			}
		}
	}
	
	private bit GetBitTransparent (int _x, int _y, int _z)
	{
		return DisplayBitsTransparent.Find(b => (b.X == _x && b.Y == _y && b.Z == _z));
	}
	
	private void CheckNeighborTransparent (bit b)
	{
		bit[] testbit = new bit[6];
		testbit[0] = GetBitTransparent(b.X + 0, b.Y + 0, b.Z - 1);
		testbit[1] = GetBitTransparent(b.X + 0, b.Y + 0, b.Z + 1);
		testbit[2] = GetBitTransparent(b.X - 1, b.Y + 0, b.Z + 0);
		testbit[3] = GetBitTransparent(b.X + 1, b.Y + 0, b.Z + 0);
		testbit[4] = GetBitTransparent(b.X + 0, b.Y - 1, b.Z + 0);
		testbit[5] = GetBitTransparent(b.X + 0, b.Y + 1, b.Z + 0);
		for (int i = 0; i < 6; i++)
		{
			if (testbit[i] != null)
			{
				b.Neighbors[i] = testbit[i];
				int iFlip = Flip(i);
				testbit[i].Neighbors[iFlip] = b;
			}
		}
	}

	public int Flip (int Side)
	{
		switch (Side)
		{
			case 0:
				return 1;

			case 1:
				return 0;

			case 2:
				return 3;

			case 3:
				return 2;

			case 4:
				return 5;

			case 5:
				return 4;

			default:
				return -1;
		}
	}

	private BlockSectionScript BuildBlockSection (int Section_Index, bool _visible)
	{
		Material _mat = (_visible) ? GM.BloxMat : GM.BloxMatTransparent;
		BlockSectionScript bSEction = new BlockSectionScript();
		BlockConstructor BC = this.gameObject.GetComponent<BlockConstructor>();

		bSEction.BlockSectionStart(Section_Index, BC, _mat, _visible);
		bSEction.GO.transform.SetParent(_ConnectorBase.transform);

		return bSEction;
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		TestChange();
		for (int i = 0; i < BlockSectionsVisible.Count; i++)
		{
			if (BlockSectionsVisible[i].Active)
			{
				BlockSectionsVisible[i].DisplayChanges();
			}
			if (BlockSectionsTransparent[i].Active)
			{
				BlockSectionsTransparent[i].DisplayChanges();
			}
		}
	}

	private void TestChange ()
	{
		//NOT AVAILABLE IN NEW RAWBIT>BIT WORKSHOP, PLAY MODE NEEDS TO BUILD STATIC LIST OF BITS AND BUILD BLOCKSECTIONS FROM THAT.
	}

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
		//currentGroup.OptimizeGroup();
		int PartCount = currentGroup.GetPartCount();
	
		CreateBlockSections(PartCount + 1);
		for (int part = 0; part < PartCount; part++)
		{
			int checkSum = currentGroup.checkSums[part];
			if (BlockSectionsVisible[part].checkSum!=checkSum||_Rebuild)//SET TO COMPARE CHECKSUM
			{ 
				BlockSectionsVisible[part].checkSum = checkSum;
				DisplayBitsVisible.Clear();
				DisplayBitsTransparent.Clear();

				var _bits = rawBits2Bits(currentGroup.GetRawBits(part,false,false));
				foreach (var B in _bits)
				{//this is where we divide visibles 
					if (IsVisible(B.X, B.Y, B.Z))
					{
						DisplayBitsVisible.Add(B);
					}
					else
					{
						DisplayBitsTransparent.Add(B);
					}
				}
				 	BuildSections(part); 
			}
		} 

		for (int clearSection = PartCount; clearSection < BlockSectionsVisible.Count; clearSection++)
		{
			DisplayBitsVisible.Clear();
			DisplayBitsTransparent.Clear();
			BuildSections(clearSection);
		}
		_Rebuild = false;
	}

	private void BuildSections (int sectionNumber)
	{
		CheckNeighbors();
		BlockSectionsVisible[sectionNumber].ResetSection();
		BlockSectionsTransparent[sectionNumber].ResetSection();
		if (DisplayBitsVisible.Count > 0)
		{
			BlockSectionsVisible[sectionNumber].SetBits(DisplayBitsVisible);
			BlockSectionsVisible[sectionNumber].Active = true;
		}
		if (DisplayBitsTransparent.Count > 0)
		{

			BlockSectionsTransparent[sectionNumber].SetBits(DisplayBitsTransparent);
			BlockSectionsTransparent[sectionNumber].Active = true;
		}
		DisplayBitsVisible.Clear();
		DisplayBitsTransparent.Clear();
	}

	public bool IsVisible (int _x, int _y, int _z)//visible inside boundries  
	{
		bool xTrue = (_x >= GM._bloxManager._minVisible.x) && (_x < GM._bloxManager._maxVisible.x);
		bool zTrue = (_z >= GM._bloxManager._minVisible.z) && (_z < GM._bloxManager._maxVisible.z);
		bool yTrue = (_y >= GM._bloxManager._minVisible.y) && (_y < GM._bloxManager._maxVisible.y);
		return (xTrue && yTrue && zTrue);
	}

	public bool IsVisible (float _x, float _y, float _z)//visible inside boundries  
	{
		return IsVisible((int)_x, (int)_y, (int)_z);
	}

	public void Dispose ()
	{
		DisplayBitsVisible.Clear();
		DisplayBitsTransparent.Clear();
	}

}