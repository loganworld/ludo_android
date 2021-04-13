using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vspc : MonoBehaviour
{
    public Game game;
    public Player player;

    // Use this for initialization
    public void pc(int c) // this function do it the computer we he have his tourn by int c is nuber tour computer 
    { // initial this tow condition 
        player.condition1 = true;
        player.condition2 = true;

        switch (c)
        {
            case 2:

                // initialisation  
                player.turnpc = true; // he still in his tourn 
                player.nu = (Random.Range(0, 4)); //random number of token 
                player.typ1 = 'y'; // yellow because he is computer nomber 2
                game.can = true;
                game.buttondice(); //reuse function button dice 

                StartCoroutine(wait(2));

                break;

            case 3:

                player.turnpc = true;
                player.nu = (Random.Range(0, 4));
                player.typ1 = 'b';
                game.can = true;
                game.buttondice();

                StartCoroutine(wait(3));

                break;
            case 4:

                player.turnpc = true;
                player.nu = (Random.Range(0, 4));
                player.typ1 = 'r';
                game.can = true;
                game.buttondice();

                StartCoroutine(wait(4));

                break;

        }

    }
    IEnumerator wait(int m)
    {
        yield return new WaitForSeconds(1.5f);
        player.buttonmov(); //reuse the functio, buttonmove by the new varriable that he change it by random

        while (!player.condition1 && player.condition2 && game.verif) //in this condition he has other chance 
        {

            player.turnpc = true;
            game.verif = true;
            player.nu = (Random.Range(0, 4));
            player.buttonmov();
            print("cond1");

        }
        if (player.condition1 && !player.condition2) //in this condition he has other chance and he will have another turn and other bounce dice 
        {
            StartCoroutine(wait2(m));
            player.condition1 = true;
            player.condition2 = true;
            print("cond2");

        }
        else if (!player.condition1 && !player.condition2) // in this condition he has not chance to play 
        {

            player.turnpc = false;
            //game.can=true;
            //game.verif=fals
            player.condition1 = true;
            player.condition2 = true;
            print("cond3");

        }

    }
    IEnumerator wait2(int m)
    {
        yield return new WaitForSeconds(1f);
        pc(m);
    }
}