/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi, Email: tim.long.yi@RapidWebDev.org

	The GNU Library General Public License (LGPL) used in RapidWebDev is 
	intended to guarantee your freedom to share and change free software - to 
	make sure the software is free for all its users.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Library General Public License (LGPL) for more details.

	You should have received a copy of the GNU Library General Public License (LGPL)
	along with this program.  
	If not, see http://www.rapidwebdev.org/Content/ByUniqueKey/OpenSourceLicense
 ****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Subtext.TestLibrary;

namespace RapidWebDev.Mocks
{
    /// <summary>
    /// Let User can mock httpcontext
    /// and set query string, form varibles ,etc
    /// </summary>
    public class HttpEnvironment : IDisposable
    {

        private HttpSimulator simulator;
        private const string localUri = "http://localhost";
        /// <summary>
        /// 
        /// </summary>
        public HttpEnvironment()

        {
			simulator = new HttpSimulator("/", AppDomain.CurrentDomain.BaseDirectory).SimulateRequest();
        }
        /// <summary>
        /// Set the request Uri
        /// </summary>
        /// <param name="uri"></param>
        public  void SetRequestUrl(string uri) 
        {
            if (simulator == null)
                simulator = new HttpSimulator("/", AppDomain.CurrentDomain.BaseDirectory).SimulateRequest(new Uri(localUri + uri));
            else
                simulator.SimulateRequest(new Uri(localUri + uri));
            
        }
        /// <summary>
        /// Set Post parameters 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetFormParameters(string key, string value) 
        {
            if (simulator == null)
				simulator = new HttpSimulator("/", AppDomain.CurrentDomain.BaseDirectory).SimulateRequest();

            simulator.SetFormVariable(key, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetSessionParaemeter(string key, object value)
        {
            if (simulator == null)
                simulator = new HttpSimulator("/", AppDomain.CurrentDomain.BaseDirectory).SimulateRequest();

            HttpContext.Current.Session[key] = value;
        }
        /// <summary>
        /// inherit from IDispose
        /// </summary>
        public void Dispose()
        {

        }
    }
}
