using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AmSpaceTools.Infrastructure.Extensions
{
    /// <summary>
    /// Source property of WebBrowser control is not DependencyProperty (not bindable to use in MVVM). This is workaround to make additional bindable property "BindableSource"
    /// Source: https://stackoverflow.com/questions/263551/databind-the-source-property-of-the-webbrowser-in-wpf
    /// </summary>
    public static class WebBrowserExtensions
    {
        public static readonly DependencyProperty BindableSourceProperty =
        DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebBrowserExtensions), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = o as WebBrowser;
            if (browser != null)
            {
                string uri = e.NewValue as string;
                browser.Source = !String.IsNullOrEmpty(uri) ? new Uri(uri) : null;
            }
        }
    }
}
