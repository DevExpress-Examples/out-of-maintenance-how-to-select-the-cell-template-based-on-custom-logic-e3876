using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml.Serialization;
using DevExpress.Xpf.PivotGrid;
using DevExpress.Xpf.PivotGrid.Internal;

namespace DXPivotGrid_SelectingCellTemplate {
    public partial class MainPage : UserControl {
        string dataFileName = "DXPivotGrid_SelectingCellTemplate.nwind.xml";
        public MainPage() {
            InitializeComponent();

            // Parses an XML file and creates a collection of data items.
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(dataFileName);
            XmlSerializer s = new XmlSerializer(typeof(OrderData));
            object dataSource = s.Deserialize(stream);

            // Binds a pivot grid to this collection.
            pivotGridControl1.DataSource = dataSource;

            try {
                pivotGridControl1.BeginUpdate();
                fieldCountry.FilterValues.FilterType = FieldFilterType.Included;
                fieldCountry.FilterValues.Values = new object[] { "UK", "USA" };
            }
            finally {
                pivotGridControl1.EndUpdate();
            }
        }

        private void ProgressBar_DataContextChanged(object sender, 
                DependencyPropertyChangedEventArgs e) {
            ProgressBar bar = ((ProgressBar)sender);
            CellsAreaItem item = bar.DataContext as CellsAreaItem;
            if (item == null)
                return;
            bar.Maximum = Convert.ToDouble(item.ColumnTotalValue);
            bar.Value = Convert.ToDouble(item.Value);
        }
    }
    public class CellTemplateSelector : DataTemplateSelector {
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            CellsAreaItem cell = (CellsAreaItem)item;

            // Calculates the share of a cell value in the Row Grand Total value.
            double share = Convert.ToDouble(cell.Value) / Convert.ToDouble(cell.ColumnTotalValue);

            // Applies the Default template to the Row Grand Total cells.
            if (cell.ColumnValue == null)
                return DefaultCellTemplate;

            // If the share is too far from 50%, the Highlighted template is selected.
            // Otherwise, the Normal template is applied to the cell.
            if (share > 0.7 || share < 0.3)
                return HighlightedCellTemplate;
            else
                return NormalCellTemplate;
        }
        public DataTemplate DefaultCellTemplate { get; set; }
        public DataTemplate HighlightedCellTemplate { get; set; }
        public DataTemplate NormalCellTemplate { get; set; }
    }
    public class RoundConverter : MarkupExtension, IValueConverter {
        #region IValueConverter Members
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture) {
            return System.Convert.ToInt32(value);
        }
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
        #endregion
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }}
