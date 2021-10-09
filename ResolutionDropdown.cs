using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionDropdown : MonoBehaviour
{
    Dropdown dropdown;
    static bool canChangeResolution = true;
    static Dropdown resolutionDropdown;

    private void Start()
    {
        dropdown = GetComponent<Dropdown>();
        if (Screen.fullScreen)
        {
            canChangeResolution = false;
            if (dropdown.options.Count == 3)
            {
                dropdown.interactable = false;
                resolutionDropdown = dropdown;
            }
        }
        else
        {
            canChangeResolution = true;
            if (dropdown.options.Count == 3)
            {
                dropdown.interactable = true;
                resolutionDropdown = dropdown;
            }
        }
        if (dropdown.options.Count == 2)
        {
            if (Screen.fullScreen)
                dropdown.value = 0;
            else
                dropdown.value = 1;
        }
        else if(dropdown.options.Count == 3)
        {
            switch(Screen.width)
            {
                case 1920:
                    dropdown.value = 0;
                    break;
                case 1366:
                    dropdown.value = 1;
                    break;
                case 1280:
                    dropdown.value = 2;
                    break;
            }
        }
    }

    public void ChangeWindow()
    {
        switch(dropdown.value)
        {
            case 0:
                Screen.fullScreen = true;
                canChangeResolution = false;
                resolutionDropdown.interactable = false;
                break;
            case 1:
                Screen.fullScreen = false;
                canChangeResolution = true;
                resolutionDropdown.interactable = true;
                break;
        }
    }
    public void ChangeResolution()
    {
        if (canChangeResolution)
        {
            switch (dropdown.value)
            {
                case 0:
                    Screen.SetResolution(1920, 1080, false);
                    break;
                case 1:
                    Screen.SetResolution(1366, 768, false);
                    break;
                case 2:
                    Screen.SetResolution(1280, 720, false);
                    break;
            }
        }
    }
}
