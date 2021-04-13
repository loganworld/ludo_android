using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Nethereum.Web3;
using UnitySocketIO;
using UnitySocketIO.Events;
using System.Collections;
using System.Collections.Generic;
using System;
public class balance_manage : MonoBehaviour
{
    public TextMeshProUGUI username;
    public TextMeshProUGUI balance;
    public TMP_InputField address;
    public TMP_InputField toaddress;
    public TMP_InputField amount;
    public GameObject tx_contents;
    public GameObject txPrefab;
    SocketIOController socket;

    // Start is called before the first frame update
    void Awake()
    {
        socket = SocketIOController.instance;
        username.text = Global.m_user.name;
        socket.On("sent balance", setbalance);
        socket.On("show transaction", ShowTransaction);
        deposit();
    }

    void ShowTransaction(SocketIOEvent socketIOEvent)
    {
        Debug.Log(socketIOEvent.data);
        TransactionList txlist = TransactionList.CreateFromJSON(socketIOEvent.data.ToString());

        foreach (Transform child in tx_contents.transform)
        {
            Destroy(child.gameObject);
        }


        int index = 1;
        GameObject temp;
        foreach (Transaction transaction in txlist.transactions)
        {
            temp = Instantiate(txPrefab) as GameObject;
            temp.GetComponent<view_history>().setProps(transaction.date, transaction.address, transaction.amount);
            temp.transform.SetParent(tx_contents.transform);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            index++;
        }
    }
    private void OnEnable()
    {
        get_info();
    }

    public void get_info()
    {
        //balance_update();
        StartCoroutine(balance_update());
        deposit();
    }
    IEnumerator balance_update()
    {
        yield return new WaitForSeconds(0.2f);

        if (Global.socketConnected)
        {
            socket.Emit("get balance", JsonUtility.ToJson(Global.m_user));
            SocketIOController.instance.Emit("get transactions", JsonUtility.ToJson(Global.m_user));
        }
        else
        {
            StartCoroutine(balance_update());
        }
    }

    void setbalance(SocketIOEvent socketIOEvent)
    {
        var data = Balance_struct.CreateFromJSON(socketIOEvent.data);
        Global.balance = data.balance;
        balance.text = Global.balance.ToString();
        //Global.balance = float.Parse(socketIOEvent.data);
    }


    public void deposit()
    {
        address.text = Global.m_user.address;
    }

    public void withdraw()
    {
        if (toaddress.text == "")
            return;
        if (amount.text == "")
            return;
        Debug.Log((float)Math.Round(double.Parse(amount.text), 6));
        socket.Emit("withdraw", JsonUtility.ToJson(new Withdraw_class(Global.m_user.id, (float)Math.Round(double.Parse(amount.text), 6), toaddress.text)));
    }
}
[Serializable]
public class Withdraw_class
{
    public long id;
    public float amount;
    public string toaddress;

    public Withdraw_class(long id, float amount, string toaddress)
    {
        this.id = id;
        this.amount = amount;
        this.toaddress = toaddress;
    }
}


[Serializable]

public class Balance_struct
{
    public float balance;
    public Balance_struct(float balance)
    {
        this.balance = balance;
    }
    public static Balance_struct CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<Balance_struct>(data);
    }
}
[Serializable]
public class Transaction
{
    public string date;
    public string address;
    public string amount;

    public static Transaction CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<Transaction>(data);
    }
    public Transaction(string date, string address, string amount)
    {
        this.date = date;
        this.address = address;
        this.amount = amount;
    }

}

[Serializable]
public class TransactionList
{

    public List<Transaction> transactions;

    public static TransactionList CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<TransactionList>(data);
    }
}