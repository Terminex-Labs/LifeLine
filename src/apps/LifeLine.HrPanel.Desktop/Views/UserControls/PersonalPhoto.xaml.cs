using LifeLine.HrPanel.Desktop.Enums;
using System.Windows;
using System.Windows.Controls;

namespace LifeLine.HrPanel.Desktop.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Avatar.xaml
    /// </summary>
    public partial class PersonalPhoto : UserControl
    {
        public PersonalPhoto()
        {
            InitializeComponent();
        }

        #region ActionProperty

        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register(
                nameof(Action),
                typeof(TypeAction),
                typeof(PersonalPhoto),
                new FrameworkPropertyMetadata(TypeAction.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public TypeAction Action
        {
            get => (TypeAction)GetValue(ActionProperty);
            set => SetValue(ActionProperty, value);
        }

        #endregion
    }
}
