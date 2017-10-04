using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public GameManagerScript GM;
	public Vector3 CameraCenter;
	public Transform CameraPos;
	public Camera MainCamera;
	public float CameraZoom = 10;
	public Transform CenterUD;
	float m_CameraTarget;
	public float ZoomSpeed = 1;
	public float ZoomDamper = 10;
	public Vector2 MouseDownLast;
	public Vector2 MouseCurrent;
	bool RightClick = false;
	//bool AltClick = false;
	public float MouseSpeedLR = 10;
	public float MouseSpeedUD = 10;
	public Vector3 UD_Rotation = new Vector3(0, 0, 0);
	float LastClick = 0;
	float ThisClick = 100;
	public bool DoubleClick = false;

	// Use this for initialization
	void Start ()
	{
		m_CameraTarget = CameraZoom;
		MouseDownLast = Input.mousePosition;
	}

	// Update is called once per frame
	void Update ()
	{
		GetInputs();
		SetZoom();
		SetRotate();
		SetCenter();
		CheckDoubleClick();
	}

	private void CheckDoubleClick ()
	{
		if (Time.time - ThisClick < .4f && ThisClick - LastClick < .4f)
		{
			RaycastHit hit;
			Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{

				if (hit.transform.tag == "Blox" || hit.transform.tag == "Cursor")
				{
					CameraCenter = hit.point;
					LastClick = 0;
				}
			}
		}
	}

	private void SetCenter ()
	{
		transform.position = Vector3.Lerp(transform.position, CameraCenter, .1f);
	}

	private void SetRotate ()
	{
		bool OverUI = GM._EventSystem.IsPointerOverGameObject();
		if (RightClick && GM._UI.GetCameraModeRotate()) //rotate
		{
			if (!OverUI)
			{
				transform.Rotate(Vector3.up * Time.deltaTime * (MouseCurrent.x - MouseDownLast.x) * MouseSpeedLR);//look left/right
				UD_Rotation.x = Mathf.Clamp(UD_Rotation.x + (Time.deltaTime * -(MouseCurrent.y - MouseDownLast.y) * MouseSpeedUD), -90, 90);
				CenterUD.localEulerAngles = UD_Rotation;
			}
			MouseDownLast = MouseCurrent;
		}
		if (RightClick && !GM._UI.GetCameraModeRotate() && !OverUI)//move
		{
			if (!OverUI)
			{
				Vector3 DirUP = CameraPos.TransformDirection(Vector3.up);
				CameraCenter += DirUP * (Time.deltaTime * -(MouseCurrent.y - MouseDownLast.y));
				Vector3 DirRight = CameraPos.TransformDirection(Vector3.right);
				CameraCenter += DirRight * (Time.deltaTime * -(MouseCurrent.x - MouseDownLast.x));
			}
			MouseDownLast = MouseCurrent;
		}
	}

	private void GetInputs ()
	{
		float zoomChange = Input.GetAxis("Mouse ScrollWheel");
		bool OverUI = GM._EventSystem.IsPointerOverGameObject();
		MouseCurrent = Input.mousePosition;

		if (Input.GetMouseButtonDown(1))
		{
			MouseDownLast = Input.mousePosition;
			MouseCurrent = MouseDownLast;

			LastClick = ThisClick;
			ThisClick = Time.time;


			RightClick = true;

		}

		if (Input.GetMouseButtonUp(1))
		{

			RightClick = false;
		}

		if (!OverUI)
		{
			if (!GM._UI.GetCameraModeRotate())
			{
				Vector3 DirForward = CameraPos.TransformDirection(Vector3.forward);
				CameraCenter += DirForward * (Time.deltaTime * zoomChange * 500);
			}
			else
			{
				m_CameraTarget = Mathf.Clamp(m_CameraTarget * (1 - (zoomChange * ZoomSpeed)), 1, 450);
			}
		}
	}

	private void SetZoom ()
	{
		CameraZoom += (m_CameraTarget - CameraZoom) / ZoomDamper;
		CameraPos.localPosition = new Vector3(0, 0, -CameraZoom);
	}

	public void OnTriggerStay (Collider Other)
	{
		if (Other.tag == "Blox" || Other.tag == "Cursor")
		{
			m_CameraTarget += Time.deltaTime * 10; //if colliding with something, move back
		}
	}
}
