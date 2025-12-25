using AtomUI.Controls;
using Shouldly;
using Avalonia.Controls;
using Xunit;

namespace AtomUI.Base.Tests;

public class MessageBoxTests
{
    [Fact]
    public void Should_apply_options_when_creating_message_box()
    {
        var placementTarget = new Button();
        var options = new MessageBoxOptions
        {
            Title = "Test",
            Style = MessageBoxStyle.Warning,
            IsDragMovable = true,
            PlacementTarget = placementTarget,
            HorizontalOffset = new Dimension(10),
            VerticalOffset = new Dimension(5),
            HostType = DialogHostType.Window,
            IsCenterOnStartup = false,
            IsLightDismissEnabled = true,
            IsLoading = true,
            IsConfirmLoading = true,
            Width = 320,
            Height = 180,
            MinWidth = 120,
            MinHeight = 80,
            MaxWidth = 640,
            MaxHeight = 480
        };

        var content = new TextBlock { Text = "Content" };
        var messageBox = MessageBox.CreateMessageBox(content, null, options);

        messageBox.Title.ShouldBe("Test");
        messageBox.Style.ShouldBe(MessageBoxStyle.Warning);
        messageBox.IsDragMovable.ShouldBeTrue();
        messageBox.PlacementTarget.ShouldBe(placementTarget);
        messageBox.HorizontalOffset.ShouldBe(options.HorizontalOffset);
        messageBox.VerticalOffset.ShouldBe(options.VerticalOffset);
        messageBox.HostType.ShouldBe(DialogHostType.Window);
        messageBox.IsCenterOnStartup.ShouldBeFalse();
        messageBox.IsLightDismissEnabled.ShouldBeTrue();
        messageBox.IsLoading.ShouldBeTrue();
        messageBox.IsConfirmLoading.ShouldBeTrue();
        messageBox.Width.ShouldBe(320);
        messageBox.Height.ShouldBe(180);
        messageBox.MinWidth.ShouldBe(120);
        messageBox.MinHeight.ShouldBe(80);
        messageBox.MaxWidth.ShouldBe(640);
        messageBox.MaxHeight.ShouldBe(480);
    }

    [Theory]
    [InlineData(MessageBoxStyle.Information)]
    [InlineData(MessageBoxStyle.Success)]
    [InlineData(MessageBoxStyle.Error)]
    [InlineData(MessageBoxStyle.Warning)]
    [InlineData(MessageBoxStyle.Confirm)]
    [InlineData(MessageBoxStyle.Normal)]
    public void Should_set_default_icon_based_on_style(MessageBoxStyle style)
    {
        var messageBox = MessageBox.CreateMessageBox(new TextBlock(), null, new MessageBoxOptions { Style = style });

        if (style == MessageBoxStyle.Normal)
        {
            messageBox.Icon.ShouldBeNull();
        }
        else
        {
            messageBox.Icon.ShouldNotBeNull();
        }
    }
}
