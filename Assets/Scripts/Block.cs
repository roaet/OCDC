using UnityEngine;
using System.Collections;

public class Block : CoreGameObject {
	
	private bool completed;

	// Use this for initialization
	virtual public void Start () {
		completed = false;
		coreType = "block";
		
	}
	
	// Update is called once per frame
	virtual public void Update () {
	
	}
	
}
