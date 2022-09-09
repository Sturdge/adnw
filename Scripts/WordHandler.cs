using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class WordHandler
{

    #region FIELDS

    private readonly string wordToGuess;

    private readonly int lettersInWord;

    #endregion

    #region PROPERTIES

    public string Guess { get; private set; } = string.Empty;

    #endregion

    public Action<bool> OnWordGuessed;

    public WordHandler(string chosenWord)
    {
        lettersInWord = chosenWord.Length;
        wordToGuess = chosenWord.ToLower();
    }

    public void AddLetter(string input) => Guess += input.ToLower();

    public void RemoveLetter(int index) => Guess = Guess.Remove(index);

    public async void IsWordGuessed(WordleTile[] word, int lettersInWord, int currentAttempt)
    {
        char[] chosenWordChars = wordToGuess.ToCharArray();

        await CheckCorrect(word, chosenWordChars);
        await CheckAppears(word, chosenWordChars);
        await CheckIncorrect(word, chosenWordChars);

        if (GetCorrectLetters(word, chosenWordChars) < lettersInWord)
        {
            Guess = string.Empty;
            OnWordGuessed.Invoke(false);
        }
        else
            OnWordGuessed.Invoke(true);
    }

    private int GetCorrectLetters(WordleTile[] word, char[] chosenWordChars)
    {
        int correctLetters = 0;
        for (int i = 0; i < lettersInWord; i++)
            if (word[i].Letter == chosenWordChars[i].ToString())
                correctLetters++;

        return correctLetters;
    }

    private async Task CheckCorrect(WordleTile[] word, char[] chosenWordChars)
    {
        for (int i = 0; i < lettersInWord; i++)
            if (word[i].Letter == chosenWordChars[i].ToString())
                if (word[i].State == GuessResult.Default)
                    word[i].SetResult(GuessResult.Correct, i);

        await Task.Yield();
    }

    private async Task CheckIncorrect(WordleTile[] word, char[] chosenWordChars)
    {
        for (int i = 0; i < lettersInWord; i++)
            if (word[i].Letter != chosenWordChars[i].ToString())
                if (word[i].State == GuessResult.Default)
                    word[i].SetResult(GuessResult.Incorrect, i);

        await Task.Yield();
    }

    private async Task CheckAppears(WordleTile[] word, char[] chosenWordChars)
    {
        int letterInstances = 0;
        List<char> uniqueLetters = new List<char>();

        foreach (char letter in chosenWordChars)
        {
            if (!uniqueLetters.Contains(letter))
                uniqueLetters.Add(letter);
        }

        foreach (char letter in uniqueLetters)
        {
            letterInstances = 0;

            foreach (char letterInSolution in chosenWordChars)
                if (letterInSolution == letter)
                    letterInstances++;

            foreach(WordleTile tile in word)
                if(tile.Letter == letter.ToString() && tile.State == GuessResult.Correct)
                    letterInstances--;

            for (int guessLetter = 0; guessLetter < lettersInWord; guessLetter++)
            {
                if (letterInstances <= 0)
                    break;
                if (word[guessLetter].Letter == letter.ToString())
                {
                    word[guessLetter].SetResult(GuessResult.Appears, guessLetter);
                    letterInstances--;
                }
            }
        }

        await Task.Yield();
    }
}