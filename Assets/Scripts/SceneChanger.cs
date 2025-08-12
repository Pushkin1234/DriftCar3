using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Button Button;
    public string NameScene;


    private void Start()
    {
        Button.onClick.AddListener(LoadScene);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(NameScene);
    }



}
