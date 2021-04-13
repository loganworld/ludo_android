using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;
using LitJson;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class MultiMenuManager : MonoBehaviour
{
    public GameObject RoomWindow;
    public GameObject ChallengeRoomWindow;
    public GameObject ChallengePopupWindow;
    public GameObject CreateRoomWindow;
    public GameObject CreatedRoomPopup;

    public GameObject room_contents;
    public GameObject roomPrefab;
    SocketIOController socket;

    public TMP_InputField c_RoomName;
    public TMP_InputField c_Bet_amount;

    public GameObject userContent;
    public GameObject userPrefab;

    public TMP_InputField roomSearchField;
    public TMP_InputField challengeSearchField;
    public TMP_InputField challengeBetAmount;

    RoomList roomList;
    ChallengeList challengeList;

    bool clickedBell = false;

    //add

    private int i = 2;
    public string[] lp = new string[2];
    public Text V1, v2;
    public Text num;


    public void Button()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "+")
        {
            if (i < 4)
                i += 1;
            num.text = i.ToString();
            lp[1] = i.ToString();

        }
        else if (EventSystem.current.currentSelectedGameObject.name == "-")
        {
            if (i > 2)
                i -= 1;
            num.text = i.ToString();
            lp[1] = i.ToString();

        }
    }

    //
    // Start is called before the first frame update
    void Start()
    {

        num.text = i.ToString();
        clickedBell = false;

        RoomWindow.SetActive(true);
        ChallengeRoomWindow.SetActive(false);
        ChallengePopupWindow.SetActive(false);
        CreateRoomWindow.SetActive(false);
        CreatedRoomPopup.SetActive(false);

        socket = SocketIOController.instance;

        //socket.Connect();

        socket.On("show room", GetRooms);
        socket.On("createdRoom", OnCreatedRoom);
        socket.On("show users", GetUsers);
        socket.On("show challenges", GetChallenges);

        socket.On("joinedRoom", OnJoinedRoom);
        socket.On("updatedRoom", OnUpdatedRoom);

        socket.On("createdChallengeRoom", CreatedChallengeRoom);
        socket.On("acceptedChallenge", OnAcceptedChallenge);

        socket.On("gameTurn", OnGetGameTurn);
        socket.On("players", OnGetGamePlayers);

        socket.On("sent balance", setbalance);



        StartCoroutine(iShowRooms());
        Debug.Log(socket.socketIO);
        //socket.Emit("get room list");

        lp[0] = "VSPer";
        lp[1] = "2";

    }

    void setbalance(SocketIOEvent socketIOEvent)
    {
        var data = Balance_struct.CreateFromJSON(socketIOEvent.data);
        Global.balance = data.balance;
        //Global.balance = float.Parse(socketIOEvent.data);
    }

    private void OnGetGamePlayers(SocketIOEvent socketIOEvent)
    {
        Player_dataList datas = Player_dataList.CreateFromJSON(socketIOEvent.data.ToString());
        int i = 0;
        Global.othernames = new List<string>();
        foreach (Player_data data in datas.players)
        {
            if (data.turn - 1 == i)
                Global.othernames.Add(data.user);
            else
                Global.othernames.Add("");
            i++;
            Debug.Log("ddddd" + data.user + " " + Global.othernames[i - 1]);
        }
    }

    private void OnGetGameTurn(SocketIOEvent socketIOEvent)
    {
        GameTurn turn = GameTurn.CreateFromJSON(socketIOEvent.data.ToString());



        if (turn.playing == 2)
        {
            Global.myTurn = turn.turn;

            V1.text = lp[0].ToString();//take the type of the game VS humaine Or vs computer 
            v2.text = lp[1].ToString();// take the number of the players 

            Debug.Log("----------Game Mode & the number of players");
            Debug.Log(V1.text);
            Debug.Log(v2.text);
            Debug.Log("---------------------------------------------");
            //PlayerPrefs.SetInt("VsCPU", 0);

            //if (turn.turn == 0)
            //{
            //    Global.mainPlayer = true;
            //}
            //else
            //{
            //    Global.mainPlayer = false;
            //}

            SceneManager.LoadScene("game");
        }


    }

    private void OnAcceptedChallenge(SocketIOEvent socketIOEvent)
    {
        Global.mainPlayer = false;

        Room room = Room.CreateFromJSON(socketIOEvent.data.ToString());
        CreatedRoomPopup.SetActive(true);
        CreatedRoomPopup.GetComponent<CreatedRoomPopup>().SetProps(room);
    }

    private void CreatedChallengeRoom(SocketIOEvent socketIOEvent)
    {
        Global.mainPlayer = true;

        Room room = Room.CreateFromJSON(socketIOEvent.data.ToString());
        CreatedRoomPopup.SetActive(true);
        ChallengeRoomWindow.SetActive(false);
        CreatedRoomPopup.GetComponent<CreatedRoomPopup>().SetProps(room);
    }

    private void OnUpdatedRoom(SocketIOEvent socketIOEvent)
    {
        Room room = Room.CreateFromJSON(socketIOEvent.data.ToString());
        CreatedRoomPopup.GetComponent<CreatedRoomPopup>().SetProps(room);
    }

    private void OnJoinedRoom(SocketIOEvent socketIOEvent)
    {
        PlayerPrefs.SetInt("VsCPU", 0);
        PlayerPrefs.SetInt("Main", 0);
        Global.mainPlayer = false;

        Room room = Room.CreateFromJSON(socketIOEvent.data.ToString());

        CreatedRoomPopup.SetActive(true);
        CreatedRoomPopup.GetComponent<CreatedRoomPopup>().SetProps(room);

        RoomWindow.SetActive(false);
    }

    void DisplayChallenges(string searchKey = "")
    {
        if (challengeList == null || userContent == null)
        {
            return;
        }

        foreach (Transform child in userContent.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject temp;
        int index = 1;

        string lowerName = "";
        string lowerKey = searchKey.ToLower();


        foreach (Challenge challenge in challengeList.challenges)
        {

            temp = Instantiate(userPrefab) as GameObject;

            temp.transform.name = index.ToString();

            if (challenge.status == -1)
            {
                //if (searchKey != "" && challenge.toUserName.IndexOf(searchKey, StringComparison.CurrentCultureIgnoreCase) < 0)
                //{
                //    continue;
                //}
                lowerName = challenge.toUserName;

                if (!lowerName.Contains(lowerKey))
                {
                    continue;
                }

                temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.toUserId, challenge.toUserName, challenge.toScore, challenge.room_amount);
            }
            else
            {
                if (challenge.fromUserId == Global.m_user.id)
                {
                    lowerName = challenge.toUserName;

                    if (!lowerName.Contains(lowerKey))
                    {
                        continue;
                    }

                    //if (searchKey != "" && challenge.toUserName.IndexOf(searchKey, StringComparison.CurrentCultureIgnoreCase) < 0)
                    //{
                    //    continue;
                    //}

                    if (challenge.status == 0)
                    {
                        temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.toUserId, challenge.toUserName, challenge.toScore, challenge.room_amount, "WAITTING");
                    }
                    else
                    {
                        temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.toUserId, challenge.toUserName, challenge.toScore, challenge.room_amount, "START", challenge.roomId);
                    }
                }
                else
                {
                    lowerName = challenge.fromUserName;

                    if (!lowerName.Contains(lowerKey))
                    {
                        continue;
                    }
                    //if (searchKey != "" && challenge.fromUserName.IndexOf(searchKey, StringComparison.CurrentCultureIgnoreCase) < 0)
                    //{
                    //    continue;
                    //}

                    temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), challenge.fromUserId, challenge.fromUserName, challenge.fromScore, challenge.room_amount, "ACCEPT", challenge.roomId);
                }
            }

            temp.transform.SetParent(userContent.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            index++;
        }

        clickedBell = false;
    }

    private void GetChallenges(SocketIOEvent socketIOEvent)
    {

        if (ChallengeRoomWindow == null || (!clickedBell && !ChallengeRoomWindow.transform.gameObject.activeInHierarchy))
        {
            Debug.Log("-----------Not found");
            return;
        }

        challengeList = ChallengeList.CreateFromJSON(socketIOEvent.data.ToString());

        DisplayChallenges();
    }

    public void OnChangedChallengeKey()
    {
        DisplayChallenges(challengeSearchField.text);
    }

    public void OnChangedBetAmount()
    {
        var value = challengeBetAmount.text;
        Debug.Log(value);
        PlayerPrefs.SetString("challenge_amount", value.ToString());
    }

    public void OnClickBell()
    {
        if (Global.socketConnected)
        {
            clickedBell = true;
            socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
        }

    }

    IEnumerator iShowRooms()
    {
        Debug.Log("get room list");
        yield return new WaitForSeconds(0.3f);

        if (Global.socketConnected)
        {
            socket.Emit("get balance", JsonUtility.ToJson(Global.m_user));

            socket.Emit("get room list", JsonUtility.ToJson(Global.m_user));
        }
        else
        {
            StartCoroutine(iShowRooms());
        }

    }
    public void OnCreatedRoom(SocketIOEvent socketIOEvent)
    {
        PlayerPrefs.SetInt("VsCPU", 0);
        PlayerPrefs.SetInt("Main", 1);
        Global.mainPlayer = true;

        CreateRoomWindow.SetActive(false);
        ChallengeRoomWindow.SetActive(false);

        Room room = Room.CreateFromJSON(socketIOEvent.data.ToString());

        CreatedRoomPopup.SetActive(true);
        CreateRoomWindow.SetActive(false);
        CreatedRoomPopup.GetComponent<CreatedRoomPopup>().SetProps(room);

        lp[0] = "VSPer";
        V1.text = lp[0].ToString();//take the type of the game VS humaine Or vs computer 
        v2.text = lp[1].ToString();// take the number of the players 
                                   //PlayerPrefs.SetString("RoomName", room.name);
                                   //PlayerPrefs.SetString("RoomID", room.id);

        //Global.room = room;

        // SceneManager.LoadScene("game");
    }

    // Update is called once per frame
    void Update()
    {

    }


    void DisplayRooms(string searchKey = "")
    {
        if (room_contents == null)
        {
            return;
        }

        foreach (Transform child in room_contents.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject temp;
        int index = 0;

        foreach (Room room in roomList.rooms)
        {

            string lowerName = room.name.ToLower();
            string lowerKey = searchKey.ToLower();

            if (!lowerName.Contains(lowerKey))
            {
                continue;
            }


            index++;
            temp = Instantiate(roomPrefab) as GameObject;
            temp.transform.name = index.ToString();
            temp.GetComponent<RoomItem>().SetProps(room);
            temp.transform.SetParent(room_contents.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        }
    }

    public void OnChangedRoomSearchKey()
    {
        DisplayRooms(roomSearchField.text);
    }


    void GetRooms(SocketIOEvent socketIOEvent)
    {
        roomList = RoomList.CreateFromJSON(socketIOEvent.data.ToString());

        DisplayRooms();

        Debug.Log(roomList.rooms);
    }

    void GetUsers(SocketIOEvent socketIOEvent)
    {
        UserList userList = UserList.CreateFromJSON(socketIOEvent.data.ToString());

        if (userList == null || userContent == null)
        {

            return;
        }

        foreach (Transform child in userContent.transform)
        {
            Destroy(child.gameObject);
        }


        GameObject temp;
        int index = 0;
        foreach (User user in userList.users)
        {
            if (user.id == Global.m_user.id)
            {
                continue;
            }

            index++;
            temp = Instantiate(userPrefab) as GameObject;
            temp.transform.name = index.ToString();
            temp.GetComponent<ChallengeElement>().SetProps(index.ToString(), user.id, user.name, (int)user.score, "1");
            temp.transform.SetParent(userContent.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

    }
    public void OnClickCreateRoomButton()
    {
        c_RoomName.text = "";
        CreateRoomWindow.SetActive(true);
        RoomWindow.SetActive(false);
    }
    public void OnClickCreateButton()
    {
        if (c_RoomName.text == "")
            return;

        if (c_Bet_amount.text == "")
            return;

        if (float.Parse(c_Bet_amount.text) > Global.balance)
            c_Bet_amount.text = Global.balance.ToString();
        //CreateRoomWindow.SetActive(false);
        //CreatedRoomPopup.SetActive(true);
        //RoomWindow.SetActive(false);

        if (Global.socketConnected)
        {
            socket.Emit("createRoom", JsonUtility.ToJson(new Room(c_RoomName.text, "12345", i, c_Bet_amount.text)));
        }

        /***** Get the response "createdRoom" *****/
    }

    public void OnClickChallengeButton()
    {
        ChallengeRoomWindow.SetActive(true);

        if (Global.socketConnected)
        {
            socket.Emit("get challenges", JsonUtility.ToJson(Global.m_user));
        }

        //socket.Emit("get user list", JsonUtility.ToJson(Global.m_user));
    }
}

[Serializable]
public class Room
{
    public string id;
    public string name;
    public int curCnt;
    public int totCnt;
    public string amount;

    public static Room CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<Room>(data);
    }
    public Room(string name, string id, int totCnt = 2, string amount = "0", int curCnt = 1)
    {
        this.name = name;
        this.id = id;
        this.totCnt = totCnt;
        this.curCnt = curCnt;
        this.amount = amount;
    }

}

[Serializable]
public class RoomList
{

    public List<Room> rooms;

    public static RoomList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<RoomList>(data);
    }
}

[Serializable]
public class UserList
{

    public List<User> users;

    public static UserList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<UserList>(data);
    }
}

[Serializable]
public class Challenge
{
    public int fromUserId;
    public string fromUserName;
    public int toUserId;
    public string toUserName;
    public int toScore;
    public int fromScore;
    public int status;
    public string roomId;
    public string room_amount;

    Challenge(int fromUserId, string fromUserName, int toUserId, string toUserName, int fromScore, int toScore, int status, string roomId, string room_amount)
    {
        this.fromUserId = fromUserId;
        this.fromUserName = fromUserName;
        this.toUserId = toUserId;
        this.toUserName = toUserName;
        this.fromScore = fromScore;
        this.toScore = toScore;
        this.status = status;
        this.roomId = roomId;
        this.room_amount = room_amount;
    }
}

[Serializable]
public class ChallengeList
{

    public List<Challenge> challenges;

    public static ChallengeList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<ChallengeList>(data);
    }
}

[Serializable]

public class GameTurn
{

    public int turn;
    public int playing;

    public static GameTurn CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<GameTurn>(data);
    }
}

[Serializable]
public class Player_data
{

    public int turn;
    public string user;

    public static Player_data CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<Player_data>(data);
    }
    public Player_data(int turn, string user)
    {
        this.turn = turn;
        this.user = user;
    }
}


public class Player_dataList
{
    public List<Player_data> players;
    //public string players;
    public static Player_dataList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<Player_dataList>(data);
    }
}

