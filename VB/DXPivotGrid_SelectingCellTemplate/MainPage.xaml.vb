Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Markup
Imports System.Xml.Serialization
Imports DevExpress.Xpf.PivotGrid
Imports DevExpress.Xpf.PivotGrid.Internal

Namespace DXPivotGrid_SelectingCellTemplate
	Partial Public Class MainPage
		Inherits UserControl
		Private dataFileName As String = "nwind.xml"
		Public Sub New()
			InitializeComponent()

			' Parses an XML file and creates a collection of data items.
			Dim [assembly] As System.Reflection.Assembly = _
				System.Reflection.Assembly.GetExecutingAssembly()
			Dim stream As Stream = [assembly].GetManifestResourceStream(dataFileName)
			Dim s As New XmlSerializer(GetType(OrderData))
			Dim dataSource As Object = s.Deserialize(stream)

			' Binds a pivot grid to this collection.
			pivotGridControl1.DataSource = dataSource

			Try
				pivotGridControl1.BeginUpdate()
				fieldCountry.FilterValues.FilterType = FieldFilterType.Included
				fieldCountry.FilterValues.Values = New Object() { "UK", "USA" }
			Finally
				pivotGridControl1.EndUpdate()
			End Try
		End Sub

		Private Sub ProgressBar_DataContextChanged(ByVal sender As Object, _
			ByVal e As DependencyPropertyChangedEventArgs)
			Dim bar As ProgressBar = (CType(sender, ProgressBar))
			Dim item As CellsAreaItem = TryCast(bar.DataContext, CellsAreaItem)
			If item Is Nothing Then
				Return
			End If
			bar.Maximum = Convert.ToDouble(item.ColumnTotalValue)
			bar.Value = Convert.ToDouble(item.Value)
		End Sub
	End Class
	Public Class CellTemplateSelector
		Inherits DataTemplateSelector
		Public Overrides Function SelectTemplate(ByVal item As Object, _
			ByVal container As DependencyObject) As DataTemplate
			Dim cell As CellsAreaItem = CType(item, CellsAreaItem)

			' Calculates the share of a cell value in the Row Grand Total value.
			Dim share As Double = _
				Convert.ToDouble(cell.Value) / Convert.ToDouble(cell.ColumnTotalValue)

			' Applies the Default template to the Row Grand Total cells.
			If cell.ColumnValue Is Nothing Then
				Return DefaultCellTemplate
			End If

			' If the share is too far from 50%, the Highlighted template is selected.
			' Otherwise, the Normal template is applied to the cell.
			If share > 0.7 OrElse share < 0.3 Then
				Return HighlightedCellTemplate
			Else
				Return NormalCellTemplate
			End If
		End Function
		Private privateDefaultCellTemplate As DataTemplate
		Public Property DefaultCellTemplate() As DataTemplate
			Get
				Return privateDefaultCellTemplate
			End Get
			Set(ByVal value As DataTemplate)
				privateDefaultCellTemplate = value
			End Set
		End Property
		Private privateHighlightedCellTemplate As DataTemplate
		Public Property HighlightedCellTemplate() As DataTemplate
			Get
				Return privateHighlightedCellTemplate
			End Get
			Set(ByVal value As DataTemplate)
				privateHighlightedCellTemplate = value
			End Set
		End Property
		Private privateNormalCellTemplate As DataTemplate
		Public Property NormalCellTemplate() As DataTemplate
			Get
				Return privateNormalCellTemplate
			End Get
			Set(ByVal value As DataTemplate)
				privateNormalCellTemplate = value
			End Set
		End Property
	End Class
	Public Class RoundConverter
		Inherits MarkupExtension
		Implements IValueConverter
		#Region "IValueConverter Members"
		Public Function Convert(ByVal value As Object, _
				ByVal targetType As Type, _
				ByVal parameter As Object, _
				ByVal culture As CultureInfo) As Object _
				Implements IValueConverter.Convert
			Return System.Convert.ToInt32(value)
		End Function
		Public Function ConvertBack(ByVal value As Object, _
				ByVal targetType As Type, _
				ByVal parameter As Object, _
				ByVal culture As CultureInfo) As Object _
				Implements IValueConverter.ConvertBack
			Throw New NotImplementedException()
		End Function
		#End Region
		Public Overrides Function ProvideValue(ByVal serviceProvider As IServiceProvider) As Object
			Return Me
		End Function
	End Class
End Namespace
