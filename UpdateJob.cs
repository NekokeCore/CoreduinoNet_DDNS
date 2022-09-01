using CoreduinoNet_DDNS.Tools;
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreduinoNet_DDNS
{
    public class UpdateJob : IJob

    {
        private readonly ILog _log = LogManager.GetLogger(typeof(UpdateJob));
        // 作业
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string time = DateTime.Now.ToString();
            _log.Debug("[" + time + "]" + "[调试]");
            _log.Error("[" + time + "]" + "[错误]");
            _log.Info("[" + time + "]" + "[信息]");
            // 任务
            Console.WriteLine("[信息] 已开始更新");
            DateTime beforDT = DateTime.Now;
            ddns.DDNSCNN.check();
            ddns.DDNSCNN.update();
            DateTime afterDT = DateTime.Now;
            TimeSpan times = afterDT.Subtract(beforDT);
            Console.WriteLine("[信息] 更新结束，用时{0}", times);
        }
    }
}