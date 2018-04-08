using System.Windows;

namespace popup
{
    /// <summary>
    /// Interaction logic for PopUp.xaml
    /// </summary>
    public partial class PopUp : Window
    {
        public string NewReason { get; private set; }
        public bool IsChanged { get; private set; }
        public PopUp(string userName, string timeStamp, string currentReason)
        {
            InitializeComponent();
            UserBox.Text = userName;
            DateBox.Text = timeStamp;
            ReasonBox.Text = currentReason;
            NewReason = currentReason;
            Closing += PopUp_Closing;
            ShowDialog();
        }

        private void PopUp_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!NewReason.Equals(ReasonBox.Text))
            {
                var box = MessageBox.Show("There are unsaved changes, do you want to save?", 
                    "Warning!", MessageBoxButton.YesNo);
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
            NewReason = ReasonBox.Text;
            IsChanged = true;
        }
    }
}
