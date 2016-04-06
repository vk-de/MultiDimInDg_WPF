using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace VK_De.WPF.NDimInDg {
	public	interface IViewColl{
		UInt64				Id				{get;}
		string				Name			{get; set;}
		Type				ItemType		{get;}
		ViewTypePass		ItemTypePass	{get;}
		bool				ItemPropAddCan		(ViewPropPass p);
		void				ItemPropAdd			(ViewPropPass p);
		bool				ItemPropDeleteCan	(ViewPropPass p);
		void				ItemPropDelete		(ViewPropPass p);
		PropertyDescriptor	ItemPropGet			(string p);
		bool				ItemAddCan			(ViewItemPass p);
		void				ItemAdd				(ViewItemPass p);
		bool				ItemDeleteCan		(ViewItemPass p);
		void				ItemDelete			(ViewItemPass p);
		string				TitleOf				(UInt64 id);
	}
}
