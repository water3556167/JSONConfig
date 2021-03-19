using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	public class SocketReceived : Disposable
	{
		/// <summary>
		///   接受的字节
		/// </summary>
		public byte[] buffer;
		/// <summary>
		/// 实际接受
		/// </summary>
		public int Length;
		/// <summary>
		/// 缓冲器长度
		/// </summary>
		public int BufferSize;

		public SocketError socketError;
		/// <summary>
		/// 是否超时
		/// </summary>
		public bool IsTimeOut;

		/// <summary>
		/// 异步回调
		/// </summary>
		public Action<SocketReceived> callback;
		/// <summary>
		/// true=指定了需要接受的数据字节长度，
		/// </summary>
		internal bool fullReceive;
		internal ManualResetEvent timeoutEvt;

		//索引器
		public byte this[int index]
		{
			get
			{

				if (index >= Length || index < 0) throw new IndexOutOfRangeException();
				return buffer[index];
			}
			set
			{
				if (index >= Length || index < 0) throw new IndexOutOfRangeException();
				buffer[index] = value;
			}

		}
		public SocketReceived(byte[]buffers,int len)
		{
			this.buffer = buffers;
			this.Length = len;

		}
	}
}
