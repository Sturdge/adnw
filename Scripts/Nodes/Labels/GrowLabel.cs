using Godot;

public class GrowLabel : AnimatedLabel
{
    [Export] private bool animateX = true;
    [Export] private bool animateY = true;

    public override void _Ready()
    {
        base._Ready();
        Set("rect_scale", GetNewSize());
    }

    public override void ShowLabel()
    {
        base.ShowLabel();
        tween = CreateTween();
        tween.TweenProperty(this, "rect_scale", new Vector2(1, 1), tweenDuration);
    }

    protected override void HideLabel()
    {
        tween = CreateTween();
        tween.TweenProperty(this, "rect_scale", GetNewSize(), tweenDuration);
    }

    private Vector2 GetNewSize()
    {
        float newX = animateX ? 0 : 1;
        float newY = animateY ? 0 : 1;

        return new Vector2(newX, newY);
    }
}