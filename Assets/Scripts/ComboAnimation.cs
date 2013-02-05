using UnityEngine;
using System.Collections;

public class ComboAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {	
	
	}
	
	void moveOnePairsAnimationBack(){
		iTween.ScaleTo(gameObject ,iTween.Hash("scale",new Vector3(0f,0f,0)));	
	}
}
