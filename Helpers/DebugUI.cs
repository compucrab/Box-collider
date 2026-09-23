using System;
using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

internal static class DebugUI
{
    public static Desktop Desktop { get; private set; }

    private static Panel _panel;
    private static VerticalStackPanel _content;

    public static void Initialize(Game game)
    {
        MyraEnvironment.Game = game;

        Desktop = new Desktop();

        // Fixed outer panel
        _panel = new Panel
        {
            Width = 300,
            Height = 400,
            Left = 10,
            Top = 10,
            Background = new SolidBrush(Color.Black * 0.8f),
        };

        Desktop.Root = _panel;

        // Scrollable area
        var scrollViewer = new ScrollViewer
        {
            Width = 280,
            Height = 380,
            Left = 10,
            Top = 10,
        };

        _panel.Widgets.Add(scrollViewer);

        // Everything gets added here
        _content = new VerticalStackPanel { Width = 260, Spacing = 15 };

        scrollViewer.Content = _content;
    }

    public static Label AddLabel(string text)
    {
        var label = new Label { Text = text };
        _content.Widgets.Add(label);
        return label;
    }

    public static HorizontalSlider AddSlider(
        string name,
        float min,
        float max,
        Action<float> onChanged
    )
    {
        var row = new VerticalStackPanel { Spacing = 5 };

        // Label
        row.Widgets.Add(new Label { Text = name });

        // Slider
        var slider = new HorizontalSlider
        {
            Minimum = min,
            Maximum = max,
            Value = min,
            Width = 250,
        };

        slider.ValueChanged += (sender, args) =>
        {
            onChanged(args.NewValue);
        };

        row.Widgets.Add(slider);

        _content.Widgets.Add(row);

        return slider;
    }

    public static void AddButton(string text, Action onClick)
    {
        var button = new Button
        {
            Content = new Label { Text = text },
            Padding = new Thickness(20, 10, 20, 10),
            Background = new SolidBrush(Color.DarkSlateBlue),
            PressedBackground = new SolidBrush(Color.Gray),
        };

        button.Click += (sender, args) =>
        {
            onClick();
        };

        _content.Widgets.Add(button);
    }

    public static void Draw()
    {
        Desktop.Render();
    }
}
