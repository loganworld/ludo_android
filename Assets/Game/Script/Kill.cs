using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Kill : MonoBehaviour {
    public Game game;
	private int[] saveplace=new int[]{1,9,14,22,27,35,40,48};// this values of the save position in the bord
	public Transform[] player =new Transform[4];
	public AudioSource audioKill;
	
	public void verifkill(Transform m,int w)// verification that the player after is movinng he kill one of the tokens or not 
	{// m transform that he move and int w number of it 
		
		for(int i =0; i <4 ; i++)// verifcation al the other tokens of the players that the near of them or not in the danger places
	  {
		
       if(Vector3.Distance(m.position, game.p1[i].position)<1 && Vector3.Distance(m.position, game.p1[i].position)>=0&&(w==1||w==2||w==3)&& ! Array.Exists(saveplace, element => element == game.pp1[i]))
	   {
		   
		   killplayer(game.pp1[i],game.p1[i],0,player[0].GetChild(i));
		   game.pp1[i]=0;
		   
	   }
	   else if(Vector3.Distance(m.position, game.p2[i].position)<1 && Vector3.Distance(m.position, game.p2[i].position)>=0&&(w==0||w==2||w==3)&& ! Array.Exists(saveplace, element => element == game.pp2[i]))
	   {
		   
		  killplayer(game.pp2[i],game.p2[i],1,player[1].GetChild(i));
		   game.pp2[i]=0;
		  
	   }
	    else if(Vector3.Distance(m.position, game.p3[i].position)<1 && Vector3.Distance(m.position, game.p3[i].position)>=0&&(w==1||w==0||w==3)&& ! Array.Exists(saveplace, element => element == game.pp3[i]))
	   {
		   
		   killplayer(game.pp3[i],game.p3[i],2,player[2].GetChild(i));
		    game.pp3[i]=0;
		  
	   }
	    else if(Vector3.Distance(m.position, game.p4[i].position)<1 && Vector3.Distance(m.position, game.p4[i].position)>=0&&(w==1||w==2||w==0)&& ! Array.Exists(saveplace, element => element == game.pp4[i]))
	   {
		   
		   killplayer(game.pp4[i],game.p4[i],3,player[3].GetChild(i));
		    game.pp4[i]=0;
		   
	   }
	 

	 }
   
	}
	public void killplayer(int x ,Transform y ,int w,Transform init)// function return the token kill in the init position 
	{
		     
	{
		audioKill.Play();
			StartCoroutine(delaijump( x , y,w,init));
		
	}

	}
	 IEnumerator delaijump(int x ,Transform y, int w, Transform init)// from is position to 0 positon that he out of it
    {    Transform targetWayPoint;
	 
     	
		 for(int i =x-1; i >=0 ; i--)
        {
		 yield return new WaitForSeconds(0.05f);
		targetWayPoint = game.wayPointList[w].GetChild(i);
	    y.position =  targetWayPoint.position; 
		if(i==0)
		{
			 y.position=init.position;
		}
	  
	   } 
	
	 audioKill.Pause();
	}
	
	

}
