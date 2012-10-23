using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using RapidWebDev.UI;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;
using System.Web.UI;

namespace RapidWebDev.Mocks.UIMocks
{
    /// <summary>
    /// This class provides the utilities
    /// </summary>
    public abstract class DynamicComponentUtil
    {
        /// <summary>
        /// 
        /// </summary>
        public object actual;//The object which will be wrapped
        /// <summary>
        /// 
        /// </summary>
        public Page page = new Page();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actual"></param>
        public DynamicComponentUtil(object actual)
        {
            this.actual = actual;
        }


        #region protected
        /// <summary>
        /// Do the basic validation
        /// </summary>
        protected void Validate()
        {
            if (actual == null)
                throw new Exception("Set the IDetailPanelPage firstly");

        }
        #endregion

        #region Set Get
        /// <summary>
        /// Set the web control programatically
        /// </summary>
        /// <param name="controlKey"></param>
        /// <param name="controlValue"></param>
        public virtual void Set(string controlKey, Control controlValue)
        {
            Validate();
            Type type = actual.GetType();

            foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (fieldInfo.Name.Equals(controlKey))
                {
                    fieldInfo.SetValue(actual, controlValue);
                    if(controlValue != null)
                        page.Controls.Add(controlValue as Control);
                    return;
                }
            }
            throw new Exception("This control doesn't exist");

        }
        /// <summary>
        /// Get the web control programatically
        /// </summary>
        /// <param name="controlKey"></param>
        /// <returns></returns>
        public virtual object Get(string controlKey)
        {
            Validate();
            Type type = actual.GetType();

            foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (fieldInfo.Name.Equals(controlKey))
                {
                    return fieldInfo.GetValue(actual);
                }
            }
            throw new Exception("This control doesn't exist");

        }
        /// <summary>
        /// 
        /// </summary>
        public virtual Action<MessageTypes, string> ShowMessage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual DynamicPageConfiguration Configuration { get; set; }
        #endregion
    }
}
