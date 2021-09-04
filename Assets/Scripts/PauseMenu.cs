﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject PauseMenuUI;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (GamePaused == true)
            {
                ResumeGame();
            }
            if (GamePaused == false)
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
    }
    public void PauseGame()
    {
        
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
        
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}