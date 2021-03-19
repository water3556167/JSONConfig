using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	public class AsyncResult<T> : IDisposable
	{
		public void Dispose()
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// 错误代码
		/// </summary>
		public int Code = -1;
		/// <summary>
		/// 返回的对象
		/// </summary>
		public T Result;
		/// <summary>
		///提交正常
		/// </summary>
		public bool Success;
		/// <summary>
		/// 异常信息
		/// </summary>
		[JsonIgnore]
		public T Data
		{
			get => Result;
			set => Result = value;
		}
	}
}
