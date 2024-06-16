using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Beny.Helpers.Behaviors
{
    public static class AutoCompleteBehavior
    {
        private static TextChangedEventHandler _onTextChanged = new TextChangedEventHandler(OnTextChanged);
        private static KeyEventHandler _onKeyDown = new KeyEventHandler(OnPreviewKeyDown);

        private static bool _isFocused;

        public static readonly DependencyProperty AutoCompleteItemsSource =
            DependencyProperty.RegisterAttached
            (
                "AutoCompleteItemsSource",
                typeof(IEnumerable<string>),
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
                typeof(string),
                typeof(AutoCompleteBehavior),
                new UIPropertyMetadata(string.Empty)
            );

        public static IEnumerable<string> GetAutoCompleteItemsSource(DependencyObject obj)
        {
            object objectReturn = obj.GetValue(AutoCompleteItemsSource);

            if (objectReturn is IEnumerable<string>)
            {
                return objectReturn as IEnumerable<string>;
            }

            return null;
        }

        public static void SetAutoCompleteItemsSource(DependencyObject obj, IEnumerable<string> value)
        {
            obj.SetValue(AutoCompleteItemsSource, value);
        }


        public static void AddFocusAction(object sender, RoutedEventArgs e)
        {
            _isFocused = true;
        }

        public static void RemoveFocusAction(object sender, RoutedEventArgs e)
        {
            _isFocused = false;
        }

        private static void OnAutoCompleteItemsSource(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox mainTextBox = sender as TextBox;

            if (sender == null)
            {
                return;
            }

            mainTextBox.TextChanged -= _onTextChanged;
            mainTextBox.PreviewKeyDown -= _onKeyDown;

            mainTextBox.LostFocus -= RemoveFocusAction;
            mainTextBox.GotFocus -= AddFocusAction;

            if (e.NewValue != null)
            {
                mainTextBox.TextChanged += _onTextChanged;
                mainTextBox.PreviewKeyDown += _onKeyDown;

                mainTextBox.LostFocus += RemoveFocusAction;
                mainTextBox.GotFocus += AddFocusAction;
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

        public static string GetAutoCompleteIndicator(DependencyObject obj)
        {
            return (string)obj.GetValue(AutoCompleteIndicator);
        }

        public static void SetAutoCompleteIndicator(DependencyObject obj, string value)
        {
            obj.SetValue(AutoCompleteIndicator, value);
        }

        static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            TextBox mainTextBox = e.OriginalSource as TextBox;

            if (mainTextBox == null)
            {
                return;
            }

            if (mainTextBox.SelectionLength > 0 && mainTextBox.SelectionStart + mainTextBox.SelectionLength == mainTextBox.Text.Length)
            {
                mainTextBox.SelectionStart = mainTextBox.CaretIndex = mainTextBox.Text.Length;
                mainTextBox.SelectionLength = 0;
            }
        }

        static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if ((from change in e.Changes where change.RemovedLength > 0 select change).Any() && (from change in e.Changes where change.AddedLength > 0 select change).Any() == false)
                return;

            TextBox mainTextBox = e.OriginalSource as TextBox;

            if (sender == null)
            {
                return;
            }

            if (_isFocused == false)
            {
                return;
            }

            IEnumerable<string> values = GetAutoCompleteItemsSource(mainTextBox);

            if (values == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(mainTextBox.Text))
            {
                return;
            }

            string indicator = GetAutoCompleteIndicator(mainTextBox);
            int startIndex = 0;
            string matchingString = mainTextBox.Text;

            if (string.IsNullOrEmpty(indicator) == false)
            {
                startIndex = mainTextBox.Text.LastIndexOf(indicator);

                if (startIndex == -1)
                {
                    return;
                }

                startIndex += indicator.Length;
                matchingString = mainTextBox.Text.Substring(startIndex, mainTextBox.Text.Length - startIndex);
            }

            if (string.IsNullOrEmpty(matchingString))
            {
                return;
            }

            int textLength = matchingString.Length;

            StringComparison comparer = GetAutoCompleteStringComparison(mainTextBox);

            string match = (from value in from subvalue in values where subvalue != null && subvalue.Length >= textLength select subvalue where value.Substring(0, textLength).Equals(matchingString, comparer) select value.Substring(textLength, value.Length - textLength)).FirstOrDefault();

            if (string.IsNullOrEmpty(match))
            {
                return;
            }

            int matchStart = startIndex + matchingString.Length;

            mainTextBox.TextChanged -= _onTextChanged;
            mainTextBox.Text += match;
            mainTextBox.CaretIndex = matchStart;
            mainTextBox.SelectionStart = matchStart;
            mainTextBox.SelectionLength = mainTextBox.Text.Length - startIndex;
            mainTextBox.TextChanged += _onTextChanged;
        }
    }
}
