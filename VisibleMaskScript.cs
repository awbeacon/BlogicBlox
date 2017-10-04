using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleMaskScript : MonoBehaviour {
public	Transform[] Masks;
	public Vector2 _LR = new Vector2(0, 0);
	public Vector2 _DU = new Vector2(0, 0);
	public Vector3 Depth  =new Vector3(0,0,0);
	
	// Update is called once per frame
	void Update ()
	{
		this.transform.localPosition = Depth;
		Masks[0].localPosition = new Vector3(_LR.x, 0, 0);
		Masks[1].localPosition = new Vector3(_LR.y  , 0, 0);
		Masks[2].localPosition = new Vector3(_LR.x, 0,  _DU.x );
		Masks[3].localPosition = new Vector3(_LR.x, 0,  _DU.y);
		float Gap = (_LR.y  - _LR.x);
		Masks[2].localScale = new Vector3(Gap * .1f, 100, 100);
		Masks[3].localScale = new Vector3(Gap * .1f, 100, 100);
	}
}
