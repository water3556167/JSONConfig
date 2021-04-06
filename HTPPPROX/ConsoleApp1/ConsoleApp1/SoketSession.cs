using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	/// <summary>
	/// 一个SOcket连接对象
	/// </summary>
	public class SoketSession : ByteTransfer, IDisposable
	{
		public override void ReceiveAsync(Action<AsyncResult<SocketReceived>> callback)
		{
			throw new NotImplementedException();
		}

		public override void SendAsync(IEnumerable<ArraySegment<byte>> datas, Action<AsyncResult> callback)
		{
			throw new NotImplementedException();
		}
	}
}
