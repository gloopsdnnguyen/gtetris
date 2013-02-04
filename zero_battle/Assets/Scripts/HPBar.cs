using UnityEngine;
using System.Collections;

public class HPBar : MonoBehaviour {	
	public float maxHP = 100;
	public float curHP = 100;	
	public float defaultLength=1.8f;	
	
	
	public void AddjustCurrentHP(float hp) {		
	  	curHP += hp;			
		if(curHP < 0)
			curHP = 0;
		if(curHP > maxHP)
			curHP = maxHP;					
	  	transform.GetComponent<LineRenderer>().SetPosition(1,new Vector3(0,0, defaultLength*(curHP/maxHP)));
	}	
	
	public void setMaxHP(int maxhp){
		maxHP=maxhp;
	}	
}