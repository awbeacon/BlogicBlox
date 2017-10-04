using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BitBinScript : MonoBehaviour
{
	public int changeValue = 0;
	float changeBitZeroTime = .5f;
	float changeBitZeroTimeLast = 0;
	public RawImage BinRawImage;
	RenderTexture BinRenderTexture;
	public CanvasRenderer BinRenderer;

	public RenderTexture StagingRenderTexture;

	string _text = "                    ";
	string _targetText = "                    ";
	//public string _newText = "Insulator";
	int _textIndex = 19;
	public Text TextArea;
	public Text TextValue;
	bool isBit = false;
	Vector2 CurrentLocation = new Vector2(0, 0);
	bool m_Grow = false;
	public RectTransform EventTriggerArea;
	public RectTransform rT;
	public RectTransform rTextArea;
	public RectTransform rClock;
	public RectTransform rClockHand;
	public RectTransform rSideThing;
	public RectTransform rValueArea;
	// RectTransform r;
	public Vector2 rTarget;
	public Vector2 rSmall;
	public Vector2 rBig;
	public Vector2 rTextTarget;
	public Vector2 rTextSmall;
	public Vector2 rTextBig;
	public Vector2 rTextPosTarget;
	public Vector2 rTextPosSmall;
	public Vector2 rTextPosBig;
	public Vector2 rValueTarget;
	public Vector2 rValueAreaSmall;
	public Vector2 rValueAreaBig;

	public Vector2 rClockTarget;
	public Vector2 rClockSmall;
	public Vector2 rClockBig;
	public float rTspeed;
	public float rClockSpeed;
	float HandRotation;
	public float HandRotationDirection;


	public Vector2 rSidePosTarget;
	public Vector2 rSidePosSmall;
	public Vector2 rSidePosBig;

	public Vector2 rSideTarget;
	public Vector2 rSideSmall;
	public Vector2 rSideBig;

	//CONTENTS BITS & PARTS  
	public GameManagerScript GM;
	public Group BinGroup;
	public int BinIndex;
	public bool MouseOver = false;
	// Use this for initialization
	public void StartUp ()
	{
		BinRenderTexture = new RenderTexture(StagingRenderTexture);
		//	BinRawImage.texture = BinRenderTexture;

		rSmall.x = 150;
		rBig.y = 250;
		rBig.x = 250;
		rTarget = rSmall;
		InvokeRepeating("ChangeHandDirection", 1, 2);
		InvokeRepeating("UpdateText", 1, .1f);//Typing speed
	}

	// Update is called once per frame
	void Update ()
	{
		ChangeBitZero();//if mousedown on bit change
		CheckState();
		ApplyState();
		ApplyPosition();
		SetText();
		BinZoom();
	}

	private void ApplyPosition ()
	{
		rT.anchoredPosition = CurrentLocation;
	}

	internal void SetTexture (Texture2D texture)//probably only works before canvas renderer is set?
	{
		BinRawImage.texture = texture;
	}

	public void SetPosition (Vector2 P)
	{
		CurrentLocation = P;
	}

	internal void SetGroup (Group group)
	{
		BinGroup = group;
		GM.StagingArea.SetGroup(BinGroup);
		SetRenderTexture();
	}

	public void SetRenderTexture ()
	{
		GM.StagingArea.StagingCamera.targetTexture = BinRenderTexture;
		BinRenderer.SetTexture(BinRenderTexture);
	}

	void ChangeHandDirection ()
	{
		HandRotationDirection = UnityEngine.Random.Range(-100, 100);
	}

	private void ApplyState ()
	{
		rT.sizeDelta = iTween.Vector2Update(rT.sizeDelta, rTarget, rTspeed);
		rTextArea.sizeDelta = rTextTarget;
		rTextArea.anchoredPosition = iTween.Vector2Update(rTextArea.anchoredPosition, rTextPosTarget, rTspeed);
		rValueArea.anchoredPosition = iTween.Vector2Update(rValueArea.anchoredPosition, rValueTarget, rTspeed);
		rClock.sizeDelta = iTween.Vector2Update(rClock.sizeDelta, rClockTarget, rClockSpeed);
		rClockHand.sizeDelta = rClock.sizeDelta;
		HandRotation = Time.deltaTime * HandRotationDirection;
		rClockHand.Rotate(new Vector3(0, 0, HandRotation));
		rSideThing.anchoredPosition = iTween.Vector2Update(rSideThing.anchoredPosition, rSidePosTarget, rTspeed);
		rSideThing.sizeDelta = iTween.Vector2Update(rSideThing.sizeDelta, rSideTarget, rTspeed);
	}

	private void CheckState ()
	{
		isBit = BinGroup.GetCount() == 1;
		rTarget = (m_Grow) ? rBig : rSmall;
		rTextTarget = (rT.sizeDelta.x > 240) ? rTextBig : rTextSmall;
		rTextPosTarget = (rT.sizeDelta.x > 240) ? rTextPosBig : rTextPosSmall;
		rValueTarget = (rT.sizeDelta.x > 240 && isBit) ? rValueAreaBig : rValueAreaSmall;
		float diff = Mathf.Abs(rTextArea.anchoredPosition.x - rTextPosBig.x) + Mathf.Abs(rTextArea.anchoredPosition.y - rTextPosBig.y);
		if (diff < 1 && m_Grow)
		{
			rClockTarget = rClockBig;
		}
		if (!m_Grow || diff > 1)
		{
			rClockTarget = rClockSmall;
			rClock.sizeDelta = rClockTarget;
		}
		rSidePosTarget.y = (rT.sizeDelta.y > 200) ? 17 : 60 - rT.sizeDelta.y;
		rSideTarget = (rT.sizeDelta.y > 200) ? rSideBig : rSideSmall;
	}

	public void Grow ()
	{
		m_Grow = true;
	}

	public void Shrink ()
	{
		m_Grow = false;
	}

	public void SetText ()
	{
		float diff = Mathf.Abs(rTextArea.anchoredPosition.x - rTextPosBig.x) + Mathf.Abs(rTextArea.anchoredPosition.y - rTextPosBig.y);
		if (diff < 1 && m_Grow)
		{
			_targetText = BinGroup.Name;
		}
		else
		{
			_text = "                    ";
			_targetText = "                    ";
		}
		TextArea.text = _text;
		if (isBit)
		{
			TextValue.text = BinGroup.GetBitZeroValue().ToString();
		}
	}

	public void SetChangeValue (int V)
	{
		changeValue = V;
	}

	public void ChangeBitZero ()
	{
		if (Input.GetMouseButton(0) && changeValue != 0)
		{
			if ((Time.time - changeBitZeroTimeLast) > changeBitZeroTime)
			{
				SetBitZero(changeValue);
				changeBitZeroTime = Mathf.Clamp(changeBitZeroTime - .05f, .05f, .5f);
				changeBitZeroTimeLast = Time.time;
			}
		}
		else
		{
			changeBitZeroTime = Mathf.Clamp(changeBitZeroTime + .05f, .05f, .5f);
		}
	}

	public void SetBitZero (int ADD)
	{
		BinGroup.SetBitZeroValue(ADD);
		SetRenderTexture();
	}

	public void UpdateText ()
	{
		_targetText = _targetText.PadRight(20, ' ');
		if (_text != _targetText)
		{
			_text = _targetText[_textIndex] + _text.Substring(0, 19);
			_textIndex--;
			if (_textIndex < 0)
			{
				_textIndex = 19;
			}
		}
	}

	public void SetCursor ()
	{
		GM._CursorConstructor.currentGroup = BinGroup;
	}
	public void ClearStaging ()
	{
		MouseOver = false;
		GameSaveScript.SaveGroup(BinGroup, "BitBin" + BinIndex.ToString("00"));     //BinRawImage.texture as Texture2D);
		GameSaveScript.SaveTexture("BitBin" + BinIndex.ToString("00"), BinRenderTexture);     //BinRawImage.texture as Texture2D);
		GM.StagingArea.StagingCamera.targetTexture = null;
	}
	public void SetStaging ()
	{
		MouseOver = true;
		if (Input.GetMouseButton(1))
		{
			Vector2 mPos = Input.mousePosition;
			Vector2 Size = new Vector2(EventTriggerArea.rect.width, EventTriggerArea.rect.height);
			Vector2 Pos = EventTriggerArea.position;
			Pos.x -= Size.x / 2;
			Pos.y += Size.y / 2;
			BinGroup.SetUD(-(mPos.y - Pos.y) / Size.y);
			BinGroup.SetLR((mPos.x - Pos.x) / Size.x);
		}
		SetRenderTexture();
		GM.StagingArea.SetGroup(BinGroup);
	}
	public void BinZoom ()
	{
		if (MouseOver)
		{
			float zoomChange = Input.GetAxis("Mouse ScrollWheel");
			BinGroup.zoom = Mathf.Clamp(BinGroup.zoom * (1 + -zoomChange), 1.4f, 100);
		}

	}
}
