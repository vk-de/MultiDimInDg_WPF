using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace VK_De.WPF.NDimInDg {
	[TypeDescriptionProvider(typeof(ViewItemTypeDescriptionProvider))]
	public class ViewItem:Object,INotifyPropertyChanged{
		internal	static	object					_itemTypeDelegatorLock	= new object();
		internal	static	ViewItemTypeDescriptor	_itemTypeDelegatorCurr	= null;
		#region Variables
		private	ViewItemsSource						_coll			= null;
		private	ViewItemPass						_pass			= null;
		#endregion
		#region Events
		public event PropertyChangedEventHandler	PropertyChanged;
		#endregion
		internal	ViewItem(){}
		internal	ViewItem(ViewItemPass pass, ViewItemsSource coll){_pass = pass; _coll = coll;}
		#region Methods Private
		private void NotifyPropertyChanged	(String info){
			if(PropertyChanged != null)	PropertyChanged(this, new PropertyChangedEventArgs(info));
		}
		#endregion
		#region Methods Public
		public	ViewItemPass	Pass			()											{return _pass;}
		#endregion
		internal	object	GetPropertyValue(ViewPropPass propPass)					{return _coll.GetPropertyValue(propPass, _pass);}
        internal	void	SetPropertyValue(ViewPropPass propPass, object value)	{_coll.SetPropertyValue(propPass, _pass, value);}
	}
	internal	sealed	class ViewItemPropertyDescr:PropertyDescriptor{
		#region Variables
		private Type			_componentClass	= null;
        private Type			_type			= null;		// the type of the property
		private ViewPropPass	_passProp		= null;
		#endregion
		#region Constructors
        private ViewItemPropertyDescr(Type componentClass, string name, Type type, Attribute[] attributes):base(name, attributes){
			if(type == null)			throw new ArgumentException("InvalidNullArgument " + name + " Type");
            if(componentClass == null)	throw new ArgumentException("InvalidNullArgument " + name + " ComponentClass");
            _type			= type;
            _componentClass	= componentClass;
        }
		public	ViewItemPropertyDescr(Type componentClass, ViewPropPass pass, Attribute[] attributes):this(componentClass, pass.Name, pass.Type, attributes){_passProp = pass;}
		#endregion
		internal ViewPropPass		Pass	{get{return _passProp;}}
		#region Public Override Properties
		public override AttributeCollection Attributes			{
			get{List<Attribute>	list	= new List<Attribute>();
				FillAttributes(list);
				return new AttributeCollection(list.ToArray());
			}
		}
		public override string			Category			{get{return _passProp.Category;}}
		public override Type			ComponentType		{get{return _componentClass;}}
		public override TypeConverter	Converter			{get{return base.Converter;}}
		public override string			Description			{get{return _passProp.Description;}}
		public override bool			DesignTimeOnly		{get{return false;}}
		public override string			DisplayName			{get{return _passProp.DisplayName;}}
		public override bool			IsBrowsable			{get{return _passProp.IsBrowsable;}}
		public override bool			IsLocalizable		{get{return false;}}
        public override bool			IsReadOnly			{get{return _passProp.IsReadOnly;}}
		public override string			Name				{get{return _passProp.Name;}}
        public override Type			PropertyType		{get{return _type;}}
		public override bool			SupportsChangeEvents{get{return true;}}
		public			ViewPropPass	PassProp			{get{return _passProp;}}
		#endregion
		#region Protected Override Functions
		protected override void FillAttributes(IList attributes)				{
            foreach (Attribute typeAttr in TypeDescriptor.GetAttributes(PropertyType))	attributes.Add(typeAttr);
            base.FillAttributes(attributes);
        }
		#endregion
		#region Public Override Functions
		public override bool	CanResetValue		(object component)						{
			bool	ret		= false;
			object	value	= GetValue(component);
            ret	= !object.Equals(_passProp.DefaultValue, value);
            return ret;
        }
        public override object	GetValue			(object component)						{
			object		ret		= null;
			ViewItem	item	= component as ViewItem;
            if(item != null) ret	= item.GetPropertyValue(_passProp);
            return ret;
        }
        public override void	ResetValue			(object component)						{SetValue(component, _passProp.DefaultValue);}
        public override void	SetValue			(object component, object value)		{
			ViewItem	item	= component as ViewItem;
			if(item != null){
				item.SetPropertyValue(_passProp, value);
				OnValueChanged(item, new EventArgs());
			}
        }
        public override bool	ShouldSerializeValue(object component)						{
			bool	ret		= false;
			object	value	= GetValue(component);
            ret	= !object.Equals(_passProp.DefaultValue, value);
			return ret;
        }
		#endregion     
    }
	internal		class ViewItemTypeDescriptor : CustomTypeDescriptor{
		private	Type							_type		= null;
		private	List<ViewItemPropertyDescr>		_propsDynam	= null;
        public ViewItemTypeDescriptor(Type type) : base(){
			_type		= type;
			_propsDynam	= new List<ViewItemPropertyDescr>();
        }
		#region Methods Internal
		internal void					PropertyAdd			(ViewPropPass p){
			if(p != null){
				_propsDynam.Add(new ViewItemPropertyDescr(_type, p, null));
			}
		}
		internal void					PropertyDelete		(ViewPropPass p){
			ViewItemPropertyDescr	toDel	= null;
			if(p != null){
				foreach(ViewItemPropertyDescr pd in _propsDynam) {
					if(pd.Pass == p) {
						toDel	= pd;
						break;
					}
				}
				if(toDel != null) _propsDynam.Remove(toDel);
			}
		}
		internal PropertyDescriptor		GetProperty			(string name)			{
			PropertyDescriptor	ret	= null;
			foreach(PropertyDescriptor pi in _propsDynam){
				if(pi.Name == name){
					ret	= pi;
					break;
				}
			}
			return ret;
		}
		#endregion
		public override PropertyDescriptorCollection GetProperties(){return GetProperties(null);}
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes){
			PropertyDescriptor[]			propsAsArray	= _propsDynam.ToArray();
			PropertyDescriptorCollection	properties		= new PropertyDescriptorCollection(propsAsArray);
			return properties;
        }
    }
	internal	class ViewItemTypeDescriptionProvider:TypeDescriptionProvider{
        private static TypeDescriptionProvider defaultTypeProvider = TypeDescriptor.GetProvider(typeof(ViewItem));
        public ViewItemTypeDescriptionProvider() : base(defaultTypeProvider){}
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance){
			ICustomTypeDescriptor	ret		= ViewItem._itemTypeDelegatorCurr;
			if(ret == null)	ret		= base.GetTypeDescriptor(objectType, instance);
			return ret;
        }
	}
}