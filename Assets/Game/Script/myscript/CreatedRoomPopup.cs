using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnitySocketIO;
using UnitySocketIO.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class CreatedRoomPopup : MonoBehaviour
{
    public TextMeshProUGUI joinedNumber;
    public TextMeshProUGUI betAmount;
    public TextMeshProUGUI c_roomID;
    public TextMeshProUGUI c_roomName;
    public GameObject objStart;
    public GameObject objBack;
    SocketIOController socket;
    int curCnt;

    public Room room;

    // Start is called before the first frame update
    void Start()
    {
        socket = SocketIOController.instance;
        //socket.On("")
        
    }

   
    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        if (Global.mainPlayer)
        {
            objStart.SetActive(true);
            objStart.GetComponent<Button>().interactable = false;
        } else
        {
            objStart.SetActive(false);
        }
    }

    public void SetProps(Room room)
    {
        this.room = room;
        this.c_roomID.text = "Room ID : " + room.id;
        this.c_roomName.text = "Room Name : " + room.name;
        this.betAmount.text = room.amount;

        if (room.name == "")
        {
            this.c_roomName.text = "Challenge Room";
        }

        joinedNumber.text = room.curCnt.ToString() + "/" + room.totCnt.ToString();

        if (Global.mainPlayer && (room.curCnt == room.totCnt))
        {
            objStart.GetComponent<Button>().interactable = true;
        }
        else
        {
            objStart.GetComponent<Button>().interactable = false;
        }

        if (room.totCnt == 0)
        {
            joinedNumber.text = "This room was deleted.";
        }
    }

    public void OnClickCloseButton()
    {
        c_roomID.text = "";
        c_roomName.text = "";

        if (Global.mainPlayer)
        {
            socket.Emit("deleteRoom", JsonUtility.ToJson(room));
        }
        else
        {
            socket.Emit("leaveRoom", JsonUtility.ToJson(room));
        }
        
    }

    public void OnClickEnrollButton()
    {
        c_roomID.text = "";
        c_roomName.text = "";

        Global.room = this.room;
        //PlayerPrefs.SetString("RoomName", name);
        //PlayerPrefs.SetString("RoomID", id);
        PlayerPrefs.SetInt("VsCPU", 0);

        socket.Emit("startGame", JsonUtility.ToJson(room));

        
        //SceneManager.LoadScene("game");

    }


}


