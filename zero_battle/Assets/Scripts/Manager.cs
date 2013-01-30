using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
	
    private int cols = 5;
    private int rows = 5;    
    private GameObject[,] cardBlockObjects;//store clone object
	private GameObject[,] pickerBlockObjects;//store clone object
	private GameObject[,] skillBlockObjects;//store clone object
	private GameObject[,] characterBlockObjects;//store clone object
	
	private Card[,] cardsOnBoard;//store card with 2D position
	private List<Card> cardsDraw =  new List<Card> ();//store list card on board(already draw)
	
    private int[,] selectionBlocks;
    private List<Card> selectedCards =  new List<Card> ();     
	
	private Deck pokerDeck;
	private bool can_shuffle=true;	
	
	private RaycastHit hit=new RaycastHit();
	private Ray ray=new Ray();
	public AudioClip pickAudio;
	public AudioClip moveAudio;
	public AudioClip handAudio;
	
	private Vector3[,] tmpPos=new Vector3[5, 5];
	private bool is_checkhand=false;
	private bool is_checking=false;
	private bool is_clickable=true;	
	private Shader lightShader;
	private Shader darkShader;
	
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
		pickerBlockObjects  = new GameObject[cols, rows];
		BuildBattleArea();
    }
	
	private void BuildBattleArea(){
		is_clickable=true;
		is_checkhand=false;
		is_checking=false;
		can_shuffle=true;		
		
		
		pokerDeck = new Deck();
		pokerDeck.Shuffle();
        
		skillBlockObjects = new GameObject[1,5];
		characterBlockObjects = new GameObject[1,3];
		selectionBlocks   = new int[cols,rows];
		cardsOnBoard = new Card[cols, rows];		
        for (int x = 0; x < cols; ++x)
        {
            for (int y = 0; y < rows; ++y)
            {
                cardsOnBoard[x, y] = pokerDeck.Draw();
				cardsDraw.Add(cardsOnBoard[x, y]);
				selectionBlocks[x,y]=0;
            }
        }       
		BuildGrid();
		BuildCharacter();
		BuildSkill();		
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

    private GameObject SetBlock(BlockPosition position)
    {
        GameObject newBlock = (GameObject)Instantiate(CardPrefab, Get3DPosition(position), Quaternion.Euler(0.0f,180.0f,0.0f));		
        newBlock.transform.localScale=Vector3.one *0.95f;
		newBlock.AddComponent("BoxCollider");
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
				GameObject newBlock = SetBlock(new BlockPosition(i, j));
			}			
		}
	}
	
	private void BuildCharacter(){
		for(int x=0; x<3; x++)
		{				
			BlockPosition pos = new BlockPosition(x,5);
			GameObject newBlock = (GameObject)Instantiate(CardPrefab, Get3DPosition(pos), Quaternion.Euler(0.0f,180.0f,0.0f));		
	        newBlock.transform.localScale=Vector3.one *0.95f;			
			newBlock.renderer.material.mainTexture = (Texture2D)Resources.Load("char"+(x+1));		
	        skillBlockObjects[0, x] = newBlock;
		}
	}
	
	private void BuildSkill(){
		removeObjectByTag("Skill");
		for(int y=0; y<5; y++)
		{							
			BlockPosition pos = new BlockPosition(5,y);			
			GameObject newBlock = (GameObject)Instantiate(CardPrefab, Get3DPosition(pos), Quaternion.Euler(0.0f,180.0f,0.0f));		
	        newBlock.transform.localScale=Vector3.one *0.95f;			
			//newBlock.AddComponent("BoxCollider");
			newBlock.tag="Skill";
			newBlock.renderer.material.mainTexture = (Texture2D)Resources.Load("s"+(y+1));	
			Shader newShader = Shader.Find("Transparent/Diffuse");
			newBlock.renderer.material.shader=newShader;
	        skillBlockObjects[0, y] = newBlock;
		}
	}
	
	private void EnableSkill(){
		is_clickable=false;
		Shader newShader = Shader.Find("Unlit/Transparent");
		for(int x=0; x<5; x++)
		{									
	        skillBlockObjects[0, x].renderer.material.shader = newShader;
		}	
		Shader darkShader = Shader.Find("Transparent/Diffuse");
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{				
				if(selectionBlocks[i,j]==0){					
					cardBlockObjects[i,j].renderer.material.shader=darkShader;
				}
			}
		}
	}
	
	private void DisableSkill(){
		is_clickable=true;
		Shader newShader = Shader.Find("Transparent/Diffuse");
		for(int x=0; x<5; x++)
		{									
	        skillBlockObjects[0, x].renderer.material.shader = newShader;
		}	
		Shader darkShader = Shader.Find("Unlit/Transparent");
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{				
				if(selectionBlocks[i,j]==0){
					
					cardBlockObjects[i,j].renderer.material.shader=darkShader;
				}
			}
		}
	}
	
	
	void Update(){
		
		if(is_checkhand && !is_checking){	
			is_checking=true;
			StartCoroutine(Check());
		}	
		
		if ( Input.GetMouseButtonDown(0)&&is_clickable )
	   	{
	      ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
	      if (Physics.Raycast(ray, out hit, 100.0f))
	      {							
			audio.PlayOneShot(pickAudio);
			BlockPosition pos2d=Get2DPosition(hit.transform.gameObject.transform);
			if(selectionBlocks[pos2d.x,pos2d.y]==1){
				selectionBlocks[pos2d.x,pos2d.y]=0;								
				hit.transform.gameObject.renderer.material.shader=lightShader;
				/*
				try{
					iTween.StopByName("h"+pos2d.x+"_"+pos2d.y);				
				}catch(Exception e){
					Debug.Log(e);	
				}*/				
				selectedCards.Remove(cardsOnBoard[pos2d.x,pos2d.y]);
				removeHightlightAtPostion(pos2d);
			}else{
				selectionBlocks[pos2d.x,pos2d.y]=1;
				//hit.transform.gameObject.renderer.material.mainTexture = (Texture2D)Resources.Load(cardsOnBoard[pos2d.x,pos2d.y].ToString()+"D");				
				hit.transform.gameObject.renderer.material.shader = darkShader;
				selectedCards.Add(cardsOnBoard[pos2d.x,pos2d.y]);			
				SetHightlight(hit.transform.gameObject,pos2d);
			}
	      }
	   	}
		if(selectedCards.Count>=5 && can_shuffle){ 
			 EnableSkill();
			 ShuffleCard();
		}		
	}		
	
	private void SetHightlight(GameObject obj,BlockPosition pos){
		Vector3 tmp = obj.transform.position;
		tmp.z=0.5f;
		GameObject pickerBlock = (GameObject)Instantiate(PickerPrefab, tmp, Quaternion.Euler(0.0f,180.0f,0.0f));	
		pickerBlockObjects[pos.x, pos.y] = pickerBlock;
		pickerBlock.tag="Picker";	
		pickerBlock.renderer.material.color = new Color(1,166.0f/255.0f,0,1);
		iTween.ColorTo(pickerBlock, iTween.Hash("name","h"+pos.x+"_"+pos.y,"r",180.0f/255.0f,"g",117.0f/255.0f,"b",0,"a",1,"looptype","pingPong","time",0.125f));
		pickerBlock.transform.localScale=Vector3.one *0.95f;	
	}
	
	private void removeHightlightAtPostion(BlockPosition pos){
		DestroyImmediate(pickerBlockObjects[pos.x,pos.y]);
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
		can_shuffle=false;				
		StartCoroutine(MoveOut());
		StartCoroutine(MoveBack());			
	}
	
	private IEnumerator MoveOut() {				
		
		//yield return new WaitForSeconds(0.5f);
		audio.PlayOneShot(moveAudio);		
		for(int k=rows-1;k>=0;k--){
			for(int y=0;y<cols;y++){
				if(selectionBlocks[y,k]==0){
					Vector3 movePos =cardBlockObjects[y,k].transform.position;
					tmpPos[y,k]=cardBlockObjects[y,k].transform.position;
					movePos.x-=150;
					//HOTween.To(cardBlockObjects[y,k].transform, 1.0f, new TweenParms().Prop("position",movePos).Ease(EaseType.EaseInOutSine,0.5f));
					iTween.MoveTo(cardBlockObjects[y,k], iTween.Hash("x", movePos.x, "easeType", "EaseInOutSine", "delay", .1));
				}				
			}	
			if(k>0){
				yield return new WaitForSeconds(0.0625f);
			}
		}		
	}
	
	
	private IEnumerator MoveBack() {
		
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
				if(selectionBlocks[i,j]==0){
					cardsOnBoard[i,j] =pokerDeck.Draw();	
				}
				cardsDraw.Add(cardsOnBoard[i,j]);
				cardBlockObjects[i,j].transform.localScale=Vector3.one *0.95f;
				cardBlockObjects[i,j].renderer.material.mainTexture = (Texture2D)Resources.Load(cardsOnBoard[i,j].ToString());
			}
		}
		
		for(int k=rows-1;k>=0;k--){
			for(int y=0;y<cols;y++){
				if(selectionBlocks[y,k]==0){						
					//HOTween.To(cardBlockObjects[y,k].transform, 1.0f, new TweenParms().Prop("position",tmpPos[y,k]).Ease(EaseType.EaseInOutSine,0.5f));
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
				selectionBlocks[i,j]=0;
			}
		}
		is_checkhand=true;
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
		////////////////////////////////
		yield return new WaitForSeconds(0.125f);
		for(int y=rows-1;y>=0;y--){
			Card[] tmp =  new Card[5];
			for (int x = 0; x < cols; ++x)
        	{
				tmp[x]=cardsOnBoard[x,y];
			}
			int hand = CheckHand.Rank(tmp);
			if(hand>0){									
				yield return new WaitForSeconds(0.5f);
				audio.PlayOneShot(handAudio);
				for (int x = 0; x < cols; ++x){
					SetHightlight(cardBlockObjects[x,y],new BlockPosition(x,y));
					cardBlockObjects[x,y].renderer.material.shader=lightShader;
				}
				yield return new WaitForSeconds(0.5f);
				removeAllHightligh();
				for (int x = 0; x < cols; ++x){
					cardBlockObjects[x,y].renderer.material.shader=darkShader;
				}
				/*
				yield return new WaitForSeconds(0.125f);				
				newShader = Shader.Find("Transparent/Diffuse");
				for (int x = 0; x < cols; ++x){
					cardBlockObjects[x,y].renderer.material.shader=newShader;
				}*/	
			}			
		}
		/////////////////////////////////////
		yield return new WaitForSeconds(0.125f);
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{				
				Shader newShader = Shader.Find("Transparent/Diffuse");
				cardBlockObjects[i,j].renderer.material.shader=newShader;
			}
		}
		
		for(int x=0;x<cols;x++){
			Card[] tmp =  new Card[5];
			for (int y = 0; y < rows; y++)
        	{
				tmp[y]=cardsOnBoard[x,y];
			}
			int hand = CheckHand.Rank(tmp);
			if(hand>0){									
				yield return new WaitForSeconds(0.5f);
				audio.PlayOneShot(handAudio);
				for (int k = 0; k < rows; ++k){
					cardBlockObjects[x,k].renderer.material.shader=lightShader;
					SetHightlight(cardBlockObjects[x,k],new BlockPosition(x,k));
				}			
				yield return new WaitForSeconds(0.5f);
				removeAllHightligh();
				for (int k = 0; k < rows; ++k){
					cardBlockObjects[x,k].renderer.material.shader=darkShader;
				}
				/*
				yield return new WaitForSeconds(0.125f);
				newShader = Shader.Find("Transparent/Diffuse");
				for (int k = 0; k < rows; ++k){
					cardBlockObjects[x,k].renderer.material.shader=newShader;
				}*/
			}			
		}
		
		///////////////////////////////////////////
		yield return new WaitForSeconds(0.125f);		
		
		Card[] tmp1 =  new Card[5];
		for(int x=0;x<cols;x++){				
			tmp1[x]=cardsOnBoard[x,x];			
		}
		int hand1 = CheckHand.Rank(tmp1);			
		if(hand1>0){									
			yield return new WaitForSeconds(0.5f);
			audio.PlayOneShot(handAudio);
			/*
			Shader newShader = Shader.Find("Unlit/Transparent");
			for(int x=0;x<cols;x++){				
				cardBlockObjects[x,x].renderer.material.shader=newShader;		
			}				
			*/	
			for(int x=0;x<cols;x++){				
				cardBlockObjects[x,x].renderer.material.shader=lightShader;
				SetHightlight(cardBlockObjects[x,x],new BlockPosition(x,x));		
			}
			
			yield return new WaitForSeconds(0.5f);
			removeAllHightligh();
			for(int x=0;x<cols;x++){				
				cardBlockObjects[x,x].renderer.material.shader=darkShader;		
			}
		}
		/////////////////////////////////////////////
		yield return new WaitForSeconds(0.125f);
		Card[] tmp2 =  new Card[5];
		int c=0;
		for(int x=cols-1;x>=0;x--){				
			tmp2[x]=cardsOnBoard[c,x];			
			c++;
		}
		int hand2 = CheckHand.Rank(tmp2);			
		if(hand2>0){									
			yield return new WaitForSeconds(0.5f);
			audio.PlayOneShot(handAudio);
			int c1=0;
			for(int x=cols-1;x>=0;x--){	
				cardBlockObjects[c1,x].renderer.material.shader=lightShader;
				SetHightlight(cardBlockObjects[c1,x],new BlockPosition(c1,x));
				c1++;
			}		
			yield return new WaitForSeconds(0.5f);
			int c2=0;
			for(int x=cols-1;x>=0;x--){	
				cardBlockObjects[c2,x].renderer.material.shader=darkShader;
				c2++;
			}
			removeAllHightligh();
		}		
		
		is_checking=false;
		is_checkhand=false;
		can_shuffle=true;
		
		//DisableSkill();		
		BuildBattleArea();	
		BuildSkill();
	}
	
} 