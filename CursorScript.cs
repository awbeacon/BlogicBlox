using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorScript : MonoBehaviour
{

	public Camera MainCamera;
	public Camera CursorCamera;
	public GameManagerScript GM;

	//Group cursorGroup;
	Vector3 OffScreen = new Vector3(10000, 10000, 10000);
	float CursorTimer = .5f;
	public BlockConstructor _bConstructor;

	public BasicConstructor Cursor;
	public Transform CursorPosition;
	public GroupConstructorScript CursorSelect;
	public Transform CameraRotationTransform;
	public Transform CameraTransform;

	public RectTransform CursorDrag;
	public Transform CursorBinTransform;
	public Transform BitBinsTransform; //can move this on/off screen  0 >> -100


	// Use this for initialization
	void Start ()
	{
		//Cursor.currentGroup = GameSaveScript.LoadTestGroup();
		Cursor.currentGroup = GameSaveScript.LoadGroup("BitBin01");
	}

	// Update is called once per frame
	void Update ()
	{
		NonMouseClickStuff();
		CheckMouseClick();
		HideCursor();
		SetTransform();
	}

	public void SetTransform ()
	{
		if (Cursor.currentGroup != null)
		{
			//Vector3 currentAngles = CameraRotationTransform.localEulerAngles;
			Vector3 targetAngles = new Vector3(Cursor.currentGroup.udRotation, Cursor.currentGroup.lrRotation, 0);
			CameraRotationTransform.localEulerAngles = targetAngles;

			Cursor.currentGroup.SetMinMax();
			Vector3 C = Cursor.currentGroup.center;
			CameraRotationTransform.localPosition = C;
			CameraTransform.localPosition = new Vector3(0, 0, 0 - (Cursor.currentGroup.zoom));
		}
	}

	private void NonMouseClickStuff ()
	{
		Common.GameState _state = GM._UI.gameState;
		bool OverUI = GM._EventSystem.IsPointerOverGameObject();
		//EXCAPE & EXIT - THIS SHOULD DEPEND SOMEWHAT ON THE CURRENT STATE, STARTING WITH CLEAR CURSOR SELECED
		if (GM._UI.GetCmdEsc())
		{
			_bConstructor.currentGroup.ClearCursor();
			_bConstructor.currentGroup.ClearSelected();
			_bConstructor._Rebuild = true;
		}
		if (GM._UI.GetCmdCopy())
		{
			Debug.Log("copy current group stuff");
			Cursor.currentGroup = new Group(_bConstructor.currentGroup.GetRawBits(false, true));
			Cursor.currentGroup.SetDefaultViews();
			Cursor.currentGroup.ClearCursor();
			Cursor.currentGroup.ClearSelected();
			_bConstructor.currentGroup.ClearCursor();
			_bConstructor.currentGroup.ClearSelected();
			Cursor.currentGroup.ReCenter();
			_bConstructor._Rebuild = true;
			if (Cursor.currentGroup.GetCount() > 0)
			{
				GM._UI.gameState = Common.GameState.Place;
			}
		}

		//********* cut ************
		if (GM._UI.GetCmdCut())
		{
			Debug.Log("cut current group stuff");
			Cursor.currentGroup = new Group(_bConstructor.currentGroup.GetRawBits(false, true));
			Cursor.currentGroup.SetDefaultViews();
			if (Cursor.currentGroup.GetCount() > 0)
			{
				GM._UI.gameState = Common.GameState.Place;
				_state = Common.GameState.Place;
				_bConstructor.currentGroup.DeleteSelected();
			}
			Cursor.currentGroup.ClearCursor();
			Cursor.currentGroup.ClearSelected();
			Cursor.currentGroup.ReCenter();
			Cursor._Rebuild = true;
		}
		//*********** DRAG *************
		if ((_state == Common.GameState.Drag))
		{
			float Start = CursorBinTransform.position.y;
			float End = BitBinsTransform.position.y;
			float Current = Input.mousePosition.y;

			float Size = Mathf.Clamp((((Current - 20) / Start) * 200), 80, 512);

			Vector2 TargetSize = new Vector2(Size, Size);
			CursorDrag.sizeDelta = TargetSize;
			Vector3 SizeDelta = CursorDrag.sizeDelta;

			SizeDelta.x /= 2;
			SizeDelta.y /= 2; 
			if (Input.GetMouseButton(0))
			{
				CursorDrag.position = Input.mousePosition - SizeDelta;				 
			}
			else
			{
				 
					Group g = new Group(Cursor.currentGroup); 
					 

					PointerEventData pointerData = new PointerEventData(EventSystem.current)
					{
						position = Input.mousePosition
					};

					List<RaycastResult> results = new List<RaycastResult>();
					EventSystem.current.RaycastAll(pointerData, results);

					results.ForEach((result) => {
						if (result.gameObject.name=="EventTriggerCenter")
						{
							BitBinScript bbs = result.gameObject.GetComponentInParent<BitBinScript>();
							if (bbs)
							{
								bbs.DragDrop(new Group(Cursor.currentGroup));
							}
						}
					});
				




				//if (!OverUI)
				{
					GM._UI.SetGameState_Previous();
				}
				CursorDrag.position = new Vector2(-1000, -1000);
			}
		}
		else
		{
			CursorDrag.position = new Vector2(-1000, -1000);
		}


	}

	private void HideCursor ()
	{
		if (CursorTimer < 0)
		{
			CursorOffscreen();
		}
		else
		{
			CursorTimer -= Time.deltaTime;
		}
	}

	private void CheckMouseClick ()
	{
		RaycastHit hit;
		Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform.tag == "Blox")
			{
				bool OverUI = GM._EventSystem.IsPointerOverGameObject();
				CursorTimer = OverUI ? -1 : .5f;
				//GET ORIGIN & DESINATION
				Vector3 rawHitPoint = hit.point;
				Vector3 rawCameraPoint = MainCamera.transform.position;

				Point Destination = Common.GetDesination(rawHitPoint, rawCameraPoint);
				Point Origin = Common.GetOrigin(rawHitPoint, rawCameraPoint);

				//DO STUFF BASED ON GAME STATE USING DESINATION & ORIGIN ******************
				Common.GameState _state = GM._UI.gameState;
				//	_bConstructor = hit.transform.gameObject.GetComponent<BlockConstructor>();

				//PLACE AND MOUSE DOWN
				if (Input.GetMouseButtonDown(0) && GM._UI.gameState == Common.GameState.Place && !OverUI)
				{
					_bConstructor.currentGroup.addGroup(Destination, Cursor.currentGroup);
				}

				if (_state == Common.GameState.Place && !OverUI)
				{
					CursorOnscreen(Destination);
				}
				//	Cursor.currentGroup = GameSaveScript.LoadTestGroup();

				//NOT SELECT AND NOT DELETE *************
				if ((_state != Common.GameState.Select && _state != Common.GameState.Delete))
				{
					_bConstructor.currentGroup.ClearCursor();
					_bConstructor.currentGroup.ClearSelected();
					//_bConstructor._Rebuild = true;
				}




				//***** DELETE ******
				if ((_state == Common.GameState.Delete) && !OverUI)
				{
					CursorOffscreen();

					_bConstructor.currentGroup.ClearCursor();
					_bConstructor.currentGroup.SetCursor(Origin, true);

					if (Input.GetMouseButtonDown(0))
					{
						_bConstructor.currentGroup.ClearSelected();
						_bConstructor.currentGroup.ChangeCursorToSelected();
						_bConstructor.currentGroup.DeleteNotConnectedToCenter();

					}
					_bConstructor.currentGroup.DeleteSelected();
				}

				//SELECT  ***********************
				if ((_state == Common.GameState.Select) && !OverUI)
				{
					CursorOffscreen();

					if (Input.GetMouseButtonDown(0))
					{
						if (!GM._UI.CtrlClick)
						{
							_bConstructor.currentGroup.ClearSelected();
						}
						_bConstructor.currentGroup.ChangeCursorToSelected();
						_bConstructor.currentGroup.ClearCursor();
					}
					else
					{
						_bConstructor.currentGroup.ClearCursor();
						_bConstructor.currentGroup.SetCursor(Origin, true);

					}
				}
				//SELECT  ***********************

			}
		}
	}

	private void CursorOnscreen (Point Destination)
	{
		Cursor.SetAllColliders(false);
		CursorPosition.position = Destination.ToVector();
	}

	private void CursorOffscreen ()//Gets called whenever cursor is offscreen
	{
		CursorPosition.position = OffScreen;
		Cursor.SetAllColliders(true);
		ForceCursorInView(100);
	}

	public void ForceCursorInView (int c)
	{
		for (int i = 0; i < c; i++)
		{
			//create 2 random edge rays and move camera if they hit something
			float Check1 = ((float)UnityEngine.Random.Range(0, 101)) / 100;
			float Check2 = UnityEngine.Random.Range(0, 2)==0?.04f:.96f;
			Vector2 PositionToCheck1 = new Vector2(Check1, Check2);
			Vector2 PositionToCheck2 = new Vector2(Check2, Check1);
			Ray ray1 = CursorCamera.ViewportPointToRay(PositionToCheck1);
			Ray ray2 = CursorCamera.ViewportPointToRay(PositionToCheck2);
			
			RaycastHit hit1;
			if (Physics.Raycast(ray1, out hit1))
			{
				if (hit1.distance < 100)
				{
					Cursor.currentGroup.SetZoom(Cursor.currentGroup.zoom * (1 + Time.deltaTime));
					i = c;
				}
			}
			RaycastHit hit2;
			if (Physics.Raycast(ray2, out hit2))
			{
				if (hit2.distance<100)
				{
					Cursor.currentGroup.SetZoom(Cursor.currentGroup.zoom * (1+Time.deltaTime));
					i = c;
				}
				
			}
		}
	}
}
