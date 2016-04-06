using System;
using System.Collections.Generic;
using VK_De.NDimInDg;

namespace VK_De.WPF.NDimInDg {
	public class Model:IModel{
		public		const	int					DimItemMax	= 100;
		internal	static	String				ItemIdLocal	(UInt64 itemId)	{return ((Int64)itemId-DimId(itemId)*DimItemMax).ToString();}
		internal	static	int					DimId		(UInt64 itemId)	{return (int)(itemId/DimItemMax);}
		private		Dictionary<int, IDimension>	_dimensions	= null;
		public	Model(){_dimensions	= new Dictionary<int, IDimension>();}
		public	bool		DimensionAdd	(IDimension dim) {
			bool	ret	= false;
			if(dim != null){
				if(!_dimensions.ContainsKey(dim.DimId)) {
					_dimensions.Add(dim.DimId, dim);
					ret	= true;
				}	
			}
			return ret;
		}
		public	IDimension	DimensionGet	(int dimId) {
			IDimension	ret	= null;
			_dimensions.TryGetValue(dimId, out ret);
			return ret;
		}
		public	bool		DimensionDel	(int dimId) {throw new NotImplementedException();}
		public	Object		Fact			(UInt64[] itemIds) {
			string		ret			= null;
			UInt64[]	itemIdsLocal= null;
			int			i			= 0;
			int[]		dimIds		= null;
			if(itemIds != null) {
				if(itemIds.Length > 0){
					itemIdsLocal= (UInt64[])itemIds.Clone();
					dimIds	= new int[itemIdsLocal.Length];
					for(i=0; i<itemIdsLocal.Length; ++i)	dimIds[i]	= DimId(itemIdsLocal[i]);
					Array.Sort(dimIds, itemIdsLocal);
					ret	= ItemIdLocal(itemIdsLocal[0]);
					for(i=1; i<itemIdsLocal.Length; ++i)	ret	= ret + "_" + ItemIdLocal(itemIdsLocal[i]);
				}else ret	= "Const";
			}
			return ret;
		}
		public	string		TitleOf			(UInt64 itemId) {
			string		ret		= null;
			int			dimId	= DimId(itemId);
			IDimension	dim		= null;
			IDimItem	item	= null;
			if(_dimensions.TryGetValue(dimId, out dim)) {
				item	= dim.DimItemGet(itemId);
				if(item != null)	ret	= item.DimItemName;
			}
			return ret;
		}
	}
	public class Dimension:IDimension {
		private	Dictionary<UInt64, IDimItem>	_dimItems	= null;	
		public	Dimension(int id) {
			DimId		= id;
			_dimItems	= new Dictionary<UInt64, IDimItem>();
		}
		public	int			DimId		{get;	private set;}
		public	String		DimName		{get {return "Dimension" + DimId.ToString();}}
		public	bool		DimItemAdd	(IDimItem item) {
			bool	ret		= false;
			int		idLocal	= 0;
			UInt64	idFull	= 0L;
			if(item != null){
				idLocal	= (int)(item.DimItemId - (item.DimItemId/Model.DimItemMax)*Model.DimItemMax);
				idFull	= (UInt64)(idLocal + DimId*Model.DimItemMax);
				if(idFull == item.DimItemId) {
					if(!_dimItems.ContainsKey(item.DimItemId)) {
						_dimItems.Add(item.DimItemId, item);
						ret	= true;
					}
				}
			}
			return ret;
		}
		public	IDimItem	DimItemGet	(UInt64 itemId) {
			IDimItem	ret	= null;
			_dimItems.TryGetValue(itemId, out ret);
			return ret;
		}
		public	bool		DimItemDel	(UInt64 itemId) {throw new NotImplementedException();}
	}
	public class DimItem:IDimItem{
		public	DimItem(UInt64 id)	{DimItemId	= id;}
		public	int		DimId		{get{return Model.DimId(DimItemId);}}
		public	UInt64	DimItemId	{get;	private set;}
		public	String	DimItemName	{get{return "Title" + DimId.ToString() + "_" + Model.ItemIdLocal(DimItemId);}}
	}
}
