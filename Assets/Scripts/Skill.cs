using UnityEngine;
using System.Collections;

public class Skill{
	
	public string name;
	public string image;
	public int sp_requirement=10;
	public int type=1;//1: Physical attack, 2: Magic attack, 3: Heal, 4~ To be defined later.
	public int ap=10;//attack point of this skill
	public int attribute=1;//1: Fire, 2: Ice, 3: Thunder, 4: Wind, 5: Light, 6: Darkness
	public int condition=0;//0: Always available, 1: Combo lock, 2: Poker hand lock
	public int condition_value=1;//If condition = 1, set how many combos are required to unlock the skill.If condition = 2, set which poker hand is required to unlock the skill.
	
}
