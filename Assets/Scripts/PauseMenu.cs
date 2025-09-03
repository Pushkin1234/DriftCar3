using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseCanvas;
    private bool _isPause = false;
    public Button ContinueButton;

    private void Awake()
    {
        Continue();
    }

    private void Start()
    {
        if(_isPause == false)
        {
            PauseCanvas.gameObject.SetActive(false);
        }
        ContinueButton.onClick.AddListener(Continue);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(_isPause == false)
            {
                Pause();
            }
            else
            {
                Continue();
            }
        }
    }

    private void Pause()
    {
        _isPause = true;
        PauseCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    private void Continue()
    {
        _isPause = false;
        PauseCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
