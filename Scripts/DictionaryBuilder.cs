using Godot;
using System;
using System.Collections.Generic;

public class DictionaryBuilder
{
    private List<string> validWords = new List<string>();

    private Random rng = new Random();

    public DictionaryBuilder()
    {
        File file = new File();
        file.Open(@"res://Resources/words.res", File.ModeFlags.Read);

        int index = 0;

        while(!file.EofReached())
        {
            string line = file.GetLine();
            validWords.Add(line);

            index++;
        }

        file.Close();
    }

    public string GetRandomWord()
    {
        int index = rng.Next(validWords.Count);
        return validWords[index];
    }

    public bool IsWordValid(string word)
    {
        return validWords.Contains(word);
    }
}
