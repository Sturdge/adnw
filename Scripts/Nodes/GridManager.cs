using Godot;
using System;

public class GridManager : Node
{
    [Export] private PackedScene tileInstance;

    private int currentAttempt = 0;
    private DictionaryBuilder dictionaryBuilder;

    private WordleTile[,] tileGrid;
    public WordHandler handler { get; private set; }
    private AnimatedLabel invalidLabel;
    public Action OnGameWin;
    public Action OnGameLose;
    public Action OnWrongGuess;

    public override void _Ready()
    {
        dictionaryBuilder = new DictionaryBuilder();

        invalidLabel = GetNode<AnimatedLabel>($"%InvalidWordLabel");
    }

    public override void _ExitTree() => handler.OnWordGuessed -= SubmitHandler;

    public void InstantiateGrid()
    {
        int horizontalOffset = ((GameHandler.WIDTH / 2) - ((GameHandler.TILE_SIZE + GameHandler.MARGIN) * GameHandler.LETTERS) / 2) + (GameHandler.TILE_SIZE / 2) + (GameHandler.MARGIN / 2);
        int verticalOffset = 214;

        tileGrid = new WordleTile[GameHandler.LETTERS, GameHandler.ATTEMPTS];

        for (int y = 0; y < GameHandler.ATTEMPTS; y++)
        {
            for (int x = 0; x < GameHandler.LETTERS; x++)
            {
                Vector2 currentPosition = new Vector2(
                    ((GameHandler.TILE_SIZE + GameHandler.MARGIN) * x) + horizontalOffset,
                    ((GameHandler.TILE_SIZE + GameHandler.MARGIN) * y) + verticalOffset
                );

                WordleTile newTile = tileInstance.Instance<WordleTile>();
                newTile.Position = currentPosition;
                AddChild(newTile);
                newTile.Owner = this;
                tileGrid[x, y] = newTile;
            }
        }

        string chosenWord = dictionaryBuilder.GetRandomWord();

        handler = new WordHandler(chosenWord);

        handler.OnWordGuessed += SubmitHandler;

        GD.Print(chosenWord);
    }

    public void AddLetterToGuess(uint inputNum)
    {
        if (currentAttempt < GameHandler.ATTEMPTS && handler.Guess.Length < GameHandler.LETTERS)
        {
            string input = OS.GetScancodeString(inputNum);
            tileGrid[handler.Guess.Length, currentAttempt].SetLetter(input);
            handler.AddLetter(input);
        }
    }


    public void RemoveLetterFromGuess()
    {
        if (handler.Guess.Length > 0)
        {
            int index = handler.Guess.Length - 1;
            tileGrid[index, currentAttempt].SetLetter(string.Empty);
            handler.RemoveLetter(index);
        }
    }

    public void SubmitGuess()
    {
        if (handler.Guess.Length < GameHandler.LETTERS)
            return;

        if (!dictionaryBuilder.IsWordValid(handler.Guess.ToLower()))
        {
            ClearGuess();
            invalidLabel.ShowLabel();
            return;
        }

        WordleTile[] currentWord = new WordleTile[GameHandler.LETTERS];

        for (int i = 0; i < GameHandler.LETTERS; i++)
            currentWord[i] = tileGrid[i, currentAttempt];

        currentAttempt++;

        handler.IsWordGuessed(currentWord, GameHandler.LETTERS, currentAttempt);
    }

    public void SubmitHandler(bool isGameWon)
    {
        if (isGameWon)
            OnGameWin.Invoke();
        else
        {
            if (currentAttempt == GameHandler.ATTEMPTS)
            {
                foreach (WordleTile tile in tileGrid)
                    tile.PlayFallAnimation();
                OnGameLose.Invoke();
            }
            else
                OnWrongGuess.Invoke();
        }
    }


    public void ClearGuess()
    {
        for (int i = 0; i < GameHandler.LETTERS; i++)
            RemoveLetterFromGuess();
    }

    public void ResetBoard()
    {
        foreach (WordleTile tile in tileGrid)
        {
            tile.QueueFree();
            RemoveChild(tile);
        }

        currentAttempt = 0;

        InstantiateGrid();
    }
}