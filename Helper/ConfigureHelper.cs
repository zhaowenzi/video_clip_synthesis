
using 素材合成.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using static 素材合成.Model.PlayerSection;
using System.Xml;

namespace 素材合成
{
    public class ConfigureHelper
    {
        public static string Read(string key)
        {
                try
                {
                    var innerText = ConfigurationManager.AppSettings[key];
                    return innerText;
                }
                catch (Exception)
                {
                    // ignored
                }
                return null;
        }
        public static bool Write(string key, string value)
        {

                try
                {
                    //更新App.config
                    var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                    if (!configuration.AppSettings.Settings.AllKeys.Contains(key))
                    {
                        configuration.AppSettings.Settings.Add(key, value);
                    }
                    else
                    {
                        configuration.AppSettings.Settings[key].Value = value;
                    }
                    configuration.Save(ConfigurationSaveMode.Full);
                    ConfigurationManager.RefreshSection("appSettings");     //重新加载新的配置文件

                    //更新配置参数 缓存字典
                   
                    return true;
                }
                catch (Exception)
                {
                    //Logger.Error("WeMew.WinFormOsr Write:" + ex.Message);
                }
                return false;
        }

        public static TheKeyValue ReadPlayerSection(string key)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            PlayerSection configSection = (PlayerSection)config.GetSection("PlayerSection");
            var value = (from kv in configSection.KeyValues.Cast<TheKeyValue>()
                         where kv.Key == key
                         select kv).FirstOrDefault();
            return value;
        }

        static public List<Flow> GetAllTopicType1(string XmlPath= "素材合成.exe.config")
        {
            List<Flow> TopicList = new List<Flow>();

            //将XML文件加载进来
            XDocument document = XDocument.Load(XmlPath);
            //获取到XML的根元素进行操作
            //  = document.Elements("Flow");
            var firstNodes = document.Nodes();
            foreach (var secondNode in firstNodes)
            {
                foreach (var thirdNode in secondNode.Document.Elements())
                {
                    if (thirdNode.Name == "configuration")
                    {
                        foreach (var fourNode in thirdNode.Elements())
                        {
                            if (fourNode.Name == "VideoControlSerialPort")
                            {
                                foreach (var fiveNode in fourNode.Elements())
                                {
                                    if (fiveNode.Name == "Flow")
                                    {
                                        Flow flow = new Flow();
                                        flow.Instructions = new List<Instruction>();
                                        foreach (var item in fiveNode.Attributes())
                                        {
                                            if (item.Name == "movie")
                                            {
                                                flow.Movie = item.Value;
                                            }
                                            if (item.Name == "key")
                                            {
                                                flow.ID = item.Value;
                                            }
                                            if (item.Name == "allow")
                                            {
                                                flow.Allow = item.Value;
                                            }
                                            if (item.Name == "password")
                                            {
                                                flow.Password = item.Value;
                                            }
                                            if (item.Name == "endtime")
                                            {
                                                flow.EndTime = item.Value;
                                            }
                                            if (item.Name == "goto")
                                            {
                                                flow.Goto = item.Value;
                                            }
                                        }

                                        foreach (var sixNode in fiveNode.Elements())
                                        {
                                            Instruction instruction = new Instruction();

                                            var collection = sixNode.Attributes();
                                            foreach (var item in collection)
                                            {
                                                if (item.Name == "时间")
                                                {
                                                    instruction.T时间 = item.Value;
                                                }
                                                if (item.Name == "灯光指令")
                                                {
                                                    instruction.Cmd灯光 = item.Value;
                                                }
                                                if (item.Name == "继电器指令")
                                                {
                                                    instruction.Cmd继电器 = item.Value;
                                                }
                                                if (item.Name == "网络指令")
                                                {
                                                    instruction.CmdTcp = item.Value;
                                                }
                                                if (item.Name == "其他信息")
                                                {
                                                    instruction.Other = item.Value;
                                                }
                                            }
                                            flow.Instructions.Add(instruction);
                                        }
                                        TopicList.Add(flow);
                                    }

                                }
                            }
                        }
                    }

                }
            }
            //获取根元素下的所有子元素


            return TopicList;
        }


        static public void SetAllTopicType1(string movie,string time,string other,string commander)
        {

            string XmlPath = "素材合成.exe.config";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(XmlPath);
            XmlNode xNode;
            XmlElement xElem1;
            XmlElement xElem2;
            xNode = xmlDocument.SelectSingleNode("//VideoControlSerialPort");//获取指定的xml子节点
            XmlNodeList xNode2 = xNode.SelectNodes("//Flow");
            bool hasNode = false;
            for (int j = 0; j < xNode2.Count; j++)
            {
                if(xNode2.Item(j).OuterXml.Contains(movie))
                {
                    hasNode = true;
                    XmlNodeList xNode3 = xNode.SelectNodes("//Instructions");
                    bool findTime = false;
                    for (int i = 0; i < xNode3.Count; i++)
                    {
                        if (xNode3.Item(i).OuterXml.Contains(time))
                        {
                            if (xNode3.Item(i).OuterXml.Contains($"其他信息=\"{other}\""))
                            {
                                findTime = true;
                                ((XmlElement)xNode3.Item(i)).SetAttribute("灯光指令", commander);
                            }
                        }
                    }
                    if (findTime == false)
                    {
                        xElem2 = xmlDocument.CreateElement("Instructions");
                        xElem2.SetAttribute("时间", time);
                        xElem2.SetAttribute("灯光指令", commander);
                        xElem2.SetAttribute("继电器指令", "");
                        xElem2.SetAttribute("网络指令", "");
                        xElem2.SetAttribute("其他信息", other);
                        xNode2.Item(j).AppendChild(xElem2);
                    }
                }
            }
            if(hasNode==false)
            {
                xElem1 = xmlDocument.CreateElement("Flow");
                xElem1.SetAttribute("key", "");
                xElem1.SetAttribute("movie", movie);
                xElem1.SetAttribute("password", "");
                xElem1.SetAttribute("allow", "");
                xElem1.SetAttribute("endtime", "");
                xElem1.SetAttribute("goto", "");
                xElem2 = xmlDocument.CreateElement("Instructions");
                xElem2.SetAttribute("时间", time);
                xElem2.SetAttribute("灯光指令", commander);
                xElem2.SetAttribute("继电器指令", "");
                xElem2.SetAttribute("网络指令", "");
                xElem2.SetAttribute("其他信息", other);
                xElem1.AppendChild(xElem2);
                xNode.AppendChild(xElem1);
            }
            xmlDocument.Save(XmlPath);//保存xml文档
            Console.WriteLine("保存成功！");
        }


        public static Flow ReadVideoControlSerialPortMovie(string Movie)
        {

            return GetAllTopicType1("分屏播放器.exe.config").Where(m=> Movie.Contains(m.Movie)).FirstOrDefault();
        }

        static public List<SerialNode> GetSerialNodeList(string XmlPath = "素材合成.exe.config")
        {
            //将XML文件加载进来
            XDocument document = XDocument.Load(XmlPath);
           
            List<SerialNode> serialNodes = new List<SerialNode>();
            //获取到XML的根元素进行操作
            //  = document.Elements("Flow");
            var firstNodes = document.Nodes();
            foreach (var secondNode in firstNodes)
            {
                foreach (var thirdNode in secondNode.Document.Elements())
                {
                    if (thirdNode.Name == "configuration")
                    {
                        foreach (var fourNode in thirdNode.Elements())
                        {
                            if (fourNode.Name == "SerialNode")
                            {

                                foreach (var fiveNode in fourNode.Elements())
                                {
                                    SerialNode flow = new SerialNode();
                                    foreach (var item in fiveNode.Attributes())
                                    {
                                        if (item.Name == "command")
                                        {
                                            flow.Command = item.Value;
                                        }
                                        if (item.Name == "key")
                                        {
                                            flow.key = item.Value;
                                        }
                                        if (item.Name == "content")
                                        {
                                            flow.Content = item.Value;
                                        }
                                    }
                                    serialNodes.Add(flow);
                                }
                            }
                        }
                    }

                }
            }
            //获取根元素下的所有子元素

            return serialNodes;
        }


        public static List<TheKeyValue> ReadPlayerSectionList()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            PlayerSection configSection = (PlayerSection)config.GetSection("PlayerSection");
            return configSection.KeyValues.Cast<TheKeyValue>().ToList();
        }

        public static PlayListSection.TheKeyValue ReadPlayListSection(string key)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            PlayListSection configSection = (PlayListSection)config.GetSection("PlayListSection");
            var value = (from kv in configSection.KeyValues.Cast<PlayListSection.TheKeyValue>()
                         where kv.Key == key
                         select kv).FirstOrDefault();
            return value;
        }
        public static int ReadPlayListSectionCount()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            PlayListSection configSection = (PlayListSection)config.GetSection("PlayListSection");
            return configSection.KeyValues.Count;
        }

    }
}
