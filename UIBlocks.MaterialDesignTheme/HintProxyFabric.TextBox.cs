﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UIBlocks.MaterialDesignTheme
{
    public static partial class HintProxyFabric
    {
        private sealed class TextBoxHintProxy : IHintProxy
        {
            private readonly TextBox _textBox;

            public object Content => _textBox.Text;
            public bool IsLoaded => _textBox.IsLoaded;
            public bool IsVisible => _textBox.IsVisible;

            public event EventHandler ContentChanged;
            public event EventHandler IsVisibleChanged;
            public event EventHandler Loaded;

            public TextBoxHintProxy(TextBox textBox)
            {
                if (textBox == null) throw new ArgumentNullException(nameof(textBox));

                _textBox = textBox;
                _textBox.TextChanged += TextBoxTextChanged;
                _textBox.Loaded += TextBoxLoaded;
                _textBox.IsVisibleChanged += TextBoxIsVisibleChanged;
            }

            private void TextBoxIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                IsVisibleChanged?.Invoke(sender, EventArgs.Empty);
            }

            private void TextBoxLoaded(object sender, RoutedEventArgs e)
            {
                Loaded?.Invoke(sender, EventArgs.Empty);
            }

            private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
            {
                ContentChanged?.Invoke(sender, EventArgs.Empty);
            }

            public void Dispose()
            {
                _textBox.TextChanged -= TextBoxTextChanged;
                _textBox.Loaded -= TextBoxLoaded;
                _textBox.IsVisibleChanged -= TextBoxIsVisibleChanged;
            }
        }
    }
}
