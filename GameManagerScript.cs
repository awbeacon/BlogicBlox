using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

	public Material BloxMat;
	public Material BloxMatTransparent;
	public BasicConstructor _CursorConstructor;
	public UIManager _UI;
	public SelectScript _Select;
	public BloxManager _bloxManager;
	public BackgroundController _backgroundController;
	public UnityEngine.EventSystems.EventSystem _EventSystem;
	public StagingAreaScript StagingArea;

	public float CPSTimeCounter;
	public int CPS;//CYCLES PER SECOND
	int CPSCounter = 0;
	public float FPSTimeCounter;
	public int FPS;// FRAMES PER SECOND
	int FPSCounter = 0;

	// Use this for initialization
	void Start ()
	{
		// CreateAndSaveDefaultBitBins();
	}

	private static void CreateAndSaveDefaultBitBins ()
	{
		for (int i = 0; i < 100; i++)
		{
			RawBit B = new RawBit(new Point(0, 0, 0), i, 0);
			Group G = new Group(B);
			G.Name = BitTypes.GetName(i);
			GameSaveScript.SaveGroup(G, "BitBin" + i.ToString("00"));
		}
	}

	// Update is called once per frame
	void Update ()
	{
		CountFPS();
	}

	void FixedUpdate ()
	{
		CountCPS();
	}

	private void CountFPS ()//COUNT FRAMES PER SECOND FOR PERFORMANCE RELATED TASKS
	{
		FPSCounter++;
		if (FPSTimeCounter <= Time.time - 1)//****** THIS HAPPENS ONCE PER SECOND IN CASE SOMETHING NEEDS TO BE SCHEDULED TO HAPPEN (OR TESTED)
		{
			FPS = FPSCounter;
			FPSCounter = 0;
			FPSTimeCounter = Time.time;
			if (FPS > 20)
			{
				//TestAddOneBit();
			}
		}

	}

	private void CountCPS ()//COUNT CYCLES PER SECOND FOR PERFORMANCE RELATED TASKS
	{
		CPSCounter++;
		if (CPSTimeCounter <= Time.time - 1)//****** THIS HAPPENS ONCE PER SECOND IN CASE SOMETHING NEEDS TO BE SCHEDULED TO HAPPEN (OR TESTED)
		{
			CPS = CPSCounter;
			CPSCounter = 0;
			CPSTimeCounter = Time.time;
		}
	}
}
