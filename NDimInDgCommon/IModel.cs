using System;

namespace VK_De.NDimInDg {
	public	interface IModel{
		bool		DimensionAdd	(IDimension dim);
		IDimension	DimensionGet	(int dimId);
		bool		DimensionDel	(int dimId);
		Object		Fact			(UInt64[] itemIds);
		string		TitleOf			(UInt64 itemId);
	}
	public	interface IDimension{
		int			DimId		{get;}
		String		DimName		{get;}
		bool		DimItemAdd	(IDimItem item);
		IDimItem	DimItemGet	(UInt64 itemId);
		bool		DimItemDel	(UInt64 itemId);
	}
	public	interface IDimItem{
		int		DimId		{get;}
		UInt64	DimItemId	{get;}
		String	DimItemName	{get;}
	}
}
