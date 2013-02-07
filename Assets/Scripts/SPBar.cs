using UnityEngine;
using System.Collections;

public class SPBar : MonoBehaviour {
	
	public int type=1;//1:monster 2:enemy
	public float maxSP = 100;
	public float curSP = 100;	
	public float defaultLength=1.8f;	
	private const float timeConst=10;
	private float timeMax;
	public float timeLeft=10;
	public float timeStart;
	private int status=1; //1 active 0 pause
	private float resume_time=0;
	public bool enemy_can_attack=false;

	
	public void AddjustCurrentSP(float sp) {		
	  	curSP += sp;			
		if(curSP < 0)
			curSP = 0;
		if(curSP > maxSP)
			curSP = maxSP;					
	  	transform.GetComponent<LineRenderer>().SetPosition(1,new Vector3(0,0, (curSP/maxSP)*defaultLength));
	}	
	
	public void setMaxSP(int maxsp){
		maxSP=maxsp;	
		timeMax=maxSP/timeConst;
	}	
	
	public void setType(int t){
		type=t;
		timeMax=timeConst;
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
				if(timeLeft<=0){
					timeLeft=0;
					enemy_can_attack=true;							
				}				
				transform.GetComponent<LineRenderer>().SetPosition(1,new Vector3(0,0, (timeLeft/timeMax)*defaultLength));
				if(timeLeft==0){
					resetTime();
				}
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
	
	private void resetTime(){
		timeLeft=timeMax;
		resume_time=0;
		timeStart = Time.time;
	}
	
	public void setAttacked(){
		enemy_can_attack=false;
	}
}
