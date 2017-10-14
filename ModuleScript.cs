using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModuleScript : MonoBehaviour {

    //take a filename as input builds module based on data
    //need to create a module class that defines a whole module, can contain other modules
  public  GameObject BLOCK_PLAIN;
    int[,,] ENERGY;
	 

    List<GameObject> BLOX;
    bool ACTIVE = true; //active = moving and colliders on, inactive=not moving, colliders on, show state outline (green=ok to place, red=overlapping somethin)
	
    // Use this for initialization
	void Start () {
        //load file
        //initialize energy and width
        CreateBlock(new Vector3(0, 0, 0));
        CreateBlock(new Vector3(0, 1, 0));
        CreateBlock(new Vector3(1, 0, 0));
        CreateBlock(new Vector3(0, 1, 1));
        CreateBlock(new Vector3(0, 1, 2));
        CreateBlock(new Vector3(1, 1, 0));

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void CreateBlock (Vector3 LOC)
    {
       GameObject GO= Instantiate(BLOCK_PLAIN,this.transform.position+ LOC, Quaternion.identity)as GameObject;
        GO.transform.parent = this.transform;
        
    }
}
