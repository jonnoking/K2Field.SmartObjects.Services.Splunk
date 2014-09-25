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

    }
}
