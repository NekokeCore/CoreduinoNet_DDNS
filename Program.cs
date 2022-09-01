using System.Configuration;
using System.Resources;
using CoreduinoNet_DDNS.Tools;
using log4net;
using Quartz;
using Quartz.Impl;

#pragma warning disable CS8600 // 禁用将 null 字面量或可能为 null 的值转换为非 null 类型。
namespace CoreduinoNet_DDNS // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));
        private static readonly string tiggerName = "TestJobTrigger";
        private static readonly string gropName = "TestJobTriggerGrop";
        private static readonly string jobName = "TestJob";
        //获取⼀个调度器实例化
        private static IScheduler? scheduler = null;
        static void Main(string[] args)
        {
            Boolean debug = false;
            ResourceManager resManager = new ResourceManager(typeof(Resource1));
            Console.WriteLine(resManager.GetString("banner"));
            Console.WriteLine(resManager.GetString("info"));
            Console.WriteLine(resManager.GetString("auther"));

            if (args.Length == 0)
            {
                Console.WriteLine("[信息] {0}", resManager.GetString("argsifempty"));
                Console.WriteLine("[信息]开始更新任务");
                string time = DateTime.Now.ToString();
                _log.Debug("[" + time + "]" + "[信息]开始更新任务");
                Start();
            }
            else
            {
                if (args[0].Equals("调试") || args[0].Equals("--v"))
                {
                    debug = true;
                };

                if (args[0].Equals("帮助") || args[0].Equals("-h"))
                {
                    Console.WriteLine(resManager.GetString("help"));
                };

                if (args[0].Equals("更新") || args[0].Equals("-u"))
                {
                    Console.WriteLine("[信息] 已开始更新");
                    DateTime beforDT = DateTime.Now;
                    ddns.DDNSCNN.check();
                    ddns.DDNSCNN.update();
                    DateTime afterDT = DateTime.Now;
                    TimeSpan time = afterDT.Subtract(beforDT);
                    Console.WriteLine("[信息] 更新结束，用时{0}", time);
                };

                if (args[0].Equals("查询") || args[0].Equals("-c"))
                {
                    Console.WriteLine("[信息] 已开始查询");
                    DateTime beforDT = DateTime.Now;
                    ddns.DDNSCNN.check();
                    DateTime afterDT = DateTime.Now;
                    TimeSpan time = afterDT.Subtract(beforDT);
                    Console.WriteLine("[信息] 查询结束，用时{0}", time);
                };
            };
            if (debug)
            {
                Console.WriteLine(string.Format("[DEBUG]接收到了{0}个参数", args.Length));//Debug 便于调试，显示接受了几个参数
                foreach (var item in args)//Debug 便于显示输入了什么参数
                {
                    Console.WriteLine(item);
                }
            };
        }

        private static async void Start()
        {
            //从⼯⼚中获取⼀个调度器实例化
            scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
            //创建⼀个作业
            IJobDetail job1 = JobBuilder.Create<UpdateJob>()
             .WithIdentity(jobName, gropName)
             .UsingJobData("key", "value")// 传递参数在Execute⽅法中获取（以什么类型值传⼊，取值就⽤相应的类型⽅法取值）
             .Build();
            //获取定时
            string sddd = ConfigurationManager.AppSettings.Get("UPDATE_TIME");
            if(sddd != null)
            {
                int timeset = int.Parse(sddd);
                // 创建触发器
                ITrigger trigger1 = TriggerBuilder.Create()
                                            .WithIdentity(tiggerName, gropName)
                                            .StartNow()                        //现在开始
                                            .WithSimpleSchedule(x => x         //触发时间
                                                .WithIntervalInSeconds(timeset)
                                                .RepeatForever())              //不间断重复执⾏
                                            .Build();
                await scheduler.ScheduleJob(job1, trigger1);      //把作业，触发器加⼊调度器。
                Console.ReadKey();
                // 清除任务和触发器
                ClearJobTrigger();
            }
            else
            {
                Console.WriteLine("[错误]未设置正常的循环时间");
            }
        }

        // 清除任务和触发器
        private static void ClearJobTrigger()
        {
            TriggerKey triggerKey = new TriggerKey(tiggerName, gropName);
            JobKey jobKey = new JobKey(jobName, gropName);
            if (scheduler != null)
            {
                scheduler.PauseTrigger(triggerKey);
                scheduler.UnscheduleJob(triggerKey);
                scheduler.DeleteJob(jobKey);
                scheduler.Shutdown();// 关闭
            }
        }
    }
}