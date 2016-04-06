using System;

using VK_De.NDimInDg;

namespace VK_De.WPF.NDimInDg {
	public enum ColumnWidthType:int {Auto, SizeToCells, SizeToHeader, Fixed}
	public	class ViewTypePass{
		#region Variables
		private	UInt64				_id					= Const.NullId;
		private	UInt64				_flags				= Const.NullFlags;
		private	UInt64[]			_fixedDims			= null;
		private	UInt64[]			_colDims			= null;
		private	UInt64[]			_rowDims			= null;
		private	UInt64[]			_fixedIds			= null;
		private	double[]			_rowHeaderPartWidth	= null;
		#endregion
		public	ViewTypePass(){}
		#region Properties
		public	virtual	UInt64				Id					{get{return _id;}					set{_id					= value;}}
		public	virtual	UInt64				Flags				{get{return _flags;}				set{_flags				= value;}}
		public	virtual	UInt64[]			FixedDims			{get{return _fixedDims;}			set{_fixedDims			= value;}}
		public	virtual	UInt64[]			ColDims				{get{return _colDims;}				set{_colDims			= value;}}
		public	virtual	UInt64[]			RowDims				{get{return _rowDims;}				set{_rowDims			= value;}}
		public	virtual	UInt64[]			FixedIds			{get{return _fixedIds;}				set{_fixedIds			= value;}}
		public	virtual	double[]			RowHeaderPartWidth	{get{return _rowHeaderPartWidth;}	set{_rowHeaderPartWidth	= value;}}
		#endregion
	}
	public	class ViewItemPass{
		#region Variables
		private	UInt64		_id				= Const.NullId;
		private	UInt64		_flags			= Const.NullFlags;
		private	UInt64[]	_idsItem		= null;
		#endregion
		public	ViewItemPass(){}
		public	ViewItemPass(UInt64 id, UInt64 flags, UInt64[] idsItem):this(){
			_id			= id;
			_flags		= flags;
			_idsItem	= idsItem;
		}
		#region Properties
		public	virtual	UInt64		Id				{get{return _id;}}
		public	virtual	UInt64		Flags			{get{return _flags;}}
		public	virtual	UInt64[]	IdsItem			{get{return _idsItem;}}
		public	virtual	UInt64[]	IdsItemTitle	{get{return _idsItem;}}
		#endregion
	}
	public	class ViewPropPass{
		#region Variables
		private	UInt64			_id				= Const.NullId;
		private	UInt64			_flags			= Const.NullFlags;
		private	UInt64[]		_idsProp		= null;
		private	bool			_readOnly		= false;
		private	bool			_browsable		= true;
		private	string			_category		= "Common";
		private	string			_description	= null;
		private	string			_displayName	= null;
		private	Type			_type			= null;
		private	object			_defaultValue	= null;
		private	int				_colWidth		= 0;
		private	ColumnWidthType	_colWidthType	= ColumnWidthType.Auto;
		#endregion
		public	ViewPropPass(){}
		public	ViewPropPass(UInt64 id, Type type, UInt64[] idsProp, int width, ColumnWidthType widthType):this(){
			_id				= id;
			_type			= type;
			_idsProp		= idsProp;
			_colWidth		= (width < 0)?0:width;
			_colWidthType	= widthType;
		}
		#region Properties
		public	virtual	bool			IsReadOnly		{get{return _readOnly;}				set{_readOnly		= value;}}
		public	virtual	bool			IsBrowsable		{get{return _browsable;}			set{_browsable		= value;}}
		public	virtual	UInt64			Id				{get{return _id;}}
		public	virtual	UInt64			Flags			{get{return _flags;}				set{_flags			= value;}}
		public	virtual	UInt64[]		IdsProp			{get{return _idsProp;}}
		public	virtual	string			Name			{get{return "n_"+_id.ToString();}}
		public	virtual	string			Category		{get{return _category;}				set{_category		= value;}}
		public	virtual	string			Description		{get{return _description;}			set{_description	= value;}}
		public	virtual	string			DisplayName		{get{return _displayName;}			set{_displayName	= value;}}
		public	virtual	Type			Type			{get{return _type;}}
		public	virtual	object			DefaultValue	{get{return _defaultValue;}			set{_defaultValue	= value;}}
		public	virtual	int				ColWidth		{get{return _colWidth;}				set{_colWidth		= (value < 0)?0:value;}}
		public	virtual	ColumnWidthType	ColWidthType	{get{return _colWidthType;}}
		#endregion
	}
}
