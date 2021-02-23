using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NRails.Reflection;
using NRails.Serialization;
using Newtonsoft.Json.Linq;
using NRails.Util;
using System.IO;
using System.Threading;
namespace TestJSON
{
	public class JsonConifgProvider : ConfigProvider
	{
		string jsonfile = "";
		string jsonfileUpper = "";
		JToken root;
		SummaryJson sjon = new SummaryJson();
		Dictionary<ConfigProvider, ConfigProvider> recoverProviders = new Dictionary<ConfigProvider, ConfigProvider>();
		public static readonly JsonConifgProvider instance = new JsonConifgProvider(null);
		public JsonConifgProvider()
		{

		}
		public JsonConifgProvider(string jsonfile)
		{
			if (jsonfile == null)
			{
				var exefile = EnvH.EntryPath;
				var file1 = exefile + ".json";
				var file2 = Path.GetDirectoryName(exefile) + "\\AppConifg.jsong";
				if (File.Exists(file1)) jsonfile = file1;
				else if (File.Exists(file2)) jsonfile = file2;
			}
			this.jsonfile = jsonfile;
			this.jsonfileUpper = jsonfile.ToUpper();
			Initilize(jsonfile);
		}
		void Initilize(string file)
		{
			try
			{
				if (File.Exists(file))
				{
					var jsonstr = ReadConfigTxt(file, false);
					var jsonobj = (JToken)SerH.JsonDeserialize(jsonstr);
					foreach (var filed in jsonobj)
					{
						switch (filed)
						{
							case JProperty jp:
							{
								if (sjon.ContainsKey(jp.Name))
								{
									var jold = sjon[jp.Name];
									if (jold.Type == null) jold.Value = jp.Value;
									else
									{
										if (jold.Type.TargetType != jp.GetType())
										{
											jold.Value = Oct.ChangeType(jp.Value, jold.Type.TargetType);
										}
										else
										{
											jold.Value = jp.Value;
										}
									}

								}
								else
								{
									sjon[jp.Name] = new JsonItem()
									{
										Name = jp.Name,
										Value = jp.Value,
										Type = jp.GetType().GetTypeInfoN(),
										Desc = jp.GetType().GetXmlDescription()


									};

								}

							}

							break;


						}

					}

				}
			}
			catch (Exception ex)
			{

			}
		}
		public void AddRecoverProviders(params ConfigProvider[] providers)
		{
			foreach (var conifg in providers)
			{
				recoverProviders[conifg] = conifg;
			}
		}
		public override object GetParameter(string name, ObjectField field, object defVal)
		{
			if (sjon.ContainsKey(name))
			{
				var jitem = sjon[name];
				if (jitem != null)
				{
					if (jitem.Value is JToken)
					{
						return ((JToken)jitem.Value).ToObject(field.Type);

					}
					else return jitem.Value;

				}
			}
			var token = this.root.GetValue(name);
			if (token != null) return token.ToObject(field.Type);
			//尝试从已有的配置文件进行恢复
			foreach (var pro in this.recoverProviders.Values)
			{
				var val = pro.GetParameter(name, field, defVal);
				if (val != null) return val;
			}
			return null;
		}

		public override void SaveToSource(ParameterSetting setting, ObjectField[] fields)
		{
			sjon.UpdateFieldItems(setting,fields);
			var jsontxt = sjon.Serialize();
			SaveConfigTxt(jsonfile,jsontxt);
		}
		#region 文件监视器
		int watchflag = 0;
		int changecount = 0;
		private void InitFileSystemWatcher()
		{
			if(TredH.GetFlag(ref watchflag))
			{
				FSH.WatchFile(this.jsonfile, (e) =>
				 {
					 if (e.FullPath.ToUpper() == this.jsonfileUpper)
					 {
						 Interlocked.Increment(ref changecount);


					 }
					 else
					 {
						 if(e.ChangeType== WatcherChangeTypes.Renamed)
						 {

							 if (((RenamedEventArgs)e).OldFullPath.ToUpper() == this.jsonfileUpper)
							 {

								 Interlocked.Increment(ref changecount);

							 }
						 }

					 }


				 });

			}

		}
		void RaiseConfigChanged()
		{
			if(Interlocked.Exchange(ref changecount,0)>0)
			{

				if (File.Exists(this.jsonfile))
				{
					try
					{
						ReadConfigTxt(this.jsonfile, true);
						Initilize(this.jsonfile);
						base.OnConfigChange();
					}
					catch (Exception ex)
					{
						throw ex.CreateNew();
					}
				}
				else
				{
					base.OnConfigFileLost();
				}
			}

		}
		#endregion 
	}
}
