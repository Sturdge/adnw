using Godot;

public class GameHandler : Node
{
    #region  CONSTANTS

    public const int LETTERS = 5;
    public const int ATTEMPTS = 6;
    public const int WIDTH = 1024;
    public const int HEIGHT = 768;
    public const int TILE_SIZE = 50;
    public const int MARGIN = 10;
    #endregion

    #region NODES

    private GridManager gridManager;
    private Particles2D leftConfetti;
    private Particles2D rightConfetti;
    private AudioStreamPlayer2D winAudio;
    private AudioStreamPlayer2D loseAudio;
    private AudioStreamPlayer2D incorrectAudio;

    #endregion

    private Timer timer;

    public override void _Ready()
    {
        gridManager = GetNode<GridManager>($"%GridManager");
        gridManager.InstantiateGrid();

        leftConfetti = GetNode<Particles2D>($"%ConfettiLeft");
        rightConfetti = GetNode<Particles2D>($"%ConfettiRight");
        winAudio = GetNode<AudioStreamPlayer2D>($"WinAudio");
        loseAudio = GetNode<AudioStreamPlayer2D>($"LoseAudio");
        incorrectAudio = GetNode<AudioStreamPlayer2D>($"IncorrectAudio");

        gridManager.OnGameWin += WinGame;
        gridManager.OnGameLose += LoseGame;
        gridManager.OnWrongGuess += () => incorrectAudio.Play();

        timer = GetNode<Timer>($"%ResetTimer");
        timer.OneShot = true;
        timer.WaitTime = 3;
        timer.Connect("timeout", this, nameof(Restart));
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventKey eventKey && !e.IsPressed())
        {
            uint inputNum = eventKey.Scancode;

            if(inputNum == (int)KeyList.Enter)
                gridManager.SubmitGuess();
                
            if (inputNum == (int)KeyList.Backspace)
                gridManager.RemoveLetterFromGuess();
            
            if (inputNum > 64 && inputNum < 91)
                gridManager.AddLetterToGuess(inputNum);
        }
    }

    public override void _ExitTree()
    {
        gridManager.OnGameWin -= WinGame;
        gridManager.OnGameLose -= LoseGame;
        gridManager.OnWrongGuess -= () => incorrectAudio.Play();
    }

    private void WinGame()
    {
        leftConfetti.Emitting = true;
        rightConfetti.Emitting = true;
        winAudio.Play();
        timer.Start();
    }

    private void LoseGame()
    {
        loseAudio.Play();
        timer.Start();
    }

    private void Restart()
    {
        leftConfetti.Emitting = false;
        rightConfetti.Emitting = false;
        gridManager.ResetBoard();
    }
}