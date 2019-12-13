﻿using Reloaded.Mod.Launcher.Commands.Dialog;
using Reloaded.Mod.Launcher.Models.ViewModel;
using Reloaded.Mod.Loader.IO;

namespace Reloaded.Mod.Launcher.Pages.BaseSubpages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : ReloadedIIPage
    {
        public SettingsPageViewModel ViewModel { get; set; }

        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = IoC.Get<SettingsPageViewModel>();
            this.AnimateOutStarted += OnAnimateOutStarted;
        }

        private void OnAnimateOutStarted()
        {
            LoaderConfigReader.WriteConfiguration(ViewModel.LoaderConfig);
        }

        private void Documents_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new OpenDocumentationCommand().Execute(null);
        }
    }
}
