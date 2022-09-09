using Godot;

public class SlideLabel : AnimatedLabel
{
    [Export] Vector2 endPosition;

    Vector2 startPosition;

    public override void _Ready()
    {
        base._Ready();
        startPosition = (Vector2)Get("rect_position");
    }

    public override void ShowLabel()
    {
        base.ShowLabel();
        tween = CreateTween();
        tween.TweenProperty(this, "rect_position", endPosition, tweenDuration);
    }

    protected override void HideLabel()
    {
        tween = CreateTween();
        tween.TweenProperty(this, "rect_position", startPosition, tweenDuration);
    }
}
