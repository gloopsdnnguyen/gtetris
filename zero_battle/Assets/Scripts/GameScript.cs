using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameScript : MonoBehaviour {
	
	public int cols=5;
	public int rows=5;
	public int cardW=100;
	public int cardH=100;
	public bool canPickCard=true;
	
	public List<Card> aCards = new List<Card> ();
	public Card[,] aGrid;
	public List<Card> aCardsPicked = new List<Card> ();
	public int[,] aPositionsPicked;
	public GUIStyle cardStyle;
	public Deck pokerDeck;
	private bool isShuffle=true;
	private bool[,] btnEnabled;
	private bool is_new_deck=false;
	
	void Start () {
		pokerDeck = new Deck();
		pokerDeck.Shuffle();
		aGrid = new Card[cols,rows];
		aPositionsPicked = new int[cols,rows];
		btnEnabled = new bool[cols,rows];
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{
				aGrid[i,j] = pokerDeck.Draw();
				aCards.Add(aGrid[i,j]);
				aPositionsPicked[i,j]= 0;
				btnEnabled[i,j]= true;
			}
		}
	}
	
	void OnGUI () {
		GUILayout.BeginArea (new Rect (0,0,Screen.width,Screen.height));
		BuildEnemy();
		GUILayout.EndArea();
		GUILayout.BeginArea (new Rect (0,Screen.width*0.25f,Screen.width,Screen.height)); 					
		if(aCardsPicked.Count>=5) ShuffleCard();
		BuildGrid();
		GUILayout.EndArea();
	}
	
	private void BuildGrid()
	{
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		for(int i=0; i<rows; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();			
			for(int j=0; j<cols; j++)
			{
				Card card = aGrid[i,j]; 
				GUI.enabled = btnEnabled[i,j];
				if(GUILayout.Button((Texture)Resources.Load(card.ToString()),cardStyle,GUILayout.Width(cardW)))
				{					
					if(canPickCard)
					{
						aCardsPicked.Add(card);
						aPositionsPicked[i,j]=1;
						btnEnabled[i,j]=false;
					}	
					Debug.Log("click"+i+" "+j);
				}
				GUI.enabled = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal(); 
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}	
	
	private void BuildEnemy(){
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label((Texture)Resources.Load("10000"),GUILayout.Width(200));		
		GUILayout.Label((Texture)Resources.Load("10090"),GUILayout.Width(200));
		GUILayout.Label((Texture)Resources.Load("10050"),GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal(); 
	}		
	
	private void ShuffleCard(){				
		foreach(Card _card in aCards){
			if(!aCardsPicked.Contains(_card)){				
				pokerDeck.Add(_card);
			}			
		}			
		aCards = new List<Card> ();
		aCardsPicked= new List<Card>();	
		for(int i=0; i<rows; i++)
		{					
			for(int j=0; j<cols; j++)
			{
				if(aPositionsPicked[i,j]==0){
					aGrid[i,j] =pokerDeck.Draw();									
				}
				aCards.Add(aGrid[i,j]);
				btnEnabled[i,j]=true;
				aPositionsPicked[i,j] = 0;
			}
		}						
	}		
	
}
