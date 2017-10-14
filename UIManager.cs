using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public GameManagerScript GM;

	public Common.GameMode gameMode;
	public Common.GameState gameState;
	private Common.GameState previousState;

	public bool AltClick = false;
	public bool ShiftClick = false;
	public bool CtrlClick = false;
	bool cmdCopy = false;
	bool cmdCut = false; 
	bool cmdEsc = false;

	public bool cameraModeRotate = true;
	public GameObject IconCameraRotate;
	public GameObject IconCameraMove;
	public GameObject IconSelect;
	public GameObject IconDelete;
	public GameObject IconPlace;

	public RectTransform rLogo;
	public RectTransform rSliderPointer;
	public iTween.EaseType MoveType;
	public GameObject BitBin;
	BitBinScript[] BitBins;
	float[] Widths;
    public int BinCount;
	public int CurrentBin = 0;
	public float TargetBinLocation = 0;  
	public float BinSpacing;
	public float LogoSize=600;
	private bool Hiding=false;

	public List<RectTransform> Icons;

	//TOOLBAR STUFF
	public float tbTextAreaSize = 200;
	public RectTransform tbTextArea;
	public RectTransform tbMiddle;
	public float iconOffset = 0;
	public float iconOffsetTarget;
	public float iconMinLocation;
	public float iconMaxLocation; 
	public float MaxIcons = 0;
	public int iconCount = 0;
    public List<RectTransform> Connectors;
	public List<RectTransform> ConnectorsLeft;
	public List<RectTransform> ConnectorsRight;

	  int HackTextureIndex = 99;
	// Use this for initialization
	void Start ()
	{
		gameMode = Common.GameMode.Workshop;
		 
		SetGameState_Place( ); 
		Invoke("HideLogo", 15);
		BitBins = new BitBinScript[BinCount];
		Widths = new float[BinCount];
		Transform BitBinParent = GameObject.Find("BitBins").transform;
		for (int i = 0; i < BinCount; i++)
		{
			GameObject GO = Instantiate(BitBin);
			GO.transform.SetParent(BitBinParent);
			BitBins[i] = GO.GetComponent<BitBinScript>();
			BitBins[i].StartUp();
			BitBins[i].GM = GM;
			BitBins[i].BinIndex = i;
			BitBins[i].SetGroup( GameSaveScript.LoadGroup("BitBin" + i.ToString("00")));
			Texture2D T = GameSaveScript.LoadTexture("BitBin" + i.ToString("00"));

			if (T!=null)
			{
				BitBins[i].SetTexture(T);
			}		
		}
	}

	

	// Update is called once per frame
	void Update ()
	{
		GetInputs();
		SetLogo();
		SetBinLocations();
		SetPointerLocation();
		SetToolbarTextAreaSize();
		SetIconLocations();
		SetConnectors();
		SetOnIcons();
    }

	private void SetOnIcons ()
	{
		IconCameraMove.SetActive(!GetCameraModeRotate());
		IconCameraRotate.SetActive(GetCameraModeRotate());
		IconSelect.SetActive(gameState == Common.GameState.Select);
		IconDelete.SetActive(gameState == Common.GameState.Delete);
		IconPlace.SetActive(gameState == Common.GameState.Place);
	}

	private void SetConnectors ()
	{
		for (int i = 0; i < Connectors.Count; i++)
		{	float Connection1 = ConnectorsLeft[i].anchoredPosition.x;
			float Connection2 = ConnectorsRight[i].anchoredPosition.x;
			Connectors[i].sizeDelta = new Vector2((Connection2-Connection1), 40);

			Connectors[i].anchoredPosition = new Vector2(Connection1 + 20, 0);

		}
	}

	private void SetIconLocations ()
	{
		float MaxTarget = Mathf.Clamp(iconCount - (MaxIcons), 0, iconCount);
		iconOffsetTarget = Mathf.Clamp(iconOffsetTarget  , 0, MaxTarget);
		iconMinLocation = tbTextAreaSize+30;
		iconMaxLocation = Screen.width - 64;
		float AvailableSpace = iconMaxLocation - iconMinLocation;
		float diff =Mathf.Clamp( (iconMaxLocation - (iconMinLocation +   (Icons.Count * 44))) / (Icons.Count + 2),8,30);
        
		
		  iconCount = Icons.Count;
		 
		float Spacing = 44+diff;
		  MaxIcons =Mathf.Floor( AvailableSpace  / Spacing);
		float Location = iconMinLocation - (iconOffset*Spacing);
		foreach (RectTransform _icon in Icons)
		{
			_icon.sizeDelta = (Location == (iconMaxLocation))? new Vector2(0, 0): new Vector2(44, 40);

			_icon.anchoredPosition = new Vector2(Location, 0);
			Location  = Mathf.Clamp(Location+ Spacing,0, iconMaxLocation);			
		}
		SetIconTarget();
	}
	public void ChangeIconOffset (float Change)
	{
		iconOffsetTarget =  iconOffsetTarget + Change ;
	}

	private void SetIconTarget ()
	{		 
			iconOffset += (iconOffsetTarget - iconOffset) * Time.deltaTime * 5;		 
	}

	public void ChangeToolbarTextAreaSize ( )
	{

		float _size =25+ Mathf.Clamp(Input.mousePosition.x, 100, 1500); 
		 
		tbTextAreaSize = _size;
	}

	private void SetToolbarTextAreaSize ()
	{
		tbTextArea.sizeDelta = new Vector2( tbTextAreaSize,50);
		tbMiddle.anchoredPosition = new Vector2(tbTextAreaSize+20, 0);
		//tbMiddle.right = new Vector3(10,20,40);
		tbMiddle.sizeDelta = new Vector2(-(tbTextAreaSize+46)	, 40);
	}

	private void SetPointerLocation ()
	{ 	float W1 = 73;
			float W2 = Screen.width - 73;
			float _range = W2 - W1;
			float location = W1 + (_range * (TargetBinLocation / BinCount));
			rSliderPointer.anchoredPosition = new Vector2(location, 14.5f);
	 
	}

	private void SetLogo ()
	{
		rLogo.sizeDelta = new Vector2(LogoSize, LogoSize / 2);
    }
	public void IncrementBin (int I)
	{
		CurrentBin = Mathf.Clamp(CurrentBin+I, 0, BinCount  );
		SetTarget();
	}
	public void SetBinByMouse ()
	{
		float _mouse = Input.mousePosition.x;
		float W1 = 73;
		float W2 = Screen.width - 73;
		float _range = W2 - W1;
		float location = W1 + (_range * (TargetBinLocation / BinCount));
		float _newBin =Mathf.RoundToInt( (_mouse - W1) / (_range / BinCount));
		CurrentBin = (int)Mathf.Clamp(_newBin , 0, BinCount  );
		SetTarget();
	}
	public void MovePointerByMouse ()
	{
		  float _mouse = Input.mousePosition.x;
		float W1 = 73;
		float W2 = Screen.width - 73;
		float _range = W2 - W1;
		float location = W1 + (_range * (TargetBinLocation / BinCount));
		float _newBin = ((_mouse - W1) / (_range / BinCount));
		TargetBinLocation =  Mathf.Clamp(_newBin, 0, BinCount);
		  CurrentBin=Mathf.RoundToInt(TargetBinLocation);		 
    }
	 
	public void SetCurrentBin (int C)
	{
		CurrentBin =Mathf.Clamp( C,0, BinCount-1);
		SetTarget();
	}
	
	private void SetBinLocations ()
	{  float TotalWidth = 0;
		float MidPoint = 0;
		for (int i = 0; i < BinCount; i++)
		{
			Widths[i]  = TotalWidth+(BinSpacing/2);
			float ThisWidth = BitBins[i].rT.sizeDelta.x + (BinSpacing / 2);
			float midpointfactor=1;
			float TargetMidpoint = TargetBinLocation-.5f;
			float T1 = Mathf.Floor(TargetMidpoint);
			float T2 = Mathf.Ceil(TargetMidpoint); 
			 
			if (i > T2)
			{
				midpointfactor = 0;
			}
			if (i>T1&&i<=T2)
			{
				midpointfactor = TargetMidpoint - T1;
            }
				MidPoint +=  ThisWidth *midpointfactor;
			 
			TotalWidth += ThisWidth;
		}

		for (int i = 0; i < BinCount; i++)
		{
			float ThisWidth =  ( BitBins[i].rT.sizeDelta.x)  ;
            BitBins[i].SetPosition(new Vector2((Widths[i] + ThisWidth + BinSpacing) - MidPoint , 24));
		}
		
	}

	private void SetTarget ( )
	{ float T = 1 + (Mathf.Abs(TargetBinLocation - (float)CurrentBin) / 100);
		 iTween.ValueTo(gameObject, iTween.Hash(
		 "from", TargetBinLocation,
		 "to", (float)CurrentBin,
		 "time", T,
		 "onupdatetarget", gameObject,
		 "onupdate", "tweenOnUpdateCallBack",
		 "easetype", MoveType
		 ));
	}
	void tweenOnUpdateCallBack (float newValue)
	{
		TargetBinLocation = newValue;		 
	}
	 
	 
	public void HideLogo ( )
	{
		if (!Hiding)
		{
			Hiding = true;
			iTween.ValueTo(gameObject, iTween.Hash(
		   "from", LogoSize,
		   "to", 0,
		   "time", 2f,
		   "onupdatetarget", gameObject,
		   "onupdate", "tweenOnUpdateCallBack2",
		   "easetype",  "easeInOutBounce"
		   ));
		}
	}
	void tweenOnUpdateCallBack2 (float newValue)
	{
		LogoSize = newValue;		 
	}
	public void ToggleCameraModeRotate (bool R)
	{
		cameraModeRotate = R;
	}
	public void ToggleCameraModeRotate ( )
	{
		cameraModeRotate = !cameraModeRotate;
	}
	public bool GetCameraModeRotate ()
	{
		if (AltClick)
		{
			return !cameraModeRotate;
		}
		return cameraModeRotate;
	}

	//set states - clunky but clean looking workaround for lack of enum support in button events...

	public void SetGameState_Delete ()
	{
		Common.GameState SetState = Common.GameState.Delete;
		previousState = (gameState == SetState) ? previousState : gameState;
		gameState = SetState;
	}
	public void SetGameState_Info ()
	{
		Common.GameState SetState = Common.GameState.Info;
		previousState = (gameState == SetState) ? previousState : gameState;
		gameState = SetState;
	}
	public void SetGameState_Open ()
	{
		Common.GameState SetState = Common.GameState.Open;
		previousState = (gameState == SetState) ? previousState : gameState;
		gameState = SetState;
	}
	public void SetGameState_Place ()
	{
		Common.GameState SetState = Common.GameState.Place;
		previousState = (gameState == SetState) ? previousState : gameState;
		gameState = SetState;
	}
	public void SetGameState_Play ()
	{
		Common.GameState SetState = Common.GameState.Play;
		previousState = (gameState == SetState) ? previousState : gameState;
		gameState = SetState;
	}
	public void SetGameState_Save ()
	{
		Common.GameState SetState = Common.GameState.View;
		previousState = (gameState == SetState) ? previousState : gameState;
		gameState = SetState;
	}
	public void SetGameState_Select ()
	{
		Common.GameState SetState = Common.GameState.Select;
		previousState = (gameState == SetState) ? previousState : gameState;
		gameState = SetState;
	}
	public void SetGameState_View ()
	{ 
		Common.GameState SetState = Common.GameState.View;
		previousState = (gameState == SetState) ? previousState : gameState;
		gameState = SetState;
	}
	public void SetGameState_Drag ()
	{
		Common.GameState SetState = Common.GameState.Drag;
        previousState = (gameState== SetState) ? previousState:gameState;
		gameState = SetState;  
	}
	public void SetGameState_Previous ()
	{ gameState = previousState;
		 
	}

	public bool GetCmdCopy ()
	{
		bool r = cmdCopy;
		cmdCopy = false;
		return r; 
	}
	public bool GetCmdCut ()
	{
		bool r = cmdCut;
		cmdCut = false;
		return r; 
	}
	public bool GetCmdEsc ()
	{
		bool r = cmdEsc;
		cmdEsc = false;
		return r; 
	}

	public void GetInputs ()
	{

		if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
		{
			AltClick = true;
		}
		else
		{
			AltClick = false;
		}
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			ShiftClick = true;
		}
		else
		{
			ShiftClick = false;
		}
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
		{
			CtrlClick = true;
		}
		else
		{
			CtrlClick = false;
		}
		if (Input.GetKeyDown(KeyCode.Delete)  )
		{
			gameState = Common.GameState.Delete;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			cmdEsc = true;
			Debug.Log("esc");
		}
		if (Input.GetKeyDown(KeyCode.C)&& CtrlClick)
		{
			cmdCopy = true;
			Debug.Log("copy");

		} 
		if (Input.GetKeyDown(KeyCode.X) && CtrlClick)
		{
			cmdCut = true;
			Debug.Log("Cut");
		}
	}

}
