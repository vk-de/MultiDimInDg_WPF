using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using VK_De.NDimInDg;

namespace VK_De.WPF.NDimInDg.Test {
	public class Command:ICommand {
		private static bool CanExecuteDefault(object parameter)	{return true;}
		private Action<object>		_execute			= null;
		private Predicate<object>	_canExecute			= CanExecuteDefault;
		private event EventHandler	_canExecuteChanged	= null;
		public Command(Action<object> execute):this(execute, null){}
		public Command(Action<object> execute, Predicate<object> canExecute) {
			if(execute		!=null)	_execute=execute;
			else					throw new ArgumentNullException("Command.Execute");
			if(canExecute	!=null)	_canExecute=canExecute;
		}
		public event EventHandler CanExecuteChanged {
			add		{CommandManager.RequerySuggested	+= value;	_canExecuteChanged	+= value;}
			remove	{CommandManager.RequerySuggested	-= value;	_canExecuteChanged	-= value;}
		}
		public bool CanExecute			(object parameter)	{return _canExecute(parameter);}
		public void Execute				(object parameter)	{_execute(parameter);}
		public void OnCanExecuteChanged	()					{_canExecuteChanged?.Invoke(this, EventArgs.Empty);}
	}
	public	class ViewModel:INotifyPropertyChanged {
		private	IModel				_model				= null;
		private	ViewItemsSource		_coll				= null;
		private	Style				_topLeftHeaderStyle	= null;
		private	Command				_commClear			= null;
		private	Command				_comm0_0			= null;
		private	Command				_comm1_0			= null;
		private	Command				_comm2_0			= null;
		private	Command				_comm1_1			= null;
		private	Command				_comm1_4			= null;
		private	Command				_comm4_1			= null;
		private	Command				_comm3_2			= null;
		private	Command				_comm2_3			= null;
		private	Command				_comm0_1			= null;
		private	Command				_comm0_2			= null;
		public event PropertyChangedEventHandler PropertyChanged	= null;
		public ViewModel(IModel	model) {
			_model				= model;
			_commClear			= new Command(OnCommClear);
			_comm0_0			= new Command(OnComm0_0);
			_comm1_0			= new Command(OnComm1_0);
			_comm2_0			= new Command(OnComm2_0);
			_comm1_1			= new Command(OnComm1_1);
			_comm1_4			= new Command(OnComm1_4);
			_comm4_1			= new Command(OnComm4_1);
			_comm3_2			= new Command(OnComm3_2);
			_comm2_3			= new Command(OnComm2_3);
			_comm0_1			= new Command(OnComm0_1);
			_comm0_2			= new Command(OnComm0_2);
			OnComm3_2(null);
		}
		public	ViewItemsSource	ItemsSource		{get{return _coll;}}
		public	Style			TLHeaderStyle	{get{return _topLeftHeaderStyle;}}
		public	Command			CommClear		{get{return _commClear;}}
		public	Command			Comm0_0			{get{return _comm0_0;}}
		public	Command			Comm1_0			{get{return _comm1_0;}}
		public	Command			Comm2_0			{get{return _comm2_0;}}
		public	Command			Comm1_1			{get{return _comm1_1;}}
		public	Command			Comm1_4			{get{return _comm1_4;}}
		public	Command			Comm4_1			{get{return _comm4_1;}}
		public	Command			Comm3_2			{get{return _comm3_2;}}
		public	Command			Comm2_3			{get{return _comm2_3;}}
		public	Command			Comm0_1			{get{return _comm0_1;}}
		public	Command			Comm0_2			{get{return _comm0_2;}}
		private	void	NotifyPropertyChanged	(String propertyName)	{if(PropertyChanged != null)	PropertyChanged(this, new PropertyChangedEventArgs(propertyName));}
		private	void	OnCommClear	(object par){
			_coll = null;
			NotifyPropertyChanged("ItemsSource");
			SetTopLeftHeaderStyle(null);
		}
		private	void	OnComm0_0	(object par){
			UInt64[]		ids			= null;
			ViewTypePass	typePass	= new ViewTypePass();
			ViewItemPass	itemPass	= null;
			typePass.Id					= 1000;
			typePass.Flags				= 1000;
			typePass.FixedDims			= new ulong[0];
			typePass.FixedIds			= new ulong[0];
			typePass.ColDims			= new ulong[0];
			typePass.RowDims			= new ulong[0];
			typePass.RowHeaderPartWidth	= new double[0];
			_coll	= new ViewItemsSource(_model, typePass);
			#region Columns Add
			_coll.ItemPropAdd(new ViewPropPass(3880,typeof(int), new UInt64[] {}, 40, ColumnWidthType.Fixed));
			#endregion
			#region Rows Add
			ids     = new UInt64[0];
			itemPass= new ViewItemPass(Const.NullId, Const.NullFlags, ids);
			_coll.ItemAdd(itemPass);
			#endregion
			NotifyPropertyChanged("ItemsSource");
			SetTopLeftHeaderStyle(null);
		}
		private	void	OnComm1_0	(object par) {
			UInt64[]		ids			= null;
			ViewTypePass	typePass	= new ViewTypePass();
			ViewItemPass	itemPass	= null;
			typePass.Id					= 1000;
			typePass.Flags				= 1000;
			typePass.FixedDims			= new ulong[0];
			typePass.FixedIds			= new ulong[0];
			typePass.ColDims			= new ulong[1] {1};
			typePass.RowDims			= new ulong[0];
			typePass.RowHeaderPartWidth	= new double[0];
			_coll	= new ViewItemsSource(_model, typePass);
			#region Columns Add
			_coll.ItemPropAdd(new ViewPropPass(3880, typeof(int),		new UInt64[] {101}, 40,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3881, typeof(double),	new UInt64[] {102}, 50,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3882, typeof(object),	new UInt64[] {106}, 60,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3883, typeof(int),		new UInt64[] {105}, 70,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3885, typeof(double),	new UInt64[] {103}, 90,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3886, typeof(double),	new UInt64[] {104}, 100,ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3887, typeof(double),	new UInt64[] {108}, 110,ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3888, typeof(double),	new UInt64[] {107}, 120,ColumnWidthType.Fixed));
			#endregion
			#region Rows Add
			ids     = new UInt64[0];
			itemPass= new ViewItemPass(Const.NullId, Const.NullFlags, ids);
			_coll.ItemAdd(itemPass);
			#endregion
			NotifyPropertyChanged("ItemsSource");
			SetTopLeftHeaderStyle(null);
		}
		private	void	OnComm2_0	(object par) {
			UInt64[]		ids			= null;
			ViewTypePass	typePass	= new ViewTypePass();
			ViewItemPass	itemPass	= null;
			typePass.Id					= 1000;
			typePass.Flags				= 1000;
			typePass.FixedDims			= new ulong[0];
			typePass.FixedIds			= new ulong[0];
			typePass.ColDims			= new ulong[2] {1,2};
			typePass.RowDims			= new ulong[0];
			typePass.RowHeaderPartWidth	= new double[0];
			_coll	= new ViewItemsSource(_model, typePass);
			#region Columns Add
			_coll.ItemPropAdd(new ViewPropPass(3880, typeof(int),		new UInt64[] { 101, 202 }, 40,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3881, typeof(double),	new UInt64[] { 101, 204 }, 50,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3882, typeof(object),	new UInt64[] { 106, 204 }, 60,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3883, typeof(int),		new UInt64[] { 106, 205 }, 70,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3885, typeof(double),	new UInt64[] { 103, 204 }, 90,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3886, typeof(double),	new UInt64[] { 106, 207 }, 100,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3887, typeof(double),	new UInt64[] { 106, 208 }, 110,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3888, typeof(double),	new UInt64[] { 106, 209 }, 120,	ColumnWidthType.Fixed));
			#endregion
			#region Rows Add
			ids     = new UInt64[0];
			itemPass= new ViewItemPass(Const.NullId, Const.NullFlags, ids);
			_coll.ItemAdd(itemPass);
			#endregion
			NotifyPropertyChanged("ItemsSource");
			SetTopLeftHeaderStyle(null);
		}
		private	void	OnComm1_1	(object par) {
			UInt64[]		ids			= null;
			int				i			= 0;
			ViewTypePass	typePass	= new ViewTypePass();
			ViewItemPass	itemPass	= null;
			typePass.Id					= 1000;
			typePass.Flags				= 1000;
			typePass.FixedDims			= new ulong[0];
			typePass.FixedIds			= new ulong[0];
			typePass.ColDims			= new ulong[1] {1};
			typePass.RowDims			= new ulong[1] {2};
			typePass.RowHeaderPartWidth	= new double[1] {60};
			_coll	= new ViewItemsSource(_model, typePass);
			#region Columns Add
			_coll.ItemPropAdd(new ViewPropPass(3880, typeof(int),		new UInt64[] { 101 }, 40,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3881, typeof(double),	new UInt64[] { 102 }, 50,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3882, typeof(object),	new UInt64[] { 103 }, 60,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3883, typeof(int),		new UInt64[] { 104 }, 70,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3884, typeof(double),	new UInt64[] { 105 }, 80,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3885, typeof(double),	new UInt64[] { 106 }, 90,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3886, typeof(double),	new UInt64[] { 107 }, 100,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3887, typeof(double),	new UInt64[] { 108 }, 110,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3888, typeof(double),	new UInt64[] { 109 }, 120,	ColumnWidthType.Fixed));
			#endregion
			#region Rows Add
			for(i=1; i<10; ++i) {
				ids     = new UInt64[1];
				ids[0]  = (UInt64)i + 200;
				itemPass= new ViewItemPass(Const.NullId, Const.NullFlags, ids);
				_coll.ItemAdd(itemPass);
			}
			#endregion
			NotifyPropertyChanged("ItemsSource");
			SetTopLeftHeaderStyle("1-1");
		}
		private	void	OnComm1_4	(object par) {
			UInt64[]		ids			= null;
			int				i, j, k, l	= 0;
			ViewTypePass	typePass	= new ViewTypePass();
			ViewItemPass	itemPass	= null;
			typePass.Id				    = 1000;
			typePass.Flags				= 1000;
			typePass.FixedDims			= new ulong[0];
			typePass.FixedIds			= new ulong[0];
			typePass.ColDims			= new ulong[1] {1};
			typePass.RowDims			= new ulong[4] {2,3,4,5};
			typePass.RowHeaderPartWidth	= new double[4] {60, 60, 60, 50};
			_coll	= new ViewItemsSource(_model, typePass);
			#region Columns Add
			_coll.ItemPropAdd(new ViewPropPass(3880, typeof(int),		new UInt64[] { 101 }, 40,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3881, typeof(double),	new UInt64[] { 102 }, 50,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3882, typeof(object),	new UInt64[] { 106 }, 60,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3885, typeof(double),	new UInt64[] { 103 }, 90,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3886, typeof(double),	new UInt64[] { 108 }, 100,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3887, typeof(double),	new UInt64[] { 105 }, 110,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3888, typeof(double),	new UInt64[] { 107 }, 120,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3889, typeof(double),	new UInt64[] { 104 }, 130,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3890, typeof(double),	new UInt64[] { 109 }, 140,	ColumnWidthType.Fixed));
			#endregion
			#region Rows Add
			for(i=1; i<10; ++i) {
				for(j=1; j<10; ++j) {
					for(k=1; k<10; ++k) {
						for(l=1; l<10; ++l) {
							ids     = new UInt64[4];
							ids[0]  = (UInt64)i + 200;
							ids[1]  = (UInt64)j + 300;
							ids[2]  = (UInt64)k + 400;
							ids[3]  = (UInt64)l + 500;
							itemPass= new ViewItemPass(Const.NullId, Const.NullFlags, ids);
							_coll.ItemAdd(itemPass);
						}
					}
				}
			}
			#endregion
			NotifyPropertyChanged("ItemsSource");
			SetTopLeftHeaderStyle("1-4");
		}
		private	void	OnComm4_1	(object par) {
			UInt64[]		ids			= null;
			int				i			= 0;
			ViewTypePass	typePass	= new ViewTypePass();
			ViewItemPass	itemPass	= null;
			typePass.Id				    = 1000;
			typePass.Flags				= 1000;
			typePass.FixedDims			= new ulong[0];
			typePass.FixedIds			= new ulong[0];
			typePass.ColDims			= new ulong[4] {1,2,3,4};
			typePass.RowDims			= new ulong[1] {5};
			typePass.RowHeaderPartWidth	= new double[1] {60};
			_coll	= new ViewItemsSource(_model, typePass);
			#region Columns Add
			_coll.ItemPropAdd(new ViewPropPass(3880, typeof(int),		new UInt64[] { 101, 202, 303, 401 }, 40,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3881, typeof(double),	new UInt64[] { 101, 204, 305, 402 }, 50,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3882, typeof(object),	new UInt64[] { 106, 204, 305, 403 }, 60,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3883, typeof(int),		new UInt64[] { 106, 205, 305, 404 }, 70,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3884, typeof(double),	new UInt64[] { 106, 205, 306, 404 }, 80,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3885, typeof(double),	new UInt64[] { 103, 204, 306, 405 }, 90,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3886, typeof(double),	new UInt64[] { 106, 206, 307, 405 }, 100,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3887, typeof(double),	new UInt64[] { 106, 206, 308, 408 }, 110,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3888, typeof(double),	new UInt64[] { 106, 206, 309, 401 }, 120,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3889, typeof(double),	new UInt64[] { 106, 207, 310, 409 }, 130,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3890, typeof(double),	new UInt64[] { 106, 208, 310, 409 }, 140,	ColumnWidthType.Fixed));
			#endregion
			#region Rows Add
			for(i=1; i<10; ++i) {
				ids     = new UInt64[1];
				ids[0]  = (UInt64)i + 500;
				itemPass= new ViewItemPass(Const.NullId, Const.NullFlags, ids);
				_coll.ItemAdd(itemPass);
			}
			#endregion
			NotifyPropertyChanged("ItemsSource");
			SetTopLeftHeaderStyle("4-1");
		}
		private	void	OnComm3_2	(object par) {
			UInt64[]		ids			= null;
			int				i, j		= 0;
			ViewTypePass	typePass	= new ViewTypePass();
			ViewItemPass	itemPass	= null;
			typePass.Id				    = 1000;
			typePass.Flags				= 1000;
			typePass.FixedDims			= new ulong[0];
			typePass.FixedIds			= new ulong[0];
			typePass.ColDims			= new ulong[3] {1,2,3};
			typePass.RowDims			= new ulong[2] {4,5};
			typePass.RowHeaderPartWidth	= new double[2] {60, 60 };
			_coll	= new ViewItemsSource(_model, typePass);
			#region Columns Add
			_coll.ItemPropAdd(new ViewPropPass(3880, typeof(int),		new UInt64[] { 101, 202, 303 }, 40,		ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3881, typeof(double),	new UInt64[] { 101, 204, 305 }, 50,		ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3882, typeof(object),	new UInt64[] { 106, 204, 305 }, 60,		ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3883, typeof(int),		new UInt64[] { 106, 205, 305 }, 70,		ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3884, typeof(double),	new UInt64[] { 106, 205, 306 }, 80,		ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3885, typeof(double),	new UInt64[] { 103, 204, 306 }, 90,		ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3886, typeof(double),	new UInt64[] { 106, 206, 307 }, 100,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3887, typeof(double),	new UInt64[] { 106, 206, 308 }, 110,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3888, typeof(double),	new UInt64[] { 106, 206, 309 }, 120,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3889, typeof(double),	new UInt64[] { 106, 207, 310 }, 130,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3890, typeof(double),	new UInt64[] { 106, 208, 310 }, 140,	ColumnWidthType.Fixed));
			#endregion
			#region Rows Add
			for(i=1; i<10; ++i) {
				for(j=1; j<10; ++j) {
					ids     = new UInt64[2];
					ids[0]  = (UInt64)i + 400;
					ids[1]  = (UInt64)j + 500;
					itemPass= new ViewItemPass(Const.NullId, Const.NullFlags, ids);
					_coll.ItemAdd(itemPass);
				}
			}
			#endregion
			NotifyPropertyChanged("ItemsSource");
			SetTopLeftHeaderStyle("3-2");
		}
		private	void	OnComm2_3	(object par) {
			UInt64[]		ids			= null;
			int				i, j, k		= 0;
			ViewTypePass	typePass	= new ViewTypePass();
			ViewItemPass	itemPass	= null;
			typePass.Id				    = 1000;
			typePass.Flags				= 1000;
			typePass.FixedDims			= new ulong[0];
			typePass.FixedIds			= new ulong[0];
			typePass.ColDims			= new ulong[2] {1,2};
			typePass.RowDims			= new ulong[3] {3,4,5};
			typePass.RowHeaderPartWidth	= new double[3] {60, 60, 60 };
			_coll	= new ViewItemsSource(_model, typePass);
			#region Columns Add
			_coll.ItemPropAdd(new ViewPropPass(3880, typeof(int),		new UInt64[] { 101, 202 }, 40,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3881, typeof(double),	new UInt64[] { 101, 204 }, 50,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3882, typeof(object),	new UInt64[] { 106, 204 }, 60,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3883, typeof(int),		new UInt64[] { 106, 205 }, 70,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3884, typeof(double),	new UInt64[] { 106, 205 }, 80,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3885, typeof(double),	new UInt64[] { 103, 204 }, 90,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3886, typeof(double),	new UInt64[] { 106, 206 }, 100,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3887, typeof(double),	new UInt64[] { 106, 206 }, 110,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3888, typeof(double),	new UInt64[] { 106, 206 }, 120,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3889, typeof(double),	new UInt64[] { 106, 207 }, 130,	ColumnWidthType.Fixed));
			_coll.ItemPropAdd(new ViewPropPass(3890, typeof(double),	new UInt64[] { 106, 208 }, 140,	ColumnWidthType.Fixed));
			#endregion
			#region Rows Add
			for(i=1; i<10; ++i) {
				for(j=1; j<10; ++j) {
					for(k=1; k<10; ++k) {
						ids     = new UInt64[3];
						ids[0]  = (UInt64)i + 300;
						ids[1]  = (UInt64)j + 400;
						ids[2]  = (UInt64)k + 500;
						itemPass= new ViewItemPass(Const.NullId, Const.NullFlags, ids);
						_coll.ItemAdd(itemPass);
					}
				}
			}
			#endregion
			NotifyPropertyChanged("ItemsSource");
			SetTopLeftHeaderStyle("2-3");
		}
		private	void	OnComm0_1	(object par) {
			UInt64[]		ids			= null;
			ViewTypePass	typePass	= new ViewTypePass();
			ViewItemPass	itemPass	= null;
			typePass.Id					= 1000;
			typePass.Flags				= 1000;
			typePass.FixedDims			= new ulong[0];
			typePass.FixedIds			= new ulong[0];
			typePass.ColDims			= new ulong[0];
			typePass.RowDims			= new ulong[1] {1};
			typePass.RowHeaderPartWidth	= new double[1] {60};
			_coll	= new ViewItemsSource(_model, typePass);
			#region Columns Add
			_coll.ItemPropAdd(new ViewPropPass(3880, typeof(int), new UInt64[] {}, 40, ColumnWidthType.Fixed));
			#endregion
			#region Rows Add
			for(int i=1; i<9; ++i) {
				ids     = new UInt64[1];
				ids[0]  = (UInt64)i + 100;
				itemPass= new ViewItemPass(Const.NullId, Const.NullFlags, ids);
				_coll.ItemAdd(itemPass);
			}
			#endregion
			NotifyPropertyChanged("ItemsSource");
			SetTopLeftHeaderStyle("0-1");
		}
		private	void	OnComm0_2	(object par) {
			UInt64[]		ids			= null;
			ViewTypePass	typePass	= new ViewTypePass();
			ViewItemPass	itemPass	= null;
			typePass.Id					= 1000;
			typePass.Flags				= 1000;
			typePass.FixedDims			= new ulong[0];
			typePass.FixedIds			= new ulong[0];
			typePass.ColDims			= new ulong[0];
			typePass.RowDims			= new ulong[2] {1, 2};
			typePass.RowHeaderPartWidth	= new double[2] {60, 60};
			_coll	= new ViewItemsSource(_model, typePass);
			#region Columns Add
			_coll.ItemPropAdd(new ViewPropPass(3880, typeof(int), new UInt64[] {}, 40, ColumnWidthType.Fixed));
			#endregion
			#region Rows Add
			for(int i=1; i<10; ++i) {
				for(int j=1; j<10; ++j) {
					ids     = new UInt64[2];
					ids[0]  = (UInt64)i+100;
					ids[1]  = (UInt64)j+200;
					itemPass= new ViewItemPass(Const.NullId, Const.NullFlags, ids);
					_coll.ItemAdd(itemPass);
				}
			}
			#endregion
			NotifyPropertyChanged("ItemsSource");
			SetTopLeftHeaderStyle("0-2");
		}
		private	void	SetTopLeftHeaderStyle(string content) {
			_topLeftHeaderStyle	= null;
			if(content != null) {
				_topLeftHeaderStyle	= new Style();
				_topLeftHeaderStyle.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Colors.Orange)));
				_topLeftHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.ContentProperty, content));
			}
			NotifyPropertyChanged("TLHeaderStyle");
		}
	}
}
