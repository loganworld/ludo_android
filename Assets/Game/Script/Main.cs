
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnitySocketIO;
using UnitySocketIO.Events;

public class Main : MonoBehaviour
{
    // Use this for initialization
    public GameObject Panel1, Panel2, Panel3, Panel4, PanelOn, PanelOf;
    public Text num;
    private int i = 2;
    public string[] lp = new string[2];
    public Text V1, v2;

    public GameObject loadContent;
    public GameObject loadGameElementPrefab;


    void Start()
    {
        Time.timeScale = 1;
        num.text = i.ToString();
        lp[1] = i.ToString();

        //SocketIOController.instance.Connect();

        Global.isLoading = false;
        Global.isMultiplayer = false;

        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            PanelOf.SetActive(false);
            PanelOn.SetActive(true);
            AudioListener.volume = 1f;
        }
        else
        {
            PanelOn.SetActive(false);
            PanelOf.SetActive(true);
            AudioListener.volume = 0f;
        }

    }
    public void Button()
    {

        if (EventSystem.current.currentSelectedGameObject.name == "VS AI")
        {

            lp[0] = "VSPc";
            Panel2.SetActive(false);
            Panel3.SetActive(false);
            Panel4.SetActive(false);
            Panel1.SetActive(true);

        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Multiplayer")
        {
            lp[0] = "VSPer";
            SceneManager.LoadScene("multiplayer");
        }

        else if (EventSystem.current.currentSelectedGameObject.name == "Retur")
        {
            Panel1.SetActive(false);
            Panel2.SetActive(true);
            Panel3.SetActive(true);
            Panel4.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Play")
        {
            V1.text = lp[0].ToString();//take the type of the game VS humaine Or vs computer 
            v2.text = lp[1].ToString();// take the number of the players 
            SceneManager.LoadScene("game");
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "+")
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
        else if (EventSystem.current.currentSelectedGameObject.name == "SoundOn")
        {
            
            PanelOf.SetActive(true);
            PanelOn.SetActive(false);

            AudioListener.volume = 0f;
            PlayerPrefs.SetInt("Sound", 0);



        }
        else if (EventSystem.current.currentSelectedGameObject.name == "SoundOf")
        {
            PanelOn.SetActive(true);
            PanelOf.SetActive(false);

            AudioListener.volume = 1f;
            PlayerPrefs.SetInt("Sound", 1);


        }



    }

    public void OnLoadGameClicked()
    {
        WWWForm formData = new WWWForm();
        formData.AddField("userId", Global.m_user.id.ToString());

        string requestURL = Global.currentDomain + "/api/get-saved-list";

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

        if (resultData.Contains("error"))
        {
            Debug.Log(resultData);
            yield break;
        }

        Debug.Log("Saved Games : " + resultData);

        Global.savedData = SaveData.CreateFromJSON(resultData);
        GameObject temp;

        foreach(Transform t in loadContent.transform)
        {
            Destroy(t.gameObject);
        }

        if (Global.savedData.cntPlayers != -1)
        {
            temp = Instantiate(loadGameElementPrefab) as GameObject;
            temp.transform.name = "MyGame";
            temp.GetComponent<LoadGame>().SetProps(resultData);
            temp.transform.SetParent(loadContent.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        
    }

}

