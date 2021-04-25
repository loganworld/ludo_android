using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{

    public Button btnLoad;
    public string data;

    // Start is called before the first frame update
    void Start()
    {
        btnLoad.onClick.AddListener(OnLoadClicked);
    }

    public void SetProps(string data)
    {
        this.data = data;
    }

    private void OnLoadClicked()
    {
        // Global.savedData = SaveData.CreateFromJSON(data);
        Global.isLoading = true;
        SceneManager.LoadScene("game");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
