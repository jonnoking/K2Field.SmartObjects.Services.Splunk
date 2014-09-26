using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using Attributes = SourceCode.SmartObjects.Services.ServiceSDK.Attributes;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;
using Splunk;
using SplunkSDKHelper;
using Splunk.Client.Helpers;


namespace K2Field.SmartObjects.Services.SplunkService
{
    [Attributes.ServiceObject("Splunk", "Splunk", "Splunk")]
    public class SplunkMessage
    {
        private string index = "";
        [Attributes.Property("Index", SoType.Text, "Index", "Index")]
        public string Index
        {
            get { return index; }
            set { index = value; }
        }

        private bool createindex = false;
        [Attributes.Property("CreateIndex", SoType.YesNo, "Create Index", "Create Index")]
        public bool CreateIndex
        {
            get { return createindex; }
            set { createindex = value; }
        }


        private string source = "";
        [Attributes.Property("Source", SoType.Text, "Source", "Source")]
        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        private string sourcetype = "";
        [Attributes.Property("SourceType", SoType.Text, "SourceType", "SourceType")]
        public string SourceType
        {
            get { return sourcetype; }
            set { sourcetype = value; }
        }

        private string message = "";
        [Attributes.Property("Message", SoType.Memo, "Message", "Message")]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private string resultstatus = "";
        [Attributes.Property("ResultStatus", SoType.Text, "ResultStatus", "ResultStatus")]
        public string ResultStatus
        {
            get { return resultstatus; }
            set { resultstatus = value; }
        }

        private string resultmessage = "";
        [Attributes.Property("ResultMessage", SoType.Text, "ResultMessage", "ResultMessage")]
        public string ResultMessage
        {
            get { return resultmessage; }
            set { resultmessage = value; }
        }


        [Attributes.Method("SubmitMessage", SourceCode.SmartObjects.Services.ServiceSDK.Types.MethodType.Execute, "SubmitMessage", "SubmitMessage",
        new string[] { "Message", "Index", "Source", "SourceType" }, //required property array (no required properties for this sample)
        new string[] { "Message", "Index", "Source", "SourceType" }, //input property array (no optional input properties for this sample)
        new string[] { "Message", "Index", "Source", "SourceType", "ResultStatus", "ResultMessage" })] //return property array (2 properties for this example)
        public SplunkMessage PostMessageToSplunk()
        {
            try
            {
                // Load connection info for Splunk server in .splunkrc file.
                var cli = Command.Splunk("submit");
                //cli.Parse(argv);

                string[] cargs = { };
                cli.Parse(cargs);


                var service = Splunk.Service.Connect(cli.Opts);

                var args = new ReceiverSubmitArgs
                {
                    Index = this.Index,
                    Source = this.Source,
                    SourceType = this.SourceType
                };

                var receiver = new Receiver(service);

                receiver.Submit(args, message);

                this.ResultStatus = "OK";
            }
            catch(Exception ex)
            {
                this.ResultStatus = "Error";
                this.ResultMessage = ex.Message;
            }


//            # Splunk host (default: localhost)
//host=localhost
//# Splunk admin port (default: 8089)
//port=8089
//# Splunk username
//username=admin
//# Splunk password
//password=K2pass!
//# Access scheme (default: https)
//scheme=https

//.splunkrc

            return this;
        }

        [Attributes.Method("SubmitMessage", SourceCode.SmartObjects.Services.ServiceSDK.Types.MethodType.Execute, "SubmitMessage", "SubmitMessage",
        new string[] { "Message", "Index", "CreateIndex", "Source", "SourceType" }, //required property array (no required properties for this sample)
        new string[] { "Message", "Index", "CreateIndex", "Source", "SourceType" }, //input property array (no optional input properties for this sample)
        new string[] { "Message", "Index", "CreateIndex", "Source", "SourceType", "ResultStatus", "ResultMessage" })] //return property array (2 properties for this example)

        public async Task<SplunkMessage> PostMessage()
        {

            using (var service = new Splunk.Client.Service(SdkHelper.Splunk.Scheme, SdkHelper.Splunk.Host, SdkHelper.Splunk.Port, new Splunk.Client.Namespace(user: "nobody", app: "K2")))
            {

                await service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);

                string indexName = this.Index;
                Splunk.Client.Index index = await service.Indexes.GetOrNullAsync(indexName);

                if (index == null && !this.CreateIndex)
                {
                    this.ResultStatus = "Error";
                    this.ResultMessage = "Index does not exist";
                    return this;
                } else if (index == null && this.CreateIndex)
                {
                    index = await service.Indexes.CreateAsync(indexName);
                }
                else if (index == null)
                {
                    this.ResultStatus = "Error";
                    this.ResultMessage = "Index does not exist";
                    return this;
                }
                
                try
                {
                    await index.EnableAsync();

                    Splunk.Client.Transmitter transmitter = service.Transmitter;
                    Splunk.Client.SearchResult result;
                    
                    result = await transmitter.SendAsync(this.Message, indexName);

                    //using (var results = await service.SearchOneShotAsync(string.Format("search index={0}", indexName)))
                    //{
                    //    foreach (Splunk.Client.SearchResult task in results)
                    //    {
                    //        Console.WriteLine(task);
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    this.ResultStatus = "Error";
                    this.ResultMessage = ex.Message;
                }

                this.ResultStatus = "OK";
            }

            return this;
        }

    }
}
