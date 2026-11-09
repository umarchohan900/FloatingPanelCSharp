namespace FloatingPanelCSharp;

public class MainPage : ContentPage
{
    readonly Border _panel;
    readonly Button _floatingButton;
    readonly Box3DDrawable _boxDrawable;
    double _panStartX;
    double _panStartY;

    public MainPage()
    {
        Title = "3D Box Demo";
        BackgroundColor = Color.FromArgb("#100D18");

        var heading = new Label
        {
            Text = "3D WIREFRAME DEMO",
            TextColor = Colors.White,
            FontSize = 23,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center
        };

        var subtitle = new Label
        {
            Text = "A local visual demo — no game or other app interaction",
            TextColor = Color.FromArgb("#BDB4CC"),
            FontSize = 12,
            HorizontalTextAlignment = TextAlignment.Center
        };

        _boxDrawable = new Box3DDrawable();
        var boxView = new GraphicsView
        {
            Drawable = _boxDrawable,
            HeightRequest = 300,
            HorizontalOptions = LayoutOptions.Fill,
            BackgroundColor = Color.FromArgb("#171222")
        };

        var rotationSlider = new Slider
        {
            Minimum = 0,
            Maximum = 360,
            Value = 25,
            MinimumTrackColor = Color.FromArgb("#C4A7F5"),
            MaximumTrackColor = Color.FromArgb("#514466"),
            ThumbColor = Color.FromArgb("#FF6B9D")
        };
        rotationSlider.ValueChanged += (_, e) =>
        {
            _boxDrawable.RotationDegrees = (float)e.NewValue;
            boxView.Invalidate();
        };

        var resetButton = new Button
        {
            Text = "RESET ROTATION",
            BackgroundColor = Color.FromArgb("#38294F"),
            TextColor = Colors.White,
            CornerRadius = 10
        };
        resetButton.Clicked += (_, _) => rotationSlider.Value = 25;

        var panelTitle = new Label
        {
            Text = "CONTROL PANEL",
            TextColor = Colors.White,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold
        };
        var panelDescription = new Label
        {
            Text = "This is an in-app demonstration. The 3D box uses sample coordinates and does not detect players, vehicles, or game objects.",
            TextColor = Color.FromArgb("#D8D1E3"),
            FontSize = 13
        };
        var closeButton = new Button
        {
            Text = "CLOSE PANEL",
            BackgroundColor = Color.FromArgb("#8B5CF6"),
            TextColor = Colors.White,
            CornerRadius = 12
        };
        closeButton.Clicked += (_, _) => _panel.IsVisible = false;

        _panel = new Border
        {
            IsVisible = false,
            BackgroundColor = Color.FromArgb("#211A30"),
            Stroke = Color.FromArgb("#8B5CF6"),
            StrokeThickness = 1,
            Padding = 18,
            StrokeShape = new Microsoft.Maui.Controls.RoundRectangle { CornerRadius = 18 },
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Children = { panelTitle, panelDescription, closeButton }
            }
        };

        _floatingButton = new Button
        {
            Text = "OPEN",
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#8B5CF6"),
            BorderColor = Color.FromArgb("#C4A7F5"),
            BorderWidth = 2,
            CornerRadius = 28,
            WidthRequest = 76,
            HeightRequest = 56,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 18, 22)
        };
        _floatingButton.Clicked += (_, _) => _panel.IsVisible = !_panel.IsVisible;

        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        _floatingButton.GestureRecognizers.Add(pan);

        var content = new VerticalStackLayout
        {
            Padding = new Thickness(18, 42, 18, 90),
            Spacing = 12,
            Children = { heading, subtitle, boxView, new Label { Text = "ROTATION", TextColor = Colors.White }, rotationSlider, resetButton }
        };

        var layout = new Grid();
        layout.Children.Add(content);
        var panelHost = new Grid
        {
            Padding = new Thickness(22),
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Fill
        };
        panelHost.Children.Add(_panel);
        layout.Children.Add(panelHost);
        layout.Children.Add(_floatingButton);
        Content = layout;
    }

    void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _panStartX = _floatingButton.TranslationX;
                _panStartY = _floatingButton.TranslationY;
                break;
            case GestureStatus.Running:
                _floatingButton.TranslationX = _panStartX + e.TotalX;
                _floatingButton.TranslationY = _panStartY + e.TotalY;
                break;
        }
    }
}

public sealed class Box3DDrawable : IDrawable
{
    public float RotationDegrees { get; set; } = 25f;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();
        canvas.FillColor = Color.FromArgb("#171222");
        canvas.FillRectangle(dirtyRect);

        float scale = Math.Min(dirtyRect.Width, dirtyRect.Height) * 0.42f;
        float cx = dirtyRect.Center.X;
        float cy = dirtyRect.Center.Y;
        const float w = 1.15f, d = 0.8f, h = 1.65f;
        float angle = RotationDegrees * MathF.PI / 180f;
        float cos = MathF.Cos(angle), sin = MathF.Sin(angle);

        PointF Project(float x, float y, float z)
        {
            float rx = x * cos - y * sin;
            float ry = x * sin + y * cos;
            return new PointF(cx + (rx - ry) * scale * 0.72f,
                cy + (rx + ry) * scale * 0.34f - z * scale * 0.78f);
        }

        PointF[] p =
        {
            Project(-w/2, -d/2, 0), Project(w/2, -d/2, 0),
            Project(w/2, d/2, 0), Project(-w/2, d/2, 0),
            Project(-w/2, -d/2, h), Project(w/2, -d/2, h),
            Project(w/2, d/2, h), Project(-w/2, d/2, h)
        };
        int[,] edges =
        {
            {0,1},{1,2},{2,3},{3,0}, {4,5},{5,6},{6,7},{7,4},
            {0,4},{1,5},{2,6},{3,7}
        };

        canvas.StrokeColor = Color.FromArgb("#C4A7F5");
        canvas.StrokeSize = 3;
        canvas.StrokeLineCap = LineCap.Round;
        for (int i = 0; i < edges.GetLength(0); i++)
            canvas.DrawLine(p[edges[i, 0]], p[edges[i, 1]]);

        canvas.FontColor = Color.FromArgb("#FF6B9D");
        canvas.FontSize = 12;
        canvas.DrawString($"3D BOX  •  {RotationDegrees:0}°", 12, 12,
            Math.Max(1, dirtyRect.Width - 24), 22,
            HorizontalAlignment.Left, VerticalAlignment.Top);
        canvas.RestoreState();
    }
}
