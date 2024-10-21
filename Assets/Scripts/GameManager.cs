using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

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

    private List<UsableLetter> collectedUsableLetters = new List<UsableLetter>();
    private List<char> currentWord = new List<char>();
    private List<GameObject> allPegs = new List<GameObject>();

    public LetterZone[] letterZones;
    //private char[] possibleLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private int maxCollectedLetters = 7;

    private float buttonSpacing = 30f;
    private float currentXPosition = -135f;

    public bool canShoot = true;
    private int totalScore = 0;
    private int highscore = 0;

    private WordValidator wordValidator;

    public Dictionary<char, int> letterPoints = new Dictionary<char, int>()
    {
        {'A', 1}, {'E', 1}, {'I', 1}, {'O', 1}, {'U', 1}, {'L', 1}, {'N', 1}, {'S', 1}, {'T', 1}, {'R', 1},
        {'D', 2}, {'G', 2},
        {'B', 3}, {'C', 3}, {'M', 3}, {'P', 3},
        {'F', 4}, {'H', 4}, {'V', 4}, {'W', 4}, {'Y', 4},
        {'K', 5},
        {'J', 8}, {'X', 8},
        {'Q', 10}, {'Z', 10}
    };
    public List<char> possibleLetters = new List<char>
{
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
};

    private void Start()
    {
        AssignLettersToZones();
        wordInputText.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false); // Hide initially
        deleteButton.onClick.AddListener(OnDeleteWord);

        wordValidator = GetComponent<WordValidator>();
        submitButton.onClick.AddListener(OnSubmitWord);

        highscore = PlayerPrefs.GetInt("Highscore", 0);
        highscoreText.text = "Highscore: " + highscore;
    }

    public void OnDeleteWord()
    {
        currentWord.Clear();

        foreach (Transform child in letterButtonContainer)
        {
            Button letterButton = child.GetComponent<Button>();
            letterButton.gameObject.SetActive(true); // Reactivate button
        }

        UpdateWordInputUI();
        deleteButton.gameObject.SetActive(false); // Hide the delete button
        Debug.Log("Word cleared and buttons reset.");
    }

    public void AssignLettersToZones()
    {
        List<char> availableLetters = new List<char>(possibleLetters);
        List<char> vowels = new List<char> { 'A', 'E', 'I', 'O', 'U' };

        // Randomly select positions for two vowels
        int firstVowelPosition = Random.Range(0, letterZones.Length);
        int secondVowelPosition;

        do
        {
            secondVowelPosition = Random.Range(0, letterZones.Length);
        } while (secondVowelPosition == firstVowelPosition);

        // Assign vowels to their respective zones
        for (int i = 0; i < 2; i++)
        {
            int vowelIndex = Random.Range(0, vowels.Count);
            char randomVowel = vowels[vowelIndex];

            letterZones[i == 0 ? firstVowelPosition : secondVowelPosition].SetLetter(randomVowel);
            Debug.Log($"Assigned vowel {randomVowel} to zone");

            vowels.RemoveAt(vowelIndex);
        }

        // Assign remaining letters to the other letter zones
        for (int i = 0; i < letterZones.Length; i++)
        {
            if (i == firstVowelPosition || i == secondVowelPosition) continue;

            if (availableLetters.Count > 0)
            {
                int randomIndex = Random.Range(0, availableLetters.Count);
                char randomLetter = availableLetters[randomIndex];

                // Set the letter and its score in the letter zone
                letterZones[i].SetLetter(randomLetter);
                availableLetters.RemoveAt(randomIndex); // Remove the letter from the available list
            }
        }
    }


    public void CollectLetter(UsableLetter usableLetter)
    {
        // Don't allow more letters to be collected than the maximum
        if (collectedUsableLetters.Count >= maxCollectedLetters) return;

        // Add the letter to the collection
        collectedUsableLetters.Add(usableLetter);

        // Create the letter button
        GameObject letterButton = Instantiate(letterButtonPrefab, letterButtonContainer);
        TextMeshProUGUI buttonText = letterButton.GetComponentInChildren<TextMeshProUGUI>();

        // Set the button text to display the letter and its score
        buttonText.text = $"{usableLetter.Letter}<size=70%><sub>{usableLetter.Score}</sub></size>";

        // Position the button in the UI
        RectTransform buttonRectTransform = letterButton.GetComponent<RectTransform>();
        buttonRectTransform.anchoredPosition = new Vector2(currentXPosition, 0);
        currentXPosition += buttonSpacing;

        // Add an onClick listener to the button
        letterButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            // Add the letter to the word (if not already at max letters)
            AddLetterToWord(usableLetter.Letter);

            // Disable the button after it's used
            letterButton.SetActive(false);
        });

        // Disable shooting if the max letters have been collected
        if (collectedUsableLetters.Count == maxCollectedLetters)
        {
            DisableShooting();
            DisableAllPegs();

        }
    }

    public void DisableAllPegs()
    {
        foreach (GameObject peg in allPegs)
        {
            peg.SetActive(false); // Disable each peg in the list
        }
    }


    public void UpdateLetterScore(UsableLetter usableLetter, int newScore)
    {
        foreach (Transform child in letterButtonContainer)
        {
            TextMeshProUGUI buttonText = child.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null && buttonText.text.StartsWith(usableLetter.Letter.ToString()))
            {
                buttonText.text = $"{usableLetter.Letter}<size=70%><sub>{newScore}</sub></size>";
            }
        }
    }

    private void DisableShooting()
    {
        canShoot = false;
        wordInputText.gameObject.SetActive(true);
        submitButton.gameObject.SetActive(true);
        feedbackText.gameObject.SetActive(true);
        resetButton.gameObject.SetActive(true);
        Debug.Log("Shooting disabled. Player must now input a word.");
    }

    public void AddLetterToWord(char letter)
    {
        currentWord.Add(letter);
        UpdateWordInputUI();
        deleteButton.gameObject.SetActive(true);
    }

    public void ResetGame()
    {
        collectedUsableLetters.Clear();
        currentWord.Clear();
        ResetPegs();

        foreach (Transform child in letterButtonContainer)
        {
            Destroy(child.gameObject);
        }

        UpdateWordInputUI();
        AssignLettersToZones();
        currentXPosition = -135f;

        wordInputText.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
        feedbackText.text = "";
        feedbackText.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
        scoreText.text = "Score: ";

        canShoot = true;
    }

    public List<UsableLetter> GetCollectedUsableLetters()
    {
        return new List<UsableLetter>(collectedUsableLetters);
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
        string playerWord = wordInputText.text;

        if (wordValidator.ValidateWord(playerWord))
        {
            int wordScore = CalculateWordScore(playerWord);
            scoreText.text = "Score: " + wordScore;
            feedbackText.text = "Valid word! You scored " + wordScore + " points!";

            if (wordScore > highscore)
            {
                highscore = wordScore;
                PlayerPrefs.SetInt("Highscore", highscore);
                PlayerPrefs.Save();
                highscoreText.text = "Highscore: " + highscore;
                Debug.Log("New highscore: " + highscore);
            }

            //wordInputText.text = string.Empty;
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
            // Check if the letter was collected and is part of the usable letters
            UsableLetter usableLetter = collectedUsableLetters.Find(l => l.Letter == letter);
            if (usableLetter != null)
            {
                // Add the score of the collected letter
                wordScore += usableLetter.Score;
            }
        }
        return wordScore;
    }

    public void AddPointsToLetter(UsableLetter usableLetter, int points)
    {
        foreach (var letter in collectedUsableLetters)
        {
            if (letter.Letter == usableLetter.Letter)
            {
                letter.Score += points;
                Debug.Log($"Letter {letter.Letter} score updated to {letter.Score}");
                return;
            }
        }
    }

    public int GetLetterBaseScore(char letter)
    {
        return letterPoints.TryGetValue(letter, out int score) ? score : 0;
    }

    public void RegisterPeg(GameObject peg)
    {
        if (!allPegs.Contains(peg))
        {
            allPegs.Add(peg);
        }
    }

    private void ResetPegs()
    {
        foreach (GameObject peg in allPegs)
        {
            if (peg != null)
            {
                peg.SetActive(true);
            }
        }
    }
}
