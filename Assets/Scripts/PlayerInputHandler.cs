using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        // Find the GameManager instance
        gameManager = FindObjectOfType<GameManager>();
    }

    //private void Update()
    //{
    //    // Check for letter input
    //    foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
    //    {
    //        if (Input.GetKeyDown(keyCode))
    //        {
    //            // Check if the key pressed is a letter (A-Z)
    //            char keyPressed = KeyCodeToChar(keyCode);
    //            if (keyPressed != '\0' && IsLetterAvailable(keyPressed))
    //            {
    //                // Add the letter to the current word in the GameManager
    //                gameManager.AddLetterToWord(keyPressed);
    //            }
    //        }
    //    }
    //}

    //// Function to check if the pressed letter is available in collected letters
    //private bool IsLetterAvailable(char letter)
    //{
    //    return gameManager.GetUsableCollectedLetters().Contains(letter);
    //}

    // Function to convert KeyCode to char (A-Z keys only)
    private char KeyCodeToChar(KeyCode keyCode)
    {
        if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
        {
            return (char)((int)keyCode);
        }
        return '\0'; // If not a letter key, return an empty character
    }
}
