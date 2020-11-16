using KillProcesses.Scripts.Events;
using KillProcesses.Scripts.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace KillProcesses.User_Controls {

    public partial class FilterCheckBox : UserControl {

        public FilterCheckBox(string text = null, bool is_enabled = false) {
            InitializeComponent();
            try {
                if (InputTB == null) {
                    throw new NullReferenceException("InputTB was null in filter check box constructor");
                }
                Current_tb = InputTB;
                if (text != null) {
                    Current_tb.Text = text;
                    EndEdit();
                    IsChecked = is_enabled;
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Failed to build a filter check box.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        TextBox Current_tb;
        TextBlock Current_tbl;
        //CheckBox MyCheckBox;
        string Old_ProcessName;

        public delegate void OnEditHandler(OnEditEventArgs args);
        public event OnEditHandler OnEdit;

        public delegate void OnDeleteHandler(string name);
        public event OnDeleteHandler OnDelete;

        public delegate void OnCheckHandler(bool new_state);
        public event OnCheckHandler OnCheckChanged;

        public bool IsChecked {
            get => MyCheckBox.IsChecked ?? false;
            set => MyCheckBox.IsChecked = value;
        }
        public string Text {
            get => Current_tbl?.Text;
        }

        public void Edit() {
            Current_tb = new TextBox() {
                Text = Current_tbl.Text
            };
            Current_tb.KeyDown += TextBox_KeyDown;
            Current_tb.SelectAll();
            ContentG.Children.Clear();
            ContentG.Children.Add(Current_tb);
        }

        public void EndEdit() {
            try {
                Current_tbl = new TextBlock() {
                    Text = Current_tb.Text
                };
                Old_ProcessName = Current_tb.Text;
                ContentG.Children.Clear();
                ContentG.Children.Add(Current_tbl);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Failed to end edit.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                var args = new OnEditEventArgs(Old_ProcessName, Current_tb.Text);
                OnEdit?.Invoke(args);
                if (args.Cancel) {
                    return;
                }
                EndEdit();
            }
        }

        private void EditMI_Click(object sender, RoutedEventArgs e) {
            Edit();
        }

        private void DeleteMI_Click(object sender, RoutedEventArgs e) {
            if (OnDelete == null) {
                throw new NotImplementedException();
            }
            OnDelete(Text);
        }

        private void MyCheckBox_Checked(object sender, RoutedEventArgs e) {
            OnCheckChanged?.Invoke(true);
        }

        private void MyCheckBox_Unchecked(object sender, RoutedEventArgs e) {
            OnCheckChanged?.Invoke(false);
        }
    }
}
