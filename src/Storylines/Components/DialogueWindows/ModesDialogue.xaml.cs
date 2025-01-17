﻿using Storylines.Scripts.Functions;
using Storylines.Scripts.Modes;
using Storylines.Scripts.Services;
using Storylines.Scripts.Variables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Storylines.Components.DialogueWindows
{
    public sealed partial class ModesDialogue : ContentDialog
    {
        public static ModesDialogue modesDialogue;

        public enum ModeType { None, Read, Focus };
        private static ModeType modeType;

        public ModesDialogue()
        {
            InitializeComponent();
            modesDialogue = this;

            InitializeClickOutToClose();

            modesDialogue.RequestedTheme = AppView.current.RequestedTheme;

            AppView.currentlyOpenedDialogue = modesDialogue;

            if (modeType != default)
            {
                chooseWhatToExportAnimation.FromVerticalOffset = 0;
                switch (modeType)
                {
                    //case ModeType.Read:
                    //    readModeButton.IsChecked = true;
                    //    modesDialogue.OnReadButton_Click(new object(), new RoutedEventArgs());
                    //    break;
                    case ModeType.Focus:
                        focusModeButton.IsChecked = true;
                        modesDialogue.OnFocusButton_Click(new object(), new RoutedEventArgs());
                        break;
                }
            }
            timePicker.Time = new System.TimeSpan(0, 15, 0);

            if (Character.characters.Count < 1)
                noCharacters.IsOpen = true;
        }

        public static void Open(ModeType mode)
        {
            modeType = mode;
            _ = new ModesDialogue().ShowAsync();
        }

        private void OnSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if((bool)autosaveCheckBox.IsChecked)
                Autosave.Enable();
            else
                Autosave.Disable();

            switch (modeType)
            {
                case ModeType.None:
                    break;
                //case ModeType.Read:
                //    ReadMode.Switch();
                //    break;
                case ModeType.Focus:
                    FocusMode.Switch((bool)fullScreenCheckBox.IsChecked, timePicker.Time, short.Parse(measureValueNumBox.Value.ToString()), (FocusMode.ToMeasure)toMeasureComboBox.SelectedIndex);
                    MicrosoftStoreAndAppCenterFunctions.SendAnalyticData_FocusMode_Start((bool)fullScreenCheckBox.IsChecked, (bool)autosaveCheckBox.IsChecked, $"{measureValueNumBox.Value} {measureValueNumBox.Value}", timePicker.Time.ToString());
                    break;
            }

            Hide();
        }

        private void OnMinimumCheckBox_Click(object sender, RoutedEventArgs e)
        {
            measureValueNumBox.Value = 0;
            measureStack.Visibility = (bool)measureCheckBox.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnReadButton_Click(object sender, RoutedEventArgs e)
        {
            chooseExportChaptersPanel.Visibility = (bool)readModeButton.IsChecked ? Visibility.Visible : Visibility.Collapsed;

            timeCheckBox.Visibility = Visibility.Collapsed;
            timePicker.Visibility = Visibility.Collapsed;
            measureCheckBox.Visibility = Visibility.Collapsed;
            measureStack.Visibility = Visibility.Collapsed;

            modeType = ModeType.Read;

            focusModeButton.IsChecked = false;
        }

        private void OnFocusButton_Click(object sender, RoutedEventArgs e)
        {
            chooseExportChaptersPanel.Visibility = (bool)focusModeButton.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            timeCheckBox.Visibility = Visibility.Visible;
            timePicker.Visibility = Visibility.Visible;
            measureCheckBox.Visibility = Visibility.Visible;
            measureStack.Visibility = Visibility.Visible;

            modeType = ModeType.Focus;

            readModeButton.IsChecked = false;
        }

        private void OnCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void ContentDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            AppView.currentlyOpenedDialogue = null;
        }

        private void OnTimeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            timePicker.Time = (bool)timeCheckBox.IsChecked ? timePicker.Time : new System.TimeSpan(0,0,0);
            timePicker.Visibility = (bool)timeCheckBox.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        bool isHide = true;
        private void InitializeClickOutToClose()
        {
            Window.Current.CoreWindow.PointerPressed += (s, e) =>
            {
                if (isHide && !isFlyoutOpen)
                    Hide();
            };

            PointerExited += (s, e) => isHide = true;
            PointerEntered += (s, e) => isHide = false;
        }

        bool isFlyoutOpen = false;
        private void OnToMeasureComboBox_DropDownOpened(object sender, object e)
        {
            isFlyoutOpen = true;
        }

        private void OnToMeasureComboBox_DropDownClosed(object sender, object e)
        {
            isFlyoutOpen = false;
        }
    }
}
