using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NRails.Core;
using NRails.Util;
namespace ConsoleApp1
{

	public enum SEX
	{

		男 = 0,
		女 =1
	}
	/// <summary>
	/// 代理
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			//var boolid=Enum.IsDefined(typeof(SEX),1);
			var a = 1L<<1;
			var b = 1L << 13;
			var c = a& b;
			Console.ReadKey();
		}



		//测试反射方法


		//var curid = max

		///// <summary>
		/////入库操作
		///// </summary>
		///// <param name="datas"></param>
		///// <param name="id"></param>
		///// <param name="isdel"></param>
		//void InsertDB(List<ResData> datas)
		//{

		//	var rdb = new NRails.Data.Rdb.RdbSystem(new NRails.Data.Rdb.Adapters.MySqlAdapter(""));

		//	using (var trans = rdb.BeginTransaction())
		//	{
		//		var delsql = "";
		//		if (isdel) { delsql = "delete from record where "; }
		//		if (delsql != "")
		//			trans.ExecuteNonQuery(delsql);
		//		trans.Insert<ResData>(datas);
		//		try
		//		{
		//			trans.Commit();
		//			var maxid = id.Max();
		//		}
		//		catch (Exception ex)
		//		{
		//			trans.Rollback();
		//		}

		//	}



		//}
		//void Process(List<ResData> resDatas)
		//{

		//	//队列数据
		//	resDatas.SortBy(a => a.CarID);
		//	var listdatas = new List<string>();
		//	var listcarid = new List<long>();
		//	foreach (var item in resDatas)
		//	{

		//		var leftnums = 200 - listdatas.Count;
		//		if (item.Datas.Count < leftnums)
		//		{
		//			//控件有足够
		//			listcarid.Add(item.CarID);
		//			listdatas.AddRange(item.Datas);
		//		}
		//		else
		//		{
		//			//执行提交
		//			//大于200就分段isfirst 就删除,ise
		//			foreach (var splititem in item.Datas.Split(200))
		//			{


		//			}
		//		}

		//	}



		//}


		//foreach (var s in item.Datas)
		//{
		//	if (queen.Count < 200)
		//	{
		//		queen.Enqueue(s);
		//	}


		//}



	}







}
