using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	public class HttpRequestTest
	{

		public Task GetAsync(HttpRequestParm requestParm)
		{
			var tsk = new TaskCompletionSource<HttpResponseData>();
			DoHttpRequestAsync(requestParm, ret =>
			{
				
			});
			return tsk.Task;
		}
		public Task<HttpResponseData> DoHttpRequestAsync(HttpRequestParm requestParm, Action<HttpAsyncState> CallBack)
		{
			return null;
			System.Attribute.GetCustomAttributes(typeof(HttpRequestTest));
	       
		}

	}
	public class HttpRequestParm { }
	public class HttpAsyncState : IDisposable
	{
		public bool Success;
		public HttpResponseData responseData;
		public Exception exception;
		internal Stream stream;
		internal TcpClient client;
		ManualResetEvent evt;
		internal Action<HttpAsyncState> callback;
		public void Dispose()
		{
			if (evt != null)
			{
				evt.Dispose();
				evt = null;
			}

		}
		public void InvockeCallback()
		{
			callback?.Invoke(this);
			evt.Set();
		}
		public void Wait()
		{

			try
			{
				if (evt != null)
					evt.WaitOne();
			}
			catch (Exception ex)
			{
				ex.WriteKnownException();
			}
		}
		internal void CloseConnection()
		{
			try
			{
				if (stream != null)
					stream.Dispose();
			}
			catch (Exception ex)
			{
				ex.WriteKnownException();

			}
			try
			{
				if (client != null)
				{
					client.Close();
					((IDisposable)client).Dispose();
				}

			}
			catch (Exception ex)
			{
				ex.WriteKnownException();
			}
			stream = null;
			client = null;
		}
	}
	public class HttpResponseData
	{

	}
}
