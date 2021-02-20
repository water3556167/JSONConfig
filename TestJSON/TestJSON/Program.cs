using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NRails.Util;
using Newtonsoft.Json.Linq;
using NRails.Serialization;
namespace TestJSON
{
	class Program
	{
		SummaryJson summary = new SummaryJson();
		static void Main(string[] args)
		{

		}
		//把字符串转化成JSON对象
		void ParseJson(string JsonStr)
		{
			var JsonObjs = (JToken)SerH.JsonDeserialize(JsonStr);
			foreach (var itemfiled in JsonObjs)
			{
				switch (itemfiled)
				{
					case JProperty jp:
					if (summary.ContainsKey(jp.Name))
					{
						var jold = summary[jp.Name];
						if (jold.Type == null)
							jold.Value = jp.Value;
						else
						{
							if (jold.Type.TargetType != typeof(JProperty))
							{

								try
								{
									jold.Value = Oct.ChangeType(jp.Value, jold.Type.TargetType);

								}
								catch { }
							}
							else jold.Value = jp.Value;
						}

					}
					else
					{
						summary[jp.Name] = new JsonItem()
						{
							Name = jp.Name,
							Type = jp.GetType().GetTypeInfoN(),
							Value = jp.Value

						};
					}
            				break;

				}

			}

		}

	}
}
