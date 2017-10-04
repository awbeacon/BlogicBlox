using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewDialogScript : MonoBehaviour
{
	string _text = "                    ";
	string _targetText = "                    ";
	string _textFull = "Visible Area        ";

	int _textIndex = 19;
	public Text TextArea;
	Vector2 CurrentLocation = new Vector2(0, 0);
	public Vector2 OnScreen;
	public Vector2 OffScreen;
	bool m_Grow = false;
	public RectTransform rT;
	public RectTransform rTextArea;
	public RectTransform rClock;
	public RectTransform rClockHand;
	public RectTransform rSideThing;
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

	public GameManagerScript GM;

	float ChangeWidth = 0;
	//CONTROLS FOR VISIBILITY

	public RectTransform[] Pointers = new RectTransform[7];//0-1:x 2-3:y 4-5:z 6:transparent
	public RectTransform[] Bars = new RectTransform[4];//0:x 1:y 2:z 3:transparent
	float m_Transparency = 0;
	Vector3 m_Mins;
	Vector3 m_Maxs;
	bool _rebuild = false;
	// Use this for initialization
	void Start ()
	{
		SetMinMax();
		rBig.x = Screen.width / 2;
		rTarget = rSmall;
		InvokeRepeating("ChangeHandDirection", 1, 2);
		InvokeRepeating("UpdateText", 1, .1f);//Typing speed	 
	}

	private void SetMinMax ()
	{
		m_Mins = GM._bloxManager._minBorder- new Vector3(1, 1, 1);
		m_Maxs = GM._bloxManager._maxBorder; 
	}

	// Update is called once per frame
	void Update ()
	{

		CheckState();
		ApplyState();
		SetText();
		ApplyChangeWidth();
		SetBars();
		CheckRebuild();
    }

	private void CheckRebuild ()
	{
		if (_rebuild && GM.CPS > 30)
		{
			GM._bloxManager._RebuildALL = true;
			_rebuild = false;
		}
	}

	private void SetBars ()
	{         
		if (m_Transparency != GM._bloxManager.Transparency)
		{
			GM._bloxManager.Transparency = m_Transparency;
			GM._bloxManager.SetTransparency( );
        }
		SetTransparencyBar();
		SetXYZbars();
	}

	private void SetXYZbars ()
	{
		SetMinMax();
		//float barLeft = 0;
		float barRight = rT.sizeDelta.x - 110;
		float barWidth = barRight;
		Vector3 xyzLeft = m_Mins;
		Vector3 xyzRight = m_Maxs;
		Vector3 xyzWidth = xyzRight - xyzLeft;

		float xVisMin = Mathf.Clamp(GM._bloxManager._minVisible.x, xyzLeft.x, xyzRight.x);
		float xVisMax = Mathf.Clamp(GM._bloxManager._maxVisible.x, xyzLeft.x, xyzRight.x);
		float xLocMin = barWidth * ((xVisMin - xyzLeft.x) / xyzWidth.x);
		float xLocMax = barWidth * ((xVisMax - xyzLeft.x) / xyzWidth.x);
		Bars[0].sizeDelta = new Vector2(xLocMax - xLocMin, 8);
		Bars[0].anchoredPosition = new Vector2(xLocMin, 0);
		Pointers[0].anchoredPosition = new Vector2(xLocMin, 0);
		Pointers[1].anchoredPosition = new Vector2(xLocMax, 0);

		float yVisMin = Mathf.Clamp(GM._bloxManager._minVisible.y, xyzLeft.y, xyzRight.y);
		float yVisMax = Mathf.Clamp(GM._bloxManager._maxVisible.y, xyzLeft.y, xyzRight.y);
		float yLocMin = barWidth * ((yVisMin - xyzLeft.y) / xyzWidth.y);
		float yLocMax = barWidth * ((yVisMax - xyzLeft.y) / xyzWidth.y);
		Bars[1].sizeDelta = new Vector2(yLocMax - yLocMin, 8);
		Bars[1].anchoredPosition = new Vector2(yLocMin, 0);
		Pointers[2].anchoredPosition = new Vector2(yLocMin, 0);
		Pointers[3].anchoredPosition = new Vector2(yLocMax, 0);

		float zVisMin = Mathf.Clamp(GM._bloxManager._minVisible.z, xyzLeft.z, xyzRight.z);
		float zVisMax = Mathf.Clamp(GM._bloxManager._maxVisible.z, xyzLeft.z, xyzRight.z);
		float zLocMin = barWidth * ((zVisMin - xyzLeft.z) / xyzWidth.z);
		float zLocMax = barWidth * ((zVisMax - xyzLeft.z) / xyzWidth.z);
		Bars[2].sizeDelta = new Vector2(zLocMax - zLocMin, 8);
		Bars[2].anchoredPosition = new Vector2(zLocMin, 0);
		Pointers[4].anchoredPosition = new Vector2(zLocMin, 0);
		Pointers[5].anchoredPosition = new Vector2(zLocMax, 0);
	}
	public void SetXYZposByMouse (int XYorZ)//0 xmin 1 xmax  2 ymin 3 ymax 4 zmin 5 zmax
	{

		SetMinMax();
		float _mouse = Input.mousePosition.x;
		float W1 = Screen.width - rT.sizeDelta.x + 50;
		float W2 = Screen.width - 60;
		float _range = W2 - W1;
		float _Loc = Mathf.Clamp(_mouse - W1, 0, _range);

		Vector3 xyzLeft = m_Mins;
		Vector3 xyzRight = m_Maxs;
		Vector3 xyzWidth = xyzRight - xyzLeft;

		float xVisMin = Mathf.Clamp(GM._bloxManager._minVisible.x, xyzLeft.x, xyzRight.x);
		float xVisMax = Mathf.Clamp(GM._bloxManager._maxVisible.x, xyzLeft.x, xyzRight.x);
		float yVisMin = Mathf.Clamp(GM._bloxManager._minVisible.y, xyzLeft.y, xyzRight.y);
		float yVisMax = Mathf.Clamp(GM._bloxManager._maxVisible.y, xyzLeft.y, xyzRight.y);
		float zVisMin = Mathf.Clamp(GM._bloxManager._minVisible.z, xyzLeft.z, xyzRight.z);
		float zVisMax = Mathf.Clamp(GM._bloxManager._maxVisible.z, xyzLeft.z, xyzRight.z);
		
		if (XYorZ == 0)
		{
			float xVisMinTEMP = ((_Loc / _range) * xyzWidth.x) + xyzLeft.x;
			xVisMin = Mathf.Floor(Mathf.Clamp(xVisMinTEMP, m_Mins.x, xVisMax - 1));
			xVisMin = (xVisMin== m_Mins.x) ? -500 : xVisMin ;
			 
			GM._bloxManager._minVisible.x = xVisMin;
		}
		if (XYorZ == 1)
		{
			float xVisMaxTEMP = ((_Loc / _range) * xyzWidth.x) + xyzLeft.x;
			xVisMax = Mathf.Floor(Mathf.Clamp(xVisMaxTEMP,xVisMin+1  , m_Maxs.x));
			xVisMax = (xVisMax == m_Maxs.x) ?  500 : xVisMax ; 
			GM._bloxManager._maxVisible.x = xVisMax;

		}
		if (XYorZ == 2)
		{
			float yVisMinTEMP = ((_Loc / _range) * xyzWidth.y) + xyzLeft.y;
			yVisMin = Mathf.Floor(Mathf.Clamp(yVisMinTEMP, m_Mins.y, yVisMax - 1));
			yVisMin = (yVisMin == m_Mins.y) ? -500 : yVisMin; 
			GM._bloxManager._minVisible.y = yVisMin;

		}
		if (XYorZ == 3)
		{
			float yVisMaxTEMP = ((_Loc / _range) * xyzWidth.y) + xyzLeft.y;
			yVisMax = Mathf.Floor(Mathf.Clamp(yVisMaxTEMP, yVisMin + 1, m_Maxs.y));
			yVisMax = (yVisMax == m_Maxs.y) ? 500 : yVisMax; 
			GM._bloxManager._maxVisible.y = yVisMax;
		}
		if (XYorZ == 4)
		{
			float zVisMinTEMP = ((_Loc / _range) * xyzWidth.z) + xyzLeft.z;
			zVisMin = Mathf.Floor(Mathf.Clamp(zVisMinTEMP, m_Mins.z, zVisMax - 1));
			zVisMin = (zVisMin == m_Mins.z ) ? -500 : zVisMin;
			Debug.Log(zVisMin);

			GM._bloxManager._minVisible.z = zVisMin;
		}
		if (XYorZ == 5)
		{
			float zVisMaxTEMP = ((_Loc / _range) * xyzWidth.z) + xyzLeft.z;
			zVisMax = Mathf.Floor(Mathf.Clamp(zVisMaxTEMP, zVisMin + 1, m_Maxs.z));
			zVisMax = (zVisMax == m_Maxs.z ) ? 500 : zVisMax; 

			GM._bloxManager._maxVisible.z = zVisMax;
		}  
	}
	public void SetRebuild ()
	{ 
		_rebuild = true;
	}

	private void SetTransparencyBar ()
	{ 
		float TransRight = rT.sizeDelta.x - 90;
		float TransWidth = TransRight * (m_Transparency / 50);
		Bars[3].sizeDelta = new Vector2(TransWidth, 8);
		Pointers[6].anchoredPosition = new Vector2(TransWidth, 0);
	}

	public void SetTransparencyByMouse ()
	{
		float _mouse = Input.mousePosition.x;
		float W1 = Screen.width - rT.sizeDelta.x + 40;
		float W2 = Screen.width - 50;
		float _range = W2 - W1;
		float _Loc = Mathf.Clamp(_mouse - W1, 0, _range);
		m_Transparency = (_Loc / _range) * 50;
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
		rClock.sizeDelta = iTween.Vector2Update(rClock.sizeDelta, rClockTarget, rClockSpeed);
		rClockHand.sizeDelta = rClock.sizeDelta;
		HandRotation = Time.deltaTime * HandRotationDirection;
		rClockHand.Rotate(new Vector3(0, 0, HandRotation));
		rSideThing.anchoredPosition = iTween.Vector2Update(rSideThing.anchoredPosition, rSidePosTarget, rTspeed);
		rSideThing.sizeDelta = iTween.Vector2Update(rSideThing.sizeDelta, rSideTarget, rTspeed);
		rT.anchoredPosition = iTween.Vector2Update(rT.anchoredPosition, CurrentLocation, rTspeed);
	}

	private void CheckState ()
	{
		m_Grow = GM._UI.gameState == Common.GameState.View;
		rTarget = (m_Grow) ? rBig : rSmall;
		rTextTarget = (rT.sizeDelta.x > 240) ? rTextBig : rTextSmall;
		rTextPosTarget = (rT.sizeDelta.x > 240) ? rTextPosBig : rTextPosSmall;
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
		CurrentLocation = (m_Grow) ? OnScreen : OffScreen;
	}
    
	public void SetText ()
	{
		float diff = Mathf.Abs(rTextArea.anchoredPosition.x - rTextPosBig.x) + Mathf.Abs(rTextArea.anchoredPosition.y - rTextPosBig.y);
		if (diff < 1 && m_Grow)
		{
			_targetText = _textFull;
		}
		else
		{
			_text = "                    ";
			_targetText = "                    ";
		}
		TextArea.text = _text;
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

	public void SetChangeWidth (float _cw)
	{
		ChangeWidth = _cw;
	}

	public void ApplyChangeWidth ()
	{
		if (Input.GetMouseButton(0))
		{
			rBig.x = Mathf.Clamp(rBig.x + (250 * Time.deltaTime * ChangeWidth), 400, Screen.width - 20);

		}
	}


}
