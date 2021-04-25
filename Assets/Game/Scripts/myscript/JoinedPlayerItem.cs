using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class JoinedPlayerItem : MonoBehaviour
{
    public TextMeshProUGUI c_name;
    public string id;
    public string name;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    public void SetProps(string name, string id)
    {
        this.id = id;
        this.name = name;
        c_name.text = name;
    }
}
