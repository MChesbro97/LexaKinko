using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI collectedLettersText; // UI element to display collected letters
    public List<char> collectedLetters = new List<char>(); // List of collected letters
    public LetterZone[] letterZones; // Array to hold references to all letter zones

    private char[] possibleLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(); // All possible letters
    private int maxCollectedLetters = 10;

    private void Start()
    {
        // Set initial letters to zones when the game starts
        AssignLettersToZones();
        UpdateCollectedLettersUI();
    }

    // Function to assign random letters to the zones
    public void AssignLettersToZones()
    {
        foreach (var zone in letterZones)
        {
            char randomLetter = possibleLetters[Random.Range(0, possibleLetters.Length)];
            zone.SetLetter(randomLetter);
        }
    }

    // Function to collect a letter when the ball collides with a zone
    public void CollectLetter(char letter)
    {
        if (collectedLetters.Count < maxCollectedLetters)
        {
            collectedLetters.Add(letter);
            UpdateCollectedLettersUI();
        }
    }

    // Function to update the UI with collected letters
    private void UpdateCollectedLettersUI()
    {
        collectedLettersText.text = "Collected Letters: " + string.Join(" ", collectedLetters);
    }

    // Function to reset letters and clear the collected list
    public void ResetGame()
    {
        collectedLetters.Clear();
        AssignLettersToZones();
        UpdateCollectedLettersUI();
    }
}
