using UnityEngine;
using System.Collections;

public class ComboAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {	
	
	}
	
	public void moveOnePairsAnimationBack(object gameObject){
		iTween.ScaleTo((GameObject)gameObject ,iTween.Hash("scale",new Vector3(0.5f,0.5f,0.5f),"time",0.25f));	
	}
	
	public void moveOnePairsAnimationBack2(object gameObject){
		iTween.ScaleTo((GameObject)gameObject ,iTween.Hash("scale",new Vector3(1.5f,1.5f,0.5f),"time",0.25f));	
	}
}
