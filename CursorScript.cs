using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
	public Camera MainCamera;
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

	// Use this for initialization
	void Start ()
	{
		 //Cursor.currentGroup = GameSaveScript.LoadTestGroup();
		 Cursor.currentGroup = GameSaveScript.LoadGroup("BitBin02");
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
			Vector3 targetAngles = new Vector3(Cursor.currentGroup.udRotation, Cursor.currentGroup.lrRotation, 0);
			CameraRotationTransform.localEulerAngles = targetAngles;
			Cursor.currentGroup.SetMinMax();
			Vector3 C = Cursor.currentGroup.center;
			CameraRotationTransform.localPosition = C;
			CameraTransform.localPosition = new Vector3(0,0,0-(Cursor.currentGroup.zoom));
		}
	}

	private void NonMouseClickStuff ()
	{
		Common.GameState _state = GM._UI.gameState;
		bool OverUI = GM._EventSystem.IsPointerOverGameObject(); 
		//EXCAPE & EXIT -  
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
				_bConstructor.currentGroup.DeleteSelected( );
			}
			Cursor.currentGroup.ClearCursor();
			Cursor.currentGroup.ClearSelected();
			Cursor.currentGroup.ReCenter();
			Cursor._Rebuild = true;
		}

	}

	private void HideCursor ()
	{
		if (CursorTimer < 0)
		{
			CursorPosition.position = OffScreen;
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

			//  Debug.Log(hit.transform.tag);
			if (hit.transform.tag == "Blox")
			{
				bool OverUI = GM._EventSystem.IsPointerOverGameObject(); 
				CursorTimer = OverUI?-1:.5f;
				//GET ORIGIN & DESINATION
				Vector3 HV = hit.point;
				//Point HP = new Point(hit.point);

				Vector3 hpFloor = new Vector3(Mathf.Floor(HV.x + .001f), Mathf.Floor(HV.y + .001f), Mathf.Floor(HV.z + .001f));//.001 cuz raycast sometimes registers at .999999999 instead of 1
				Vector3 cPos = MainCamera.transform.position;
				Vector3 Diff = HV - hpFloor;

				bool X_middle = (Diff.x > .01f && Diff.x < .99f);
				bool Y_middle = (Diff.y > .01f && Diff.y < .99f);
				bool Z_middle = (Diff.z > .01f && Diff.z < .99f);
				bool X_side = Y_middle && Z_middle;
				bool Y_side = !X_side && X_middle && Z_middle;
				bool Z_side = !Y_side && X_middle && Y_middle;
				bool isSide = (X_side || Y_side || Z_side);

				//DESTINATION ****************
				Vector3 OffSet = new Vector3(0, 0, 0);
				if (X_side && cPos.x < hpFloor.x)
				{
					OffSet.x = -1;
				}
				else if (Y_side && cPos.y < hpFloor.y)
				{
					OffSet.y = -1;
				}
				else if (Z_side && cPos.z < hpFloor.z)
				{
					OffSet.z = -1;
				}
				Point Destination = new Point(hpFloor + OffSet);

				//ORIGIN ************************
				OffSet = new Vector3(0, 0, 0);
				if (X_side && cPos.x > hpFloor.x)
				{
					OffSet.x = -1;
				}
				else if (Y_side && cPos.y > hpFloor.y)
				{
					OffSet.y = -1;
				}
				else if (Z_side && cPos.z > hpFloor.z)
				{
					OffSet.z = -1;
				}
				Point Origin = new Point(hpFloor + OffSet);

				//DO STUFF BASED ON GAME STATE USING DESINATION & ORIGIN ******************
				Common.GameState _state = GM._UI.gameState;
				//	_bConstructor = hit.transform.gameObject.GetComponent<BlockConstructor>();

				//PLACE AND MOUSE DOWN
				if (Input.GetMouseButtonDown(0) && GM._UI.gameState == Common.GameState.Place&&!OverUI)
				{ 
					_bConstructor.currentGroup.addGroup(Destination, Cursor.currentGroup); 
				}

				if (_state == Common.GameState.Place && !OverUI)
				{
						CursorPosition.position = Destination.ToVector();  
				} 

				//NOT SELECT AND NOT DELETE *************
				if ((_state != Common.GameState.Select && _state != Common.GameState.Delete))
				{
					_bConstructor.currentGroup.ClearCursor();
					_bConstructor.currentGroup.ClearSelected(); 
				}		
			 
				//***** DELETE ******
				if ((_state == Common.GameState.Delete) && !OverUI) 
				{
					CursorPosition.position = OffScreen;

					_bConstructor.currentGroup.ClearCursor();
					_bConstructor.currentGroup.SetCursor(Origin, true);

					if (Input.GetMouseButtonDown(0))
					{
						_bConstructor.currentGroup.ClearSelected();
						_bConstructor.currentGroup.ChangeCursorToSelected();
						_bConstructor.currentGroup.DeleteNotConnectedToCenter();

					}
					_bConstructor.currentGroup.DeleteSelected( );
				}

				//SELECT  ***********************
				if ((_state == Common.GameState.Select) && !OverUI)
				{
					CursorPosition.position = OffScreen;

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
			}

		}
	}


	 
}
