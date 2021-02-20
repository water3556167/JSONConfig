using NRails.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NRails.Util;

namespace TestJSON
{
	/// <summary>
	/// 提供一个参数存储器类型变换支持
	/// </summary>
	public abstract class ParameterSetting : Disposable
	{
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{

			}
			base.Dispose(disposing);
		}
		#region Declarea
		static ConfigProvider _defaultProvider = AppConfigProvider.instanse;
		/// <summary>
		/// 默认配置项
		/// </summary>
		public static ConfigProvider DefaultConfigProvider
		{
			get { return _defaultProvider; }
			set
			{
				if (value != null)
					_defaultProvider = value;
				else
					_defaultProvider = AppConfigProvider.instanse;
			}

		}

		Dictionary<ConfigProvider, ConfigProvider> providers;
		object syncroot = new object();
		ObjectField[] fields;
		[Ignore]
		internal bool AutoReconverConfig = true;
		Dictionary<ConfigProvider, List<ObjectField>> configFrom = new Dictionary<ConfigProvider, List<ObjectField>>();
		#endregion

		#region  构造函数
		public ParameterSetting()
		{
			var thistype = this.GetType();
			var typen = thistype.GetTypeInfoN();
			this.fields = typen.GetFields(FieldFlag.Instance | FieldFlag.Static | FieldFlag.Public).Where(t => t.CanRead && t.CanWrite && t.GetAttribute<IgnoreAttribute>() == null && t.Type != thistype).ToArray();
		}
		public ParameterSetting(params ConfigProvider[]providers):this()
		{
			this.providers = providers.ToDictionary(t=>t,ToDictionaryRepeatOpt.Replace);
			Initialize();
		}
		/// <summary>
		/// 初始化参数
		/// </summary>
		/// <returns></returns>
		public virtual ParameterSetting Initialize(params ConfigProvider[]providers)
		{

			return this;
		}
		#endregion 
	}
}
