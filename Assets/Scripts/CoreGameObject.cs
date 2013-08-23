using UnityEngine;
using System.Collections;

public class CoreGameObject : MonoBehaviour {
	protected string coreType;
	
	public string CoreType {
		get {
			return 	coreType;
		}	
	}
	
	// Use this for initialization
	void Start () {
		coreType = "basic";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
