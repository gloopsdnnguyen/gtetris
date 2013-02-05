using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour {
	
	public GUISkin customSkin;
	// Use this for initialization
	public int buttonW=100;
	public int buttonH=50;
	// half of the Screen width:
	public float halfScreenW = Screen.width/2; 
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () {		
		if(GUI.Button(new Rect(halfScreenW-(buttonW/2),560,buttonW,buttonH),"Battle")) {
			Application.LoadLevel("game");
		}
	}
}
