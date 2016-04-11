using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Runtime.CompilerServices;

using VK_De.NDimInDg;

namespace VK_De.WPF.NDimInDg {
	//	Framework 4.5 VirtualizingPanel	.IsVirtualizingWhenGrouping = true
	//									.CashLength = 
	public partial class DataGridEx:DataGrid, INotifyPropertyChanged, IDisposable{
		#region Const
		#region Common
		public	const	int		DimInColsMax	= 4;
		public	const	int		DimInRowsMax	= 4;
		private	const	double	RHWidthMin		= 4D;
		private	const	double	RHPWidthMin		= 20D;
		private	const	string	keyTLH			= "TLHeader";
		private	const	string	keyCH0			= "CHS0";
		private	const	string	keyCH1			= "CHS1";
		private	const	string	keyCH2			= "CHS2";
		private	const	string	keyCH3			= "CHS3";
		private	const	string	keyCH4			= "CHS4";
		private	const	string	keyRH0			= "RHS0";
		private	const	string	keyRH1			= "RHS1";
		private	const	string	keyRH2			= "RHS2";
		private	const	string	keyRH3			= "RHS3";
		private	const	string	keyRH4			= "RHS4";
		private	const	string	nameRHP			= "PART_RowHeaderGripper";
		private	const	string	nameRHPW		= "RowHeaderPartWidth";
		private	const	string	nameTGC			= "TitlesGridCols";
		private	const	string	nameTGR			= "TitlesGridRows";
		private	const	string	nameVisStateStd	= "StandardColumn";
		#endregion
		#endregion
		private class BindingParams{
			public	BindingParams(SynchronizationContext context, IEnumerable source) {Context = context; Source = source;}
			public	SynchronizationContext	Context {get; private set;}
			public	IEnumerable				Source	{get; private set;}
		}
		private	class HeaderParam:INotifyPropertyChanged		{
			private	const	double	_widthDelim		= 1D;
			#region Static Variables
			private	static	Brush	_brushFalse	= Transparent;	//	No border
			private	static	Brush	_brushTrue	= Black;		//	Border
			#endregion
			#region Static Properties
			public	static	GridLength	WidthDelimSta	{get; private set;}
			#endregion
			#region Variables
			private	bool	_modified	= false;
			private	ulong[]	_ids		= null;
			private	bool[]	_bNext		= null;
			#endregion
			public event PropertyChangedEventHandler PropertyChanged	= null;
			static		HeaderParam(){WidthDelimSta	= new GridLength(_widthDelim, GridUnitType.Pixel);}
			internal	HeaderParam(){Initialize();}
			internal	HeaderParam(ulong[] p){_ids = p; Initialize();}
			#region Properties
			public	bool	IsModified	{get{return _modified;}}
			public	ulong[]	Ids			{get{return _ids;}}
			[IndexerName("NextBrush")]
			public	Brush	this[int i]	{get{return Next(i);}}
			#endregion
			#region Methods Private
			private	void	Initialize				()		{
				int	i, count;
				if(_ids != null){
					count	= _ids.Length;
					_bNext	= new bool[count];
					for(i=0; i<count; ++i)	_bNext[i]	= true;
				}
			}
			private	Brush	Next					(int i) {
				if(_bNext != null && _bNext.Length > i && !_bNext[i])	return _brushFalse;
				else													return _brushTrue;
			}
			private	void	NotifyPropertyChanged	(String propertyName)	{if(PropertyChanged != null)	PropertyChanged(this, new PropertyChangedEventArgs(propertyName));}
			#endregion
			#region Methods Public
			public	void	ResetModified	()			{_modified	= false;}   //	TO DO! Not used!
			public	void	Initialize		(int dim)	{
				int i = 0;
				ulong[] idsNew = null;
				dim = (dim > 0) ? dim : 0;
				if(_ids == null) {
					_ids    = new ulong[dim];
					for(i=0; i<dim; ++i) _ids[i] = Const.NullId;
				} else if(_ids.Length < dim) {
					idsNew= new ulong[dim];
					for(i=0; i<_ids.Length; ++i) idsNew[i]   = _ids[i];
					for(i=_ids.Length; i<dim; ++i) idsNew[i]   = Const.NullId;
					_ids    = idsNew;
				} else if(_ids.Length > dim) {
					idsNew= new ulong[dim];
					for(i=0; i<dim; ++i) idsNew[i]   = _ids[i];
					_ids    = idsNew;
				}
				Initialize();
			}
			public	void	SetNext			(int index, bool value)	{
				if(_bNext != null && _bNext.Length > index){
					if(_bNext[index] != value){
						_bNext[index]	= value;
						_modified		= true;
					}
				}
			}
			public	void	NotifyPropertyChangedNext() {NotifyPropertyChanged("NextBrush");}
			#endregion
		}
		private	class HeaderAddTitle	{
			private	int		_indexDim	= -1;
			private	int		_indexSpan	= -1;
			private	int		_span		= 0;
			private	ulong	_id			= Const.NullId;
			public	HeaderAddTitle(int indexDim, int indexSpan, int span, ulong id){
				_indexDim	= indexDim;
				_indexSpan	= indexSpan;
				_span		= span;
				_id			= id;
			}
			public	int		IndexDim	{get{return _indexDim;}}
			public	int		IndexSpan	{get{return _indexSpan;}}
			public	int		Span		{get{return _span;}}
			public	ulong	Id			{get{return _id;}}
		}
		#region Static
		private	static	readonly	Brush	Transparent	=	new SolidColorBrush(Colors.Transparent);
		private	static	readonly	Brush	Blue		=	new SolidColorBrush(Colors.Blue);
		private	static	readonly	Brush	Black		=	new SolidColorBrush(Colors.Black);
		#endregion
		#region DependencyProperty
		#region ViewItemsSource
		public static readonly DependencyProperty ViewItemsSourceProperty = DependencyProperty.Register( "ViewItemsSource", typeof(IEnumerable), typeof(DataGridEx),
			new FrameworkPropertyMetadata(null, OnViewItemsSourcePropertyChanged, OnCoerceViewItemsSourceProperty), OnValidateViewItemsSourceProperty);
		public ViewItemsSource ViewItemsSource{
			get{return (ViewItemsSource)GetValue(ViewItemsSourceProperty);}
			set{SetValue(ViewItemsSourceProperty, value);}
		}
		private static void OnViewItemsSourcePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e){
			DataGridEx		control			= source as DataGridEx;
			Action<object>	bindingAction	= new Action<object>(control.BindingFromAnotherThread);
			Task			bindingTask		= new Task(bindingAction, new BindingParams(SynchronizationContext.Current, e.NewValue as IEnumerable));
			bindingTask.Start();
		}
		private static object OnCoerceViewItemsSourceProperty(DependencyObject sender, object data){return data;}
		private static bool OnValidateViewItemsSourceProperty(object data){
			bool ret	= true;
			if(data != null)	ret	= 	data is ViewItemsSource;
			return ret;
		}
		#endregion
		#region TopLeftHeaderStyle
		public static readonly DependencyProperty TopLeftHeaderStyleProperty = DependencyProperty.Register( "TopLeftHeaderStyle", typeof(Style), typeof(DataGridEx),
			new FrameworkPropertyMetadata(null, OnTopLeftHeaderStylePropertyChanged, OnCoerceTopLeftHeaderStyleProperty), OnValidateTopLeftHeaderStyleProperty);
		public Style TopLeftHeaderStyle{
			get{return (Style)GetValue(TopLeftHeaderStyleProperty);}
			set{SetValue(TopLeftHeaderStyleProperty, value);}
		}
		private static void OnTopLeftHeaderStylePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e){
			DataGridEx	control	= source as DataGridEx;
			Style		style	= e.NewValue as Style;
			if(control != null && control._topLeftHeader != null) {
				if(style != null)	control._topLeftHeader.Style	= style;
				else				control._topLeftHeader.Style	= control._initTopLeftHeaderStyle;
			}
		}
		private static object OnCoerceTopLeftHeaderStyleProperty(DependencyObject sender, object data){return data;}
		private static bool OnValidateTopLeftHeaderStyleProperty(object data){
			bool ret	= true;
			if(data != null)	ret	= 	data is Style;
			return ret;
		}
		#endregion
		#region MergingHeadersCol
		public static readonly DependencyProperty MergingHeadersColProperty = DependencyProperty.Register( "MergingHeadersCol", typeof(bool), typeof(DataGridEx),
			new FrameworkPropertyMetadata(true, OnMergingHeadersColPropertyChanged, OnCoerceMergingHeadersColProperty), OnValidateMergingHeadersColProperty);
		public bool MergingHeadersCol{
			get{return (bool)GetValue(MergingHeadersColProperty);}
			set{SetValue(MergingHeadersColProperty, value);}
		}
		private static void OnMergingHeadersColPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e){
			DataGridEx		control			= source as DataGridEx;
			control.RefreshHeadersCol();
		}
		private static object OnCoerceMergingHeadersColProperty(DependencyObject sender, object data){
			return data;
		}
		private static bool OnValidateMergingHeadersColProperty(object data){
			bool ret	= true;
			if(data != null)	ret	= 	data is bool;
			return ret;
		}
		#endregion
		#region MergingHeadersRow
		public static readonly DependencyProperty MergingHeadersRowProperty = DependencyProperty.Register( "MergingHeadersRow", typeof(bool), typeof(DataGridEx),
			new FrameworkPropertyMetadata(true, OnMergingHeadersRowPropertyChanged, OnCoerceMergingHeadersRowProperty), OnValidateMergingHeadersRowProperty);
		public bool MergingHeadersRow{
			get{return (bool)GetValue(MergingHeadersRowProperty);}
			set{SetValue(MergingHeadersRowProperty, value);}
		}
		private static void OnMergingHeadersRowPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e){
			DataGridEx		control			= source as DataGridEx;
			control.RefreshHeadersRow();
		}
		private static object OnCoerceMergingHeadersRowProperty(DependencyObject sender, object data){
			return data;
		}
		private static bool OnValidateMergingHeadersRowProperty(object data){
			bool ret	= true;
			if(data != null)	ret	= 	data is bool;
			return ret;
		}
		#endregion
		#region DgColHeadProperty
		public	static	readonly	DependencyProperty	DgColHeadProperty		= DependencyProperty.Register("HeaderParamCol", typeof(HeaderParam), typeof(DataGridColumnHeader),
				new PropertyMetadata(null, new PropertyChangedCallback(OnHeaderParamChangedC)));
		private	static	void	OnHeaderParamChangedC(DependencyObject o, DependencyPropertyChangedEventArgs e){}
		#endregion
		#region DgRowHeadProperty
		public	static	readonly	DependencyProperty	DgRowHeadProperty		= DependencyProperty.Register("HeaderParamRow", typeof(HeaderParam), typeof(DataGridRowHeader),
				new PropertyMetadata(null, new PropertyChangedCallback(OnHeaderParamChangedR)));
		private	static	void	OnHeaderParamChangedR(DependencyObject o, DependencyPropertyChangedEventArgs e){}
		#endregion
		#endregion
		#region Variables
		public event PropertyChangedEventHandler PropertyChanged;
		#region ItemsSource, Binding
		private	IViewColl			_viewColl				= null;
		private	ManualResetEvent	_isReady				= new ManualResetEvent(false);
		#endregion
		#region DataGrid Template Controls Standard
		private	Button				_topLeftHeader			= null;
		private	UIElementCollection	_colHeadersColl			= null;
		private	UIElementCollection	_rowsColl				= null;
		private	ScrollBar			_scrollbarVert			= null;
		private	ScrollBar			_scrollBarHoriz			= null;
		private	ScrollViewer		_colHeadersViewPort		= null;
		#endregion
		#region DataGrid Template Controls Additional
		private	Grid				_titlesGridCols			= null;
		private	Grid				_titlesGridRows			= null;
		#endregion
		private	Style				_initTopLeftHeaderStyle	= null;
		private	Style				_initColHeaderStyle		= null;
		private	Style				_initRowHeaderStyle		= null;
		private	int					_idsColHeadCount		= 0;
		private	int					_idsRowHeadCount		= 0;
		private	GridLength[]		_rowHeaderPartWidth		= null;
		private	bool				_disposed				= false;
		#endregion
		public DataGridEx():base() {
			InitializeComponent();
			HeadersVisibility			= DataGridHeadersVisibility.All;
			RowHeaderWidth				= RHWidthMin;
			IsReadOnly					= true;							//	TO DO!
			AutoGenerateColumns			= true;
			FrozenColumnCount			= 5;
			GridLinesVisibility			= DataGridGridLinesVisibility.All;
			HeadersVisibility			= DataGridHeadersVisibility.All;
			RowDetailsVisibilityMode	= DataGridRowDetailsVisibilityMode.Collapsed;
			_rowHeaderPartWidth			= new GridLength[DimInRowsMax];
			for(int i=0; i<DimInRowsMax; ++i)	_rowHeaderPartWidth[i]	= new GridLength(RHPWidthMin, GridUnitType.Pixel);
		}
		#region Properties
		[BindableAttribute(true)]
		public	GridLength[]		RowHeaderPartWidth	{get{return _rowHeaderPartWidth;}}
		//[BindableAttribute(true)]
		public new IEnumerable		ItemsSource			{
			get{return base.ItemsSource;}
			set{ViewTypePass	typePass		= null;
				Object			styleColHeaders	= null;
				Object			styleRowHeaders	= null;
				ColumnHeaderStyle	= _initColHeaderStyle;
				RowHeaderStyle		= _initRowHeaderStyle;
				_viewColl			= value as IViewColl;
				lock(ViewItem._itemTypeDelegatorLock) {
					ViewItem._itemTypeDelegatorCurr	= (_viewColl as ViewItemsSource)?.ItemTypeDelegator;	//	ViewItemTypeDescriptionProvider.GetTypeDescriptor - Synchronisation!
					if(_viewColl != null){
						typePass	= _viewColl.ItemTypePass;
						if(typePass != null){
							_idsColHeadCount	= (typePass.ColDims != null)?typePass.ColDims.Length:0;
							_idsRowHeadCount	= ((typePass.RowDims != null)?typePass.RowDims.Length:0);
							if(_idsColHeadCount > DimInColsMax)	throw new ArgumentOutOfRangeException("Too many dimensions in columns!");
							if(_idsRowHeadCount > DimInRowsMax)	throw new ArgumentOutOfRangeException("Too many dimensions in rows!");
							NotifyPropertyChanged("IdsColHeadCount");
							NotifyPropertyChanged("IdsRowHeadCount");
							switch(_idsColHeadCount){
								case 1:		styleColHeaders	= Resources[keyCH1];	break;
								case 2:		styleColHeaders	= Resources[keyCH2];	break;
								case 3:		styleColHeaders	= Resources[keyCH3];	break;
								case 4:		styleColHeaders	= Resources[keyCH4];	break;
								default:	styleColHeaders	= Resources[keyCH0];	break;
							}
							ColumnHeaderStyle	= styleColHeaders as Style;
							SetRowHeaderWidth(null);
							switch(_idsRowHeadCount){
								case 1:		styleRowHeaders	= Resources[keyRH1];	break;
								case 2:		styleRowHeaders	= Resources[keyRH2];	break;
								case 3:		styleRowHeaders	= Resources[keyRH3];	break;
								case 4:		styleRowHeaders	= Resources[keyRH4];	break;
								default:	styleRowHeaders	= Resources[keyRH0];	break;
							}
							RowHeaderStyle		= styleRowHeaders as Style;
							SetRowHeaderWidth(typePass.RowHeaderPartWidth);
						}
					}
					base.ItemsSource	= _viewColl as IEnumerable;
				}
				#region Binding After
				if(value != null){
					Action	after	= new Action(BindingAfter);
					Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);	// DispatcherPriority.Loaded(6) < DispatcherPriority.DataBind(8)
				}
				#endregion
			}
		}
		[BindableAttribute(true)]
		public	int					IdsColHeadCount		{get{return _idsColHeadCount;}}
		[BindableAttribute(true)]
		public	int					IdsRowHeadCount		{get{return _idsRowHeadCount;}}
		#endregion
		#region Methods Private - Binding
		private void	BindingFromAnotherThread(object source) {
			BindingParams p = null;
			if(_isReady != null) {
				_isReady.WaitOne();
				_isReady.Dispose();
				_isReady	= null;
			}
			p   = source as BindingParams;
			p?.Context.Post(Binding,p.Source);
		}
		private void	Binding					(object source) {
			ItemsSource = source as IEnumerable;
		}
		private	void	BindingAfter			(){
			Action	after	= null;
			after	= new Action(RefreshHeadersCol);
			Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);	// DispatcherPriority.Loaded(6) < DispatcherPriority.DataBind(8)
			after	= new Action(RefreshHeadersRow);
			Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);	// DispatcherPriority.Loaded(6) < DispatcherPriority.DataBind(8)
			_colHeadersViewPort.ScrollChanged			   += new ScrollChangedEventHandler(OnScrollChanged);
		}
		#endregion
		#region IDisposable
		public				void Dispose() {Dispose(true);}
		protected	virtual	void Dispose(bool disposing) {
			if(!_disposed){
				_isReady.Dispose();
				_isReady	= null;
				_disposed	= true;
			}
		}
		#endregion
		#region Methods Private - Refresh Headers Column
		private	void					RefreshHeadersCol	()																		{
			double[]				colWidths	= null;
			double[]				rowHeightS	= null;
			DataGridColumnHeader[]	colVisible	= null;
			HeaderParam[]			colParams	= null;
			List<HeaderAddTitle>	partAddTitle= null;
			colParams	= VisibleHeadersCol(out colVisible, out colWidths);
			if(colVisible != null && colVisible.Length > 0){
			    rowHeightS	= ColHeaderRows(colVisible[0]);
			    partAddTitle= CheckFullCols(colVisible, colParams);
			}
			RefreshGridCols(_titlesGridCols, partAddTitle, rowHeightS, colWidths);
		}
		private	HeaderParam[]			VisibleHeadersCol	(out DataGridColumnHeader[] headers, out double[] width)				{
			HeaderParam[]				ret				= null;
			int							i				= 0;
			double						w				= 0D;
			double						widthFrozen		= FrozenColsWidth();
			DataGridColumnHeader		h				= null;
			List<DataGridColumnHeader>	listVisibleCols	= null;
			List<DataGridColumnHeader>	listVisibleRest	= null;
			List<HeaderParam>			listRet			= null;
			List<DataGridColumnHeader>	listHeadersCols	= null;
			List<double>				listWidth		= null;
			double						widthViewPort	= _colHeadersViewPort.ViewportWidth - RowHeaderWidth;
			double						displace		= _colHeadersViewPort.HorizontalOffset;
			headers	= null;
			width	= null;
			if(widthViewPort > 0D){
				listVisibleCols	= new List<DataGridColumnHeader>();
				listVisibleRest	= new List<DataGridColumnHeader>();
				listRet			= new List<HeaderParam>();
				listHeadersCols	= new List<DataGridColumnHeader>();
				listWidth		= new List<double>();
				foreach(UIElement e in _colHeadersColl){
					h	= e as DataGridColumnHeader;
					if(h != null && h.ActualWidth > 0D){
						if(DataGridExHelper.ColumnHeader2Column(h) != null)	listVisibleCols.Add(h);
						else												listVisibleRest.Add(h);
					}
				}
				listVisibleCols.Sort(VisibleHeadersComp);
				for(i=0; i<FrozenColumnCount; ++i){
					h	= listVisibleCols[i];
					w	= Math.Min(widthViewPort, h.ActualWidth);
					listHeadersCols.Add(h);
					listWidth.Add(w);
					widthViewPort	= widthViewPort - w;
					if(widthViewPort <= 0D)	break;
				}
				if(widthViewPort > 0D){
					for(i=FrozenColumnCount; i<listVisibleCols.Count; ++i){
						h	= listVisibleCols[i];
						if(displace > 0D){
							displace	= displace - h.ActualWidth;
							if(displace < 0D){
								w	= Math.Min(widthViewPort, -displace);
								listHeadersCols.Add(h);
								listWidth.Add(w);
							}else w	= 0D;	//	Column is not visible
						}else{
							w	= Math.Min(widthViewPort, h.ActualWidth);
							listHeadersCols.Add(h);
							listWidth.Add(w);
						}
						widthViewPort	= widthViewPort - w;
						if(widthViewPort <= 0D) break;
					}
				}
				ret		= new HeaderParam[listHeadersCols.Count];
				headers	= listHeadersCols.ToArray();
				width	= listWidth.ToArray();
				for(i=0; i<headers.Length; ++i){
					ret[i]	=  headers[i].GetValue(DgColHeadProperty) as HeaderParam;
					if(ret[i] == null)										VisualStateManager.GoToState(headers[i], nameVisStateStd, true);
					else if(ret[i].Ids == null || ret[i].Ids.Length == 0)	headers[i].Content	= String.Empty;
				}
			}
			return ret;
		}
		private	double					FrozenColsWidth		()	{return NonFrozenColumnsViewportHorizontalOffset;}
		private	int						VisibleHeadersComp	(DataGridColumnHeader ch1, DataGridColumnHeader ch2)					{
			int				ret	= 0;
			DataGridColumn	c1	= null;
			DataGridColumn	c2	= null;
			if(		 ch1 != null && ch2 != null){
				c1	= DataGridExHelper.ColumnHeader2Column(ch1);
				c2	= DataGridExHelper.ColumnHeader2Column(ch2);
				if(		c1.DisplayIndex < c2.DisplayIndex)	ret	= -1;
				else if(c1.DisplayIndex > c2.DisplayIndex)	ret	=  1;
				if(		ch1.TabIndex < ch2.TabIndex)		ret	= -1;
				else if(ch1.TabIndex > ch2.TabIndex)		ret	=  1;
			}else if(ch1 == null && ch2 != null){			ret	=  1;
			}else if(ch1 != null && ch2 == null){			ret	= -1;
			}
			return ret;
		}
		private	double[]				ColHeaderRows		(DataGridColumnHeader h)												{
			int						i	= 0;
			double[]				ret	= null;
			Grid					g	= null;
			RowDefinitionCollection	rdc	= null;
			if(h != null){
				g	= DataGridExHelper.ColumnHeader2ColGrid(h);
				if(g != null){
					rdc	= g.RowDefinitions;
					ret	= new double[rdc.Count];
					foreach(RowDefinition	rd in rdc)	ret[i++]	= rd.ActualHeight;
				}
			}
			return ret;
		}
		private	List<HeaderAddTitle>	CheckFullCols		(DataGridColumnHeader[] p, HeaderParam[] colParams)								{
			const int				noSpan		= -1;		// Impossible for index
			int						i, j		= 0;
			int						itemsCount	= 0;
			int[]					spanIndex	= null;
			List<HeaderAddTitle>	ret			= new List<HeaderAddTitle>();
			if(p != null && colParams != null && p.Length > 0 && p.Length == colParams.Length) {
				#region Prefix
				itemsCount	= p.Length;
				spanIndex	= new int[_idsColHeadCount];
				for(j=0; j<_idsColHeadCount; ++j)	spanIndex[j]	= noSpan;
				for(i=0; i<itemsCount; ++i){
					if(colParams[i] != null)	colParams[i].Initialize(_idsColHeadCount);
				}
				#endregion
				for(i=0; i<itemsCount; ++i){
					if(colParams[i] != null){
						for(j=0; j<_idsColHeadCount; ++j){
							if(i < itemsCount-1){									//	Next
								if(colParams[i+1] != null && (colParams[i].Ids[j] == colParams[i+1].Ids[j] && MergingHeadersCol)){
									colParams[i].SetNext(j, false);
									if(spanIndex[j] == noSpan) spanIndex[j]	= i;	//	Open new span
								}else{
									colParams[i].SetNext(j, true);
									if(spanIndex[j] != noSpan){						//	Close span
										ret.Add(new HeaderAddTitle(j, spanIndex[j], i - spanIndex[j] + 1, colParams[i].Ids[j]));
										spanIndex[j]		= noSpan;
									}else ret.Add(new HeaderAddTitle(j, i, 1, colParams[i].Ids[j]));
								}
							}else	colParams[i].SetNext(j, true);
						}
						if(colParams[i].IsModified)	colParams[i].NotifyPropertyChangedNext();
					}
				}
				#region Suffix
				if(colParams[itemsCount-1] != null){
					for(j=0; j<_idsColHeadCount; ++j){								//	Close opened spans
						if(spanIndex[j] != noSpan){									//	Close span
							ret.Add(new HeaderAddTitle(j, spanIndex[j], itemsCount - spanIndex[j], colParams[itemsCount-1].Ids[j]));
							spanIndex[j]					= noSpan;
						}else ret.Add(new HeaderAddTitle(j, itemsCount-1, 1, colParams[itemsCount-1].Ids[j]));
					}
				}
				#endregion
			}
			return ret;
		}
		private	void					RefreshGridCols		(Grid grid, List<HeaderAddTitle> list, double[] rows, double[] columns)	{
			RowDefinition		rd	= null;
			ColumnDefinition	cd	= null;
			TextBlock			tb	= null;
			if(grid != null){
				grid.Children.Clear();
				grid.RowDefinitions.Clear();
				grid.ColumnDefinitions.Clear();
				if(list != null && list.Count > 0 && rows != null && rows.Length > 0 && columns != null && columns.Length >0){
					grid.Background				= Transparent;
					grid.HorizontalAlignment	= System.Windows.HorizontalAlignment.Left;
					foreach(double d in rows){
						rd				= new RowDefinition();
						rd.MaxHeight	= d;
						rd.MinHeight	= d;
						grid.RowDefinitions.Add(rd);
					}
					foreach(double d in columns){
						cd			= new ColumnDefinition();
						cd.MaxWidth	= d;
						cd.MinWidth	= d;
						grid.ColumnDefinitions.Add(cd);
					}
					foreach(HeaderAddTitle dpat in list){
						tb						= new TextBlock();
						tb.Text					= _viewColl.TitleOf(dpat.Id);
						tb.HorizontalAlignment	= HorizontalAlignment.Center;
						Grid.SetColumn(tb, dpat.IndexSpan);
						Grid.SetRow(tb, 2*dpat.IndexDim);
						Grid.SetColumnSpan(tb, dpat.Span);
						grid.Children.Add(tb);
					}
				}
			}
		}
		#endregion
		#region Methods Private - Refresh Headers Row
		private	void					RefreshHeadersRow	()	{RefreshHeadersRow(false);}
		private	void					RefreshHeadersRow	(bool widthIsChanged)													{
			double[]				rowHeights	= null;
			DataGridRowHeader[]		rowHeaders	= null;
			HeaderParam[]			rowParams	= null;
			List<HeaderAddTitle>	partAddTitle= null;
			rowParams    = VisibleHeadersRow(out rowHeaders, out rowHeights);
			if(rowHeaders != null && rowHeaders.Length > 0) {
				partAddTitle= CheckFullRows(rowHeaders, rowParams, widthIsChanged);
			}
			RefreshGridRows(_titlesGridRows, partAddTitle, rowHeights);
		}
		private	HeaderParam[]			VisibleHeadersRow	(out DataGridRowHeader[] headers, out double[] height)					{
			int							i, count;
			DataGridRowHeader			h			= null;
			HeaderParam					rowParam	= null;
			HeaderParam[]				ret			= null;
			DataGridRow					r			= null;
			List<double>				retHeight	= null;
			List<HeaderParam>			retList		= null;
			List<DataGridRowHeader>		headerList	= null;
			List<double>				topList		= null;
			ViewItem					dc			= null;
			UInt64[]					idsItem		= null;
			double[]					topArray	= null;
			FrameworkElement			fe			= null;
			double						rowTop		= 0D;
			double						rowBottom	= 0D;
			headers	= null;
			height	= null;
			if(_rowsColl != null){
				retList		= new List<HeaderParam>();
				headerList	= new List<DataGridRowHeader>();
				topList		= new List<double>();
				retHeight	= new List<double>();
				foreach(UIElement ui in _rowsColl){
					fe			= ui as FrameworkElement;
					if(fe != null){
						rowTop		= LayoutInformation.GetLayoutSlot(fe).Top;
						rowBottom	= LayoutInformation.GetLayoutSlot(fe).Bottom;
						if(ui is DataGridRow){
							r	= ui as DataGridRow;
							if(r != null && r.ActualHeight > 0D && r.Visibility == Visibility.Visible){
								h	= DataGridExHelper.Row2Header(r);
								if(h != null){
									if(h.Tag	== null)	h.Tag	= r;
									if(rowBottom > 0D){
										headerList.Add(h);
										topList.Add(rowTop);
									}
								}
								RHPSetHandlers(r, h, _idsRowHeadCount, true);
							}
						}
					}
				}
				headers	=headerList.ToArray();
				topArray= topList.ToArray();
				Array.Sort<double,DataGridRowHeader>(topArray, headers);
				if(headers.Length > _colHeadersViewPort.ViewportHeight + 1) {  //	CanContentScroll = true --> ViewportHeight = max count of items;
					Array.Resize<DataGridRowHeader>(ref headers, (int)(_colHeadersViewPort.ViewportHeight + 1));
				}
				count   = headers.Length;
				ret		= new HeaderParam[count];
				height	= new double[count];
				for(i=0; i<count; ++i){
					h			= headers[i];
					rowParam	= h.GetValue(DgRowHeadProperty) as HeaderParam;
					if(rowParam == null){
						dc		= h.DataContext as ViewItem;
						if(dc != null){	idsItem	= dc.Pass().IdsItemTitle;
							rowParam	= new HeaderParam(idsItem);
							h.SetValue(DgRowHeadProperty, rowParam);
						}
					}
					ret[i]		= rowParam;
					height[i]	= h.ActualHeight;
					if(topArray[i] < 0 && Math.Abs(topArray[i]) < height[i])	height[i]	= height[i] - Math.Abs(topArray[i]);
				}
			}
			return ret;
		}
		private	List<HeaderAddTitle>	CheckFullRows		(DataGridRowHeader[] rowHeaders, HeaderParam[] rowParams, bool widthIsChanged)	{
			const int noSpan	= -1;											// Impossible for index
			int			i, j		= 0;
			int			itemsCount	= 0;
			int[]		spanIndex	= null;
			List<HeaderAddTitle>	ret	= new List<HeaderAddTitle>();
			if(rowParams != null && rowParams.Length > 0) {
				#region Prefix
				itemsCount	= rowParams.Length;
				spanIndex	= new int[_idsRowHeadCount];
				for(j=0; j<_idsRowHeadCount; ++j)	spanIndex[j]	= noSpan;
				for(i=0; i<itemsCount; ++i){
					if(rowParams[i] != null)	rowParams[i].Initialize(_idsRowHeadCount);
				}
				#endregion
				for(i=0; i<itemsCount; ++i){
					if(rowParams[i] != null){
					    for(j=0; j<_idsRowHeadCount; ++j){
							if(i < itemsCount-1){									//	Next
								if(rowParams[i+1] != null && (rowParams[i].Ids[j] == rowParams[i+1].Ids[j] && MergingHeadersRow)){
									rowParams[i].SetNext(j, false);
									if(spanIndex[j] == noSpan) spanIndex[j]	= i;	//	Open new span
								}else{
									rowParams[i].SetNext(j, true);
									if(spanIndex[j] != noSpan){						//	Close span
									    ret.Add(new HeaderAddTitle(j, spanIndex[j], i - spanIndex[j] + 1, rowParams[i].Ids[j]));
									    spanIndex[j]		= noSpan;
									}else ret.Add(new HeaderAddTitle(j, i, 1, rowParams[i].Ids[j]));
								}
							}else	rowParams[i].SetNext(j, true);
					    }
						if(rowParams[i].IsModified || widthIsChanged)	rowParams[i].NotifyPropertyChangedNext();
					}
				}
				#region Suffix
				if(rowParams[itemsCount-1] != null){
					for(j=0; j<_idsRowHeadCount; ++j){								//	Close opened spans
						if(spanIndex[j] != noSpan){									//	Close span
						    ret.Add(new HeaderAddTitle(j, spanIndex[j], itemsCount - spanIndex[j], rowParams[itemsCount-1].Ids[j]));
						    spanIndex[j]					= noSpan;
						}else ret.Add(new HeaderAddTitle(j, itemsCount-1, 1, rowParams[itemsCount-1].Ids[j]));
					}
				}
				#endregion
			}
			return ret;
		}
		private	void					RefreshGridRows		(Grid grid, List<HeaderAddTitle> list, double[] rows)	{
			int					i	= 0;
			RowDefinition		rd	= null;
			ColumnDefinition	cd	= null;
			TextBlock			tb	= null;
			if(grid != null){
				grid.Children.Clear();
				grid.RowDefinitions.Clear();
				grid.ColumnDefinitions.Clear();
				if(list != null && list.Count > 0 && rows != null && rows.Length > 0 && _idsRowHeadCount>0){
				    grid.HorizontalAlignment	= System.Windows.HorizontalAlignment.Left;
				    for(i=0; i<_idsRowHeadCount; ++i){
				        cd			= new ColumnDefinition();
				        cd.MaxWidth	= _rowHeaderPartWidth[i].Value;
				        cd.MinWidth	= _rowHeaderPartWidth[i].Value;
				        grid.ColumnDefinitions.Add(cd);
						cd			= new ColumnDefinition();
				        cd.MaxWidth	= HeaderParam.WidthDelimSta.Value;
				        cd.MinWidth	= HeaderParam.WidthDelimSta.Value;
				        grid.ColumnDefinitions.Add(cd);
				    }
					foreach(double d in rows){
					    rd				= new RowDefinition();
					    rd.MaxHeight	= d;
					    rd.MinHeight	= d;
					    grid.RowDefinitions.Add(rd);
					}
				    foreach(HeaderAddTitle dpat in list){
				        tb						= new TextBlock();
				        tb.Text					=_viewColl.TitleOf(dpat.Id);
				        tb.VerticalAlignment	= VerticalAlignment.Center;
				        Grid.SetColumn(tb, 2*dpat.IndexDim);
				        Grid.SetRow(tb, dpat.IndexSpan);
				        Grid.SetRowSpan(tb, dpat.Span);
				        grid.Children.Add(tb);
				    }
				}
			}
		}
		#endregion
		#region Methods Private Drag Headers Row
		private	void	RHPSetHandlers		(DataGridRow r, DataGridRowHeader header, int count, bool add){
			int					i	= 0;
			Grid				g0	= null;
			Thumb				th	= null;
			FrameworkElement	fe	= null;
			foreach(DataGridColumn col in Columns){
				fe		= col.GetCellContent(r);
		        if(fe != null){
					fe.DataContextChanged			-=new DependencyPropertyChangedEventHandler(OnFeDataContextChanged);
					if(add)	fe.DataContextChanged	+=new DependencyPropertyChangedEventHandler(OnFeDataContextChanged);
				}
			}
			if(header != null && VisualTreeHelper.GetChildrenCount(header) > 0){
				g0	= VisualTreeHelper.GetChild(header, 0) as Grid;
				if(g0 != null){
					for(i=0; i<count; ++i){
						th	= g0.FindName(nameRHP+i.ToString()) as Thumb;
						if(th != null){
							th.DragStarted			-= new DragStartedEventHandler	(RHPOnDragStarted);
							th.DragDelta			-= new DragDeltaEventHandler	(RHPOnDragDelta);
							th.DragCompleted		-= new DragCompletedEventHandler(RHPOnDragCompleted);
							if(add){
								th.DragStarted		+= new DragStartedEventHandler	(RHPOnDragStarted);
								th.DragDelta		+= new DragDeltaEventHandler	(RHPOnDragDelta);
								th.DragCompleted	+= new DragCompletedEventHandler(RHPOnDragCompleted);
							}
						}
					}
				}
			}
		}
		private	int		RHPIndex			(object sender)								{
			int		ret		= -1;
			Thumb	th		= sender as Thumb;
			string	name	= (th != null)?th.Name:null;
			if(name != null && name.Length > 0){
				name	= name.Substring(name.Length-1);
				try{ret	= Convert.ToInt32(name);}catch{ret=-1;}
			}
			return ret;
		}
		private	void	RHPOnDragStarted	(object sender, DragStartedEventArgs e)		{
			int			i		= 0;
			int			index	= RHPIndex(sender);
			double		newVal	= _rowHeaderPartWidth[index].Value + e.HorizontalOffset;
			double		offset	= 0D;
			double[]	parts	= null;
			offset	= e.HorizontalOffset;
			newVal	= _rowHeaderPartWidth[index].Value + offset;
			if(newVal > RHPWidthMin){
				parts	= new double[_idsRowHeadCount];
				for(i=0; i<_idsRowHeadCount; ++i)	parts[i]	= _rowHeaderPartWidth[i].Value;
				parts[index]	= newVal;
				SetRowHeaderWidth(parts);
			}
		}
		private	void	RHPOnDragCompleted	(object sender, DragCompletedEventArgs e)	{}
		private	void	RHPOnDragDelta		(object sender, DragDeltaEventArgs e)		{
			int		index	= RHPIndex(sender);
			double	newVal	= _rowHeaderPartWidth[index].Value + e.HorizontalChange;
			if(newVal > RHPWidthMin){
				double[]	parts	= new double[_idsRowHeadCount];
				for(int i=0; i<_idsRowHeadCount; ++i)	parts[i]	= _rowHeaderPartWidth[i].Value;
				parts[index]	= newVal;
				SetRowHeaderWidth(parts);
			}
		}
		#endregion
		#region Methods Private On
		private void	OnLoaded					(object sender,RoutedEventArgs e)		{
			Grid	parentGridColRow= null;
			_idsColHeadCount	= 0;
			_idsRowHeadCount	= 0;
			_initColHeaderStyle	= ColumnHeaderStyle;
			_initRowHeaderStyle	= RowHeaderStyle;
			parentGridColRow	= DataGridExHelper.ParentGridColRow(this);
			_scrollbarVert		= DataGridExHelper.ScrollBarVert(this);
			_scrollBarHoriz		= DataGridExHelper.ScrollBarHoriz(this);
			_colHeadersViewPort	= DataGridExHelper.ColHeadersViewPort(this);
			_colHeadersViewPort.IsDeferredScrollingEnabled	= false;	//	true - no scroll by tracking of Thump
			_colHeadersViewPort.CanContentScroll			= true;		//	false -> disable row virtualising!
			_colHeadersColl		= DataGridExHelper.ColHeadersColl(this);
			_rowsColl			= DataGridExHelper.RowsColl(this);
			_topLeftHeader		= DataGridExHelper.TopLeftHeader(this);

			_titlesGridCols					= new Grid();
			_titlesGridCols.Name			= nameTGC;
			_titlesGridCols.Background		= Transparent;
			_titlesGridCols.IsHitTestVisible= false;
			_titlesGridRows					= new Grid();
			_titlesGridRows.Name			= nameTGR;
			_titlesGridRows.Background		= Transparent;
			_titlesGridRows.IsHitTestVisible= false;
			if(parentGridColRow != null) {
				parentGridColRow.Children.Add(_titlesGridCols);
				parentGridColRow.Children.Add(_titlesGridRows);
				Grid.SetColumn	(_titlesGridCols, 1);
				Grid.SetRow		(_titlesGridCols, 0);
				Grid.SetColumn	(_titlesGridRows, 0);
				Grid.SetRow		(_titlesGridRows, 1);
			}
			if(_scrollbarVert != null)	_scrollbarVert.IsVisibleChanged		+=new DependencyPropertyChangedEventHandler(OnScrollBarVisibleChanged);
			if(_scrollBarHoriz != null)	_scrollBarHoriz.IsVisibleChanged	+=new DependencyPropertyChangedEventHandler(OnScrollBarVisibleChanged);
			if(_topLeftHeader != null) {
				_initTopLeftHeaderStyle	= _topLeftHeader.Style;
				_topLeftHeader.Style	= Resources[keyTLH] as Style;
			}
			if(_isReady	 != null)	_isReady.Set();
		}
		private	void	OnColumnSizeChanged			(object sender, SizeChangedEventArgs e)	{RefreshHeadersCol();}
		private	void	OnMouseLeftButtonUp			(object sender, MouseButtonEventArgs e)	{
			Action					after		= null;
			DataGridColumnHeader	colHeader	= sender as DataGridColumnHeader;
			DataGridColumn			column		= DataGridExHelper.ColumnHeader2Column(colHeader);
			if(column != null){
				if(column.CanUserSort){
					if(CommitEdit()){
						after		= new Action(RefreshHeadersRow);
						Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);
					}
				}
			}
		}
		private void	OnScrollVert				(object sender, RoutedPropertyChangedEventArgs<double> e){
			Action		after	= new Action(RefreshHeadersRow);
			Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);
		}
		private void	OnScrollHori				(object sender, ScrollEventArgs e)		{
			Action		after	= new Action(RefreshHeadersCol);
			Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);
		}
		private void	OnRefreshHeadersRow			(object sender, RoutedEventArgs e)		{
			Action		after	= new Action(RefreshHeadersRow);
			Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);
		}
		private	void	OnScrollChanged				(object sender, ScrollChangedEventArgs e)	{
			Action		after	= null;
			if(e.HorizontalChange != 0D || e.ExtentWidthChange != 0D || e.ViewportWidthChange != 0D){
			    after	= new Action(RefreshHeadersCol);
			    Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);
			}
			if(e.VerticalChange != 0D || e.ExtentHeightChange != 0D || e.ViewportHeightChange != 0D){
			    after	= new Action(RefreshHeadersRow);
			    Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);
			}
		}
		private	void	OnScrollBarVisibleChanged	(Object sender, DependencyPropertyChangedEventArgs e){
			Action		after	= null;
			if(sender == _scrollbarVert){
			    after	= new Action(RefreshHeadersCol);
			    Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);
			}
			if(sender == _scrollBarHoriz){
			    after	= new Action(RefreshHeadersRow);
			    Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);
			}
		}
		private void	OnSortChanged				(Object sender, VisualStateChangedEventArgs e) {}
		private void	OnFeDataContextChanged		(object sender, DependencyPropertyChangedEventArgs e){
			FrameworkElement	fe		= sender as FrameworkElement;
		    if(fe != null) FeStyleCheck(fe);
		}
		private	void	OnRowLoaded					(object sender, RoutedEventArgs e)			{
		    DataGridRow			r		= sender as DataGridRow;
		    DataGridRowHeader	header	= null;
		    if(r != null){
		        r.Loaded	-= new RoutedEventHandler(OnRowLoaded);
				header		= DataGridExHelper.Row2Header(r);
		        if(header != null){
					RowLoadingAction(r, header);
					RHPSetHandlers(r, header, _idsRowHeadCount, true);
					r.Unloaded	+= new RoutedEventHandler(OnRowUnloaded);
				}
		    }
		}
		private	void	OnRowUnloaded				(object sender, RoutedEventArgs e)			{
		    DataGridRow			r		= sender as DataGridRow;
		    DataGridRowHeader	header	= null;
		    if(r != null){
		        r.Unloaded	-= new RoutedEventHandler(OnRowUnloaded);
				header		= DataGridExHelper.Row2Header(r);
				RHPSetHandlers(r, header, _idsRowHeadCount, false);
		    }
		}
		#endregion
		#region Methods Private - Additional
		private	UInt64[]	NormalIds				(UInt64[] ids, int norm)	{
			int			i	= 0;
			UInt64[]	ret	= null;
			if(ids == null || ids.Length != norm){
				ret	= new ulong[norm];
				for(i=0; i<norm; ++i)	ret[i]	= Const.NullId;
				if(ids != null){
					for(i=0; i<Math.Min(norm, ids.Length); ++i)	ret[i]	= ids[i];
				}
			}else ret	= ids;
			return ret;
		}
		private	void		ColHeaderInitialize		(string colname, UInt64[] colIds)	{
			DataGridColumnHeader	ch			= null;
			HeaderParam				colParam	= null;
			foreach(UIElement uie in _colHeadersColl){
				ch	= uie as DataGridColumnHeader;
				if(ch != null && ch.Content != null && colname == ch.Content.ToString()){
					if(colIds != null){
						colParam	= new HeaderParam(colIds);
				        ch.SetValue(DgColHeadProperty, colParam);
						Action	after	= new Action(RefreshHeadersCol);
						Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);	// DispatcherPriority.Loaded(6) < DispatcherPriority.DataBind(8)
					}
				    ch.SizeChanged		-= new SizeChangedEventHandler(OnColumnSizeChanged);
				    ch.SizeChanged		+= new SizeChangedEventHandler(OnColumnSizeChanged);
				    ch.RemoveHandler	(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUp));
				    ch.AddHandler		(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUp), true);
					break;
				}
			}
		}
		private void		NotifyPropertyChanged	(String info)			{if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(info));}
		private void		FeStyleCheck			(FrameworkElement fe)	{
			DataGridCell	cell		= null;
			DataGridColumn	col			= null;
			if(fe != null) {
				cell    = fe.Parent as DataGridCell;
				if(cell != null) {
					col = cell.Column;
					if(col != null) {
						PropertyDescriptor pi = col.GetValue(DataGridExHelper.DgColPropInfoProperty) as PropertyDescriptor;
					}
				}
			}
		}
		private	void		RowLoadingAction		(DataGridRow r, DataGridRowHeader header)			{
		    ViewItem			vi			= null;
		    UInt64[]			rowIds		= null;
		    HeaderParam			rowParam	= null;
			FrameworkElement	fe			= null;
		    if(r != null){
		        if(header != null){
		            vi			= r.DataContext as ViewItem;
		            if(vi != null){
		                rowIds		= vi.Pass().IdsItemTitle;
		                rowParam	= new HeaderParam(rowIds);
		            }
		            header.Tag	= r;
		            header.SetValue(DgRowHeadProperty, rowParam);
		        }
		        foreach(DataGridColumn col in Columns){
		            fe		= col.GetCellContent(r);
	                FeStyleCheck(fe);
		        }
		    }
		}
		#endregion
		#region Methods Protected Override On
		protected override void OnAutoGeneratingColumn	(DataGridAutoGeneratingColumnEventArgs e)	{
			PropertyDescriptor			pi				= null;
			ViewPropPass				pass			= null;
			UInt64[]					colIds			= null;
			Action<string, UInt64[]>	colHeaderIni	= null;
			DataGridLengthUnitType		unitType		= DataGridLengthUnitType.Auto;
			base.OnAutoGeneratingColumn(e);
			if(_viewColl != null){
				pi	= _viewColl.ItemPropGet(e.PropertyName);
				e.Column.SetValue(DataGridExHelper.DgColPropInfoProperty, pi);
				if(pi is ViewItemPropertyDescr){
					pass	= ((ViewItemPropertyDescr)pi).Pass;
					e.Column.CanUserResize			= true;
					e.Column.CanUserReorder			= true;
					e.Column.IsReadOnly				= false;
					switch(pass.ColWidthType){
						case ColumnWidthType.Auto:			unitType	= DataGridLengthUnitType.Auto;			break;
						case ColumnWidthType.Fixed:			unitType	= DataGridLengthUnitType.Pixel;			break;
						case ColumnWidthType.SizeToCells:	unitType	= DataGridLengthUnitType.SizeToCells;	break;
						case ColumnWidthType.SizeToHeader:	unitType	= DataGridLengthUnitType.SizeToHeader;	break;
						default:							unitType	= DataGridLengthUnitType.Auto;			break;
					}
					e.Column.Width	= new DataGridLength(pass.ColWidth, unitType);
					colIds			= NormalIds(pass.IdsProp, _idsColHeadCount);
				}			//	else e.Cancel	= true;
			}
			if(!e.Cancel){
				colHeaderIni	= new Action<string,ulong[]>(ColHeaderInitialize);
				Dispatcher.BeginInvoke(colHeaderIni, DispatcherPriority.Loaded, new object[]{e.PropertyName, colIds});	// DispatcherPriority.Loaded(6) < DispatcherPriority.DataBind(8)
			}
		}
		protected override void OnColumnReordered		(DataGridColumnEventArgs e)					{
			base.OnColumnReordered(e);
			Action	after	= new Action(RefreshHeadersCol);
			Dispatcher.BeginInvoke(after, DispatcherPriority.Loaded, null);	// DispatcherPriority.Loaded(6) < DispatcherPriority.DataBind(8)
		}
		protected override void OnLoadingRow			(DataGridRowEventArgs e)					{
			DataGridRow			r			= e.Row;
			DataGridRowHeader	header		= null;
			base.OnLoadingRow(e);
			if(r != null){
				if(VisualTreeHelper.GetChildrenCount(r) > 0)	header		= DataGridExHelper.Row2Header(r);
				if(header == null)	r.Loaded	+=new RoutedEventHandler(OnRowLoaded);
				else				RowLoadingAction(r, header);
			}
		}
		protected override void OnLoadingRowDetails		(DataGridRowDetailsEventArgs e)				{base.OnLoadingRowDetails(e);}
		protected override void OnUnloadingRow			(DataGridRowEventArgs e)					{
			DataGridRow			r		= e.Row;
			DataGridRowHeader	header	= null;
			base.OnUnloadingRow(e);
			header	= DataGridExHelper.Row2Header(r);
			if(header != null){
				header.Tag	= null;
				header.SetValue(DgRowHeadProperty, null);
			}
		}
		protected override void OnUnloadingRowDetails	(DataGridRowDetailsEventArgs e)				{base.OnUnloadingRowDetails(e);}
		#endregion
		#region Methods Public
		public	void	SetRowHeaderWidth	(double[] parts){
			int		i		= 0;
			double	part	= 0D;
			double	width	= 0D;
			if(parts != null && parts.Length > 0) {
				for(i=0; i<parts.Length; ++i) {
					part	= (RHPWidthMin > parts[i])?RHPWidthMin:parts[i];
					_rowHeaderPartWidth[i]	= new GridLength(part, GridUnitType.Pixel);
					width					= width + part + HeaderParam.WidthDelimSta.Value;
				}
			}
			if(width < RHWidthMin)	width	= RHWidthMin;
			RowHeaderWidth	= width;
			NotifyPropertyChanged(nameRHPW);
			RefreshHeadersRow(true);
		}
		public void Refresh() {
			Dispatcher.BeginInvoke(new Action(RefreshHeadersCol), DispatcherPriority.Loaded, null); // DispatcherPriority.Loaded(6) < DispatcherPriority.DataBind(8)
			Dispatcher.BeginInvoke(new Action(RefreshHeadersRow), DispatcherPriority.Loaded, null); // DispatcherPriority.Loaded(6) < DispatcherPriority.DataBind(8)
		}
		public	void	ResetHorizontalOffset(){_colHeadersViewPort?.ScrollToHorizontalOffset(0D);}
		//public	void	Print(DrawingContext dc){
		//    Rectangle	rect	= new Rectangle();
		//    VisualBrush	vb	= new VisualBrush();
		//    vb.Visual	= this;
		//    //dc.DrawRectangle(vb, null, rect);
		//}
		#endregion
	}
	internal	static	class DataGridExHelper		{
		#region Const
		private	const	string	nameSV	= "DG_ScrollViewer";
		private	const	string	nameSBV	= "PART_VerticalScrollBar";
		private	const	string	nameSBH	= "PART_HorizontalScrollBar";
		private	const	string	nameCHP	= "PART_ColumnHeadersPresenter";
		private	const	string	nameSCP	= "PART_ScrollContentPresenter";
		private	const	string	nameRoot= "Root";
		private	const	string	nameRP	= "PART_RowsPresenter";
		private	const	string	nameTLH	= "PART_FillerColumnHeader";	//	"TopLeftCornerHeader";
		#endregion
		#region DependencyProperty
		#region DgColPropInfoProperty
		public static readonly DependencyProperty DgColPropInfoProperty = DependencyProperty.Register("PropertyInfo", typeof(PropertyDescriptor), typeof(DataGridColumn),
				new PropertyMetadata(null, new PropertyChangedCallback(OnDgColPropInfoChangedR)));
		private static void OnDgColPropInfoChangedR(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			DataGridColumn		col	= o				as DataGridColumn;
			PropertyDescriptor	v	= e.NewValue	as PropertyDescriptor;
			if(col != null && e.OldValue != null &&  e.NewValue == null) {
			}
		}
		#endregion
		#region DgCellColumnProperty
		#endregion
		#endregion
		#region DataGrid -> Part
		internal	static	Grid					ParentGridColRow	(DataGrid p)				{
			ScrollViewer					sc	= FindDescendant(p, nameSV)	as ScrollViewer;
			Grid							ret	= (sc	!= null)?VisualTreeHelper.GetChild(sc,0)	as Grid								:null;
			return ret;
		}
		internal	static	ScrollBar				ScrollBarVert		(DataGrid p)				{
			ScrollBar		ret	= FindDescendant(p, nameSBV) as ScrollBar;
			return ret;
		}
		internal	static	ScrollBar				ScrollBarHoriz		(DataGrid p)				{
			ScrollBar		ret	= FindDescendant(p, nameSBH) as ScrollBar;
			return ret;
		}
		internal	static	ScrollViewer			ColHeadersViewPort	(DataGrid p)				{
			ScrollViewer					ret	= FindDescendant(p, nameSV)	as ScrollViewer;
			return ret;
		}
		internal	static	UIElementCollection		ColHeadersColl		(DataGrid p)				{
			UIElementCollection	ret	= null;
			DataGridColumnHeadersPresenter	p0	= FindDescendant(p, nameCHP) as DataGridColumnHeadersPresenter;
			Grid							g2	= (p0	!= null)?VisualTreeHelper.GetChild(p0, 0)	as Grid								:null;
			ItemsPresenter					ip	= (g2	!= null)?VisualTreeHelper.GetChild(g2, 1)	as ItemsPresenter					:null;
			DataGridCellsPanel				cp	= (ip	!= null)?VisualTreeHelper.GetChild(ip, 0)	as DataGridCellsPanel				:null;
			ret	= (cp != null)?cp.Children:null;
			return ret;
		}
		internal	static	UIElementCollection		RowsColl			(DataGrid p)				{
			UIElementCollection	ret	= null;
			DataGridRowsPresenter			rp	= FindDescendant(p, nameRP) as DataGridRowsPresenter;
			ret	= (rp != null)?rp.Children:null;
			return ret;
		}
		internal	static	Button					TopLeftHeader		(DataGrid p){
			ScrollViewer					sc	= FindDescendant(p, nameSV)	as ScrollViewer;
			Grid							g1	= (sc	!= null)?VisualTreeHelper.GetChild(sc,0)	as Grid								:null;
			Button							ret	= (g1	!= null)?VisualTreeHelper.GetChild(g1,0)	as Button							:null;
			return ret;
		}
		#endregion
		#region Part -> Part
		public	static	DataGridRowHeader		Row2Header			(DataGridRow r)				{
			DataGridRowHeader		ret	= null;
			Border					br	= null;
			SelectiveScrollingGrid	sg	= null;
			if(VisualTreeHelper.GetChildrenCount(r) > 0){
				br	= (r	!= null)?VisualTreeHelper.GetChild(r, 0)	as Border					:null;
				sg	= (br	!= null)?VisualTreeHelper.GetChild(br, 0)	as SelectiveScrollingGrid	:null;
				ret	= (sg	!= null)?VisualTreeHelper.GetChild(sg, 2)	as DataGridRowHeader		:null;
			}
			return ret;
		}
		public	static	DataGridColumn			ColumnHeader2Column	(DataGridColumnHeader header){
			DataGridColumn	ret		= null;
			if(header != null)	ret	= header.Column;
			return ret;
		}
		public	static	Grid					ColumnHeader2ColGrid(DataGridColumnHeader h)	{
			Grid	ret	= (h	!= null)?VisualTreeHelper.GetChild(h, 0)		as Grid	:null;
			return ret;
		}
		#endregion
		private	static	FrameworkElement	FindDescendant(DependencyObject root, string name) {
			FrameworkElement		ret			= null;
			FrameworkElement		fe			= null;
			List<DependencyObject>	children	= new List<DependencyObject>();
			if(root != null){
				if(root is Visual) {
					for(int i=0; i<VisualTreeHelper.GetChildrenCount(root); ++i) children.Add(VisualTreeHelper.GetChild(root, i));
				}else{
					foreach(DependencyObject	child in LogicalTreeHelper.GetChildren(root)) children.Add(child);
				}
				foreach(DependencyObject	child in children) {
					fe	= child as FrameworkElement;
					if(fe != null && fe.Name.IndexOf(name) >= 0)	ret= fe;
					else											ret= FindDescendant(child, name);
					if(ret != null)	break;
				}
			}
			return ret;
		}
	}
}