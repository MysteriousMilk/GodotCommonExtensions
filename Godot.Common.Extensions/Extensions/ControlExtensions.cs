using Godot;

public static class ControlExtensions
{
    /// <summary>
    /// "Fades" a UI Control in over a given duration (seconds).
    /// </summary>
    /// <param name="ctrl">The control to "fade in".</param>
    /// <param name="duration">Animation duration.</param>
    public static void FadeIn(this Control ctrl, float duration)
    {
        if (ctrl.Visible)
            return;

        if (ctrl.HasMeta("is_fade_transitioning") &&
            ctrl.GetMeta("is_fade_transitioning").AsBool())
        {
            return;
        }

        var modulate = ctrl.Modulate;

        ctrl.SetMeta("is_fade_transitioning", true);
        ctrl.Visible = true;
        ctrl.Modulate = Colors.Transparent;

        var tween = ctrl.CreateTween();
        tween.TweenProperty(ctrl, (string)Control.PropertyName.Modulate, modulate, duration).SetEase(Tween.EaseType.In);
        tween.Finished += () =>
        {
            ctrl.SetMeta("is_fade_transitioning", false);
        };
    }

    /// <summary>
    /// "Fades" a UI Control out over a given duration (seconds).
    /// </summary>
    /// <param name="ctrl">The control to "fade out".</param>
    /// <param name="duration">Animation duration.</param>
    public static void FadeOut(this Control ctrl, float duration)
    {
        if (ctrl == null)
            return;

        if (!ctrl.Visible)
            return;

        if (ctrl.HasMeta("is_fade_transitioning") &&
            ctrl.GetMeta("is_fade_transitioning").AsBool())
        {
            return;
        }

        var modulate = ctrl.Modulate;
        ctrl.SetMeta("is_fade_transitioning", true);

        var tween = ctrl.CreateTween();
        tween.TweenProperty(ctrl, (string)Control.PropertyName.Modulate, Colors.Transparent, duration).SetEase(Tween.EaseType.In);
        tween.Finished += () =>
        {
            ctrl.SetMeta("is_fade_transitioning", false);
            ctrl.Hide();
            ctrl.Modulate = modulate;
        };
    }
}