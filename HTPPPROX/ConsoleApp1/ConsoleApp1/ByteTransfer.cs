using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bytes = System.ArraySegment<byte>;
namespace ConsoleApp1
{
	/// <summary>
	/// 抽象类
	/// </summary>
	public abstract class ByteTransfer:Disposable
	{
		public abstract void SendAsync(IEnumerable<bytes>datas,Action<AsyncResult>callback);
		public abstract void ReceiveAsync(Action<AsyncResult<SocketReceived>>callback);

		#region Send重载
		public void SendAsync(bytes data, Action<AsyncResult> callback) => SendAsync(new bytes[] {data},callback);
		public void SendAsync(byte[] data, Action<AsyncResult> callback) => SendAsync(new bytes[] { new bytes(data)},callback);
		public void SendAsync(byte[] data, int offset, int len, Action<AsyncResult> callback) =>SendAsync(new bytes[] { new bytes(data, offset, len) }, callback);
		public void Send(IEnumerable<bytes> datas)
		{
			NRails.Util.SyncH.InvokeSync(callback=>SendAsync(datas,callback));
		}

		public void Send(byte[] buffer,int offset,int len) => Send(new bytes[] { new bytes(buffer,offset,len)});
		public void Send(byte[] buffer) => Send(buffer,0,buffer.Length);

		public Task SendAsync(IEnumerable<bytes> datas)
		{
			return NRails.Util.SyncH.InvokeTask(callback=>SendAsync(datas,callback));
		}
		#endregion

		#region RecvieAsync重载

		#endregion
		#region
		public Task<SocketReceived> ReceiveAsync()
		{
			return NRails.Util.SyncH.InvokeTask<SocketReceived>(callback=>ReceiveAsync());

		}
		#endregion
	}
}
