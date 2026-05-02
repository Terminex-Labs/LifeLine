using LifeLine.HrPanel.Desktop.Enums;
using System.Windows;
using System.Windows.Controls;

namespace LifeLine.HrPanel.Desktop.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AssigmentsContracts.xaml
    /// </summary>
    public partial class AssigmentsContracts : UserControl
    {
        public AssigmentsContracts()
        {
            InitializeComponent();
        }

        #region ActionProperty

        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register(
                nameof(Action),
                typeof(TypeAction),
                typeof(AssigmentsContracts),
                new FrameworkPropertyMetadata(TypeAction.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public TypeAction Action
        {
            get => (TypeAction)GetValue(ActionProperty);
            set => SetValue(ActionProperty, value);
        }

        #endregion
    }
}
