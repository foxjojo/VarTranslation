using Flow.Launcher.Plugin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Flow.Launcher.Plugin.VarTranslation
{
    //CompletePreform
    public class Main : IAsyncPlugin
    {
        private PluginInitContext _context;

        public Task<List<Result>> QueryAsync(Query query, CancellationToken token)
        {
            return GetData(query.FirstSearch);
        }

        public Task InitAsync(PluginInitContext context)
        {
            return Task.Run(Init);

            void Init()
            {
                _context = context;
            }
        }

        private async Task<List<Result>> GetData(string str)
        {
            if(string.IsNullOrWhiteSpace(str))
                return new List<Result>();

            List<Result> results = new List<Result>();
            var url = new StringBuilder();
            url.Append("https://v.api.aa1.cn/api/api-fanyi-yd/index.php?msg=");
            url.Append(str);
            url.Append("&type=1");
            var json = await _context.API.HttpGetStringAsync(url.ToString());
            if (string.IsNullOrWhiteSpace(json))
                return new List<Result> { };
            try
            {
                var jsonData = JsonConvert.DeserializeObject<JsonData>(json);

                if (jsonData == null)
                    return new List<Result> { };

                var varArray = jsonData.text.Split(' ');
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

                string snakeCase = myTI.ToLower(jsonData.text).Replace(' ', '_');
                string camelCaseB =Regex.Replace( myTI.ToTitleCase(jsonData.text),@"\s","");
                string camelCaseS = camelCaseB.Substring(0, 1).ToLower() + camelCaseB.Substring(1);

                results.Add(new Result()
                {
                    Title = snakeCase,
                    SubTitle = "下划线命名",
                    IcoPath ="icon.png",
                    Action = delegate (ActionContext context)
                    {
                        _context.API.CopyToClipboard(snakeCase);
                        return true;
                    }
                }) ;
                results.Add(new Result()
                {
                    Title = camelCaseB,
                    SubTitle = "帕斯卡命名",
                    IcoPath = "icon.png",
                    Action = delegate (ActionContext context)
                    {
                        _context.API.CopyToClipboard(camelCaseB);
                        return true;
                    }
                });
                results.Add(new Result()
                {
                    Title = camelCaseS,
                    SubTitle ="驼峰命名",
                    IcoPath = "icon.png",
                    Action = delegate (ActionContext context)
                    {
                        _context.API.CopyToClipboard(camelCaseS);
                        return true;
                    }
                });


                return results;
            }
            catch (Exception)
            {
                _context.API.LogInfo("VarTranslation", string.Format("需要查询的str:{0}", str));
                _context.API.LogInfo("VarTranslation", string.Format("无法格式化json:{0}",json));
               return new List<Result>();
            }
          
        }

        public class JsonData
        {
            public string type;
            public string desc;
            public string text;
        }
    }
}