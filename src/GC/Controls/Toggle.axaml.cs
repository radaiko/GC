using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GC.Controls;

public partial class Toggle : UserControl {
  public static readonly StyledProperty<bool> StateProperty =
    AvaloniaProperty.Register<Toggle, bool>(nameof(State));

  public bool State {
    get => GetValue(StateProperty);
    set => SetValue(StateProperty, value);
  }

  public static readonly StyledProperty<string?> TextProperty =
    AvaloniaProperty.Register<Toggle, string?>(nameof(Text));

  public string? Text {
    get => GetValue(TextProperty);
    set => SetValue(TextProperty, value);
  }

  public Toggle() {
    InitializeComponent();
  }
}