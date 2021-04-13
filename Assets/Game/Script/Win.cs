using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Win : MonoBehaviour {
public Game game;
public Manager manager;

public int win1,rang=0,w1=0,w2=0,w3=0,w4=0;// variable to verifiaction 
public GameObject[] wingamepanel=new GameObject[4];// win panel of every player
public Text[] range=new Text[4];
	// Use this for initialization
	 void Start() {
		 w1=0;
		 w2=0;
		 w3=0;
		 w4=0;
		
	}
	public void verifwin(int c)// verification if the number of player win or not
	{
		switch (c)
        {
        case 0:
		wingame(c);
		if(w1==1)//if he win we pass to other turn of player 
		{
			 rang+=1;
			 range[0].text=(rang).ToString();
           manager.tour();
		}
		

		break;
		case 1:
		wingame(c);
		if(w2==2)
		{ rang+=1;
			 range[1].text=(rang).ToString();
         manager.tour();
		}
		
		
		break;
		case 2:
		wingame(c);
		if(w3==3)
		{
			 rang+=1;
			 range[2].text=(rang).ToString();
         manager.tour();
		}
		
		
		break;
		case 3:
		wingame(c);
		if(w4==4)
		{
			rang+=1;
			 range[3].text=(rang).ToString();
         manager.tour();
		}
		
		
		break;
		}
	}
	public void wingame (int c)// verification if all the token of the player in the fianl position
	{
		switch (c)
        {
        case 0:
       
		win1=0;
         for(int i =0; i <4 ; i++)
		 {
			 if(game.pp1[i]==57)
		 {
           win1+=1;
			
		 }
		 if(win1==4)
		 {w1=1;
			 wingamepanel[0].SetActive(true);
			
		 }
		 

		 }
		 
		 
        break;
		case 1:
		win1=0;
         for(int i =0; i <4 ; i++)
		 {
			 if(game.pp2[i]==57)
		 {
           win1+=1;
			
		 }
		 if(win1==4)
		 {
		     w2=2;
			 wingamepanel[1].SetActive(true);
			
			

		 }
		
		 }
        break;
		case 2:
		
		win1=0;
         for(int i =0; i <4 ; i++)
		 {
			 if(game.pp3[i]==57)
		 {
           win1+=1;
			
		 }
		 if(win1==4)
		 {
		 w3=3;
			 wingamepanel[2].SetActive(true);
		 }
		
		 }
        break;
		case 3:
		
       win1=0;
         for(int i =0; i <4 ; i++)
		 {
			 if(game.pp4[i]==57)
		 {
           win1+=1;
			
		 }
		 if(win1==4)
		 {
		 w4=4;
			 wingamepanel[3].SetActive(true);
			 
		 	 

		 }
		
		 }
		 
        break;
		}
	       
		
 
	}
}
