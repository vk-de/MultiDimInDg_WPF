using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using VK_De.NDimInDg;

namespace VK_De.WPF.NDimInDg {
	public class ViewItemsSource:ObservableCollection<ViewItem>, IViewColl{
		#region Variables
		private	string					_name				= null;
		private	IModel					_model				= null;
		private	ViewTypePass			_pass				= null;
		private	ViewItemTypeDescriptor	_itemTypeDelegator	= null;
		#endregion
		public	ViewItemsSource(IModel model, ViewTypePass pass):base(){
			_model				= model;
			_pass				= pass;
			_itemTypeDelegator	= new ViewItemTypeDescriptor(typeof(ViewItem));
		}
		private	UInt64[]	IdsJoin(ViewPropPass propPass, ViewItemPass itemPass) {
			UInt64[]		ret		= null;
			List<UInt64>	ids	= new List<ulong>();
			if(_pass.FixedIds != null) {
				foreach(UInt64 id in _pass.FixedIds)	ids.Add(id);
			}
			if(propPass != null && propPass.IdsProp != null) {
				foreach(UInt64 id in propPass.IdsProp)	ids.Add(id);
			}
			if(itemPass != null && itemPass.IdsItem != null) {
				foreach(UInt64 id in itemPass.IdsItem)	ids.Add(id);
			}
			ret	= ids.ToArray();
			return ret;
		}
		internal	ViewItemTypeDescriptor	ItemTypeDelegator	{get{return _itemTypeDelegator;}}
		#region IViewColl
		public	UInt64				Id				{get{return _pass.Id;}}
		public	string				Name			{get{return _name;}	set{_name = value;}}
		public	Type				ItemType		{get{return typeof(ViewItem);}}
		public	ViewTypePass		ItemTypePass	{get{return _pass;}}
		public	bool				ItemPropAddCan		(ViewPropPass p)	{return true;}
		public	void				ItemPropAdd			(ViewPropPass p)	{_itemTypeDelegator.PropertyAdd(p);}
		public	bool				ItemPropDeleteCan	(ViewPropPass p)	{return true;}
		public	void				ItemPropDelete		(ViewPropPass p)	{_itemTypeDelegator.PropertyDelete(p);}
		public	PropertyDescriptor	ItemPropGet			(string name)			{return _itemTypeDelegator.GetProperty(name);}
		public	bool				ItemAddCan			(ViewItemPass p)	{return true;}
		public	void				ItemAdd				(ViewItemPass p)	{Add(new ViewItem(p, this));}
		public	bool				ItemDeleteCan		(ViewItemPass p)	{return true;}
		public	void				ItemDelete			(ViewItemPass p)	{
			foreach(ViewItem vi in this) {
				if(vi.Pass() == p){
					Remove(vi);
					break;
				}
			}
		}
		public	string				TitleOf				(UInt64 id)			{
			string	ret	= null;
			if(		_model != null)	ret	= _model.TitleOf(id);
			return ret;
		}
		#endregion
		public	object	GetPropertyValue(ViewPropPass propPass, ViewItemPass itemPass){
			object	ret	= null;
			if(_model != null)	ret	= _model.Fact(IdsJoin(propPass, itemPass));
			return ret;
		}
		public	void	SetPropertyValue(ViewPropPass propPass, ViewItemPass itemPass, object value){}
	}
}