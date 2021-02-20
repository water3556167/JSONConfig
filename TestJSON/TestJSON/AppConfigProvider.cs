using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using NRails.Maths;
using NRails.Reflection;
using NRails.Util;

namespace TestJSON
{
	/// <summary>
	/// 默认实现配置
	/// </summary>
	public class AppConfigProvider : ConfigProvider
	{
		XmlNode appSettings;
		XmlNode conifgNode;
		string xmlFile;
		string xmlFileUpper;

		/// <summary>
		/// 单一实例
		/// </summary>
		public static readonly AppConfigProvider instanse = new AppConfigProvider();

		public AppConfigProvider(string xmlFile)
		{
			Initialize(xmlFile);

		}
		public AppConfigProvider()
		{
			Initialize(null);
		}
		void Initialize(string file)
		{
			if (file.IsEmpty())
			{

				var exefile = EnvH.EntryPath;
				var file1 = exefile + ".conifg";
				var file2 = Path.GetDirectoryName(exefile) + "\\AppConifg.conifg";
				if (File.Exists(file1))
					file = file1;
				else if (File.Exists(file2))
					file = file2;
			}
			this.xmlFile = FSH.GetPath(file);
			this.xmlFileUpper = this.xmlFile.ToUpper();
			var document = new XmlDocument();
			var retry = 3;
			start:
			try
			{
				if (!file.IsEmpty() && File.Exists(file))
				{
					var xmlcontent = ReadConfigTxt(file, false);
					document.LoadXml(xmlcontent);
					appSettings = XmlHelper.GetChild(document, "appSettings");
					Debug.WriteLine("成功加载配置文件" + file);
				}
			}
			catch (Exception ex)
			{
				if(retry-- > 0)
				{
					TredH.SleepAsync(1000);
					goto start;
				}
			}
			if (appSettings != null)
				conifgNode = appSettings.ParentNode;
			else {
				if (document.ChildNodes.Count == 0)
					document.AppendChild(document.CreateNode(XmlNodeType.XmlDeclaration,null,null));
				conifgNode = XmlHelper.GetChild(document,"conifiguration");
				if (conifgNode == null)
					conifgNode = document.CreateNode(XmlNodeType.Element,"conifiguration",null);
				document.AppendChild(conifgNode);
				appSettings = document.CreateNode(XmlNodeType.Element,"appSettings",null);
				document.AppendChild(appSettings);
			}
		}

		#region 文件变更监视器
		int watcherFlag;
		int changeCount = 0;
		private void InitFileSystemWatcher()
		{
			if(!TredH.GetFlag(ref watcherFlag))
			{
				try
				{
					FSH.WatchFile(this.xmlFile, (e) =>
					{

						if (e.FullPath.ToUpper() == this.xmlFileUpper)
						{
							Interlocked.Increment(ref changeCount);
							ThreadHelper.InvokeOnce(RaiseConifgChanged, SMath.Second);

						}
						else
						{
							if (e.ChangeType == WatcherChangeTypes.Renamed)
							{
								if (((RenamedEventArgs)e).OldFullPath.ToUpper() == this.xmlFileUpper)
								{
									Interlocked.Increment(ref changeCount);
									TredH.InvokeOnce(RaiseConifgChanged, SMath.Second);
								}
							}
						}


					});
				}
				catch (Exception ex)
				{

				}

			}


		}
		private void RaiseConifgChanged()
		{
			if(Interlocked.Exchange(ref changeCount, 0) > 0)
			{
				if (File.Exists(this.xmlFile))
				{
					ReadConfigTxt(this.xmlFile, true);
					Initialize(this.xmlFile);
					base.OnConfigChange();
				}
				else
				{
					conifgNode.RemoveChild(appSettings);
					appSettings = conifgNode.OwnerDocument.CreateNode(XmlNodeType.Element,"appSettings",null);
					if (conifgNode.ChildNodes.Count == 0)
					{
						conifgNode.AppendChild(appSettings);
					}
					else
					{
						conifgNode.InsertBefore(appSettings,conifgNode.ChildNodes[0]);
					}
					base.OnConfigFileLost();
				}
			}

		}
		#endregion

		/// <summary>
		/// 保存资源
		/// </summary>
		public override void SaveToSource(ParameterSetting setting, ObjectField[] fields)
		{
			foreach(var field in fields)
			{
				var thin = field.Type.GetTypeInfoN();
				var value = field.GetValue(setting);
				var name = field.Name;
				if (thin.IsArray || thin.IsIList)
				{
					var values = (IList)value;
					var nodes = appSettings.SelectNodes("add[@key='"+name+"']").ToArray<XmlNode>();
					if (nodes.Length > 0)
					{

					}
				}
			}
		}


		#region 辅助类
		void RemoveSummary(XmlNode node)
		{
			if (node.PreviousSibling != null && node.PreviousSibling.NodeType == XmlNodeType.Comment)
				appSettings.RemoveChild(node.PreviousSibling);
		}
		void UpdateSummary()
		{

		}
		/// <summary>
		/// 添加节点
		/// </summary>
		/// <param name="member"></param>
		/// <param name="value"></param>
		void AddSetting(ObjectField member,object value)
		{
			var name = member.Name;
			var valuestr = string.Empty;
			if (!Oct.IsNull(value)) valuestr = Oct.ToString(value);
			var node = appSettings.OwnerDocument.CreateNode(XmlNodeType.Element,"add",null);
			appSettings.AppendChild(node);
			var key=appSettings.OwnerDocument.CreateAttribute("key");
			key.Value = name;
			node.Attributes.Append(key);
			var valattr = appSettings.OwnerDocument.CreateAttribute("value");
			valattr.Value = valuestr;
			node.Attributes.Append(valattr);
			var destxt = appSettings.OwnerDocument.CreateAttribute("desc");
			destxt.Value = member.GetXmlDescription();
			node.Attributes.Append(destxt);
		}
		/// <summary>
		/// 更新节点
		/// </summary>
		/// <param name="member"></param>
		/// <param name="value"></param>
		/// <param name="node"></param>
		void updateSetting(ObjectField member,object value,XmlNode node)
		{
			var valattr = node.GetAttrib("value");
			if (valattr == null)
			{
				valattr = appSettings.OwnerDocument.CreateAttribute("value");
				node.Attributes.Append(valattr);
			}
			valattr.Value = Oct.ToString(value);
			RemoveSummary(node);

			var descAt = node.GetAttrib("desc");
			if (descAt == null)
			{
				descAt = appSettings.OwnerDocument.CreateAttribute("desc");
				node.Attributes.Append(descAt);
			}
			descAt.Value = member.GetXmlDescription();
		}
		#endregion

		string GetNodeVal(XmlNode node)
		{
			var att = node.Attributes["value"];
			if (att != null) return att.Value;
			return null;
		}
		/// <summary>
		/// 获取给定名称的配置项
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object GetParameter(string name)
		{
			return GetParameter(name, null, null);
		}
		public override object GetParameter(string name, ObjectField field, object defVal)
		{
			lock(appSettings)
			{
				var nodes = appSettings.SelectNodes("add[@key='"+name+"']");
				var count = nodes.TryFunc(a=>a.Count);
				if (nodes != null && count > 0)
				{
					if (field == null)
					{
						for (int i = 0; i < count; i++)
						{
							var valstr = GetNodeVal(nodes[i]);
							if (!valstr.IsEmpty()) return valstr;
						}
						return null;
					}
					else
					{
						var thin = field.Type.GetTypeInfoN();
						if (thin.IsArray)
						{
							var eltype = thin.ElementType;
							var array = Array.CreateInstance(eltype.TargetType,count);
							for (int i = 0; i < count; i++)
							{
								var valstr = GetNodeVal(nodes[i]);
								array.SetValue(Oct.ToObject(eltype.TargetType,valstr),i);
							}
							return array;
						}
						else if (thin.IsIList)
						{
							var eltype = thin.ElementType;
							var array = typeof(List<>).MakeGenericType(eltype.TargetType).CreateInstance(count) as System.Collections.IList;
							for (int i = 0; i < count; i++)
							{
								var valstr = GetNodeVal(nodes[i]);
								array[i]=Oct.ToObject(eltype.TargetType, valstr);
							}
							return array;
						}
						else
						{
							for (int i = 0; i < count; i++)
							{
								var valstr = GetNodeVal(nodes[i]);
								if (!valstr.IsEmpty()) return Oct.ToObject(field.Type,valstr);
							}
							return null;
						}

					}

				}
			}
			return null;
		}
	}
}
