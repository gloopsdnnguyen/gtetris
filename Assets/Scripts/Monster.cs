using UnityEngine;
using System.Collections;

public class Monster{
	
	public string name;
	public float HP;
	public int SP;
	public int DP;
	public int AP;
	public string image;
	public int type;//1 monster,2 enemy
	public int position;
	public int status=1;
	public Skill[] skills= new Skill[5];
	
}
