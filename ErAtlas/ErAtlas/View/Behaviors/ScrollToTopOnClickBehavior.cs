using System;
using Microsoft.Maui.Controls;

namespace ErAtlas.View.Behaviors
{
    public class ScrollToTopOnClickBehavior : Behavior<Button>
    {
        public static readonly BindableProperty TargetScrollNameProperty =
            BindableProperty.Create(nameof(TargetScrollName), typeof(string), typeof(ScrollToTopOnClickBehavior), string.Empty);

        public string TargetScrollName
        {
            get => (string)GetValue(TargetScrollNameProperty);
            set => SetValue(TargetScrollNameProperty, value);
        }

        private Button? _associatedButton;

        protected override void OnAttachedTo(Button bindable)
        {
            base.OnAttachedTo(bindable);
            _associatedButton = bindable;
            bindable.Clicked += OnButtonClicked;
        }

        protected override void OnDetachingFrom(Button bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Clicked -= OnButtonClicked;
            _associatedButton = null;
        }

        private async void OnButtonClicked(object? sender, EventArgs e)
        {
            try
            {
                if (_associatedButton is null)
                    return;

                // remonter jusqu'à la page pour utiliser FindByName
                Element? elm = _associatedButton;
                while (elm != null && elm is not Page)
                {
                    elm = elm.Parent as Element;
                }

                if (elm is Page page)
                {
                    var scroll = page.FindByName<ScrollView>(TargetScrollName);
                    if (scroll != null)
                    {
                        await scroll.ScrollToAsync(0, 0, true);
                    }
                }
            }
            catch
            {
                // ignore
            }
        }
    }
}

