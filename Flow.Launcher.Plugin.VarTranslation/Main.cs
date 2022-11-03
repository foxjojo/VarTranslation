using Flow.Launcher.Plugin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace Flow.Launcher.Plugin.VarTranslation
{
    public class Main : IAsyncPlugin
    {
        private PluginInitContext _context;

        public Task<List<Result>> QueryAsync(Query query, CancellationToken token)
        {
            return GetData(query.Search);
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
            List<Result> results = new List<Result>();
            var url = new StringBuilder();
            url.Append("https://v.api.aa1.cn/api/api-fanyi-yd/index.php?msg=");
            url.Append(str);
            url.Append("&type=1");
            _context.API.ShowMsg(url.ToString());
            var json = await _context.API.HttpGetStringAsync(url.ToString());
            _context.API.ShowMsg("定制", json);
            var jsonData = JsonConvert.DeserializeObject<JsonData>(json);
            _context.API.ShowMsg("翻译结果" + jsonData.text);
            results.Add(new Result()
            {
                Title = jsonData.text,
                SubTitle = jsonData.text,
                Action = delegate (ActionContext context)
                {
                    _context.API.CopyToClipboard(jsonData.text);
                    return true;
                }
            });


            return results;
        }

        public class JsonData
        {
            public string type;
            public string desc;
            public string text;
        }
    }
}