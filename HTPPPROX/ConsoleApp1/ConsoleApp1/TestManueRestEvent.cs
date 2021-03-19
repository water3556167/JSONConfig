using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	public class TestManueRestEvent
	{
		/// <summary>
		/// 把一个异步执行的方法转换为同步执行
		/// </summary>
		/// <param name="Method"></param>
		/// <param name="TimeOut"></param>
		public static object TestManuEvent(ActionMethod Method, int TimeOut = 60000)
		{
			AsyncResult Result = null;
			int SetFlag;
			var EvtManual = ManualResetEventPool.GetOne(out SetFlag);
			bool evtbool = false;
			Method(ret =>
			{
				Result = ret;
				try
				{
					if (!evtbool) EvtManual.Set(SetFlag);
				}
				catch (Exception ex)
				{
					ex.WriteKnownException();
				}

			});
			EvtManual.WaitOne(TimeOut);
			evtbool = true;
			ManualResetEventPool.PutOne(EvtManual);
			EvtManual = null;
			if (Result == null) throw new System.TimeoutException("超时了");
			if (!Result.Success) throw Result.Exception.CreateNew();
			return Result;
		}

		public static T TestManuEvent<T>(ActionMethodT<T> action, int TimeOut = 60000)
		{
			AsyncResult<T> res = null;
			int sign = 0;
			bool evtbool = false;
			var evtManu = ManualResetEventPool.GetOne(out sign);
			action(ret =>
			{
				res = ret;
				try
				{
					if (!evtbool)
					{
						evtManu.Set(sign);
					}
				}
				catch (Exception ex)
				{
					ex.WriteKnownException();
				}
			});
			evtManu.WaitOne(TimeOut);
			evtbool = true;
			ManualResetEventPool.PutOne(evtManu);
			evtManu = null;
			if (res == null) throw new TimeoutException().CreateNew();
			if (!res.Success) throw new Exception().CreateNew();
			return res.Result;
		}
		/// <summary>
		/// 异步委托
		/// </summary>
		/// <param name="action"></param>
		public delegate void ActionMethod(Action<AsyncResult> action);

		public delegate void ActionMethodT<T>(Action<AsyncResult<T>> action);
	}
}
