using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	public interface ISocketSession:IDisposable
	{
		/// <summary>
		/// 异步断开
		/// </summary>
		void DisConnectAsync();
	}
}
