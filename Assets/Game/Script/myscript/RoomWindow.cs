using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;

public class RoomWindow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        if (Global.socketConnected)
        {
            SocketIOController.instance.Emit("get room list", JsonUtility.ToJson(Global.m_user));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
