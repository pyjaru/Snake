using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Snake : MonoBehaviour
{

    float speedMove = 3f;
    float speedRot = 120f;

    bool isDead = false;

    Transform coin;
    List<Transform> tails = new List<Transform>();

    GameObject panelOver;
    Text txtCoin;
    Text txtTime;
    int coinCnt = 0;
    float startTime;

    GameObject panelStick;
    Joystick stick;
    bool isMobile;

    void Awake()
    {
        InitGame();
        startTime = Time.time;
    }

    void Update()
    {
        if (!isDead)
        {
            MoveHead();
            MoveTail();
            SetTime();
        }
        if (Input.GetKey(KeyCode.Y))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.transform.tag)
        {
            case "Coin":
                MoveCoin();
                AddTail();
                break;
            case "Wall":
            case "Tail":
                isDead = true;
                panelOver.SetActive(isDead);
                break;

        }
    }

    void InitGame()
    {
        coin = GameObject.Find("Coin").transform;

        panelOver = GameObject.Find("PanelOver");
        panelOver.SetActive(false);

        txtCoin = GameObject.Find("TxtCoin").GetComponent<Text>();
        txtTime = GameObject.Find("TxtTime").GetComponent<Text>();

        isMobile = (Application.platform == RuntimePlatform.Android)
                || (Application.platform == RuntimePlatform.IPhonePlayer);

        //isMobile = true;//Test코드
        panelStick = GameObject.Find("PanelStick");
        panelStick.SetActive(isMobile);
        stick = panelStick.transform.GetChild(0).GetComponent<Joystick>();
    }

    private void SetTime()
    {
        txtCoin.text = coinCnt.ToString("Coin : 0");

        float span = Time.time - startTime;
        int h = Mathf.FloorToInt(span / 3600);
        int m = Mathf.FloorToInt(span / 60 % 60);
        float s = span % 60;

        txtTime.text = string.Format("Time : {0:0}:{1:0}:{2:00.0}", h, m, s);
    }

    void MoveCoin()
    {
        coinCnt++;

        float x = Random.Range(-9f, 9f);
        float z = Random.Range(-4f, 4f);

        coin.gameObject.SetActive(false);
        coin.position = new Vector3(x, 0, z);
        StartCoroutine(ActiveCoin());
    }

    IEnumerator ActiveCoin()
    {
        yield return new WaitForSeconds(1);
        coin.gameObject.SetActive(true);
    }

    void MoveHead()
    {
        float amount = speedMove * Time.deltaTime;
        transform.Translate(Vector3.forward * amount);

        if (!isMobile)
        {
            amount = Input.GetAxis("Horizontal") * speedRot;
        }
        else
        {
            amount = stick.Horizontal() * speedRot;
        }
        transform.Rotate(Vector3.up * amount * Time.deltaTime);

    }

    void AddTail()
    {
        GameObject tail = Instantiate(Resources.Load("Tail")) as GameObject;
        Vector3 pos = transform.position;

        int cnt = tails.Count;
        if (cnt == 0)
        {
            tail.tag = "Untagged";
        }
        else
        {
            pos = tails[cnt - 1].position;
        }

        tail.transform.position = pos;

        Color[] colors = { new Color(0, 0.5f, 0, 1), new Color(0, 0.5f, 1, 1) };
        int n = cnt / 3 % 2;
        tail.GetComponent<Renderer>().material.color = colors[n];
        tails.Add(tail.transform);
    }

    void MoveTail()
    {
        Transform target = transform;

        foreach (Transform tail in tails)
        {
            Vector3 pos = target.position;
            Quaternion rot = target.rotation;

            tail.position = Vector3.Lerp(tail.position, pos, 4 * Time.deltaTime);
            tail.rotation = Quaternion.Lerp(tail.rotation, rot, 4 * Time.deltaTime);

            target = tail;
        }
    }

    public void OnButtonClick(Button button)
    {
        switch (button.name)
        {
            case "BtnYes":
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case "BtnNo":
                Application.Quit();
                break;
        }
    }

}
