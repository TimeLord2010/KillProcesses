using KillProcesses.Scripts;
using KillProcesses.User_Controls;
using SqliteHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KillProcesses {

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            try {
                if (Manager == null) {
                    throw new Exception($"Manager was null");
                }
                Manager.GetProcesses((p, s) => {
                    if (p == null) {
                        throw new Exception($"Process name was null");
                    }
                    FiltersWP.Children.Add(CreateFilterCB(p, s));
                });
                UpdateInterval();
                if (Timer == null) {
                    throw new Exception($"Timer was null");
                }
                Timer.Tick += Timer_Tick;
                Timer.Start();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, $"Couldn't start program", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        const string DB_NAME = "KillProcess_Settings";
        readonly SettingsManager Manager = new SettingsManager(DB_NAME);
        readonly DispatcherTimer Timer = new DispatcherTimer();

        private void Timer_Tick(object sender, EventArgs e) {
            try {
                FoundLV.Items.Clear();
                var list = Process.GetProcesses().Where(p => ShouldBeAdded(p.ProcessName));
                foreach (var p in list) {
                    FoundLV.Items.Add(p.ProcessName);
                }
                if (AutoKillChB.IsChecked ?? false) {
                    foreach (var p in list) {
                        try {
                            p.Kill();
                        } catch (Exception) {
                        }
                    }
                }
                UpdateInterval();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, $"Erro in getting processes", MessageBoxButton.OK, MessageBoxImage.Error);
                Timer.Stop();
            }
        }

        void UpdateInterval() {
            try {
                var selected_mili = Convert.ToInt32(((ComboBoxItem)RefreshRateCB.SelectedItem).Content.ToString());
                Timer.Interval = TimeSpan.FromMilliseconds(selected_mili);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, $"Erro in getting timer interval", MessageBoxButton.OK, MessageBoxImage.Error);
                Timer.Interval = TimeSpan.FromMilliseconds(1000);
            }
        }

        bool ShouldBeAdded(string name) {
            foreach (var child in FiltersWP.Children.OfType<FilterCheckBox>()) {
                if (!child.IsChecked) {
                    continue;
                }
                var pattern = child.Text;
                if (pattern == null) {
                    continue;
                }
                if (Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase)) {
                    //if (Regex.IsMatch(name, $".*{pattern}.*", RegexOptions.IgnoreCase)) {
                    return true;
                }
            }
            return false;
        }

        private void AddB_Click(object sender, RoutedEventArgs e) {
            var filterc = CreateFilterCB();
            //var cm = new ContextMenu();
            //var edit_mi = new MenuItem() {
            //    Header = "Edit"
            //};
            //edit_mi.Click += delegate {
            //    filterc.Edit();
            //};
            //cm.Items.Add(edit_mi);
            //var delete_mi = new MenuItem() { 
            //    Header = "Delete"
            //};
            //cm.Items.Add(delete_mi);
            //filterc.ContextMenu = cm;
            FiltersWP.Children.Add(filterc);
            filterc.InputTB.SelectAll();
        }

        FilterCheckBox CreateFilterCB(string name = null, bool is_enabled = false) {
            var filterc = new FilterCheckBox(name, is_enabled);
            filterc.OnEdit += (args) => {
                try {
                    Manager.UpdateProcess(args.OldName, args.NewName);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    args.Cancel = true;
                }
            };
            filterc.OnDelete += (p) => {
                Manager.DeleteProcess(p);
                FiltersWP.Children.Remove(filterc);
            };
            filterc.OnCheckChanged += (new_state) => {
                Manager.SetEnabled(filterc.Text, new_state);
            };
            return filterc;
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
        }
    }
}
