using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudienceController : MonoBehaviour
{
    [SerializeField]
    private Button testButton;
    [SerializeField]
    private Button testLeave;
    [SerializeField]
    private string _appId;

    [SerializeField]
    GameObject renderObject;
    private AudienceClient _app = null;
    private bool _joined = false;



    // Start is called before the first frame update
    void Start()
    {
        testButton.onClick.AddListener(Test);
        testLeave.onClick.AddListener(LeaveStream);
        EntranceInput.OnPasswordSubmited += DisplayPassword;
        _app = new AudienceClient();
        _app.LoadEngine(_appId);
        _app.SetRenderObject(renderObject);
    }

    private void Test()
    {
        _app.Join("test");
    }

    private void DisplayPassword(string text)
    {
        _app.Join("test");
    }

    private void OnApplicationQuit()
    {
        _app.DisableEngine();
    }

    private void LeaveStream()
    {
        _app.Leave();
        _joined = false;
    }
}
