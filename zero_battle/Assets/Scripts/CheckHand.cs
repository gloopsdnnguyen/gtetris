using UnityEngine;
using System;
using System.Collections;

enum Hand
{
	royal_flush = 1,
	straight_flush,
	four_kind,
	full_house,
	flush,
	straight,
	three_kind,
	two_pair,
	one_pair
};


static public class CheckHand
{
	public static int Rank(Card[] a)
        {
		
            int[] b = new int[a.Length];
            for (int i = 0; i < a.Length; i++){
                b[i] = a[i].Value;
			}
            Array.Sort(b);//important

            bool flush = true;//check flush
            for (int i = 0; i < 4; i++){
                if (a[i].Shape != a[i + 1].Shape)
                    flush = false;
			}
			
            bool straight = true;//straight
            for (int i = 0; i < 4; i++){
                if (b[i + 1] != b[i] + 1){
                    straight = false;
				}
            	if (b[0] == 2 && b[1] == 3 && b[2] == 4 && b[3] == 5 && b[4] == 14){//A,2,3,4,5
                	straight = true;
				}
			}
			
            if (flush && straight) {
				if(a[0].Value==(int)Shapes.diamond){
					return (int)Hand.royal_flush;
				}else{
					return (int)Hand.straight_flush;
				}
				
			}
            if (flush){ return (int)Hand.flush;}
            if (straight){ return (int)Hand.straight;}
            if (b[0] == b[3] || b[1] == b[4]){ //four of a kind
                return (int)Hand.four_kind;
			}
            bool threesome = false;
            if (b[0] == b[2] || b[1] == b[3] || b[2] == b[4]){
                threesome = true;
			}
            //check pairs
            int tmp1 = 0, tmp2 = 0;
            for (int i = 0; i < b.Length; i++)
            {
                int count = 0;
                for (int n = 0; n < b.Length; n++)
                    if (b[i] == b[n]){
                        count++;
					}
                	if (count == 2)
                	{
                    	if (tmp1 > 0 && b[i] != tmp1){
							tmp2 = b[i];
						}
                    	else { 
							tmp1 = b[i]; 
						}
                	}
            }
            if (threesome && tmp1 > 0) return (int)Hand.full_house;//full house
            if (threesome) return (int)Hand.three_kind;//three of a kind

            if (tmp1 > 0 && tmp2 > 0){
                return (int)Hand.two_pair;// 2 pairs
			}
			
            if (tmp1 > 0){//1 pairs
                return (int)Hand.one_pair;
			}
            return 0;
     }
}