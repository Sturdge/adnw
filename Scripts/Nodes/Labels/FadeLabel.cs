using Godot;

public class FadeLabel : AnimatedLabel
{
    [Export] private float newAlpha = 1;

    public override void _Ready()
    {
        base._Ready();
        Set("modulate", new Color(1, 1, 1, 0));
    }

    public override void ShowLabel()
    {
        base.ShowLabel();
        tween = CreateTween();
        tween.TweenProperty(this, "modulate", new Color(1, 1, 1, newAlpha), tweenDuration);
    }

    protected override void HideLabel()
    {
        tween = CreateTween();
        tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0), tweenDuration);
    }
}
