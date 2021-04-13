using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnitySocketIO;
using UnitySocketIO.Events;
using UnityEngine.SceneManagement;

public class ChallengeElement : MonoBehaviour
{
    public TextMeshProUGUI no;
    public TextMeshProUGUI name;
    public TextMeshProUGUI score;
    public TextMeshProUGUI bet_mount;
    public long userId;
    public TextMeshProUGUI btnName;
    public Button btn;
    public string roomId;
    public string room_amount;

    SocketIOController socket;

    // Start is called before the first frame update
    void Start()
    {
        socket = SocketIOController.instance;
    }

    public void SetProps(string no, long userId, string userName, int score, string room_amount, string type = "CHALLENGE", string roomId = "")
    {
        this.no.text = no;
        this.name.text = userName;
        this.score.text=score.ToString();
        this.userId = userId;
        this.btnName.text = type;
        this.roomId = roomId;

        this.room_amount = room_amount;
        bet_mount.text = room_amount;

        btn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = type;

        switch (type)
        {
            case "CHALLENGE":
                btn.onClick.AddListener(OnClickChallenge);
                break;
            case "ACCEPT":
                btn.onClick.AddListener(OnClickAccept);
                break;
            //case "START":
            //    btn.onClick.AddListener(OnClickStart);
            //    break;
            //case "WAITTING":
            //    btn.transform.gameObject.SetActive(false);
            //    this.name.text = userName + "(waitting...)";
            //    break;
        }
        
    }

    void Update()
    {
        if (btnName.text == "CHALLENGE")
        {
            bet_mount.text = PlayerPrefs.GetString("challenge_amount", "0");
            if (float.Parse(bet_mount.text) >= Global.balance)
                bet_mount.text = Global.balance.ToString();
        }
    }
    public void OnClickChallenge()
    {
        UserList userList = new UserList();
        userList.users = new List<User>();

        userList.users.Add(Global.m_user);
        userList.users.Add(new User(userId, name.text));

        bet_mount.text = PlayerPrefs.GetString("challenge_amount", "0");
        if (bet_mount.text == "" || bet_mount.text == null)
            return;

        if (float.Parse(bet_mount.text) >= Global.balance)
        {
            bet_mount.text = Global.balance.ToString();
        }
        userList.users.Add(new User(-1, bet_mount.text));

        socket.Emit("invite a challenge", JsonUtility.ToJson(userList));
        /***** Get the response "createdChallengeRoom" in MultiMenuManager.cs  *****/


        // socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
        // socket.Emit("deleteRoom", JsonUtility.ToJson(new Room(roomName, roomID)));
    }

    public void OnClickAccept()
    {
        //UserList userList = new UserList();
        //userList.users = new List<User>();

        //userList.users.Add(new User(userId, name.text));
        //userList.users.Add(Global.m_user);


        //socket.Emit("createChallenge", JsonUtility.ToJson(userList));

        if (room_amount == "" || room_amount == null || float.Parse(room_amount) > Global.balance)
            return;

        Global.room = new Room("", roomId);

        socket.Emit("acceptChallenge", JsonUtility.ToJson(new Room("", roomId)));
        /***** Get the response "acceptedChallenge" in MultiMenuManager.cs  *****/

        // socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
    }

    //public void OnClickStart()
    //{
        
    //    socket.Emit("startGame", JsonUtility.ToJson(new Room("", roomId)));
    //    //SceneManager.LoadScene("Main");
    //}

}
