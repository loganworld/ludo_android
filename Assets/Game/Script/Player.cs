using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnitySocketIO;
using UnitySocketIO.Events;

public class Player : MonoBehaviour
{
    public Game game;
    public int nu;// number if token
    public char typ1, typ2;
    public Manager manager;
    public GameObject buttonbouncedive;// object button bounce dice

    public bool turnpc = false, condition1 = false, condition2 = false;//condition belongue to the computer playing

    void Update()
    {
        if (Global.isMultiplayer)
        {
            if (Global.myTurn != manager.num)
            {
                buttonbouncedive.SetActive(false);
            }
            else
            {
                buttonbouncedive.SetActive(true);
            }
        }
        else
        {
            if (turnpc && manager.num != 1)
            {
                buttonbouncedive.SetActive(false);//desable the boutton of bounce dice for the humain player
            }
            else if (!turnpc && manager.num == 1)
            {
                buttonbouncedive.SetActive(true);
            }
        }
        
    }
    public void buttonmov()//function when pressing on a token 
    {

        if (Global.isMultiplayer && Global.myTurn != manager.num)
        {
            return;
        }

        moveAction();

    }

    /*****************/
    public void moveAction(string receivedtyp = "")
    {
        if (Global.isMultiplayer && receivedtyp != "")
        {
            string typ = receivedtyp;
            typ1 = typ[0];
            typ2 = typ[1];
            nu = (int)char.GetNumericValue(typ2);
        }
        else if (!turnpc)//when is it not the turn of the computer we use typ1 and type2 a variable 
        {
            string typ = EventSystem.current.currentSelectedGameObject.name;// name of the button we press 
            typ1 = typ[0];// type1 take the alpahbet g,b,r,y =yellow ,green , red,bleau 
            typ2 = typ[1];//type2 take the number of token 
            nu = (int)char.GetNumericValue(typ2);// converte from chat to int

            if (Global.isMultiplayer)
            {
                SocketIOController.instance.Emit("click", JsonUtility.ToJson(new PlayerTurn(1, Global.myTurn, 0, typ)));
            }
        }


        switch (typ1)
        {// case 'g' meaninig the player press on the green token 
            case 'g':
                if (manager.veriftour(1) && game.verif) // conditon initial befor usinig this function 
                {
                    if ((game.pp1[nu] == 0 && game.facedice == 6))///********************************************** *///in this situation the tokin is in inti position and ahs 6 so he can move 
                    {
                        game.pp1[nu] = game.moveplayer(game.pp1[nu], game.p1[nu], 0);



                    }
                    else if ((game.pp1[nu] != 0 && game.facedice == 6))/////*********************************************** */
                    {                                            // the token that we pressed he is not in init position but he has 6
                        if (game.pp1[nu] + game.facedice <= 57)//veification that he don't pass the limit of the end
                        {

                            game.pp1[nu] = game.moveplayer(game.pp1[nu], game.p1[nu], 0);//normal movement



                        }
                        else if (manager.automatiquetour(1) | game.pp1[0] == 0 | game.pp1[1] == 0 | game.pp1[2] == 0 | game.pp1[3] == 0)//the player he is wrong he has some of his token can move it 
                        {
                            //	print("there is other1");
                            game.can = false;

                        }
                        else
                        {
                            // is he don't have any chance to play we pass to change the tourn
                            StartCoroutine(wait());


                        }
                    }
                    else if ((game.pp1[nu] != 0) && game.facedice != 6)///********************************************************* */
                    {
                        if (game.pp1[nu] == 57)// if he not pass the limite of the end 
                        {
                            //	print("there is other2");
                            // but we false can to disable bounce dice again
                            game.can = false;
                        }                                           // he has  number not 6 and he is not in initial position 
                        else if (game.pp1[nu] + game.facedice <= 57)// if he not pass the limite of the end 
                        {
                            // the tojken can move
                            game.pp1[nu] = game.moveplayer(game.pp1[nu], game.p1[nu], 0);
                            //afteter he move we call change tour 
                            StartCoroutine(wait());


                        }
                        else if (manager.automatiquetour(1))//her is there are othere option for the player 
                        {
                            //	print("there is other2");
                            // but we false can to disable bounce dice again
                            game.can = false;

                        }
                        else if (!manager.automatiquetour(1))
                        {
                            // the is not option for the player automatique change tour
                            StartCoroutine(wait());


                        }

                    }
                    else if (game.pp1[nu] == 0 && game.facedice != 6) ///******************************************************* */
                    {
                        if (!manager.automatiquetour(1))// not option for the player we change the tour 
                        {

                            StartCoroutine(wait());



                        }
                        else
                        {
                            //	 print("there is other3");
                            // in this condition wwe don't do nothing because there is other option for th eplayer

                        }


                    }


                }


                break;
            // case 'y' meaninig the player press on the yellow token
            case 'y':
                if (manager.veriftour(2) && game.verif)
                {
                    if ((game.pp2[nu] == 0 && game.facedice == 6))///********************************************** */
                    {
                        game.pp2[nu] = game.moveplayer(game.pp2[nu], game.p2[nu], 1);

                        // this tow conditon belongue to the computer to know his sitiuation in the calss vspc
                        condition1 = true;
                        condition2 = false;

                    }
                    else if ((game.pp2[nu] != 0 && game.facedice == 6))/////*********************************************** */
                    {

                        if (game.pp2[nu] + game.facedice <= 57)
                        {

                            game.pp2[nu] = game.moveplayer(game.pp2[nu], game.p2[nu], 1);

                            // this tow conditon belongue to the computer to know his sitiuation in the calss vspc
                            condition1 = true;
                            condition2 = false;

                        }
                        else if (manager.automatiquetour(2) | game.pp2[0] == 0 | game.pp2[1] == 0 | game.pp2[2] == 0 | game.pp2[3] == 0)
                        {
                            //	print("there is other");
                            // this tow conditon belongue to the computer to know his sitiuation in the calss vspc
                            game.can = false;
                            condition1 = false;
                            condition2 = true;

                        }
                        else
                        {
                            // this tow conditon belongue to the computer to know his sitiuation in the calss vspc

                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());
                        }
                    }
                    else if ((game.pp2[nu] != 0) && game.facedice != 6)///********************************************************* */
                    {
                        if (game.pp2[nu] == 57)// if he not pass the limite of the end 
                        {
                            //	print("there is other2");
                            // but we false can to disable bounce dice again
                            game.can = false;
                            condition1 = false;
                            condition2 = true;
                        }
                        else if (game.pp2[nu] + game.facedice <= 57)
                        {

                            game.pp2[nu] = game.moveplayer(game.pp2[nu], game.p2[nu], 1);
                            // this tow conditon belongue to the computer to know his sitiuation in the calss vspc
                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());

                        }
                        else if (manager.automatiquetour(2))
                        {
                            //	print("there is other");
                            game.can = false;
                            condition1 = false;
                            condition2 = true;

                        }
                        else if (!manager.automatiquetour(2))
                        {
                            // this tow conditon belongue to the computer to know his sitiuation in the calss vspc

                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());

                        }

                    }
                    else if (game.pp2[nu] == 0 && game.facedice != 6) ///******************************************************* */
                    {
                        if (!manager.automatiquetour(2))
                        {
                            // this tow conditon belongue to the computer to know his sitiuation in the calss vspc

                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());


                        }
                        else
                        {
                            //	 print("there is other");
                            game.can = false;
                            condition1 = false;
                            condition2 = true;

                        }


                    }


                }


                break;
            case 'b':
                if (manager.veriftour(3) && game.verif)
                {
                    if ((game.pp3[nu] == 0 && game.facedice == 6))///********************************************** */
                    {
                        game.pp3[nu] = game.moveplayer(game.pp3[nu], game.p3[nu], 2);


                        condition1 = true;
                        condition2 = false;

                    }
                    else if ((game.pp3[nu] != 0 && game.facedice == 6))/////*********************************************** */
                    {
                        if (game.pp3[nu] + game.facedice <= 57)
                        {

                            game.pp3[nu] = game.moveplayer(game.pp3[nu], game.p3[nu], 2);


                            condition1 = true;
                            condition2 = false;

                        }
                        else if (manager.automatiquetour(3) | game.pp3[0] == 0 | game.pp3[1] == 0 | game.pp3[2] == 0 | game.pp3[3] == 0)
                        {
                            //	print("there is other");
                            game.can = false;
                            condition1 = false;
                            condition2 = true;

                        }
                        else
                        {


                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());

                        }
                    }
                    else if ((game.pp3[nu] != 0) && game.facedice != 6)///********************************************************* */
                    {
                        if (game.pp3[nu] == 57)// if he not pass the limite of the end 
                        {
                            //	print("there is other2");
                            // but we false can to disable bounce dice again
                            game.can = false;
                            condition1 = false;
                            condition2 = true;
                        }

                        else if (game.pp3[nu] + game.facedice <= 57)
                        {

                            game.pp3[nu] = game.moveplayer(game.pp3[nu], game.p3[nu], 2);
                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());



                        }
                        else if (manager.automatiquetour(3))
                        {
                            //	print("there is other");
                            game.can = false;
                            condition1 = false;
                            condition2 = true;

                        }
                        else if (!manager.automatiquetour(3))
                        {


                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());

                        }

                    }
                    else if (game.pp3[nu] == 0 && game.facedice != 6) ///******************************************************* */
                    {
                        if (!manager.automatiquetour(3))
                        {


                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());


                        }
                        else
                        {
                            // print("there is other");
                            game.can = false;
                            condition1 = false;
                            condition2 = true;

                        }


                    }


                }


                break;

            case 'r':
                if (manager.veriftour(4) && game.verif)
                {
                    if ((game.pp4[nu] == 0 && game.facedice == 6))///********************************************** */
                    {
                        game.pp4[nu] = game.moveplayer(game.pp4[nu], game.p4[nu], 3);


                        condition1 = true;
                        condition2 = false;

                    }
                    else if ((game.pp4[nu] != 0 && game.facedice == 6))/////*********************************************** */
                    {
                        if (game.pp4[nu] + game.facedice <= 57)
                        {

                            game.pp4[nu] = game.moveplayer(game.pp4[nu], game.p4[nu], 3);


                            condition1 = true;
                            condition2 = false;

                        }
                        else if (manager.automatiquetour(4) | game.pp4[0] == 0 | game.pp4[1] == 0 | game.pp4[2] == 0 | game.pp4[3] == 0)
                        {
                            //	print("there is other");
                            game.can = false;
                            condition1 = false;
                            condition2 = true;

                        }
                        else
                        {


                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());
                        }
                    }
                    else if ((game.pp4[nu] != 0) && game.facedice != 6)///********************************************************* */
                    {
                        if (game.pp4[nu] == 57)// if he not pass the limite of the end 
                        {
                            //	print("there is other2");
                            // but we false can to disable bounce dice again
                            game.can = false;
                            condition1 = false;
                            condition2 = true;
                        }

                        else if (game.pp4[nu] + game.facedice <= 57)
                        {

                            game.pp4[nu] = game.moveplayer(game.pp4[nu], game.p4[nu], 3);
                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());

                        }
                        else if (manager.automatiquetour(4))
                        {
                            //	print("there is other");
                            game.can = false;
                            condition1 = false;
                            condition2 = true;

                        }
                        else if (!manager.automatiquetour(4))
                        {


                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());

                        }

                    }
                    else if (game.pp4[nu] == 0 && game.facedice != 6) ///******************************************************* */
                    {
                        if (!manager.automatiquetour(4))
                        {


                            condition1 = false;
                            condition2 = false;
                            StartCoroutine(wait());


                        }
                        else
                        {
                            //	 print("there is other");
                            game.can = false;
                            condition1 = false;
                            condition2 = true;

                        }


                    }


                }


                break;

        }
    }


    // waitting to synchronise the game and ivoid the debug
    IEnumerator wait()
    {
        game.can = false;
        game.verif = false;
        turnpc = false;
        yield return new WaitForSeconds(1f);
        // change tour
        manager.tour();


    }
}

