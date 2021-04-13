using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnitySocketIO;
using UnitySocketIO.Events;

public class Game : MonoBehaviour
{

    public int[] pp1 = new int[4]; //four nnumber of the position  token of the player one  green 
    public int[] pp2 = new int[4]; //four nnumber of the position  token of the player tow yellow  
    public int[] pp3 = new int[4]; //four nnumber of the position  token of the player three bleau  
    public int[] pp4 = new int[4]; //four nnumber of the position  token of the player four red  

    public bool can = true, initial = false, verif = false; // bool condition to know :can ==> can bounce the dice boutton /initial: ==> the first play of the player
                                                            //verif ==> verification that he comite bounce and move to allouer change tour /
    public Transform[] postour = new Transform[6]; // four transform of th efour position of the dice 
    public GameObject Dice; //dice
    private Image face; //face dice 
    public Sprite[] im = new Sprite[6]; //6 face of the dice
    public int facedice = 1, numlist = 0; // face dice number of the dice / numlist number of the list we have four players 

    public Transform[] wayPointList = new Transform[4]; // four waypoint to the four players

    Transform targetWayPoint; // the next positon that the token move to it
    public Transform[] p1 = new Transform[4]; // the four token of the first player green 
    public Transform[] p2 = new Transform[4]; // the four token of the second player yellow 
    public Transform[] p3 = new Transform[4]; // the four token of the thred player bleau  
    public Transform[] p4 = new Transform[4]; // the four token of the fourest player red 
    public Transform[] scal1 = new Transform[4]; // the four images of the tokens of the player green 
    public Transform[] scal2 = new Transform[4]; // the four images of the tokens of the player yellow
    public Transform[] scal3 = new Transform[4]; // the four images of the tokens of the player bleau 
    public Transform[] scal4 = new Transform[4]; // the four images of the tokens of the player red
    public Manager manager;
    public Kill kill;
    public Win win;
    public AudioSource audioDICE, audioJump, audioJump1;

    public Player player;

    SocketIOController socket;

    int finalfacedice;

    // Use this for initialization

    void Start()
    {
        initial = true;

        Dice.transform.position = postour[0].transform.position;
        face = Dice.GetComponent<Image>();

        face.sprite = im[UnityEngine.Random.Range(0, 6)];

        socket = SocketIOController.instance;

        if (Global.isMultiplayer)
        {
            socket.On("other player turned", OnOtherPlayerTurned);
        }
    }

    private void OnOtherPlayerTurned(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        PlayerTurn turnJson = PlayerTurn.CreateFromJson(data);

        if (turnJson.type == 0)
        {
            Debug.Log("other player turned = player" + turnJson.turn + ", facedice " + turnJson.facedice);
            multiplayerButtonDice(false, turnJson.facedice);
        }
        else
        {
            Debug.Log("other player turned = player" + turnJson.turn + ", moveAction " + turnJson.moveAction);
            player.moveAction(turnJson.moveAction);
        }
    }

    public void automatiquescale(int c) // change the scale of the 4 token when they have they tourn  else rest in there scale 
    {
        switch (c)
        {
            case 1:
                for (int i = 0; i < scal1.Length; i++)
                {
                    scal1[i].localScale = new Vector3(1.5f, 1.5f, 1);
                    scal2[i].localScale = new Vector3(1, 1, 1);
                    scal3[i].localScale = new Vector3(1, 1, 1);
                    scal4[i].localScale = new Vector3(1, 1, 1);
                }

                break;
            case 2:
                for (int i = 0; i < scal1.Length; i++)
                {
                    scal1[i].localScale = new Vector3(1, 1, 1);
                    scal2[i].localScale = new Vector3(1.5f, 1.5f, 1);
                    scal3[i].localScale = new Vector3(1, 1, 1);
                    scal4[i].localScale = new Vector3(1, 1, 1);
                }
                break;
            case 3:
                for (int i = 0; i < scal1.Length; i++)
                {
                    scal1[i].localScale = new Vector3(1, 1, 1);
                    scal2[i].localScale = new Vector3(1, 1, 1);
                    scal3[i].localScale = new Vector3(1.5f, 1.5f, 1);
                    scal4[i].localScale = new Vector3(1, 1, 1);
                }
                break;
            case 4:
                for (int i = 0; i < scal1.Length; i++)
                {
                    scal1[i].localScale = new Vector3(1, 1, 1);
                    scal2[i].localScale = new Vector3(1, 1, 1);
                    scal3[i].localScale = new Vector3(1, 1, 1);
                    scal4[i].localScale = new Vector3(1.5f, 1.5f, 1);
                }
                break;
        }
    }

    private void Update()
    {
        // change the position of the dice depenfd of the tour 
        Dice.transform.position = postour[manager.num].transform.position;
        automatiquescale(manager.num); // change the scale depend of the tour 

    }
    public int moveplayer(int x, Transform y, int w) // moving the player 

    { // int w numpler of the player green =0, yellow =1, bleau =2, red=3
        numlist = w; // 
        can = false; // disable the button of the bounce dice 
        verif = false; // disable pressing to the token 

        if (x == 0) // if the player int the initial position he will be int the positon number one 
        {
            audioJump1.Play();
            x = 1;
            can = true;
            targetWayPoint = wayPointList[numlist].GetChild(0);
            y.position = targetWayPoint.position;
            return x;
        }
        else // else he follow the numbere of his dice 
        {
            audioJump.Play();
            StartCoroutine(delaijump(x, y, w));

        }
        return (x = x + facedice);
    }
    IEnumerator delaijump(int x, Transform y, int w)
    {
        for (int i = x; i < x + facedice; i++)
        {
            can = false;
            yield return new WaitForSeconds(0.15f);
            targetWayPoint = wayPointList[numlist].GetChild(i);
            y.position = targetWayPoint.position;
        }
        can = true;
        verif = false;
        audioJump.Pause();
        kill.verifkill(y, w); /// verification if he when he after his moving he kill one of the other tokens

        win.verifwin(w); // verification that he win or not 

    }
    public void buttondice() // bounce the dice 
    {
        if (manager.listplayer[0] == "VSPer")
        {
            multiplayerButtonDice();
            return;
        }

        if (can)
        {
            finalfacedice = UnityEngine.Random.Range(0, 6);

            verif = false;
            can = false;
            float i = 0;
            StartCoroutine(delai());
            while (i < 0.8f) {
                i += 0.15f;
                StartCoroutine(Example(i));

            }
        }

    }

    public void multiplayerButtonDice(bool me = true, int receivedFaceDice = 0) // bounce the dice 
    {
        if (can)
        {

            finalfacedice = UnityEngine.Random.Range(0, 6);
            
            if (me)
            {
                socket.Emit("click", JsonUtility.ToJson(new PlayerTurn(0, Global.myTurn, finalfacedice)));
            }
            else
            {
                finalfacedice = receivedFaceDice;
            }

            //Debug.Log("Final Face Dice : " + finalfacedice);

            verif = false;
            can = false;
            float i = 0;
            StartCoroutine(delai());
            while (i < 0.8f)
            {
                i += 0.15f;
                StartCoroutine(Example(i));
            }

        }

    }

    IEnumerator delai()
    {
        audioDICE.Play();
        yield return new WaitForSeconds(0.5f);
        audioDICE.Pause();

        //if (Global.isMultiplayer)
        {
            facedice = finalfacedice;
            face.sprite = im[facedice];
        }


        // Debug.Log("Last face dice : " + facedice);

        facedice += 1;

        verif = true; // the player can press on his token after he rolling the dice 

        if (!manager.automatiquetour(manager.num) && facedice != 6 && initial && !manager.automatiquetour2(manager.num))
        { // if he not in first tour and he don't have not option to play we call to change the tour 

            StartCoroutine(wait());

        }
        else if (manager.automatiquetour2(manager.num) && facedice != 6 && initial)
        {
            // he have a option to play it 
            verif = true;

        }
        else if (!initial && facedice != 6) // he is int he first tour abd he don't have any otion we call to change the tour 
        {
            manager.num = 1;

            initial = true;
            StartCoroutine(wait());

        }
        else if (!initial && facedice == 6) // the same but he have dice 6 we lat it play 
        {
            manager.num = 1;

            verif = true;

        }

    }

    IEnumerator Example(float i)
    {
        facedice = UnityEngine.Random.Range(0, 6);
        face.sprite = im[facedice];

        yield return new WaitForSeconds(i / 2);

        facedice = UnityEngine.Random.Range(0, 6);
        face.sprite = im[facedice];

        Debug.Log("Example Face Dice : " + facedice);
    }
    IEnumerator wait()
    {
        can = false;
        verif = false;

        yield return new WaitForSeconds(1f);
        manager.tour();

    }
}


[Serializable]
public class PlayerTurn
{
    public int type;
    public int turn;
    public int facedice;
    public string moveAction;
    public string roomId;

    public PlayerTurn(int type, int turn, int facedice = 0, string moveAction = "")
    {
        this.type = type;
        this.turn = turn;
        this.facedice = facedice;
        this.moveAction = moveAction;
        this.roomId = Global.room.id;
    }
    public static PlayerTurn CreateFromJson(string data)
    {
        return JsonUtility.FromJson<PlayerTurn>(data);
    }
}