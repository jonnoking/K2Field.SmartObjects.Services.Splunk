using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.SmartObjects.Services.ServiceSDK;

namespace K2Field.SmartObject.Services.SplunkService
{
    public class ServiceBroker : ServiceAssemblyBase
    {
        #region DescribeSchema
        /// <summary>
        /// Validates configuration items and registers any service objects in this assembly.
        /// </summary>
        /// <returns>Schema (required by infrastructure)</returns>
        public override string DescribeSchema()
        {
            base.Service.Name = "SplunkService";
            base.Service.MetaData.DisplayName = "Splunk Service";
            base.Service.MetaData.Description = "Splunk Service";

            //Type newType = typeof(RESTService);
            //base.Service.ServiceObjects.Add(new SourceCode.SmartObjects.Services.ServiceSDK.Objects.ServiceObject(newType));


            /*Register the service methods*/
            Type[] types = this.GetType().Assembly.GetTypes();

            foreach (Type t in types)
            {
                if (t.IsClass)
                {
                    if (t.GetCustomAttributes(typeof(SourceCode.SmartObjects.Services.ServiceSDK.Attributes.ServiceObjectAttribute), false).Length > 0)
                    {
                        this.Service.ServiceObjects.Add(new SourceCode.SmartObjects.Services.ServiceSDK.Objects.ServiceObject(t));
                    }
                }
            }

            return base.DescribeSchema();
        }
        #endregion

        /// <summary>
        /// Specify the service's configurable items.
        /// The termstore is the GUID of the SharePoint termstore where the terms are located.
        /// See http://msdn.microsoft.com/en-us/library/0h88fahh%28v=VS.85%29.aspx for LocaleId values.
        /// The SharePoint site collection url must be specified if bindings are not placed in the assembly's config file.
        /// The name of the wcf binding must be specified if the default binding is not used.
        /// </summary>
        /// <returns>config section (required by infrastructure)</returns>
        public override string GetConfigSection()
        {
            //Service.ServiceConfiguration.Add("TwitterUsername", true, "");

            //Service.ServiceConfiguration.Add(StringResources.ExceptionOnEmptyResult, true, false);
            //Service.ServiceConfiguration.Add(StringResources.SharePointURL, false, string.Empty);
            //Service.ServiceConfiguration.Add(StringResources.BindingName, false, string.Empty);

            return base.GetConfigSection();
        }

        public override void Execute()
        {
            try
            {
                base.Execute();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Extend

        /// <summary>
        /// Override the Extend abstract method to include additional exception handling as needed
        /// </summary>
        public override void Extend()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
