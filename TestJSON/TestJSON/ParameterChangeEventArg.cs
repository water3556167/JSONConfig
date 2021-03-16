using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NRails.Serialization;
using NRails.Reflection;

namespace TestJSON
{
	/// <summary>
	/// 配置变化参数
	/// </summary>
	public class ParameterChangeEventArg
	{
		public ObjectField Field;
		public object NewValue;
		public object OldValue;

	}
}
