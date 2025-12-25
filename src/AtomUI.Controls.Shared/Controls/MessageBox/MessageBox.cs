using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public enum MessageBoxOkButtonStyle
{
    Default,
    Primary
}

public class MessageBox : Window
{
    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<MessageBox, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<MessageBoxStyle> StyleProperty =
        AvaloniaProperty.Register<MessageBox, MessageBoxStyle>(nameof(Style), MessageBoxStyle.Information);

    public static readonly StyledProperty<MessageBoxOkButtonStyle> OkButtonStyleProperty =
        AvaloniaProperty.Register<MessageBox, MessageBoxOkButtonStyle>(nameof(OkButtonStyle), MessageBoxOkButtonStyle.Primary);

    public static readonly StyledProperty<string?> OkButtonTextProperty = AvaloniaProperty.Register<MessageBox, string?>(nameof(OkButtonText));

    public static readonly StyledProperty<string?> CancelButtonTextProperty = AvaloniaProperty.Register<MessageBox, string?>(nameof(CancelButtonText));

    public static readonly StyledProperty<bool> IsLoadingProperty = AvaloniaProperty.Register<MessageBox, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> IsConfirmLoadingProperty = AvaloniaProperty.Register<MessageBox, bool>(nameof(IsConfirmLoading));

    public static readonly StyledProperty<bool> IsLightDismissEnabledProperty =
        AvaloniaProperty.Register<MessageBox, bool>(nameof(IsLightDismissEnabled));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<MessageBox, bool>(nameof(IsOpen));

    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<MessageBox, bool>(nameof(IsModal));

    public static readonly StyledProperty<bool> IsDragMovableProperty =
        AvaloniaProperty.Register<MessageBox, bool>(nameof(IsDragMovable));

    public static readonly StyledProperty<object?> ResultProperty =
        AvaloniaProperty.Register<MessageBox, object?>(nameof(Result));

    public static readonly StyledProperty<bool> IsCenterOnStartupProperty =
        AvaloniaProperty.Register<MessageBox, bool>(nameof(IsCenterOnStartup), true);

    public static readonly StyledProperty<Dimension?> HorizontalOffsetProperty =
        AvaloniaProperty.Register<MessageBox, Dimension?>(nameof(HorizontalOffset));

    public static readonly StyledProperty<Dimension?> VerticalOffsetProperty =
        AvaloniaProperty.Register<MessageBox, Dimension?>(nameof(VerticalOffset));

    public static readonly StyledProperty<Control?> PlacementTargetProperty =
        AvaloniaProperty.Register<MessageBox, Control?>(nameof(PlacementTarget));

    public static readonly StyledProperty<DialogHostType> HostTypeProperty =
        AvaloniaProperty.Register<MessageBox, DialogHostType>(nameof(HostType), DialogHostType.Overlay);

    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public MessageBoxStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }

    public MessageBoxOkButtonStyle OkButtonStyle
    {
        get => GetValue(OkButtonStyleProperty);
        set => SetValue(OkButtonStyleProperty, value);
    }

    public string? OkButtonText
    {
        get => GetValue(OkButtonTextProperty);
        set => SetValue(OkButtonTextProperty, value);
    }

    public string? CancelButtonText
    {
        get => GetValue(CancelButtonTextProperty);
        set => SetValue(CancelButtonTextProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool IsConfirmLoading
    {
        get => GetValue(IsConfirmLoadingProperty);
        set => SetValue(IsConfirmLoadingProperty, value);
    }

    public bool IsLightDismissEnabled
    {
        get => GetValue(IsLightDismissEnabledProperty);
        set => SetValue(IsLightDismissEnabledProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public bool IsDragMovable
    {
        get => GetValue(IsDragMovableProperty);
        set => SetValue(IsDragMovableProperty, value);
    }

    public object? Result
    {
        get => GetValue(ResultProperty);
        set => SetValue(ResultProperty, value);
    }

    public bool IsCenterOnStartup
    {
        get => GetValue(IsCenterOnStartupProperty);
        set => SetValue(IsCenterOnStartupProperty, value);
    }

    public Dimension? HorizontalOffset
    {
        get => GetValue(HorizontalOffsetProperty);
        set => SetValue(HorizontalOffsetProperty, value);
    }

    public Dimension? VerticalOffset
    {
        get => GetValue(VerticalOffsetProperty);
        set => SetValue(VerticalOffsetProperty, value);
    }

    public Control? PlacementTarget
    {
        get => GetValue(PlacementTargetProperty);
        set => SetValue(PlacementTargetProperty, value);
    }

    public DialogHostType HostType
    {
        get => GetValue(HostTypeProperty);
        set => SetValue(HostTypeProperty, value);
    }

    public event EventHandler? OpenedSimple;
    public event EventHandler? ClosedSimple;
    public event EventHandler? Cancelled;
    public event EventHandler? Confirmed;

    private readonly Button _confirmButton;
    private readonly Button _cancelButton;
    private readonly ContentPresenter _contentPresenter;
    private readonly PathIcon _styleIcon;
    private readonly Border _dialogRoot;

    public MessageBox()
    {
        Background = Brushes.Transparent;
        SystemDecorations = SystemDecorations.BorderOnly;
        CanResize = false;
        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        _styleIcon = new PathIcon
        {
            Width = 20,
            Height = 20,
            Stretch = Stretch.Uniform,
            Foreground = Brushes.SteelBlue
        };

        var titleText = new TextBlock
        {
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center
        };
        this.GetObservable(TitleProperty).Subscribe(title => titleText.Text = title);

        var header = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                _styleIcon,
                titleText
            }
        };

        _contentPresenter = new ContentPresenter
        {
            Margin = new Thickness(0, 8, 0, 8)
        };

        _confirmButton = new Button();
        _confirmButton.Click += (_, _) => Confirm();

        _cancelButton = new Button();
        _cancelButton.Click += (_, _) => Cancel();

        var buttonsPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Right,
            Children =
            {
                _cancelButton,
                _confirmButton
            }
        };

        var dialog = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto"),
            Children =
            {
                header,
                _contentPresenter,
                buttonsPanel
            }
        };

        Grid.SetRow(header, 0);
        Grid.SetRow(_contentPresenter, 1);
        Grid.SetRow(buttonsPanel, 2);

        _dialogRoot = new Border
        {
            Padding = new Thickness(16),
            Background = Brushes.White,
            CornerRadius = new CornerRadius(6),
            Child = dialog
        };

        Content = _dialogRoot;

        Opened += (_, _) => OpenedSimple?.Invoke(this, EventArgs.Empty);
        Closed += (_, _) => ClosedSimple?.Invoke(this, EventArgs.Empty);
        Deactivated += (_, _) => HandleLightDismiss();
        PointerPressed += HandlePointerPressed;

        UpdateStyleVisuals();
        UpdateButtons();
    }

    public static object? ShowMessageBox<TView, TViewModel>(TViewModel? dataContext, MessageBoxOptions? options = null,
                                                            Window? owner = null)
        where TView : Control, new()
    {
        var messageBox = CreateMessageBox(new TView(), dataContext, options);
        return messageBox.Open(owner);
    }

    public static object? ShowMessageBox(Control content, object? dataContext = null, MessageBoxOptions? options = null, Window? owner = null)
    {
        var messageBox = CreateMessageBox(content, dataContext, options);
        return messageBox.Open(owner);
    }

    public static async Task ShowMessageBoxAsync<TView, TViewModel>(TViewModel? dataContext,
                                                                    MessageBoxOptions? options = null,
                                                                    Action<object?>? closed = null,
                                                                    Window? owner = null)
        where TView : Control, new()
    {
        var messageBox = CreateMessageBox(new TView(), dataContext, options);
        await messageBox.OpenAsync(owner);
        closed?.Invoke(messageBox.Result);
    }

    public static async Task ShowMessageBoxAsync(Control content, object? dataContext = null, MessageBoxOptions? options = null,
                                                 Action<object?>? closed = null, Window? owner = null)
    {
        var messageBox = CreateMessageBox(content, dataContext, options);
        await messageBox.OpenAsync(owner);
        closed?.Invoke(messageBox.Result);
    }

    public static async Task<object?> ShowMessageBoxModalAsync<TView, TViewModel>(TViewModel? dataContext, MessageBoxOptions? options = null, Window? owner = null)
        where TView : Control, new()
    {
        var messageBox = CreateMessageBox(new TView(), dataContext, options);
        return await messageBox.OpenModalAsync(owner);
    }

    public static async Task<object?> ShowMessageModalAsync(Control content, object? dataContext = null, MessageBoxOptions? options = null, Window? owner = null)
    {
        var messageBox = CreateMessageBox(content, dataContext, options);
        return await messageBox.OpenModalAsync(owner);
    }

    public static MessageBox CreateMessageBox(Control content, object? dataContext, MessageBoxOptions? options)
    {
        var messageBox = new MessageBox
        {
            Title = options?.Title,
            IsLightDismissEnabled = options?.IsLightDismissEnabled ?? false,
            IsDragMovable = options?.IsDragMovable ?? false,
            Style = options?.Style ?? MessageBoxStyle.Information,
            PlacementTarget = options?.PlacementTarget,
            HorizontalOffset = options?.HorizontalOffset,
            VerticalOffset = options?.VerticalOffset,
            HostType = options?.HostType ?? DialogHostType.Overlay,
            IsCenterOnStartup = options?.IsCenterOnStartup ?? true,
            DataContext = dataContext,
            Width = options?.Width ?? double.NaN,
            Height = options?.Height ?? double.NaN,
            MinWidth = options?.MinWidth ?? 0d,
            MinHeight = options?.MinHeight ?? 0d,
            MaxWidth = options?.MaxWidth ?? double.PositiveInfinity,
            MaxHeight = options?.MaxHeight ?? double.PositiveInfinity,
            IsConfirmLoading = options?.IsConfirmLoading ?? false,
            IsLoading = options?.IsLoading ?? false,
        };

        messageBox._contentPresenter.Content = content;
        if (options?.Icon != null)
        {
            messageBox.Icon = options.Icon;
        }

        return messageBox;
    }

    public object? Open(Window? owner = null)
    {
        return OpenModalAsync(owner).GetAwaiter().GetResult();
    }

    public async Task OpenAsync(Window? owner = null)
    {
        await OpenModalAsync(owner);
    }

    public async Task<object?> OpenModalAsync(Window? owner = null)
    {
        Owner = owner ?? GetMainWindow();
        IsModal = true;
        IsOpen = true;
        ApplyStartupPosition();
        var result = await ShowDialog<object?>(Owner);
        IsOpen = false;
        return result;
    }

    public void Cancel()
    {
        SetCurrentValue(ResultProperty, null);
        Cancelled?.Invoke(this, EventArgs.Empty);
        CloseWithResult();
    }

    public void Confirm()
    {
        SetCurrentValue(ResultProperty, true);
        Confirmed?.Invoke(this, EventArgs.Empty);
        CloseWithResult();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StyleProperty)
        {
            UpdateStyleVisuals();
        }
        else if (change.Property == OkButtonStyleProperty)
        {
            UpdateButtons();
        }
        else if (change.Property == IsLoadingProperty || change.Property == IsConfirmLoadingProperty)
        {
            UpdateLoadingState();
        }
        else if (change.Property == IconProperty)
        {
            _styleIcon.Data = Icon?.Data;
            _styleIcon.Foreground = Icon?.Foreground ?? _styleIcon.Foreground;
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        ApplyStartupPosition();
    }

    private void HandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (IsDragMovable)
        {
            BeginMoveDrag(e);
        }
    }

    private void HandleLightDismiss()
    {
        if (IsLightDismissEnabled && !IsModal)
        {
            Cancel();
        }
    }

    private void UpdateButtons()
    {
        _confirmButton.Content = OkButtonText ?? "OK";
        _cancelButton.Content = CancelButtonText ?? "Cancel";
        _confirmButton.Classes.Set("Primary", OkButtonStyle == MessageBoxOkButtonStyle.Primary);
        _cancelButton.IsVisible = Style == MessageBoxStyle.Confirm;
        UpdateLoadingState();
    }

    private void UpdateLoadingState()
    {
        _confirmButton.IsEnabled = !IsConfirmLoading && !IsLoading;
        _cancelButton.IsEnabled = !IsLoading;
        _confirmButton.Content = IsConfirmLoading ? BuildLoadingContent(OkButtonText ?? "OK") : _confirmButton.Content is StackPanel ? OkButtonText ?? "OK" : _confirmButton.Content;
        _cancelButton.Content = CancelButtonText ?? "Cancel";
        _dialogRoot.IsEnabled = !IsLoading;
    }

    private static Control BuildLoadingContent(string text)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 4,
            Children =
            {
                new TextBlock { Text = text },
                new PathIcon
                {
                    Data = Geometry.Parse(LoadingGeometry),
                    Width = 12,
                    Height = 12,
                    Foreground = Brushes.Gray
                }
            }
        };
    }

    private void UpdateStyleVisuals()
    {
        var (geometry, brush) = GetStyleVisual(Style);
        if (Icon == null && geometry != null)
        {
            Icon = new PathIcon
            {
                Data = geometry,
                Foreground = brush,
                Width = _styleIcon.Width,
                Height = _styleIcon.Height
            };
        }

        _styleIcon.Data = Icon?.Data ?? geometry;
        _styleIcon.Foreground = Icon?.Foreground ?? brush;
        _cancelButton.IsVisible = Style == MessageBoxStyle.Confirm;
    }

    private void ApplyStartupPosition()
    {
        if (IsCenterOnStartup)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return;
        }

        WindowStartupLocation = WindowStartupLocation.Manual;
        if (PlacementTarget != null)
        {
            var screenPoint = PlacementTarget.PointToScreen(new Point(0, 0));
            var xOffset = HorizontalOffset?.Resolve(PlacementTarget.Bounds.Width) ?? 0;
            var yOffset = VerticalOffset?.Resolve(PlacementTarget.Bounds.Height) ?? 0;
            Position = new PixelPoint((int)(screenPoint.X + xOffset), (int)(screenPoint.Y + yOffset));
        }
        else
        {
            var xOffset = HorizontalOffset?.Resolve(0) ?? 0;
            var yOffset = VerticalOffset?.Resolve(0) ?? 0;
            Position = new PixelPoint((int)xOffset, (int)yOffset);
        }
    }

    private void CloseWithResult()
    {
        Close(Result);
        IsOpen = false;
    }

    private static (Geometry? geometry, IBrush brush) GetStyleVisual(MessageBoxStyle style)
    {
        return style switch
        {
            MessageBoxStyle.Success => (Geometry.Parse(SuccessGeometry), Brushes.SeaGreen),
            MessageBoxStyle.Error => (Geometry.Parse(ErrorGeometry), Brushes.Firebrick),
            MessageBoxStyle.Warning => (Geometry.Parse(WarningGeometry), Brushes.DarkOrange),
            MessageBoxStyle.Confirm => (Geometry.Parse(WarningGeometry), Brushes.DarkOrange),
            MessageBoxStyle.Normal => (null, Brushes.SteelBlue),
            _ => (Geometry.Parse(InfoGeometry), Brushes.SteelBlue)
        };
    }

    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        return null;
    }

    private const string InfoGeometry = "M12 0C5.371 0 0 5.371 0 12s5.371 12 12 12 12-5.371 12-12S18.629 0 12 0zm1 18h-2V10h2v8zm0-10h-2V6h2v2z";
    private const string SuccessGeometry = "M12 0C5.371 0 0 5.371 0 12s5.371 12 12 12 12-5.371 12-12S18.629 0 12 0zm-1.2 17.4l-4.2-4.2 1.4-1.4 2.8 2.8 5.8-5.8 1.4 1.4-7.2 7.2z";
    private const string ErrorGeometry = "M12 0C5.372 0 0 5.372 0 12s5.372 12 12 12 12-5.372 12-12S18.628 0 12 0zm5 15.59L15.59 17 12 13.41 8.41 17 7 15.59 10.59 12 7 8.41 8.41 7 12 10.59 15.59 7 17 8.41 13.41 12 17 15.59z";
    private const string WarningGeometry = "M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z";
    private const string LoadingGeometry = "M12 0a12 12 0 100 24 12 12 0 000-24zm0 4a8 8 0 018 8h-2a6 6 0 10-6 6v2a8 8 0 010-16z";
}
