using System.Windows;
using System.Windows.Controls;

namespace CognosantGUI.Utilities
{
    class WebBrowserExtension
    {
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.RegisterAttached("Document", typeof(string), typeof(WebBrowserExtension), new UIPropertyMetadata(null, DocumentPropertyChanged));

        public static string GetDocument(DependencyObject element)
        {
            return (string)element.GetValue(DocumentProperty);
        }

        public static void SetDocument(DependencyObject element, string value)
        {
            element.SetValue(DocumentProperty, value);
        }

        public static void DocumentPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = target as WebBrowser;

            if (browser != null)
            {
                string document = e.NewValue as string;
                browser.NavigateToString(document);
            }
        }
    }
}
