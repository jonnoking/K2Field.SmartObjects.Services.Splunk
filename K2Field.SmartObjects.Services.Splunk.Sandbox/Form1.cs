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

                await service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);

                Console.WriteLine("Create an index");

                string indexName = "jk-index";
                Index index = await service.Indexes.GetOrNullAsync(indexName);

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
    }
}
