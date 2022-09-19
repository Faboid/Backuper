using System.Windows;
using System.Windows.Input;

namespace Backuper.UI.WPF.Components;

public class TextBoxWithButton : TextBoxWithPreview {

    public ICommand ButtonCommand {
        get { return (ICommand)GetValue(ButtonCommandProperty); }
        set { SetValue(ButtonCommandProperty, value); }
    }

    public static readonly DependencyProperty ButtonCommandProperty =
        DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(TextBoxWithButton), new PropertyMetadata(null));

    public string ButtonText {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(TextBoxWithButton), new PropertyMetadata(string.Empty));

    static TextBoxWithButton() {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxWithButton), new FrameworkPropertyMetadata(typeof(TextBoxWithButton)));
    }

}
