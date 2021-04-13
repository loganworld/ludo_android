using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UnitySocketIO;
using UnitySocketIO.Events;
using System;
using UnityEngine.Networking;

public class Manager : MonoBehaviour
{
    public Game game;
    public Player player;
    public Win win;
    public Vspc vsps;
    public Text V1, V2;
    public bool v = false;
    public string[] listplayer = new string[2];
    public int numplayer, num = 0;
    
    public GameObject PanelEnd;
    public GameObject objEndText;

    public GameObject objSaveButton;

    SocketIOController socket;

    // Use this for initialization
    void Start()
    {
        PanelEnd.SetActive(false);

        if (Global.isLoading)
        {
            V1.text = "VSPc";
            V2.text = Global.savedData.cntPlayers.ToString();
            StartCoroutine(LoadGame());

        }

        listplayer[0] = V1.text; // take type of game humaine or vs computer 

        
        socket = SocketIOController.instance;

        Global.isMultiplayer = false;

        num = 1;

        if (listplayer[0] == "VSPer")
        {
            objSaveButton.SetActive(false);

            V2.text = Global.room.totCnt.ToString();

            Global.isMultiplayer = true;
            socket.On("gave up", GaveUp);
            socket.On("gameOver", EndGame);

            if (Global.myTurn != num)
            {
                player.buttonbouncedive.SetActive(false);
            }
            else
            {
                player.buttonbouncedive.SetActive(true);
            }
        }
        else
        {
            objSaveButton.SetActive(true);

            
            
        }

        listplayer[1] = V2.text; // take number of the player they wuill play 

        int.TryParse(V2.text, out numplayer);
    }

    private void EndGame(SocketIOEvent obj)
    {
        PanelEnd.SetActive(true);
        objEndText.SetActive(false);
    }

    void Move(int x, Transform y, int w)
    {
        if (x <= 0)
        {
            return;
        }

        Transform targetWayPoint;
        targetWayPoint = game.wayPointList[w].GetChild(x - 1);
        y.position = targetWayPoint.position;
    }

    IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(0.3f);

        game.initial = Global.savedData.initial;

        for (int i = 0; i < 4; i++)
        {
            game.pp1[i] = Global.savedData.positions[i];
            Move(game.pp1[i], game.p1[i], 0);
        }

        for (int i = 0; i < 4; i++)
        {
            game.pp2[i] = Global.savedData.positions[i + 4];
            Move(game.pp2[i], game.p2[i], 1);
        }

        for (int i = 0; i < 4; i++)
        {
            game.pp3[i] = Global.savedData.positions[i + 8];
            Move(game.pp3[i], game.p3[i], 2);
        }

        for (int i = 0; i < 4; i++)
        {
            game.pp4[i] = Global.savedData.positions[i + 12];
            Move(game.pp4[i], game.p4[i], 3);
        }


        num = Global.savedData.turn;
        if (num > 1)
        {
            vsps.pc(num);
        }

    }

    void SavePlayer(int[] positions)
    {
        for (int i = 0; i < 4; i++)
        {
            Global.savedData.positions.Add(positions[i]);
        }
    }

    public void SaveAndExit()
    {
        if (Global.savedData.positions != null && Global.savedData.positions.Count > 0)
        {
            Global.savedData.positions.Clear();
        }
        
        Global.savedData.turn = num;
        Global.savedData.cntPlayers = numplayer;
        Global.savedData.initial = game.initial;

        SavePlayer(game.pp1);
        SavePlayer(game.pp2);
        SavePlayer(game.pp3);
        SavePlayer(game.pp4);

        WWWForm formData = new WWWForm();
        formData.AddField("data", JsonUtility.ToJson(Global.savedData));
        formData.AddField("userId", Global.m_user.id.ToString());


        string requestURL = Global.currentDomain + "/api/savegame";

        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);

        www.SetRequestHeader("Accept", "application/json");
        www.uploadHandler.contentType = "application/json";
        StartCoroutine(iRequest(www));
    }

    IEnumerator iRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //objFailed.SetActive(true);
            yield break;
        }

        string resultData = www.downloadHandler.text;

        if (string.IsNullOrEmpty(resultData))
        {
            Debug.Log("Result Data Empty");
            // objFailed.SetActive(true);
            yield break;
        }

        SceneManager.LoadScene("main");
    }

    private void GaveUp(SocketIOEvent socketIOEvent)
    {
        string otherplayer_turn = socketIOEvent.data;

        if(otherplayer_turn != Global.myTurn.ToString()) {
            Global.m_user.score++;
            socket.Emit("increaseScore", JsonUtility.ToJson(Global.m_user));


            User winUser = new User();
            winUser.name = Global.m_user.name;
            winUser.address = Global.room.id;

            socket.Emit("set winner", JsonUtility.ToJson(winUser));
        }

        PanelEnd.SetActive(true);
        objEndText.SetActive(true);
    }

    public bool veriftour(int c) // verification to tour 
    {
        if (c == num)
            return true;
        else return false;

    }

    public bool automatiquetour(int c) // function return true if one or more token of the player when we add to his positon not initial <57
    {
        switch (c)
        {
            case 1:

                for (int i = 0; i < 4; i++)
                {
                    if (game.pp1[i] != 0 && game.pp1[i] + game.facedice < 57)
                    {
                        return true;

                    }

                }

                return false;

            case 2:

                for (int i = 0; i < 4; i++)
                {
                    if (game.pp2[i] != 0 && game.pp2[i] + game.facedice < 57)
                    {
                        return true;

                    }

                }

                return false;

            case 3:

                for (int i = 0; i < 4; i++)
                {
                    if (game.pp3[i] != 0 && game.pp3[i] + game.facedice < 57)
                    {
                        return true;

                    }

                }

                return false;

            case 4:

                for (int i = 0; i < 4; i++)
                {
                    if (game.pp4[i] != 0 && game.pp4[i] + game.facedice < 57)
                    {
                        return true;

                    }

                }

                return false;

        }

        return v;

    }
    public bool automatiquetour2(int c) // function return true if one or more token of the player when we add to his positon not initial <=57
    {
        switch (c)
        {
            case 1:

                for (int i = 0; i < 4; i++)
                {
                    if (game.pp1[i] != 0 && game.pp1[i] + game.facedice <= 57)
                    {
                        return true;

                    }

                }

                return false;

            case 2:

                for (int i = 0; i < 4; i++)
                {
                    if (game.pp2[i] != 0 && game.pp2[i] + game.facedice <= 57)
                    {
                        return true;

                    }

                }

                return false;

            case 3:

                for (int i = 0; i < 4; i++)
                {
                    if (game.pp3[i] != 0 && game.pp3[i] + game.facedice <= 57)
                    {
                        return true;

                    }

                }

                return false;

            case 4:

                for (int i = 0; i < 4; i++)
                {
                    if (game.pp4[i] != 0 && game.pp4[i] + game.facedice <= 57)
                    {
                        return true;

                    }

                }

                return false;

        }

        return v;

    }

    public void tour() // function that change the  number num and change the tourn beteween the player 
    {
        game.can = true;
        game.verif = true;

        if (listplayer[0] == "VSPer") // if the custom choose to play 2 person 
        {
            User winUser = new User();
            winUser.name = Global.m_user.name;
            winUser.address = Global.room.id;

            if (numplayer == 2)
            {
                
                if (win.w1 == 1 | win.w3 == 3) // if one of them win ====> end of game
                {
                    if(win.w1==1&&Global.myTurn==1||win.w2==1&&Global.myTurn==2||win.w3==1&&Global.myTurn==3||win.w4==1&&Global.myTurn==4){
                        Global.m_user.score++;
                        socket.Emit("increaseScore", JsonUtility.ToJson(Global.m_user));

                        socket.Emit("set winner", JsonUtility.ToJson(winUser));
                    }
                    print("end game");
                    PanelEnd.SetActive(true);
                    objEndText.SetActive(false);
                    Time.timeScale = 0;
                }
                else if (num == 1) //commutation tourn 
                {
                    num = 3; // 3 because we use the belau

                }
                else if (num == 3) // commutation the tour 
                {
                    num = 1;

                }

            }
            else
            {
                num += 1;
                if (win.win1 + win.w3 + win.w4 + win.w2 >= 6) //end the game if three person win 
                {
                    if(win.w1 == 1 && Global.myTurn == 1 || win.w2 == 1 && Global.myTurn==2 || win.w3 == 1 && Global.myTurn == 3 || win.w4 == 1 && Global.myTurn == 4) {
                        Global.m_user.score++;
                        socket.Emit("increaseScore", JsonUtility.ToJson(Global.m_user));
                        socket.Emit("set winner", JsonUtility.ToJson(winUser));
                    }
                    print("end game");
                    PanelEnd.SetActive(true);

                    objEndText.SetActive(false);
                    Time.timeScale = 0;
                }
                else if (num == win.w1 | num == win.w2 | num == win.w3 | num == win.w4)
                {
                    tour();
                }
                if (num > numplayer)
                {
                    num = 1;
                    if (num == win.w1)
                    {
                        tour();
                    }

                }
            }

            // num : turn

        }
        else if (listplayer[0] == "VSPc")
            if (numplayer == 2)
            {
                if (win.w1 == 1 | win.w3 == 3)
                {
                    if(win.w1==1&&Global.myTurn==1||win.w2==1&&Global.myTurn==2||win.w3==1&&Global.myTurn==3||win.w4==1&&Global.myTurn==4){
                    Global.m_user.score++;
                    socket.Emit("increaseScore", JsonUtility.ToJson(Global.m_user));
                    }
                    print("end game");
                    PanelEnd.SetActive(true);
                    objEndText.SetActive(false);
                    Time.timeScale = 0;
                }
                else if (num == 1)
                {
                    num = 3;

                    vsps.pc(num);

                }
                else if (num == 3)
                {
                    num = 1;
                    player.turnpc = false;
                    game.can = true;

                }

            }
            else
            {
                num += 1;
                if (win.w1 + win.w3 + win.w4 + win.w2 >= 6)
                {
                    if(win.w1==1&&Global.myTurn==1||win.w2==1&&Global.myTurn==2||win.w3==1&&Global.myTurn==3||win.w4==1&&Global.myTurn==4){
                    Global.m_user.score++;
                    socket.Emit("increaseScore", JsonUtility.ToJson(Global.m_user));
                    }
                    print("end game");
                    PanelEnd.SetActive(true);
                    objEndText.SetActive(false);
                    Time.timeScale = 0;

                }
                else if (num == win.w2 | num == win.w3 | num == win.w4) // if thr number of turn like one of the win prson we change the tour
                {
                    tour();
                }
                else if (num > numplayer)
                {
                    num = 1;
                    if (num == win.w1)
                    {
                        tour();
                    }
                    else
                    {
                        player.turnpc = false;

                    }

                }
                else
                {
                    vsps.pc(num); // computer turn to play
                }
                {

                }
            }

    }

    public void ToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("main");
    }

    public void Menu()
    {
        if (!Global.isMultiplayer)
        {
            ToMenu();
        }
        else
        {
            socket.Emit("give up", JsonUtility.ToJson(new PlayerTurn(0, Global.myTurn)));
            ToMenu();
        }
        
    }
}
