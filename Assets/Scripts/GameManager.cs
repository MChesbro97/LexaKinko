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
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;
    public Button submitButton;
    public Button resetButton;
    public Button deleteButton;

    private List<char> collectedLetters = new List<char>();
    private List<char> currentWord = new List<char>();

    public LetterZone[] letterZones; 
    private char[] possibleLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(); 
    private int maxCollectedLetters = 7;

    private float buttonSpacing = 30f;
    private float currentXPosition = -135f;

    public bool canShoot = true;
    private int totalScore = 0;
    private int highscore = 0;

    private WordValidator wordValidator;

    private Dictionary<char, int> letterPoints = new Dictionary<char, int>()
    {
        {'A', 1}, {'E', 1}, {'I', 1}, {'O', 1}, {'U', 1}, {'L', 1}, {'N', 1}, {'S', 1}, {'T', 1}, {'R', 1},
        {'D', 2}, {'G', 2},
        {'B', 3}, {'C', 3}, {'M', 3}, {'P', 3},
        {'F', 4}, {'H', 4}, {'V', 4}, {'W', 4}, {'Y', 4},
        {'K', 5},
        {'J', 8}, {'X', 8},
        {'Q', 10}, {'Z', 10}
    };

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
        if (deleteButton != null)
        {
            deleteButton.gameObject.SetActive(false); // Hide initially
            deleteButton.onClick.AddListener(OnDeleteWord); // Add listener for delete button
        }

        wordValidator = GetComponent<WordValidator>();

        // Add listener to the submit button
        submitButton.onClick.AddListener(OnSubmitWord);

        highscore = PlayerPrefs.GetInt("Highscore", 0);

        // Update high score UI
        if (highscoreText != null)
        {
            highscoreText.text = "Highscore: " + highscore;
        }
    }

    public void OnDeleteWord()
    {
        // Clear the current word
        currentWord.Clear();

        // Reactivate all the letter buttons that were previously used
        foreach (Transform child in letterButtonContainer)
        {
            Button letterButton = child.GetComponent<Button>();
            if (!letterButton.gameObject.activeSelf)
            {
                letterButton.gameObject.SetActive(true); // Reactivate button
            }
        }

        // Reset word input text UI
        UpdateWordInputUI();

        // Optionally hide the delete button if no word is formed
        if (deleteButton != null)
        {
            deleteButton.gameObject.SetActive(false);
        }

        Debug.Log("Word cleared and buttons reset.");
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

        if (deleteButton != null)
        {
            deleteButton.gameObject.SetActive(true);
        }
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

        if (scoreText != null)
        {
            scoreText.text = "Score: ";
            //scoreText.gameObject.SetActive(false);
        }

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
        if (wordValidator.ValidateWord(playerWord))
        {
            int wordScore = CalculateWordScore(playerWord);
            //totalScore += wordScore; // Update total score
            if (scoreText != null)
                scoreText.text = "Score: " + wordScore; // Display updated score
            feedbackText.text = "Valid word! You scored " + wordScore + " points!";

            if (wordScore > highscore)
            {
                highscore = wordScore;

                // Save the new high score using PlayerPrefs
                PlayerPrefs.SetInt("Highscore", highscore);
                PlayerPrefs.Save(); // Ensure the high score is saved

                // Update the high score UI
                if (highscoreText != null)
                {
                    highscoreText.text = "Highscore: " + highscore;
                }

                Debug.Log("New highscore: " + highscore);
            }
        }
        else
        {
            feedbackText.text = "Invalid word!";
        }
    }
    private int CalculateWordScore(string word)
    {
        int wordScore = 0;
        foreach (char letter in word.ToUpper())
        {
            if (letterPoints.ContainsKey(letter))
            {
                wordScore += letterPoints[letter];
            }
        }
        return wordScore;
    }

}
