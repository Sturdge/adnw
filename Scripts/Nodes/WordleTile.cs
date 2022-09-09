using Godot;
using System;

public class WordleTile : Sprite
{
    [Export] private Texture[] textures;
    [Export] private float textTweenDuration;
    [Export] private float largeSize;
    [Export] private float tileTweenDuration;

    private Random rng = new Random();

    private AudioStreamPlayer2D typingAudio;
    private AudioStreamPlayer2D cancelAudio;
    private Label letterLabel;
    private SceneTreeTween tween;

    public string Letter { get; private set; } = string.Empty;
    public GuessResult State { get; private set; } = GuessResult.Default;

    public override void _Ready()
    {
        typingAudio = GetNode<AudioStreamPlayer2D>($"TypingAudio");
        cancelAudio = GetNode<AudioStreamPlayer2D>($"CancelAudio");
        letterLabel = GetNode<Label>($"%Letter");

        letterLabel.Set("rect_scale", Vector2.Zero);
    }

    public void SetLetter(string newLetter)
    {
        Letter = newLetter.ToLower();
        letterLabel.Text = Letter;

        if (Letter != string.Empty)
        {
            tween = CreateTween();
            tween.TweenProperty(letterLabel, "rect_scale", new Vector2(largeSize, largeSize), textTweenDuration);
            tween.TweenProperty(letterLabel, "rect_scale", Vector2.One, textTweenDuration);

            typingAudio.Play();
        }
        else
        {
            letterLabel.Set("rect_scale", Vector2.Zero);
            cancelAudio.Play();
        }
    }

    public void SetResult(GuessResult result, int index)
    {
        State = result;

        float delay = 0.05f * index;

        tween = CreateTween();
        tween.TweenInterval(delay);
        tween.TweenProperty(this, "texture", textures[(int)State], tileTweenDuration);

        switch (result)
        {
            case GuessResult.Correct:
                tween.TweenProperty(this, "scale", new Vector2(largeSize, largeSize), tileTweenDuration);
                tween.TweenProperty(this, "scale", Vector2.One, tileTweenDuration);
                break;

            case GuessResult.Incorrect:
                tween.TweenProperty(this, "rotation_degrees", -10f, tileTweenDuration);
                tween.TweenProperty(this, "rotation_degrees", 10f, tileTweenDuration);
                tween.TweenProperty(this, "rotation_degrees", 0f, tileTweenDuration);
                break;
        }
    }

    public void PlayFallAnimation()
    {
        float angle = 45;
        float angleOffset = rng.Next(-15, 16);
        angle += angleOffset;
        float dir = rng.Next(2);
        if(dir % 2 == 0)
            angle *= -1;

        float speed = 0.5f;
        float speedOffset = rng.Next(-1, 2);
        speedOffset /= 2;
        speed += speedOffset;

        Vector2 currentPosition = Position;
        Vector2 newPosition = new Vector2(
            currentPosition.x,
            1000
        );

        tween = CreateTween();
        tween.TweenProperty(this, "rotation_degrees", angle, speed);
        tween.Parallel().TweenProperty(this, "position", newPosition, speed);
    }
}
