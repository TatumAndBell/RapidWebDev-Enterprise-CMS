/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Eunge, Legal Name: Jian Liu, Email: eunge.liu@RapidWebDev.org

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
using System.Threading;

namespace RapidWebDev.Common.Validation
{
	/// <summary>
	/// The class to manage validation scope in multiple threads.
	/// </summary>
	internal sealed class ValidationManager
	{
		private static object syncObject = new object();
		private static volatile ValidationManager instance;

		private Dictionary<int, Stack<ValidationScope>> validationScopeRefContainer = new Dictionary<int, Stack<ValidationScope>>();

		/// <summary>
		/// Gets the singleton instance of validation manager.
		/// </summary>
		internal static ValidationManager Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncObject)
					{
						if (instance == null)
						{
							instance = new ValidationManager();
						}
					}
				}

				return instance;
			}
		}

		private ValidationManager() { }

		/// <summary>
		/// Push an ValidationScope into current executing thread and return the total number of alive validation scopes in current executing thread.
		/// </summary>
		/// <param name="validationScope"></param>
		internal void Push(ValidationScope validationScope)
		{
			int threadId = Thread.CurrentThread.ManagedThreadId;
			Stack<ValidationScope> stack = null;
			if (this.validationScopeRefContainer.ContainsKey(threadId))
				stack = this.validationScopeRefContainer[threadId];
			else
			{
				stack = new Stack<ValidationScope>();
				this.validationScopeRefContainer[threadId] = stack;
			}

			stack.Push(validationScope);
		}

		/// <summary>
		/// Pop the top ValidationScope reference in current executing thread. 
		/// </summary>
		internal void Pop()
		{
			int threadId = Thread.CurrentThread.ManagedThreadId;
			if (this.validationScopeRefContainer.ContainsKey(threadId))
			{
				Stack<ValidationScope> stack = this.validationScopeRefContainer[threadId];
				stack.Pop();
				if (stack.Count == 0)
					this.validationScopeRefContainer.Remove(threadId);
			}
		}

		/// <summary>
		/// Peek the top validation scope without removing it from the validation scope stack.
		/// Returns null if there has no validation scope in the stack.
		/// </summary>
		/// <returns></returns>
		internal ValidationScope Peek()
		{
			int threadId = Thread.CurrentThread.ManagedThreadId;
			if (this.validationScopeRefContainer.ContainsKey(threadId))
				return this.validationScopeRefContainer[threadId].Peek();

			return null;
		}
	}
}
