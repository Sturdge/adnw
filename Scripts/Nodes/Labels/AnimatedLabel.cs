using Godot;

public abstract class AnimatedLabel : Panel
{
    [Export] protected readonly float tweenDuration;
    [Export] protected readonly float displayDuration;

    protected SceneTreeTween tween;
    protected RichTextLabel text;
    protected Timer timer;

    public override void _Ready()
    {
        timer = GetNode<Timer>("%LabelTimer");
        timer.WaitTime = displayDuration;
        timer.Connect("timeout", this, nameof(HideLabel));
    }

    public virtual void ShowLabel()
    {
        timer.Start();
    }

    protected abstract void HideLabel();
}