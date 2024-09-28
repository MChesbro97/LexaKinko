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
    public TextMeshProUGUI feedbackText;
    public Button submitButton;
    public Button resetButton;

    private List<char> collectedLetters = new List<char>();
    private List<char> currentWord = new List<char>();

    public LetterZone[] letterZones; 
    private char[] possibleLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(); 
    private int maxCollectedLetters = 7;

    private float buttonSpacing = 30f;
    private float currentXPosition = -135f;

    public bool canShoot = true;

    private WordValidator wordValidator;
    private void Start()
    {
        AssignLettersToZones();
        if (wordInputText != null)
            wordInputText.gameObject.SetActive(false);

        if (submitButton != null)
            submitButton.gameObject.SetActive(false);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        if (resetButton != null)
            resetButton.gameObject.SetActive(false);

        wordValidator = GetComponent<WordValidator>();

        // Add listener to the submit button
        submitButton.onClick.AddListener(OnSubmitWord);
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
        canShoot = false;

        if (wordInputText != null)
            wordInputText.gameObject.SetActive(true); 
        if (submitButton != null)
            submitButton.gameObject.SetActive(true); 
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(true);
        if (resetButton != null)
            resetButton.gameObject.SetActive(true);

        Debug.Log("Shooting disabled. Player must now input a word.");
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
        currentXPosition = -135f;
        if (wordInputText != null)
            wordInputText.gameObject.SetActive(false);

        if (submitButton != null)
            submitButton.gameObject.SetActive(false);

        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.gameObject.SetActive(false);
        }

        if (resetButton != null)
            resetButton.gameObject.SetActive(false);

        canShoot = true;
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

    public void OnSubmitWord()
    {
        // Get the word from the input field
        string playerWord = wordInputText.text;

        // Validate the word using the WordValidator script
        wordValidator.ValidateWord(playerWord);
    }
}
