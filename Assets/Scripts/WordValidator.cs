using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using UnityEngine;

public class WordValidator : MonoBehaviour
{
    public TextMeshProUGUI feedbackText; // For displaying feedback to the player
    private HashSet<string> validWords;

    private void Start()
    {
        // Load the word list from the Resources folder
        LoadWordList();
    }

    private void LoadWordList()
    {
        // Initialize the HashSet to store valid words
        validWords = new HashSet<string>();

        // Load the word list file from the Resources folder
        TextAsset wordListFile = Resources.Load<TextAsset>("unsorted");

        if (wordListFile != null)
        {
            // Split the content by lines (each line is a word)
            string[] words = wordListFile.text.Split('\n');

            // Add each word to the HashSet
            foreach (string word in words)
            {
                // Trim to remove any trailing spaces or new lines
                validWords.Add(word.Trim().ToUpper());
            }

            Debug.Log("Word list loaded with " + validWords.Count + " words.");
        }
        else
        {
            Debug.LogError("Word list file not found!");
        }
    }

    public bool ValidateWord(string playerWord)
    {
        if (validWords.Contains(playerWord.ToUpper().Trim()))
        {
            feedbackText.text = "Valid word!";
            Debug.Log("Word is valid: " + playerWord);
            return true;
        }
        else
        {
            feedbackText.text = "Invalid word.";
            Debug.Log("Word is invalid: " + playerWord);
            return false;
        }
    }
}
