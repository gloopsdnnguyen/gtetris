using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//TODO: Move some const to config
//check int value
public class BlockPosition
{
    public int x;
    public int y;

    public BlockPosition(int setX, int setY)
    {
        x = setX;
        y = setY;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}


public class Manager : MonoBehaviour
{
    public GameObject CardPrefab;
	public GameObject PickerPrefab;
	public GameObject MonsterStatPrefab;
	public GameObject CharacterPrefab;
	public GameObject CharacterActivePrefab;
	public GameObject ComboCenterNumberPrefab;
	public GameObject ComboCenterTextPrefab;
	public GameObject SmallComboNumberPrefab;
	public GameObject SmallComboTypePrefab;
	public GameObject SmallComboTypeLargePrefab;
	public GameObject TargetSelectPrefab;
	public GameObject LargeComboNumberPrefab;
	public GameObject LargeComboTypePrefab;
	public GameObject LineRenderPrefab;
	
    private int cols = 5;
    private int rows = 5;    
    private GameObject[,] cardBlockObjects;
	private GameObject[,] selectedBlockObjects;
	private GameObject[,] skillBlockObjects;
	private GameObject[,] monsterBlockObjects;
	private GameObject[,] enemyBlockObjects;
	
	private Card[,] cardsOnBoard;//store card with 2D position	
	private List<Card> cardsDraw =  new List<Card> ();//store list card on board(already draw)
	//private Array monsterSkill= new Array();
    private int[,] selectedBlocks;
    private List<Card> selectedCards =  new List<Card> ();     
	private List<Skill> selectedSkills= new List<Skill>();
	private List<Monster> monsters;
	private List<Monster> enemies;
		
	private Monster targetEnemy=null;	
	private Monster activeMonster=null;
	
	private Deck pokerDeck;
	private bool available_shuffle=true;	
	
	private RaycastHit hit=new RaycastHit();
	private Ray ray=new Ray();
	public AudioClip pickAudio;
	public AudioClip moveAudio;
	public AudioClip handAudio;
	public AudioClip arcadeAudio;
	
	private Vector3[,] tmpPos=new Vector3[5, 5];
	private bool available_checkhand=false;
	private bool in_checking_hand=false;
	private bool skill_selectable=false;
	private bool card_selectable=true;
	private bool enemy_selectable=false;
	
	private Shader lightShader;
	private Shader darkShader;	
	
	private bool generateNewComboNumber=false;
	private GameObject comboTopText=null;
	private GameObject comboTopNumber=null;
	private int skill_selected_count=0;
	private GameObject targetEnemyGameObject=null;	
	private GameObject activeMonsterGameObject;
	private GameObject activeSkill;
	
	
	
	private int active_monster_index=2;
	private List<int> combos;
	private int numberOnePairs=0;
	private GameObject selectEnemyBlock;
	private GameObject selectSkillBlock;
	
	private GameObject largeComboNumber;
	private GameObject largeComboType;
	private List<GameObject> smallComboNumberBlock;
	private List<GameObject> smallComboTypeBlock;
	private bool canDisplayOnePairCombo=true;
	
	
    void Start()
    {      
		lightShader = Shader.Find("Unlit/Transparent");
		darkShader  = Shader.Find("Transparent/Diffuse");
		hit=new RaycastHit();
		ray=new Ray();		
		selectedCards =  new List<Card> (); 
		cardsDraw =  new List<Card> ();
		tmpPos=new Vector3[5, 5];
		cardBlockObjects    = new GameObject[cols, rows];
		selectedBlockObjects  = new GameObject[Screen.width, Screen.height];
		enemyBlockObjects = new GameObject[1,3];
		skillBlockObjects = new GameObject[1,5];
		monsterBlockObjects = new GameObject[1,3];
		smallComboTypeBlock=	new List<GameObject>();
		smallComboNumberBlock=	new List<GameObject>();
		monsters= new List<Monster>();
		enemies = new List<Monster>();
		combos  = new List<int>();
		Monster m1 = new Monster();
		m1.name="Monster 1";
		m1.image="char1";
		m1.HP=100;
		m1.SP=100;
		m1.type=1;
		m1.position=1;
		monsters.Add(m1);
		
		Monster m2 = new Monster();
		m2.name="Monster 2";
		m2.image="char2";
		m2.HP=100;
		m2.SP=100;
		m2.type=1;
		m2.position=2;
		monsters.Add(m2);
		
		Monster m3 = new Monster();
		m3.name="Monster 3";
		m3.image="char3";
		m3.HP=100;
		m3.SP=100;
		m3.type=1;
		m3.position=3;
		monsters.Add(m3);
		
		Monster e1 = new Monster();
		e1.name="Enemy 1";
		e1.image="m1";
		e1.HP=100;
		e1.SP=100;
		e1.type=2;
		e1.position=1;
		enemies.Add(e1);
		
		Monster e2 = new Monster();
		e2.name="Enemy 2";
		e2.image="m2";
		e2.HP=100;
		e2.SP=100;
		e2.type=2;
		e2.position=2;
		enemies.Add(e2);
		
		Monster e3 = new Monster();
		e3.name="Enemy 3";
		e3.image="m3";
		e3.HP=100;
		e3.SP=100;
		e3.type=2;
		e3.position=3;
		enemies.Add(e3);
		
		buildBattleArea();
		buildEnemy();
		buildMonster();
		buildSkill();
    }
	
	private void buildBattleArea(){
		card_selectable=true;
		
		available_checkhand=false;
		in_checking_hand=false;
		available_shuffle=true;				
		
		pokerDeck = new Deck();
		pokerDeck.Shuffle();      		
		
		selectedBlocks   = new int[10,10];//out large grid
		cardsOnBoard = new Card[cols, rows];		
        for (int x = 0; x < cols; ++x)
        {
            for (int y = 0; y < rows; ++y)
            {
                cardsOnBoard[x, y] = pokerDeck.Draw();
				cardsDraw.Add(cardsOnBoard[x, y]);
				selectedBlocks[x,y]=0;
            }
        }       
		BuildGrid();		
	}
	
    private Vector3 Get3DPosition(BlockPosition position)
    {
        return new Vector3(position.x-3, position.y-3f, 1.0f);//move to 0 0 1
    }
	
	private BlockPosition Get2DPosition(UnityEngine.Transform transform)
    {
        return new BlockPosition((int)transform.position.x+3, (int)transform.position.y+3);
    }

    private GameObject GetBlockGameObjectAtPosition(BlockPosition position)
    {
        return cardBlockObjects[position.x, position.y];
    }
	
	 private GameObject GetEnemyGameObjectAtPosition(BlockPosition position)
    {
        return enemyBlockObjects[position.x, position.y];
    }

    private GameObject SetBlock(BlockPosition position)
    {
        GameObject newBlock = (GameObject)Instantiate(CardPrefab, Get3DPosition(position), Quaternion.Euler(0.0f,180.0f,0.0f));		
        newBlock.transform.localScale=Vector3.one *0.9f;
		newBlock.AddComponent("BoxCollider");
		newBlock.tag="Card";
		GameObject oldGameObject = GetBlockGameObjectAtPosition(position);
        if (oldGameObject != null)
        {
            DestroyImmediate(oldGameObject);
        }
		newBlock.renderer.material.mainTexture = (Texture2D)Resources.Load(cardsOnBoard[position.x,position.y].ToString());		
        cardBlockObjects[position.x, position.y] = newBlock;
        return newBlock;
    }
	
    private void RemoveBlock(BlockPosition position)
    {
        if (cardBlockObjects[position.x, position.y] != null)
        {
            DestroyImmediate(cardBlockObjects[position.x, position.y]);
        }
    }
   

    void OnGUI()
    {
		
    }   

    private void BuildGrid(){
		for(int i=0; i<cols; i++)
		{				
			for(int j=0; j<rows; j++)
			{				
				SetBlock(new BlockPosition(i, j));
			}			
		}
	}
	
	private void buildEnemy(){
		removeObjectByTag("Monster");		
		foreach(Monster m in enemies){		
			GameObject enemyBlock = (GameObject)Instantiate(CharacterPrefab, new Vector3(m.position*2-5, 4.5f, 1.0f), Quaternion.Euler(0.0f,0.0f,0.0f));//fix me,bullshit here
			enemyBlock.transform.GetChild(0).renderer.material.mainTexture= (Texture2D)Resources.Load(m.image);	
			enemyBlock.transform.GetChild(2).GetComponent<SPBar>().setType(2);
			enemyBlock.transform.GetChild(0).tag="Enemy"+m.position.ToString();
			enemyBlock.tag="Enemy"+m.position.ToString();
			enemyBlockObjects[0,m.position-1]=enemyBlock;
		}
	}
	
	private void buildMonster(){
		removeObjectByTag("Monster");		
		foreach(Monster m in monsters){
			BlockPosition pos = new BlockPosition(m.position-1,5);
			if(m.position==3){				
				GameObject monsterBlock = (GameObject)Instantiate(CharacterActivePrefab, Get3DPosition(pos), Quaternion.identity);				   
				monsterBlock.transform.localScale=Vector3.one*0.5f;
				monsterBlock.transform.GetChild(0).renderer.material.mainTexture= (Texture2D)Resources.Load(m.image);
				monsterBlock.tag="Monster";			
				activeMonsterGameObject=monsterBlock;
				GameObject monsterStat = (GameObject)Instantiate(MonsterStatPrefab, new Vector3(0.5f,0.5f,1f),Quaternion.identity);//fixme
				monsterStat.guiText.text =m.name+"\nHP\nSP";
				
				monsterBlockObjects[0,m.position-1]=monsterBlock;
			}else{
				GameObject monsterBlock = (GameObject)Instantiate(CharacterPrefab, Get3DPosition(pos), Quaternion.identity);				  
				monsterBlock.transform.localScale=Vector3.one*0.5f;
				monsterBlock.transform.GetChild(0).renderer.material.mainTexture= (Texture2D)Resources.Load(m.image);
				monsterBlock.transform.GetChild(1).GetComponent<LineRenderer>().SetWidth(0.1f,0.1f);
				monsterBlock.transform.GetChild(2).GetComponent<LineRenderer>().SetWidth(0.1f,0.1f);
				monsterBlock.tag="Monster";				
				monsterBlockObjects[0,m.position-1]=monsterBlock;
			}			
		}			
	}
	
	
	private void buildSkill(){
		skill_selectable=false;
		removeObjectByTag("Skill");
		for(int y=0; y<5; y++)
		{							
			BlockPosition pos = new BlockPosition(5,y);			
			GameObject newBlock = (GameObject)Instantiate(CardPrefab, Get3DPosition(pos), Quaternion.Euler(0.0f,180.0f,0.0f));		
	        newBlock.transform.localScale=Vector3.one *0.9f;			
			newBlock.AddComponent("BoxCollider");
			newBlock.tag="Skill";
			newBlock.renderer.material.mainTexture = (Texture2D)Resources.Load("s"+(y+1));	
			newBlock.renderer.material.shader=darkShader;
	        skillBlockObjects[0, y] = newBlock;
		}		
	}
	
	private void EnableSkill(){
		card_selectable=false;
		for(int x=0; x<5; x++)
		{									
	        skillBlockObjects[0, x].renderer.material.shader = lightShader;
		}	
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{				
				if(selectedBlocks[i,j]==0){					
					cardBlockObjects[i,j].renderer.material.shader=darkShader;
				}
			}
		}
	}
	
	private void DisableSkill(){
		card_selectable=true;
		for(int x=0; x<5; x++)
		{									
	        skillBlockObjects[0, x].renderer.material.shader = darkShader;
		}	
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{				
				if(selectedBlocks[i,j]==0){
					
					cardBlockObjects[i,j].renderer.material.shader=lightShader;
				}
			}
		}
	}
	
	private void randomAttackMonster(){		
		StartCoroutine(monsterAttackedAnimation());
	}
		
	private IEnumerator monsterAttackedAnimation(){
		yield return new WaitForSeconds(0.25f);
		GameObject monsterBlock;
		foreach(Monster m in monsters){
			monsterBlock = monsterBlockObjects[0,m.position-1];	
			audio.PlayOneShot(handAudio);			
			Vector3 movePos =monsterBlock.transform.position;
			movePos.x-=0.0625f;
			iTween.PunchPosition(monsterBlock, iTween.Hash("x", movePos.x));			
			m.HP-=10;
			monsterBlock.transform.GetChild(1).GetComponent<HPBar>().AddjustCurrentHP(-10);			
			if(m.HP<=0){
				m.HP=0;
			}			
			if(m.HP==0){
				monsterBlock.transform.GetChild(0).renderer.material.shader=darkShader;
				monsters.Remove(m);
			}
			break;
		}		
		
		yield return new WaitForSeconds(0.25f);		
	}
	
	void Update(){	
		
		foreach(GameObject enemies in enemyBlockObjects){
			if(enemies!=null){
				if(enemies.transform.GetChild(2).GetComponent<SPBar>().enemy_attack){
					randomAttackMonster();
					enemies.transform.GetChild(2).GetComponent<SPBar>().setAttack();
				}
			}
		}
		if(generateNewComboNumber){
			displayComboIndex();
			displayOnePairCombo();
			generateNewComboNumber=false;
		}		
		
		if(targetEnemyGameObject!=null){
			skill_selectable=true;	
			card_selectable=false;
		}
		
		if(available_checkhand && !in_checking_hand){	
			in_checking_hand=true;
			StartCoroutine(Check());
		}	
		
		if ( Input.GetMouseButtonDown(0))
	   	{
	      ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
	      if (Physics.Raycast(ray, out hit))
	      {							
				BlockPosition pos2d=Get2DPosition(hit.transform.gameObject.transform);	
				if(card_selectable&& hit.collider.tag=="Card" ){
					if(isBlockSelected(pos2d)){
						selectedCards.Remove(cardsOnBoard[pos2d.x,pos2d.y]);	
						deselectBlock(pos2d,true);	
					}else{
						selectedCards.Add(cardsOnBoard[pos2d.x,pos2d.y]);
						selectBlock(pos2d, true);	
					}	
				}
				if(skill_selectable&& hit.collider.tag=="Skill"){					
					activeSkill=hit.transform.gameObject;
					audio.PlayOneShot(pickAudio);
					selectSkill(hit.transform.gameObject);
					attackEnemy();
				}
				if(enemy_selectable&& (hit.collider.tag=="Enemy1"||hit.collider.tag=="Enemy2"||hit.collider.tag=="Enemy3") ){
					selectEnemy(hit.transform.parent.gameObject);
					audio.PlayOneShot(pickAudio);
					targetEnemyGameObject=hit.transform.parent.gameObject;	
				}				
	      }
	   	}
		if(selectedCards.Count>=5 && available_shuffle){ 
			 EnableSkill();
			 ShuffleCard();
		}		
	}		
	
	private bool isBlockSelected(BlockPosition pos2d){		
		return selectedBlocks[pos2d.x,pos2d.y]==1;
	}
	
	private void selectBlock(BlockPosition pos2d, bool changeShader){
		audio.PlayOneShot(pickAudio);
		selectedBlocks[pos2d.x,pos2d.y]=1;
		if(changeShader){
			hit.transform.gameObject.renderer.material.shader = darkShader;
		}
		SetHightlight(hit.transform.gameObject,pos2d);
	}
	
	private void deselectBlock(BlockPosition pos2d, bool changeShader){
		audio.PlayOneShot(pickAudio);
		selectedBlocks[pos2d.x,pos2d.y]=0;	
		if(changeShader){
			hit.transform.gameObject.renderer.material.shader=lightShader;		
		}
		removeHightlightAtPostion(pos2d);
	}	
	
	private void SetHightlight(GameObject obj,BlockPosition pos){
		Vector3 tmp = obj.transform.position;
		tmp.z=0.5f;
		GameObject pickerBlock = (GameObject)Instantiate(PickerPrefab, tmp, Quaternion.Euler(0.0f,180.0f,0.0f));	
		selectedBlockObjects[pos.x, pos.y] = pickerBlock;
		pickerBlock.tag="Picker";	
		pickerBlock.renderer.material.color = new Color(1,166.0f/255.0f,0,1);
		iTween.ColorTo(pickerBlock, iTween.Hash("name","h"+pos.x+"_"+pos.y,"r",180.0f/255.0f,"g",117.0f/255.0f,"b",0,"a",1,"looptype","pingPong","time",0.125f));
		pickerBlock.transform.localScale=Vector3.one *0.95f;	
	}
	
	
	private void removeHightlightAtPostion(BlockPosition pos){
		DestroyImmediate(selectedBlockObjects[pos.x,pos.y]);
		//iTween.StopByName("h"+pos.x+"_"+pos.y);
	}
	
	private void removeAllHightligh(){
		GameObject[] previews = GameObject.FindGameObjectsWithTag("Picker");
		foreach(GameObject p in previews){
		        DestroyImmediate(p);
		}
	}
	
	private void removeObjectByTag(string tagname){
		GameObject[] gos = GameObject.FindGameObjectsWithTag(tagname);
		foreach(GameObject p in gos){
		        DestroyImmediate(p);
		}
	}
	
	
	private void ShuffleCard(){	
		available_shuffle=false;				
		StartCoroutine(moveOut());
		StartCoroutine(moveBack());			
	}
	
	private IEnumerator moveOut() {				
		pauseEnemyGaugeBar();
		//yield return new WaitForSeconds(0.5f);
		audio.PlayOneShot(moveAudio);		
		for(int y=rows-1;y>=0;y--){			
			for(int x=0;x<cols;x++){
				if(selectedBlocks[x,y]==0){
					Vector3 movePos =cardBlockObjects[x,y].transform.position;
					tmpPos[x,y]=cardBlockObjects[x,y].transform.position;
					movePos.x-=150;
					iTween.MoveTo(cardBlockObjects[x,y], iTween.Hash("x", movePos.x, "easeType", "EaseInOutSine", "delay", .1));
				}				
			}				
			if(y>0){
				yield return new WaitForSeconds(0.0625f);
			}
		}		
	}
	
	
	private IEnumerator moveBack() {
		
		yield return new WaitForSeconds(0.25f);
		
		foreach(Card _card in cardsDraw){
			if(!selectedCards.Contains(_card)){				
				pokerDeck.Add(_card);
			}			
		}				
		cardsDraw = new List<Card>();
		selectedCards= new List<Card>();	
		
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{
				if(selectedBlocks[i,j]==0){
					cardsOnBoard[i,j] =pokerDeck.Draw();	
				}
				cardsDraw.Add(cardsOnBoard[i,j]);
				cardBlockObjects[i,j].transform.localScale=Vector3.one *0.95f;
				cardBlockObjects[i,j].renderer.material.mainTexture = (Texture2D)Resources.Load(cardsOnBoard[i,j].ToString());
			}
		}
		
		for(int k=rows-1;k>=0;k--){
			for(int y=0;y<cols;y++){
				if(selectedBlocks[y,k]==0){						
					iTween.MoveTo(cardBlockObjects[y,k], iTween.Hash("x", tmpPos[y,k].x, "easeType", "EaseInOutSine", "delay", .1));
				}				
			}				
			yield return new WaitForSeconds(0.125f);			
		}				
		audio.PlayOneShot(moveAudio);
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{
				selectedBlocks[i,j]=0;
			}
		}
		available_checkhand=true;
	}
	
	private IEnumerator moveBackDeal() {
		
		yield return new WaitForSeconds(0.25f);
		
		pokerDeck = new Deck();
		pokerDeck.Shuffle();   
		
		cardsDraw = new List<Card>();
		selectedCards= new List<Card>();	
		
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{
				cardsOnBoard[i,j] =pokerDeck.Draw();
				cardsDraw.Add(cardsOnBoard[i,j]);
				cardBlockObjects[i,j].transform.localScale=Vector3.one *0.95f;
				cardBlockObjects[i,j].renderer.material.mainTexture = (Texture2D)Resources.Load(cardsOnBoard[i,j].ToString());
				cardBlockObjects[i,j].renderer.material.shader=lightShader;
				selectedBlocks[i,j]=0;
			}
		}
		
		for(int k=rows-1;k>=0;k--){
			for(int y=0;y<cols;y++){
				iTween.MoveTo(cardBlockObjects[y,k], iTween.Hash("x", tmpPos[y,k].x, "easeType", "EaseInOutSine", "delay", .1));				
			}				
			yield return new WaitForSeconds(0.125f);			
		}				
		audio.PlayOneShot(moveAudio);
		
		available_checkhand=false;
		card_selectable=true;
		available_shuffle=true;
	}
	
	private IEnumerator Check() {	
		
		yield return new WaitForSeconds(0.5f);			
		removeAllHightligh();		
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{				
				cardBlockObjects[i,j].renderer.material.shader=darkShader;
			}
		}
		//////////////////////////////////
		//check one pairs
		yield return new WaitForSeconds(0.125f);
		StartCoroutine(checkAllOnePairs());		
		////////////////////////////////
		yield return new WaitForSeconds(1.5f);		
		
		for(int y=rows-1;y>=0;y--){
			Card[] tmp =  new Card[5];
			for (int x = 0; x < cols; ++x)
        	{
				tmp[x]=cardsOnBoard[x,y];
			}
			int hand = CheckHand.Rank(tmp,false);
			if(hand>0){					
				yield return new WaitForSeconds(0.5f);						
				combos.Add(hand);
				generateNewComboNumber=true;
				audio.PlayOneShot(handAudio);
				displayOtherCombo(hand,new Vector3(0,0,0));
				for (int x = 0; x < cols; ++x){
					SetHightlight(cardBlockObjects[x,y],new BlockPosition(x,y));
					cardBlockObjects[x,y].renderer.material.shader=lightShader;
				}
				displayYellowLine(new BlockPosition(0,y),0.0f,2);
				yield return new WaitForSeconds(0.5f);
				removeAllHightligh();
				for (int x = 0; x < cols; ++x){
					cardBlockObjects[x,y].renderer.material.shader=darkShader;
				}		
				destroyOtherCombo();
				destroyYellowLine();
			}			
		}
		/////////////////////////////////////
		yield return new WaitForSeconds(0.125f);
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{				
				cardBlockObjects[i,j].renderer.material.shader=darkShader;
			}
		}
		
		for(int x=0;x<cols;x++){
			Card[] tmp =  new Card[5];
			for (int y = 0; y < rows; y++)
        	{
				tmp[y]=cardsOnBoard[x,y];
			}
			int hand = CheckHand.Rank(tmp,false);
			if(hand>0){									
				yield return new WaitForSeconds(0.5f);		
				combos.Add(hand);
				generateNewComboNumber=true;
				displayOtherCombo(hand,new Vector3(0,0,0));
				audio.PlayOneShot(handAudio);
				for (int k = 0; k < rows; ++k){
					cardBlockObjects[x,k].renderer.material.shader=lightShader;
					SetHightlight(cardBlockObjects[x,k],new BlockPosition(x,k));
				}			
				displayYellowLine(new BlockPosition(x,0),270.0f,1);
				yield return new WaitForSeconds(0.5f);
				removeAllHightligh();
				for (int k = 0; k < rows; ++k){
					cardBlockObjects[x,k].renderer.material.shader=darkShader;
				}
				destroyOtherCombo();
				destroyYellowLine();
				
			}			
		}
		
		///////////////////////////////////////////
		yield return new WaitForSeconds(0.125f);		
		
		Card[] tmp1 =  new Card[5];
		for(int x=0;x<cols;x++){				
			tmp1[x]=cardsOnBoard[x,x];			
		}
		int hand1 = CheckHand.Rank(tmp1,false);			
		if(hand1>0){						
			yield return new WaitForSeconds(0.5f);		
			combos.Add(hand1);
			generateNewComboNumber=true;
			displayOtherCombo(hand1,new Vector3(0,0,0));
			audio.PlayOneShot(handAudio);
				
			for(int x=0;x<cols;x++){				
				cardBlockObjects[x,x].renderer.material.shader=lightShader;
				SetHightlight(cardBlockObjects[x,x],new BlockPosition(x,x));		
			}
			displayYellowLine(new BlockPosition(0,0),45.0f,3);
			yield return new WaitForSeconds(0.5f);
			removeAllHightligh();
			for(int x=0;x<cols;x++){				
				cardBlockObjects[x,x].renderer.material.shader=darkShader;		
			}
			destroyOtherCombo();
			destroyYellowLine();
		}
		/////////////////////////////////////////////
		yield return new WaitForSeconds(0.125f);
		Card[] tmp2 =  new Card[5];
		int c=0;
		for(int x=cols-1;x>=0;x--){				
			tmp2[x]=cardsOnBoard[c,x];			
			c++;
		}
		int hand2 = CheckHand.Rank(tmp2,false);			
		if(hand2>0){					
			yield return new WaitForSeconds(0.5f);	
			combos.Add(hand2);
			generateNewComboNumber=true;
			displayOtherCombo(hand2,new Vector3(0,0,0));
			audio.PlayOneShot(handAudio);
			int c1=0;
			for(int x=cols-1;x>=0;x--){	
				cardBlockObjects[c1,x].renderer.material.shader=lightShader;
				SetHightlight(cardBlockObjects[c1,x],new BlockPosition(c1,x));
				c1++;
			}		
			displayYellowLine(new BlockPosition(cols,0),45.0f,4);
			yield return new WaitForSeconds(0.5f);
			int c2=0;
			for(int x=cols-1;x>=0;x--){	
				cardBlockObjects[c2,x].renderer.material.shader=darkShader;
				c2++;
			}
			removeAllHightligh();
			destroyOtherCombo();
			destroyYellowLine();
			
		}		
		
		yield return new WaitForSeconds(0.5f);
		destroyCombo();
		in_checking_hand=false;
		available_checkhand=false;
		available_shuffle=true;		
		enemy_selectable=true;	
		
		resumeEnemyGaugeBar();
		//auto select enemy		
		selectEnemy(enemyBlockObjects[0,0]);//fixme,change enemy
		targetEnemyGameObject=enemyBlockObjects[0,0];
	}	
	
	private void resumeEnemyGaugeBar(){
		foreach(GameObject enemies in enemyBlockObjects){
			if(enemies!=null){
				enemies.transform.GetChild(2).GetComponent<SPBar>().resumeTime();
			}
		}
	}
	
	private void pauseEnemyGaugeBar(){
		foreach(GameObject enemies in enemyBlockObjects){
			if(enemies!=null){
				enemies.transform.GetChild(2).GetComponent<SPBar>().pauseTime();
			}
		}
	}
	
	private void selectSkill(){
		
	}
	
	private void attackEnemy(){		
		enemy_selectable=false;		
		StartCoroutine(attackAnimation());	
	}
	
	private IEnumerator attackAnimation(){
		//yield return new WaitForSeconds(0.5f);		
		audio.PlayOneShot(handAudio);			
		Vector3 movePos =targetEnemyGameObject.transform.position;
		movePos.x-=0.0625f;
		iTween.PunchPosition(targetEnemyGameObject, iTween.Hash("x", movePos.x));				
		
		targetEnemy.HP-=50;
		targetEnemyGameObject.transform.GetChild(1).GetComponent<HPBar>().AddjustCurrentHP(-50);
		activeMonsterGameObject.transform.GetChild(2).GetComponent<SPBar>().AddjustCurrentSP(-50);	
		
		if(targetEnemy.HP<=0){
			targetEnemy.HP=0;
		}
		if(targetEnemy.HP==0){
			enemies.Remove(targetEnemy);
			DestroyImmediate(enemyBlockObjects[0,targetEnemy.position-1]);		
			targetEnemyGameObject=null;
		}	
		reset();
		selectedCards.Clear();		
		selectedBlockObjects  = new GameObject[Screen.width, Screen.height];	
		removeObjectByTag("Picker");		
		available_shuffle=false;	
		yield return new WaitForSeconds(1.0f);
		StartCoroutine(moveOut());
		StartCoroutine(moveBackDeal());						
		swapMonster();
	}
	
	
	private void reset(){
		comboTopText=null;
		combos.Clear();
		targetEnemyGameObject=null;
		active_monster_index--;
		if(active_monster_index<0){
			active_monster_index=0;
		}
		in_checking_hand=false;
		numberOnePairs=0;
		DestroyImmediate(selectEnemyBlock);
		DestroyImmediate(largeComboNumber);
		DestroyImmediate(largeComboType);
		smallComboTypeBlock.Clear();
		smallComboNumberBlock.Clear();
		canDisplayOnePairCombo=true;
	}
	
	private void swapMonster(){
		foreach(Monster m in monsters){
			if(m.position==3){
				m.position=1;
			}else{
				m.position++;
			}			
		}	
		buildMonster();
	}
	
	private void displayOnePairCombo(){
		if(canDisplayOnePairCombo){
			audio.PlayOneShot(arcadeAudio);
			DestroyImmediate(largeComboNumber);
			if(numberOnePairs>0){
				largeComboNumber= (GameObject)Instantiate(LargeComboNumberPrefab, new Vector3(0.5f,0.5f,0), Quaternion.identity);
				iTween.ScaleTo(largeComboNumber,iTween.Hash("scale",new Vector3(0.125f,0.125f,0), "delay", .1,"oncomplete","moveOnePairsAnimationBack"));
				largeComboType = (GameObject)Instantiate(LargeComboTypePrefab, new Vector3(0.5f,0.5f,0), Quaternion.identity);
				iTween.ScaleTo(largeComboType,iTween.Hash("scale",new Vector3(0.125f,0.125f,0), "delay", .1,"oncomplete","moveOnePairsAnimationBack"));
			}
			largeComboNumber.guiTexture.texture = (Texture)Resources.Load(combos.Count.ToString()+"p");	
			canDisplayOnePairCombo=false;
		}
					
	}
	
	private void displayComboIndex(){
		DestroyImmediate(comboTopNumber);
		comboTopNumber =  (GameObject)Instantiate(ComboCenterNumberPrefab, new Vector3(0.5f,0.5f,0), Quaternion.identity);
		comboTopNumber.guiTexture.texture =(Texture)Resources.Load(combos.Count.ToString());			
		if(comboTopText==null){
			comboTopText = (GameObject)Instantiate(ComboCenterTextPrefab, new Vector3(0.5f,0.5f,0), Quaternion.identity);			
		}
	}
	
	private void destroyOtherCombo(){
		DestroyImmediate(largeComboNumber);
		DestroyImmediate(largeComboType);
	}
	
	private void displaySmallCountOnePairs(int number){		
		GameObject smallComboType=null;
		GameObject smallComboNumber=null;
		smallComboType = (GameObject)Instantiate(SmallComboTypePrefab, new Vector3(0.5f,0.5f,0.5f), Quaternion.identity);
		smallComboType.tag="SmallCombo";
		smallComboType.guiTexture.texture =(Texture)Resources.Load("onepair");
		smallComboNumber = (GameObject)Instantiate(SmallComboNumberPrefab, new Vector3(0.5f,0.5f,0.5f), Quaternion.identity);
		smallComboNumber.guiTexture.texture=(Texture)Resources.Load(number.ToString()+"p");
		smallComboNumber.tag="SmallCombo";
		
		smallComboTypeBlock.Add(smallComboType);
		smallComboNumberBlock.Add(smallComboNumber);
	}
	
	private void displayOtherCombo(int type,Vector3 rotate){
		
		scrollSmallComboUp();
		
		DestroyImmediate(largeComboNumber);
		DestroyImmediate(largeComboType);
		largeComboType =  (GameObject)Instantiate(LargeComboTypePrefab, new Vector3(0.5f,0.5f,0.5f), Quaternion.identity);
		GameObject smallComboType=null;
		
		string res="";
		int prefab_type=1;
		switch(type){
			case 1: 
				res="royalflush";
				prefab_type=2;
				break;
			case 2:
				res="straightsflush";
				prefab_type=2;
				break;
			case 3:
				res="fourofkind";
				prefab_type=2;
				break;
			case 4:
				res="fullhouse";
				break;
			case 5:
				res="flush";
				break;
			case 6:
				res="straight";
				break;
			case 7:
				res="threeofkind";
				prefab_type=2;
				break;
			case 8:
				res="twopairs";
				break;
			case 9:
				res="onepair";
				break;			
		}		
		if(prefab_type==1){
			smallComboType = (GameObject)Instantiate(SmallComboTypePrefab, new Vector3(0.5f,0.5f,0.5f), Quaternion.identity);
		}else{
			smallComboType = (GameObject)Instantiate(SmallComboTypeLargePrefab, new Vector3(0.5f,0.5f,0.5f), Quaternion.identity);
		}
		smallComboType.tag="SmallCombo";
		smallComboType.guiTexture.texture =(Texture)Resources.Load(res);		
		largeComboType.guiTexture.texture =(Texture)Resources.Load(res);		
		smallComboTypeBlock.Add(smallComboType);
	}
		
	private void destroyCombo(){	
		DestroyImmediate(comboTopNumber);
		DestroyImmediate(comboTopText);
		
		DestroyImmediate(largeComboNumber);
		DestroyImmediate(largeComboType);
		
		GameObject[] combos_list = GameObject.FindGameObjectsWithTag("SmallCombo");
		foreach(GameObject c in combos_list){
		    DestroyImmediate(c);
		}
		
	}
	
	private void selectSkill(GameObject obj){
		DestroyImmediate(selectSkillBlock);
		Vector3 tmp = obj.transform.position;
		tmp.z=0.5f;
		selectSkillBlock = (GameObject)Instantiate(PickerPrefab, tmp, Quaternion.Euler(0.0f,180.0f,0.0f));	
		selectSkillBlock.renderer.material.color = new Color(1,166.0f/255.0f,0,1);
		selectSkillBlock.transform.localScale=Vector3.one *0.9f;
		selectSkillBlock.tag="Picker";
		iTween.ColorTo(selectSkillBlock, iTween.Hash("r",180.0f/255.0f,"g",117.0f/255.0f,"b",0,"a",1,"looptype","pingPong","time",0.125f));
	}
	
	private void selectEnemy(GameObject obj){		
		switch(obj.tag){
			case "Enemy1":
				targetEnemy=enemies.Find(Monster => Monster.position ==1);
				break;
			case "Enemy2":
				targetEnemy=enemies.Find(Monster => Monster.position ==2);
				break;
			case "Enemy3":
				targetEnemy=enemies.Find(Monster => Monster.position ==3);
				break;			
		}
		DestroyImmediate(selectEnemyBlock);
		selectEnemyBlock = (GameObject)Instantiate(TargetSelectPrefab, new Vector3(obj.transform.position.x+1,3.5f,0.5f), Quaternion.Euler(0.0f,180.0f,0.0f));
		iTween.ScaleTo(selectEnemyBlock,iTween.Hash("x",1.0f,"y",1.0f,"looptype","pingPong","time",0.25));
	}
	
	
	private IEnumerator checkAllOnePairs(){
		yield return new WaitForSeconds(0.5f);
		for(int y=rows-1;y>=0;y--){
			Card[] tmp =  new Card[5];
			for (int x = 0; x < cols; ++x)
        	{
				tmp[x]=cardsOnBoard[x,y];
			}
			int hand = CheckHand.Rank(tmp,true);
			if(hand==(int)Hand.one_pair){
				yield return new WaitForSeconds(0.0625f);
				
				for (int x = 0; x < cols; ++x){
					SetHightlight(cardBlockObjects[x,y],new BlockPosition(x,y));
					cardBlockObjects[x,y].renderer.material.shader=lightShader;
				}
				displayYellowLine(new BlockPosition(0,y),0.0f,2);
				generateNewComboNumber=true;
				combos.Add(hand);
				numberOnePairs++;
			}			
		}
		
		for(int x=0;x<cols;x++){
			Card[] tmp =  new Card[5];
			for (int y = 0; y < rows; y++)
        	{
				tmp[y]=cardsOnBoard[x,y];
			}
			int hand = CheckHand.Rank(tmp,true);
			if(hand==(int)Hand.one_pair){	
				yield return new WaitForSeconds(0.0625f);
				
				for (int k = 0; k < rows; ++k){
					cardBlockObjects[x,k].renderer.material.shader=lightShader;
					SetHightlight(cardBlockObjects[x,k],new BlockPosition(x,k));
				}
				displayYellowLine(new BlockPosition(x,0),270.0f,1);
				generateNewComboNumber=true;
				combos.Add(hand);
				numberOnePairs++;
			}			
		}		
		
		Card[] tmp1 =  new Card[5];
		for(int x=0;x<cols;x++){				
			tmp1[x]=cardsOnBoard[x,x];			
		}
		int hand1 = CheckHand.Rank(tmp1,true);			
		if(hand1==(int)Hand.one_pair){		
			yield return new WaitForSeconds(0.0625f);
			
			for(int x=0;x<cols;x++){				
				cardBlockObjects[x,x].renderer.material.shader=lightShader;
				SetHightlight(cardBlockObjects[x,x],new BlockPosition(x,x));		
			}
			displayYellowLine(new BlockPosition(0,0),45.0f,3);
			numberOnePairs++;
			generateNewComboNumber=true;
			combos.Add(hand1);
		}
		Card[] tmp2 =  new Card[5];
		int c=0;
		for(int x=cols-1;x>=0;x--){				
			tmp2[x]=cardsOnBoard[c,x];			
			c++;
		}
		int hand2 = CheckHand.Rank(tmp2,true);			
		if(hand2==(int)Hand.one_pair){	
			yield return new WaitForSeconds(0.0625f);
			
			int c1=0;
			for(int x=cols-1;x>=0;x--){	
				cardBlockObjects[c1,x].renderer.material.shader=lightShader;
				SetHightlight(cardBlockObjects[c1,x],new BlockPosition(c1,x));
				c1++;
			}
			displayYellowLine(new BlockPosition(cols,0),45.0f,4);
			generateNewComboNumber=true;
			combos.Add(hand2);
			numberOnePairs++;
		}		
		displaySmallCountOnePairs(numberOnePairs);
		yield return new WaitForSeconds(1.0f);		
		removeAllHightligh();		
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{				
				cardBlockObjects[i,j].renderer.material.shader=darkShader;
			}
		}	
		DestroyImmediate(largeComboNumber);
		DestroyImmediate(largeComboType);
		destroyYellowLine();
	}
	
	private void displayYellowLine(BlockPosition pos,float rot,int type){	//type 1 horical, 2 vertical 3 cross l-r 4 cross r-l
		
		Vector3 pos3d =new Vector3(pos.x,pos.y,0.5f);
		Quaternion qua= Quaternion.Euler(rot,90.0f,1.5f);
		if(type==1){
			pos3d=new Vector3(pos.x-2.5f, pos.y-4f, 1.5f);
			qua= Quaternion.Euler(rot,90.0f,0.5f);
		}
		
		if(type==2){
			pos3d=new Vector3(pos.x-3, pos.y-3.5f, 1.5f);
			qua= Quaternion.Euler(rot,90.0f,0.5f);
		}
		
		if(type==3){
			pos3d=new Vector3(2, 1, 1.5f);
			qua= Quaternion.Euler(rot,270.0f,0.5f);
		}
		
		if(type==4){
			pos3d=new Vector3(-3.0f,1.0f, 1.5f);
			qua= Quaternion.Euler(rot,90.0f,0.5f);
		}
		GameObject lineBlock = (GameObject)Instantiate(LineRenderPrefab, pos3d, qua);	
		if(type==4||type==3){
			lineBlock.transform.GetComponent<LineRenderer>().SetPosition(1,new Vector3(0.0f,0.0f,7.0f));
		}
		lineBlock.tag="Line";
		
	}	
	
	private void destroyYellowLine(){
		GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
		foreach(GameObject l in lines){
		    DestroyImmediate(l);
		}
	}
	private void scrollSmallComboUp(){						
		foreach(GameObject g in smallComboTypeBlock){
			iTween.MoveTo(g,iTween.Hash("y",g.transform.position.y+0.03f));
		}
		
		foreach(GameObject g in smallComboNumberBlock){
			iTween.MoveTo(g,iTween.Hash("y",g.transform.position.y+0.03f));
		}
	}
} 