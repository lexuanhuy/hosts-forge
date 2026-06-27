using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HostForge.Models;
using HostForge.Services;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HostForge.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        private readonly StorageService _storageService;
        private readonly DnsService _dnsService;

        public ObservableCollection<HostsProfile> Profiles { get; set; }

        [ObservableProperty]
        private HostsProfile _selectedProfile;
        [ObservableProperty]
        private string _textBuffer;

        public ICommand ApplyCommand { get; }
        public ICommand AddProfileCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }

        public MainViewModel()
        {
            _storageService = new StorageService();
            _dnsService = new DnsService();

            Profiles = new ObservableCollection<HostsProfile>(_storageService.LoadProfiles());

            // Declare command connect with handle method
            ApplyCommand = new RelayCommand(ExecuteApply);
            AddProfileCommand = new RelayCommand(ExecuteAddProfile);
            if (Profiles.Count > 0) SelectedProfile = Profiles[0];

            MinimizeCommand = new RelayCommand<Window>(ExecuteMinimize);
            MaximizeCommand = new RelayCommand<Window>(ExecuteMaximize);
            CloseCommand = new RelayCommand<Window>(ExecuteClose);
        }

        partial void OnSelectedProfileChanged(HostsProfile value)
        {
            if (value != null) TextBuffer = value.Content;
        }

        private void ExecuteApply()
        {
            if (SelectedProfile == null) return;

            try
            {
                SelectedProfile.Content = TextBuffer;
                _storageService.WriteToSystemHosts(TextBuffer);
                _storageService.SaveProfiles(new System.Collections.Generic.List<HostsProfile>(Profiles));

                if(_dnsService.FlushCache())
                {
                    MessageBox.Show("Apply and Flush DNS success.", "Host Forge", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch(UnauthorizedAccessException)
            {
                MessageBox.Show("Please run application with Administrator permission", "Host Forge", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void ExecuteAddProfile()
        {
            string profileName = Interaction.InputBox("Enter new profile name:", "Host Forge", "New Profile");
            if (string.IsNullOrWhiteSpace(profileName)) return;
            string defaultContent = $"# Profile: {profileName}\n127.0.0.1 localhost";
            var newProfile = new HostsProfile(profileName, defaultContent);

            Profiles.Add(newProfile);
            SelectedProfile = newProfile;
            _storageService.SaveProfiles(new System.Collections.Generic.List<HostsProfile>(Profiles));
        }

        private void ExecuteMinimize(Window window)
        {
            if (window != null) window.WindowState = WindowState.Minimized;
        }

        private void ExecuteMaximize(Window window)
        {
            if (window == null) return;

            if (window.WindowState == WindowState.Maximized)
                window.WindowState = WindowState.Normal;
            else
                window.WindowState = WindowState.Maximized;
        }

        private void ExecuteClose(Window window)
        {
            if (window != null) window.Close();
        }
    }
}
