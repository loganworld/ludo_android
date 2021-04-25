using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnitySocketIO;
using UnitySocketIO.Events;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        // SocketIOController.instance.Emit("deleteRoom", JsonUtility.ToJson(Global.room));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
