using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused;
    public GameObject pauseMenu;
    public static GameObject loadingScreen;

    public static GameObject progressText;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        loadingScreen = GameObject.Find("LoadingScreen");
        progressText = GameObject.Find("ProgressText");
        
        loadingScreen.SetActive(false);
    }
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            if(isPaused){
            resumeGame();
        }else{
            pauseGame();
        }
        }
    }

    // Update is called once per frame
    public void pauseGame(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }
    public void resumeGame(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }
    public void quitGame(){
        Application.Quit();
    }

    public void backToHub(){
        SceneManager.LoadScene("PostAI");
        Time.timeScale = 1;
       
    }

    
}
