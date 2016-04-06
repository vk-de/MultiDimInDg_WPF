using System;
using System.Windows;
using System.Windows.Controls;
using VK_De.NDimInDg;

namespace VK_De.WPF.NDimInDg.Test {
	public partial class MainWindow:Window {
		public MainWindow() {
			IModel		model	= null;
			ViewModel	vm		= null;
			InitializeComponent();
			model		= CreateModel();
			vm			= new ViewModel(model);
			DataContext	= vm;
		}
		private	IModel	CreateModel	() {
			IModel		ret		= null;
			IDimension	dim		= null;
			IDimItem	item	= null;
			ret	= new Model();
			for(int i=1; i<=5; ++i) {
				dim	= new Dimension(i);
				for(int j=1; j<=10; ++j) {
					item	= new DimItem((UInt64)(i*Model.DimItemMax + j));
					dim.DimItemAdd(item);
				}
				ret.DimensionAdd(dim);
			}
			return ret;
		}
		private void OnRowHeaderWidth(object sender,RoutedEventArgs e) {
			int			i		= 0;
			double[]	width	= null;
			if(dataGrid.IdsRowHeadCount > 0) {
				width	= new double[dataGrid.IdsRowHeadCount];
				for(i=0; i<dataGrid.IdsRowHeadCount; ++i) {
					width[i]	= dataGrid.RowHeaderPartWidth[i].Value + 40;
				}
				dataGrid.SetRowHeaderWidth(width);
			}
		}
		private void OnMIClick(object sender,RoutedEventArgs e) {
			int	count = 0;
			try{count = Convert.ToInt32(((MenuItem)sender).Tag);}catch{count=0;}
			if(count < 0)	count= 0;
			if(count > 6)	count= 6;
			dataGrid.ResetHorizontalOffset();
			dataGrid.FrozenColumnCount	= count;
		}
		private void OnCheckedChangedRow(object sender, RoutedEventArgs e) {
			CheckBox	cb	= sender as CheckBox;
			dataGrid.MergingHeadersRow	= (bool)cb.IsChecked;
		}
		private void OnCheckedChangedCol(object sender, RoutedEventArgs e) {
			CheckBox	cb	= sender as CheckBox;
			dataGrid.MergingHeadersCol	= (bool)cb.IsChecked;
		}
	}
}
