using UnityEngine;
using System.Collections;

public class SPBar : MonoBehaviour {
	
	public int type=1;//1:monster 2:enemy
	public float maxSP = 100;
	public float curSP = 100;	
	public float defaultLength=1.8f;	
	public const float timeMax=30;
	public float timeLeft=30;
	public float timeStart;
	private int status=1; //1 active 0 pause
	private float resume_time=0;
	
	
	public void AddjustCurrentSP(float sp) {		
	  	curSP += sp;			
		if(curSP < 0)
			curSP = 0;
		if(curSP > maxSP)
			curSP = maxSP;					
	  	transform.GetComponent<LineRenderer>().SetPosition(1,new Vector3(0,0, defaultLength*(curSP/maxSP)));
	}	
	
	public void setMaxSP(int maxsp){
		maxSP=maxsp;
	}	
	
	public void setType(int t){
		type=t;
	}
	
	// Use this for initialization
	void Start () {
		timeStart = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(type==2){
			if(status==1){
				if(resume_time!=0){
					timeLeft = resume_time - (Time.time - timeStart);					
				}else{
					timeLeft = timeMax - (Time.time - timeStart);
				}				
				if(timeLeft<0){
					timeLeft=0;
				}
				transform.GetComponent<LineRenderer>().SetPosition(1,new Vector3(0,0, (timeLeft/timeMax)*defaultLength));
			}
		}
	}
	
	public void pauseTime(){
		status=0;
	}
	
	public void resumeTime(){
		status=1;
		resume_time=timeLeft;
		timeStart=Time.time;
	}
}
