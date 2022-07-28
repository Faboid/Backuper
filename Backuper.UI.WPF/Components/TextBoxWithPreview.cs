using System.Windows;
using System.Windows.Controls;

namespace Backuper.UI.WPF.Components; 

public class TextBoxWithPreview : TextBox {

    public string PreviewText {
        get { return (string)GetValue(PreviewTextProperty); }
        set { SetValue(PreviewTextProperty, value); }
    }

    public static readonly DependencyProperty PreviewTextProperty =
        DependencyProperty.Register("PreviewText", typeof(string), typeof(TextBoxWithPreview), new PropertyMetadata(""));

    static TextBoxWithPreview() {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxWithPreview), new FrameworkPropertyMetadata(typeof(TextBoxWithPreview)));
    }
}
