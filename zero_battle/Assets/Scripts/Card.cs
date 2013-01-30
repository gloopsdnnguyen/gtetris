using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

enum Shapes
{
	spade = 1,
	heart,
	diamond,
	club
};

enum Signs
{
	T = 10,
	J,
	Q,
	K,
	A
};

public class Card
{
	public int Value;
	public int Shape;
	public string Image="AR";
	
	public Card (int Value, int Shape)
	{
		this.Value = Value;
		this.Shape = Shape;
	}

	public override string ToString ()
	{
		return (Value + "$" + Shape + "$");
	}
}

public class Deck
{
	private List<Card> deck = new List<Card> ();

	public Deck ()
	{
		for (int i = 2; i <= 14; i++)
			for (int n = (int)Shapes.spade; n <= (int)Shapes.club; n++)
				deck.Add (new Card (i, n));		
	}	
		
	public void Shuffle ()
	{
		List<Card> tmp = new List<Card> (52);
		System.Random rand = new System.Random ();
		for (int i = 0; i < 52; i++) {
			int c = rand.Next (deck.Count);
			tmp.Add (deck [c]);
			deck.RemoveAt (c);
		}
		deck = tmp;
	}

	public Card Draw ()
	{
		Card tmp = deck[0];
		deck.RemoveAt(0);
		return tmp;
	}	
	
	public void Add (Card a)
	{
		deck.Add (a);
	}

	public void Add (Card[] a)
	{
		foreach (Card i in a)
			deck.Add (i);
	}
	
	public int Count {
		get {
			int i = 0;
			foreach (Card n in deck)
				i++;
			return i;
		}
	}
}
