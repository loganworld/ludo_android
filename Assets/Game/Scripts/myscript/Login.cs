using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using LitJson;
using UnitySocketIO;
using UnitySocketIO.Events;
using TMPro;

public class Login : MonoBehaviour
{
    public TMP_InputField c_username;
    public TMP_InputField c_password;
    public TMP_InputField c_confirm;
    public GameObject objFailed;
    private int mode = 0;

    private void Awake()
    {
        Global.GetDomain();
    }
    // Start is called before the first frame update
    void Start()
    {
        objFailed.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickLoginButton()
    {
        mode = 0;

        string username = c_username.text;
        string password = c_password.text;

        WWWForm formData = new WWWForm();
        formData.AddField("username", username);
        formData.AddField("password", password);

        string requestURL = Global.currentDomain + "/api/login";

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
            objFailed.SetActive(true);
            objFailed.transform.GetComponent<TextMeshProUGUI>().SetText("Network Error." + www.error);
            yield break;
        }

        string resultData = www.downloadHandler.text;

        if (string.IsNullOrEmpty(resultData))
        {
            Debug.Log("Result Data Empty");
            objFailed.SetActive(true);
            objFailed.transform.GetComponent<TextMeshProUGUI>().SetText("Failed");
            yield break;
        }


        JsonData json = JsonMapper.ToObject(resultData);
        string response = json["success"].ToString();

        if (response != "1")
        {
            Debug.Log(resultData);
            Debug.Log("Login Failed");

            objFailed.SetActive(true);

            if (mode == 0)
            {

                objFailed.transform.GetComponent<TextMeshProUGUI>().SetText("Login Failed");
            }
            else
            {
                if (response == "0")
                {
                    objFailed.transform.GetComponent<TextMeshProUGUI>().SetText("UserName already exists.");
                }
                else
                {
                    objFailed.transform.GetComponent<TextMeshProUGUI>().SetText("Signup Failed");
                }
            }

        }
        else
        {
            if (mode == 0)
            {
                Global.m_user = new User();
                Global.m_user.id = long.Parse(json["data"]["id"].ToString());
                Global.m_user.name = json["data"]["name"].ToString();
                Global.m_user.score = long.Parse(json["data"]["score"].ToString());
                Global.m_user.address = json["data"]["address"].ToString();

                objFailed.SetActive(false);
                SceneManager.LoadScene("main");
            }
            else
            {
                objFailed.SetActive(true);
                objFailed.transform.GetComponent<TextMeshProUGUI>().SetText("Success! Please log in.");
            }

        }


    }


    public void OnClickSignUpButton()
    {
        mode = 1;

        if (c_username.text == "")
        {
            objFailed.SetActive(true);
            objFailed.transform.GetComponent<TextMeshProUGUI>().SetText("The UserName Field is empty.");
            return;
        }
        if (c_password.text != c_confirm.text)
        {
            objFailed.SetActive(true);
            objFailed.transform.GetComponent<TextMeshProUGUI>().SetText("Password Mismatch");
            return;
        }
        string username = c_username.text;
        string password = c_password.text;

        WWWForm formData = new WWWForm();
        formData.AddField("username", username);
        formData.AddField("password", password);

        string requestURL = Global.currentDomain + "/api/signup";

        UnityWebRequest www = UnityWebRequest.Post(requestURL, formData);
        www.SetRequestHeader("Accept", "application/json");
        www.uploadHandler.contentType = "application/json";
        StartCoroutine(iRequest(www));

    }
}
