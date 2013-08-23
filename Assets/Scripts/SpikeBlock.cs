using UnityEngine;
using System.Collections;

public class SpikeBlock : Block {
	
	
	// Use this for initialization
	public override void Start () {
		coreType = "spike";
		
	}
	
	// Update is called once per frame
	public override void Update () {
	
	}

	void OnCollisionEnter(Collision collision) {
		foreach(ContactPoint contact in collision.contacts) {
			if(contact.otherCollider.gameObject.GetComponent("PlayerAvatar")) {
				MasterGameController.Instance.PlayerTouchedSpike();
				break;
			}
			if(contact.otherCollider.gameObject.GetComponent("EggBlock")) {
				MasterGameController.Instance.SpikeTouchedEgg();
				break;
			}
		}
	}
}
