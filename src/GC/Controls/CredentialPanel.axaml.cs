using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GC.Controls
{
    public partial class CredentialPanel : UserControl
    {
        public static readonly StyledProperty<string> UsernameProperty =
            AvaloniaProperty.Register<CredentialPanel, string>(nameof(Username));

        public static readonly StyledProperty<string> PasswordProperty =
            AvaloniaProperty.Register<CredentialPanel, string>(nameof(Password));

        public static readonly StyledProperty<string> UsernameWatermarkProperty =
            AvaloniaProperty.Register<CredentialPanel, string>(nameof(UsernameWatermark));

        public static readonly StyledProperty<string> PasswordWatermarkProperty =
            AvaloniaProperty.Register<CredentialPanel, string>(nameof(PasswordWatermark));

        // Header ------------------------------------------------------------------
        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<CredentialPanel, string>(nameof(Header), string.Empty);

        public string Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public string Username
        {
            get => GetValue(UsernameProperty);
            set => SetValue(UsernameProperty, value);
        }

        public string Password
        {
            get => GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public string UsernameWatermark
        {
            get => GetValue(UsernameWatermarkProperty);
            set => SetValue(UsernameWatermarkProperty, value);
        }

        public string PasswordWatermark
        {
            get => GetValue(PasswordWatermarkProperty);
            set => SetValue(PasswordWatermarkProperty, value);
        }

        public CredentialPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
