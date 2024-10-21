using TMPro;
using UnityEngine;

public class LetterZone : MonoBehaviour
{
    public char assignedLetter; // The letter assigned to this zone
    private int baseScore; // The base score for the assigned letter

    // Optional: Reference to GameManager if you need to access it directly
    private GameManager gameManager;

    public TMP_Text letterDisplay;

    private void Awake()
    {
        // Optionally, find the GameManager
        gameManager = FindObjectOfType<GameManager>();
    }

    // Method to assign a letter to this zone
    public void SetLetter(char letter)
    {
        assignedLetter = letter;
        baseScore = gameManager.GetLetterBaseScore(letter);

        UpdateLetterDisplay();
    }

    // Method to get a UsableLetter object from this zone
    public UsableLetter GetUsableLetter()
    {
        return new UsableLetter(assignedLetter, baseScore);
    }

    private void UpdateLetterDisplay()
    {
        if (letterDisplay != null)
        {
            letterDisplay.text = $"{assignedLetter}<size=70%><sub>{baseScore}</sub></size>";
        }
        else
        {
            Debug.LogWarning("Letter display is not assigned!");
        }
    }
    // Add more functionality as needed (like triggering when a ball enters the zone)
}
