using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject letterButtonPrefab;
    public Transform letterButtonContainer;
    public TextMeshProUGUI wordInputText;

    private List<char> collectedLetters = new List<char>();
    private List<char> currentWord = new List<char>();

    public LetterZone[] letterZones; 
    private char[] possibleLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(); 
    private int maxCollectedLetters = 7;

    private float buttonSpacing = 30f;
    private float currentXPosition = -135f;

    public bool canShoot = true;
    private void Start()
    {
        AssignLettersToZones();
    }

    public void AssignLettersToZones()
    {
        List<char> availableLetters = new List<char>(possibleLetters);

        foreach (var zone in letterZones)
        {
            if (availableLetters.Count > 0)
            {
                int randomIndex = Random.Range(0, availableLetters.Count);

                char randomLetter = availableLetters[randomIndex];
                zone.SetLetter(randomLetter);

                availableLetters.RemoveAt(randomIndex);
            }
        }
    }

    // Function to collect a letter when the ball collides with a zone
    public void CollectLetter(char letter)
    {
        if (collectedLetters.Count >= maxCollectedLetters) return;

        collectedLetters.Add(letter);

        GameObject letterButton = Instantiate(letterButtonPrefab, letterButtonContainer);
        letterButton.GetComponentInChildren<TextMeshProUGUI>().text = letter.ToString(); // Set button text to letter

        RectTransform buttonRectTransform = letterButton.GetComponent<RectTransform>();
        buttonRectTransform.anchoredPosition = new Vector2(currentXPosition, 0);
        currentXPosition += buttonSpacing;

        // Add a listener to the button click event
        letterButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (collectedLetters.Count < maxCollectedLetters) return;
            AddLetterToWord(letter);
            letterButton.SetActive(false); // Disable button after it's clicked
        });

        if (collectedLetters.Count == maxCollectedLetters)
        {
            DisableShooting();
        }
    }

    private void DisableShooting()
    {
        canShoot = false; // Disable shooting
        // You can add additional logic here to visually indicate that shooting is disabled
        // For example: show a message or change the color of buttons
    }

    public void AddLetterToWord(char letter)
    {
        currentWord.Add(letter);
        UpdateWordInputUI();
    }

    public void ResetGame()
    {
        collectedLetters.Clear();
        currentWord.Clear();
        foreach (Transform child in letterButtonContainer)
        {
            Destroy(child.gameObject); // Remove all letter buttons from UI
        }
        UpdateWordInputUI();
        AssignLettersToZones();
    }

    public List<char> GetCollectedLetters()
    {
        return new List<char>(collectedLetters);
    }

    public string GetCurrentWord()
    {
        return new string(currentWord.ToArray());
    }

    private void UpdateWordInputUI()
    {
        wordInputText.text = new string(currentWord.ToArray());
    }
}
