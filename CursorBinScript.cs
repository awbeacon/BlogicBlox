using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorBinScript : MonoBehaviour
{
	public GameManagerScript GM;
	bool CursorBinVisible = false;
	public Vector2 OnScreen;
	public Vector2 OffScreen;
	Vector2 ScreenTarget;
	public RectTransform rtCursorBin;

	bool detailsCursorBinVisible = false;
	//Common.CursorBinDetailsState detailsState = Common.CursorBinDetailsState.Detail;
	public Vector2 detailsOnScreen;//relative to base
	public Vector2 detailsOffScreen;//relative to base
	Vector2 detailsScreenTarget;
	public RectTransform detailsrtCursorBin;


	public float rTspeed;

	public RawImage CursorBinRawImage;
	public Transform CursorPosition;
	//RenderTexture CursorBinRenderTexture;
	public CanvasRenderer CursorBinRenderer;
	public RectTransform EventTriggerArea;
	public bool MouseOver = false;
	public BasicConstructor CursorConstructor;

	//STUFF TO FIND BIT ON CURSOR TO RESET CENTER (OR GET DETAILS?)
	public Camera CursorCamera;

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		CheckState();
		SetPosition();
		SetTexture();
	}

	private void SetPosition ()
	{
		rtCursorBin.anchoredPosition = iTween.Vector2Update(rtCursorBin.anchoredPosition, ScreenTarget, rTspeed);
		detailsrtCursorBin.anchoredPosition = iTween.Vector2Update(detailsrtCursorBin.anchoredPosition, detailsScreenTarget, rTspeed);
	}

	private void CheckState ()
	{
		 
		if (CursorBinVisible)
		{
			ScreenTarget = OnScreen;
			detailsScreenTarget = (detailsCursorBinVisible) ?   detailsOnScreen : detailsOffScreen ;
		}
		else  
		{
			ScreenTarget = OffScreen;
			detailsScreenTarget = new Vector2(-300,0);
		}
	}
	public void SetOffscreen ()
	{
		CursorBinVisible = false;
	}
	public void SetOnscreen ()
	{
		CursorBinVisible = true; 
	}
	public void SetDetailsOffscreen ()
	{
		detailsCursorBinVisible = false;
	}
	public void SetDetailsOnscreen ()
	{
		detailsCursorBinVisible = true;
	}
	public void ClearStaging () //TODO *************** SAVE GROUP WITH ANY CHANGES TO CENTER, ROTATE, AND VIEW ANGLE/ZOOM
	{
		MouseOver = false;
		//GameSaveScript.SaveGroup(BinGroup, "BitBin" + BinIndex.ToString("00"));     //BinRawImage.texture as Texture2D);
		//GameSaveScript.SaveTexture("BitBin" + BinIndex.ToString("00"), BinRenderTexture);     //BinRawImage.texture as Texture2D);

	}

	public void SetStaging ()
	{
		MouseOver = true;
	}
	void SetTexture ()
	{
		if (MouseOver)
		{
			if (Input.GetMouseButton(1))
			{
				Vector2 mPos = Input.mousePosition;
				Vector2 Size = new Vector2(EventTriggerArea.rect.width, EventTriggerArea.rect.height);
				Vector2 Pos = EventTriggerArea.position;
			//	Pos.x -= Size.x;// / 2;
				Pos.y += Size.y;// / 2;
				CursorConstructor.currentGroup.SetUD(-(mPos.y - Pos.y) / Size.y);
				CursorConstructor.currentGroup.SetLR((mPos.x - Pos.x) / Size.x);
			}
			
			float zoomChange = Input.GetAxis("Mouse ScrollWheel");
		 	CursorConstructor.currentGroup.SetZoom(CursorConstructor.currentGroup.zoom * (1   -zoomChange));
			}
	}
	public void RotateGroupX (int _X)
	{
		CursorConstructor.currentGroup.rotateGroup(_X,0,0);
		CursorConstructor._Rebuild = true;
	}
	public void RotateGroupY (int _Y)
	{
		CursorConstructor.currentGroup.rotateGroup(0, _Y, 0);
		CursorConstructor._Rebuild = true;
	}
	public void RotateGroupZ (int _Z)
	{
		CursorConstructor.currentGroup.rotateGroup(0, 0, _Z);
		CursorConstructor._Rebuild = true;
	}

	public void SetDrag ()
	{
		GM._UI.SetGameState_Drag();
	}

	public void SelectCenter ()
	{
		if (GM._UI.gameState!=Common.GameState.Drag)
		{	 
			
			Vector2 mousePos = Input.mousePosition;
			Vector2 clickAreaPos = EventTriggerArea.position;
			Vector2 relativePos1 = mousePos - clickAreaPos;
			Vector2 relativePos2 = new Vector2(relativePos1.x/353, relativePos1.y/360);//i hate magic numbers (this is the parent minus the borders of the event trigger area)

			//RAYCAST FROM CURSOR CAMERA
			//Debug.Log(relativePos2); 
			Ray ray = CursorCamera.ViewportPointToRay(relativePos2);

			Debug.Log(relativePos2  );
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				Vector3 rawHitPoint = hit.point - CursorPosition.position;
				Vector3 rawCameraPoint = CursorCamera.transform.position - CursorPosition.position;

				Point origin = Common.GetOrigin(rawHitPoint, rawCameraPoint);
				CursorConstructor.currentGroup.ChangeCenter(origin);
				//SET GROUP CENTER
				CursorConstructor._Rebuild = true;
			}
			 
			GM._UI.SetGameState_Previous();
		}
		
	}

}
