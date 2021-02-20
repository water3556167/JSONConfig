using NRails.Reflection;
using NRails.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestJSON
{
	/// <summary>
	/// 配置文件抽象类
	/// </summary>
	public abstract class ConfigProvider : IDisposable
	{
		#region IDisposable
		/// <summary>
		/// 在调用Dispose是会被设置为1，在调用析构函数时会被设置为2
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		protected int _IsDisposed;
		/// <summary>
		/// 
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//在这里编写释放前需要执行的业务逻辑
			}
			NRails.Util.Ash.Dispose(this, disposing);
		}
		/// <summary>
		/// 
		/// </summary>
		~ConfigProvider()
		{
			if (System.Threading.Interlocked.CompareExchange(ref _IsDisposed, 2, 0) != 0) return;
			Dispose(false);
		}
		/// <summary>
		/// 释放所占用的资源
		/// </summary>
		public void Dispose()
		{
			if (System.Threading.Interlocked.CompareExchange(ref _IsDisposed, 1, 0) != 0) return;
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// 如果此对象已释放则抛出异常
		/// </summary>
		protected void CheckDisposed()
		{
			if (_IsDisposed != 0) throw new ObjectDisposedException(this.GetType().Name);
		}
		/// <summary>
		/// 获取该对象是否已经被释放
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return System.Threading.Interlocked.CompareExchange(ref _IsDisposed, 0, 0) != 0;
			}
		}
		#endregion
		/// <summary>
		/// 保存资源
		/// </summary>
		public abstract void SaveToSource(ParameterSetting setting, ObjectField[] fields);
		/// <summary>
		/// 读取参数值
		/// </summary>
		/// <param name="name"></param>
		/// <param name="field"></param>
		/// <param name="defVal"></param>
		/// <returns></returns>
		public abstract object GetParameter(string name, ObjectField field, object defVal);

		#region 事件
		/// <summary>
		/// 配置文件改变事件
		/// </summary>
		Action<ConfigProvider> _ConfigChanged;
		public event Action<ConfigProvider> ConfigChanged
		{

			add
			{
				_ConfigChanged += value;
			}
			remove
			{
				_ConfigChanged -= value;
			}

		}
		public virtual void OnConfigChange()
		{
			_ConfigChanged.Invoke(this);
		}
		/// <summary>
		/// 配置文件丢失事件
		/// </summary>
		Action<ConfigProvider> _ConfigFileLost;
		public event Action<ConfigProvider> ConfigFileLost
		{
			add
			{
				_ConfigFileLost += value;
			}
			remove
			{
				_ConfigFileLost -= value;
			}
		}
		public virtual void OnConfigFileLost()
		{
			_ConfigFileLost.Invoke(this);
		}
		#endregion

		#region 读取文件和写文件
		static Dictionary<string, string> configtxt = new Dictionary<string, string>();
		/// <summary>
		///  读取配置文件
		/// </summary>
		/// <param name="file"></param>
		/// <param name="force"></param>
		/// <returns></returns>
		public static string ReadConfigTxt(string file, bool force)
		{
			lock (configtxt)
			{
				var uptxt = file.ToUpper().Replace("\\", "/");
				if (force || !configtxt.ContainsKey(uptxt))
				{
					var text = FSH.ReadAllText(uptxt);
					configtxt[uptxt] = text;
					return text;
				}
				return configtxt[uptxt];
			}
		}
		/// <summary>
		/// 写配置文件
		/// </summary>
		/// <param name="file"></param>
		/// <param name="text"></param>
		public static void SaveConfigTxt(string file, string text)
		{
			lock (configtxt)
			{
				var upptxt = file.ToUpper().Replace("\\", "/");
				configtxt[upptxt] = text;
				FSH.WriteAllText(file, text, Encoding.UTF8);
			}
		}
		#endregion
	}
}
