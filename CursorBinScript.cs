using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorBinScript : MonoBehaviour
{
	Common.CursorBinState CursorBinState = Common.CursorBinState.OnScreen;
	public Vector2 OnScreen;
	public Vector2 OffScreen;
	Vector2 ScreenTarget;
	public RectTransform rtCursorBin;
	public float rTspeed;

	public RawImage CursorBinRawImage;
	RenderTexture CursorBinRenderTexture;
	public CanvasRenderer CursorBinRenderer;
	public RectTransform EventTriggerArea;
	public bool MouseOver = false;
	public BasicConstructor CursorConstructor;
     

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
	}

	private void CheckState ()
	{
		if (CursorBinState == Common.CursorBinState.OnScreen)
		{
			ScreenTarget = OnScreen;
		}
		if (CursorBinState == Common.CursorBinState.OffScreen)
		{
			ScreenTarget = OffScreen;
		}
	}

	public void SetOffscreen ()
	{
		CursorBinState = Common.CursorBinState.OffScreen;
	}

	public void SetOnscreen ()
	{
		CursorBinState = Common.CursorBinState.OnScreen;
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
				Pos.x -= Size.x / 2;
				Pos.y += Size.y / 2;
				CursorConstructor.currentGroup.SetUD(-(mPos.y - Pos.y) / Size.y);
				CursorConstructor.currentGroup.SetLR((mPos.x - Pos.x) / Size.x);
			}
			float zoomChange = Input.GetAxis("Mouse ScrollWheel");
			CursorConstructor.currentGroup.zoom = Mathf.Clamp(CursorConstructor.currentGroup.zoom * (1 + -zoomChange), 1.4f, 100);		}
	}

	public void BinZoom ()
	{


	}

}
