using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LetterZone : MonoBehaviour
{
    public char assignedLetter;
    public TextMeshProUGUI letterText;
    private GameManager gameManager; 

    private void Start()
    {
        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();

        UpdateLetterText();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();

            if (ball != null && ball.CanCollectLetter()) // Check if the ball can collect the letter
            {
                // Add the letter to the GameManager
                gameManager.CollectLetter(assignedLetter);
                ball.MarkLetterAsCollected(); // Mark letter as collected

                // Disable this collider to prevent further collections (optional)
                // collider.enabled = false;

                Debug.Log("Collected letter: " + assignedLetter);
            }
        }
    }

    // Function to set a new letter when the ball resets
    public void SetLetter(char newLetter)
    {
        assignedLetter = newLetter;
        UpdateLetterText();
    }

    private void UpdateLetterText()
    {
        if (letterText != null)
        {
            letterText.text = assignedLetter.ToString();
        }
    }
}
