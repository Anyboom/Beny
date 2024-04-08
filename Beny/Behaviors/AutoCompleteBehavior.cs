using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Beny.Behaviors
{
    public static class AutoCompleteBehavior
    {
        private static TextChangedEventHandler _onTextChanged = new TextChangedEventHandler(OnTextChanged);
        private static KeyEventHandler _onKeyDown = new KeyEventHandler(OnPreviewKeyDown);

        public static readonly DependencyProperty AutoCompleteItemsSource =
            DependencyProperty.RegisterAttached
            (
                "AutoCompleteItemsSource",
                typeof(IEnumerable<String>),
                typeof(AutoCompleteBehavior),
                new UIPropertyMetadata(null, OnAutoCompleteItemsSource)
            );

        public static readonly DependencyProperty AutoCompleteStringComparison =
            DependencyProperty.RegisterAttached
            (
                "AutoCompleteStringComparison",
                typeof(StringComparison),
                typeof(AutoCompleteBehavior),
                new UIPropertyMetadata(StringComparison.Ordinal)
            );

		public static readonly DependencyProperty AutoCompleteIndicator =
            DependencyProperty.RegisterAttached
            (
                "AutoCompleteIndicator",
                typeof(String),
                typeof(AutoCompleteBehavior),
                new UIPropertyMetadata(String.Empty)
            );

        public static IEnumerable<String> GetAutoCompleteItemsSource(DependencyObject obj)
        {
            object objRtn = obj.GetValue(AutoCompleteItemsSource);
            if (objRtn is IEnumerable<String>)
                return objRtn as IEnumerable<String>;

            return null;
        }

        public static void SetAutoCompleteItemsSource(DependencyObject obj, IEnumerable<String> value)
        {
            obj.SetValue(AutoCompleteItemsSource, value);
        }

        private static void OnAutoCompleteItemsSource(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (sender == null)
                return;

            tb.TextChanged -= _onTextChanged;
            tb.PreviewKeyDown -= _onKeyDown;
            if (e.NewValue != null)
            {
                tb.TextChanged += _onTextChanged;
                tb.PreviewKeyDown += _onKeyDown;
            }
        }
        public static StringComparison GetAutoCompleteStringComparison(DependencyObject obj)
        {
            return (StringComparison)obj.GetValue(AutoCompleteStringComparison);
        }

        public static void SetAutoCompleteStringComparison(DependencyObject obj, StringComparison value)
        {
            obj.SetValue(AutoCompleteStringComparison, value);
        }

        public static String GetAutoCompleteIndicator(DependencyObject obj)
        {
            return (String)obj.GetValue(AutoCompleteIndicator);
        }

        public static void SetAutoCompleteIndicator(DependencyObject obj, String value)
        {
            obj.SetValue(AutoCompleteIndicator, value);
        }

        static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            TextBox tb = e.OriginalSource as TextBox;
            if (tb == null)
                return;

            if (tb.SelectionLength > 0 && (tb.SelectionStart + tb.SelectionLength == tb.Text.Length))
            {
                tb.SelectionStart = tb.CaretIndex = tb.Text.Length;
                tb.SelectionLength = 0;
            }
        }

        static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if
            (
                (from change in e.Changes where change.RemovedLength > 0 select change).Any() &&
                (from change in e.Changes where change.AddedLength > 0 select change).Any() == false
            )
                return;

            TextBox tb = e.OriginalSource as TextBox;
            if (sender == null)
                return;

            IEnumerable<String> values = GetAutoCompleteItemsSource(tb);
            if (values == null)
                return;

            if (String.IsNullOrEmpty(tb.Text))
                return;

            string indicator = GetAutoCompleteIndicator(tb);
            int startIndex = 0;
            string matchingString = tb.Text;
            if (!String.IsNullOrEmpty(indicator))
            {
                startIndex = tb.Text.LastIndexOf(indicator);
                if (startIndex == -1)
                    return;

                startIndex += indicator.Length;
                matchingString = tb.Text.Substring(startIndex, (tb.Text.Length - startIndex));
            }

            if (String.IsNullOrEmpty(matchingString))
                return;

            Int32 textLength = matchingString.Length;

            StringComparison comparer = GetAutoCompleteStringComparison(tb);

            String match =
            (
                from
                    value
                in
                (
                    from subvalue
                    in values
                    where subvalue != null && subvalue.Length >= textLength
                    select subvalue
                )
                where value.Substring(0, textLength).Equals(matchingString, comparer)
                select value.Substring(textLength, value.Length - textLength)/*Only select the last part of the suggestion*/
            ).FirstOrDefault();

            if (String.IsNullOrEmpty(match))
                return;

            int matchStart = (startIndex + matchingString.Length);
            tb.TextChanged -= _onTextChanged;
            tb.Text += match;
            tb.CaretIndex = matchStart;
            tb.SelectionStart = matchStart;
            tb.SelectionLength = (tb.Text.Length - startIndex);
            tb.TextChanged += _onTextChanged;
        }
    }
}
