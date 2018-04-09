using System.Data;
using System.Windows;
using System.Collections.Generic;

namespace popup
{
    /// <summary>
    /// Interaction logic for PopUp.xaml
    /// </summary>
    public partial class PopUp : Window
    {
        private DataTable logged;

        public bool IsSaved { get; private set; }
        public DataTable DTable { get; private set; }
        public HashSet<string> IdsToUpdate { get; private set; }
        private bool IsChanged;

        public PopUp(DataTable logged)
        {
            InitializeComponent();
            dataGrid.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
            dataGrid.DataContext = logged.DefaultView;
            dataGrid.CellEditEnding += DataGrid_CellEditEnding;
            this.logged = logged;
            IdsToUpdate = new HashSet<string>();
            ShowDialog();
        }

        private void DataGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            IsChanged = true;
            var rowView = e.Row.DataContext as DataRowView;
            IdsToUpdate.Add(rowView.Row.ItemArray[0].ToString());
        }
        
        private void DataGrid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            if (!e.Column.Header.ToString().Equals("reason"))
            {
                e.Column.IsReadOnly = true;
            }
            else
            {
                e.Column.Width = new System.Windows.Controls.DataGridLength(150);
            }
        }
        
        private void PopUp_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!IsSaved && IsChanged)
            {
                var box = MessageBox.Show("There are unsaved changes, do you want to save now?",
                    "WARNING!", MessageBoxButton.YesNo);
                if(box == MessageBoxResult.Yes)
                {
                    Update();
                }
            }
        }
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            Update();
        }
        private void Update()
        {
            DTable = ((DataView)dataGrid.ItemsSource).ToTable();
            IsSaved = true;
        }
    }
}
