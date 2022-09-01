using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8600 // 禁用将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8604 // 禁用将 null 字面量或可能为 null 的值转换为非 null 类型。
namespace CoreduinoNet_DDNS.Tools
{
    internal class ddns
    {
        public class DDNSCNN
        {
            public static void check()
            {
                try
                {
                    string checkipurl = ConfigurationManager.AppSettings.Get("CHECK_IP_URL");
                    HttpWebResponse response = CreateGetHttpResponse(checkipurl);
                    //获取流
                    Stream streamResponse = response.GetResponseStream();
                    //使用UTF8解码
                    StreamReader streanReader = new StreamReader(streamResponse, Encoding.UTF8);
                    string myip = streanReader.ReadToEnd();
                    //清空TXT
                    FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\myip.txt", FileMode.OpenOrCreate, FileAccess.Write);
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.SetLength(0);
                    fs.Close();
                    //写入TXT
                    StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\myip.txt", true, Encoding.Default);
                    sw.Write(myip);
                    //释放资源
                    sw.Close();
                    //获取当前IP
                    StreamReader myiptxt = new StreamReader(Directory.GetCurrentDirectory() + "\\myip.txt");
                    string myipp = myiptxt.ReadLine();
                    Console.WriteLine("[信息]您当前的IP是：{0}", myipp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                };

            }
            public static void update()
            {
                try
                {
                    //获取当前IP
                    StreamReader myiptxt = new StreamReader(Directory.GetCurrentDirectory() + "\\myip.txt");
                    string myip = myiptxt.ReadLine();
                    string host = ConfigurationManager.AppSettings.Get("DDNS_HOST");
                    string ddnsurl = "http://members.3322.net/dyndns/update?hostname=" + host + "&myip=" + myip;
                    HttpWebResponse ddnsresponse = CreateGetHttpResponse(ddnsurl);
                    //获取流
                    Stream ddnsstreamResponse = ddnsresponse.GetResponseStream();
                    //使用UTF8解码
                    StreamReader ddnsstreanReader = new StreamReader(ddnsstreamResponse, Encoding.UTF8);
                    string status = ddnsstreanReader.ReadToEnd();
                    string[] mystatus = status.Split(' ');
                    if (mystatus.Length > 0)
                    {
                        //更新成功
                        if (mystatus[0] == "good")
                        {
                            Console.WriteLine("[信息]DDNS已更新，您的IP为：{0}", myip);
                        };
                        if (mystatus[0] == "nochg")
                        {
                            Console.WriteLine("[信息]您的IP没有被更改，无需更新");
                        };
                        //更新失败
                        if (mystatus[0] == "badauth")
                        {
                            Console.WriteLine("[信息]身份认证出错，请检查用户名和密码");
                        };
                        if (mystatus[0] == "badsys")
                        {
                            Console.WriteLine("[信息]该域名不是动态域名，可能是其他类型的域名（智能域名、静态域名、域名转向、子域名）");
                        };
                        if (mystatus[0] == "badagent")
                        {
                            Console.WriteLine("[信息]由于发送大量垃圾数据，客户端名称被系统封杀");
                        };
                        if (mystatus[0] == "notfqdn")
                        {
                            Console.WriteLine("[信息]没有提供域名参数，必须提供一个在公云注册的动态域名域名");
                        };
                        if (mystatus[0] == "nohost")
                        {
                            Console.WriteLine("[信息]域名不存在，请检查域名是否填写正确");
                        };
                        if (mystatus[0] == "!donator")
                        {
                            Console.WriteLine("[信息]必须是收费用户，才能使用 offline 离线功能");
                        };
                        if (mystatus[0] == "!yours")
                        {
                            Console.WriteLine("[信息]该域名存在，但是不是您所有");
                        };
                        if (mystatus[0] == "!active")
                        {
                            Console.WriteLine("[信息]该域名被系统关闭，请联系公云客服人员");
                        };
                        if (mystatus[0] == "abuse")
                        {
                            Console.WriteLine("[信息]该域名由于段时间大量发送更新请求，被系统禁止，请联系公云客服人员");
                        };
                        if (mystatus[0] == "dnserr")
                        {
                            Console.WriteLine("[信息]DNS 服务器更新失败");
                        };
                        if (mystatus[0] == "interror")
                        {
                            Console.WriteLine("[信息]服务器内部严重错误，比如数据库出错或者DNS服务器出错");
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                };
            }

                public static HttpWebResponse CreateGetHttpResponse(string url)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "text/html;chartset=UTF-8";
                request.UserAgent = "Mozilla / 5.0(Windows NT 10.0; Win64; x64; rv: 48.0) Gecko / 20100101 Firefox / 48.0";
                request.Method = "GET";
                //读取用户名密码
                string username = ConfigurationManager.AppSettings.Get("DDNS_USERNAME");
                string userkey = ConfigurationManager.AppSettings.Get("DDNS_KEY");
                // 设置HTTP头Http Basic认证
                string authorization = username + ":" + userkey;
                string base64 = Convert.ToBase64String(Encoding.Default.GetBytes(authorization));
                request.Headers.Add("Authorization", "Basic " + base64);
                return (HttpWebResponse)request.GetResponse();
            }
        }
    }
}
