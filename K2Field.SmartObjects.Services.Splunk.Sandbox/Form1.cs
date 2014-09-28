using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Splunk.Client;
//using Splunk.Client.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Splunk.Client.Helpers;
using System.Threading;

namespace K2Field.SmartObjects.Services.Splunk.Sandbox
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
        }

        private async void btnPost_Click(object sender, EventArgs e)
        {

            using (var service = new Service(SdkHelper.Splunk.Scheme, SdkHelper.Splunk.Host, SdkHelper.Splunk.Port, new Namespace(user: "nobody", app: "search")))
            {

                Console.WriteLine("Login as " + SdkHelper.Splunk.Username);

                service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password).Wait();

                Console.WriteLine("Create an index");

                string indexName = "k2";
                Index index = Task.Run(() => service.Indexes.GetOrNullAsync(indexName).Result).Result;
                //var result = Task.Run(() => SaveAssetDataAsDraft().Result).Result;
                
                //service.SearchAsync("", 100, ExecutionMode.)

                if (index != null)
                {
                    await index.RemoveAsync();
                }

                index = await service.Indexes.CreateAsync(indexName);
                Exception exception = null;

                try
                {
                    await index.EnableAsync();

                    Transmitter transmitter = service.Transmitter;
                    SearchResult result;

                    result = await transmitter.SendAsync("Hello World.", indexName);
                    result = await transmitter.SendAsync("Goodbye world.", indexName);

                    using (var results = await service.SearchOneShotAsync(string.Format("search index={0}", indexName)))
                    {
                        foreach (SearchResult task in results)
                        {
                            Console.WriteLine(task);
                        }
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                await index.RemoveAsync();

                if (exception != null)
                {
                    throw exception;
                }
            }
        }

        private async void btnSearch_Click(object sender, System.EventArgs e)
        {
            using (var service = new Service(SdkHelper.Splunk.Scheme, SdkHelper.Splunk.Host, SdkHelper.Splunk.Port, new Namespace(user: "nobody", app: "search")))
            {
                //Task task = Run(service);

                //while (!task.IsCanceled)
                //{
                //    Task.Delay(500).Wait();
                //}
                await Run(service);
            }
        }

        static async Task Run(Service service)
        {
            await service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);
            //Console.WriteLine("Press return to cancel.");
            //index=k2 | stats count by name
            //search index=_internal | stats count by method
            string searchQuery = "search index=k2 name=*jonno*";

            Job realtimeJob = await service.Jobs.CreateAsync(searchQuery, args: new JobArgs
            {
                SearchMode = SearchMode.RealTime,
                EarliestTime = "rt-1m",
                LatestTime = "rt",
            });

            var tokenSource = new CancellationTokenSource();

//#pragma warning disable 4014
//            //// Because this call is not awaited, execution of the current 
//            //// method continues before the call is completed. Consider 
//            //// applying the 'await' operator to the result of the call.

//            Task.Run(async () =>
//            {
//                Console.ReadLine();

//                await realtimeJob.CancelAsync();
//                tokenSource.Cancel();
//            });

//#pragma warning restore 4014

            while (!tokenSource.IsCancellationRequested)
            {
                using (SearchResultStream stream = await realtimeJob.GetSearchPreviewAsync())
                {
                    Console.WriteLine("fieldnames: " + string.Join(";", stream.FieldNames));
                    Console.WriteLine("fieldname count: " + stream.FieldNames.Count);
                    Console.WriteLine("final result: " + stream.IsFinal);

                    foreach (SearchResult result in stream)
                    {
                        Console.WriteLine(result.GetValue("_raw"));
                    }

                    Console.WriteLine("");
                    await Task.Delay(60000, tokenSource.Token);
                }
            }
        }
    }
}
