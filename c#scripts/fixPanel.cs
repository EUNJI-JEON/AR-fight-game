using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.SceneManagement;

public class fixPanel : MonoBehaviour
{
    public GameObject errorPanel;
    public GameObject refreshButton;
    public GameObject cameraButton;
    
    private string s = "";
    // Start is called before the first frame update
    void Start()
    {
        s = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        if(DefaultTrackableEventHandler.trueFalse == true){
            errorPanel.SetActive(false);
            cameraButton.SetActive(false);
        }

    }

    public void refresh(){
        SceneManager.LoadScene(s);
    }
}
