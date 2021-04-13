using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class RoomItem : MonoBehaviour
{
    public TextMeshProUGUI c_name;
    public string id;
    public string name;
    SocketIOController socket;
    public Room room;

    // Start is called before the first frame update
    void Start()
    {
        socket = SocketIOController.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetProps(Room room)
    {
        this.room = room;
        c_name.text = string.Format("{0}({1}/{2})(Bet:{3})", room.name, room.curCnt, room.totCnt, room.amount);
    }

    //public void SetProps(string name, string id)
    //{
    //    this.id = id;
    //    this.name = name;
    //    c_name.text = name;
    //}
    public void OnclickButtonJoin()
    {

        socket.Emit("joinRoom", JsonUtility.ToJson(room));
        Global.room = room;
        //PlayerPrefs.SetString("RoomName", name);
        //PlayerPrefs.SetString("RoomID", id);
        //PlayerPrefs.SetInt("VsCPU", 0);
        //PlayerPrefs.SetInt("Main", 0);

        //SceneManager.LoadScene("game");
    }
}
