using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.Interfaces
{
	/// <summary>
	/// This interface must be implemented by TypeResolvers.  Can be used to change where DLL's and types are loaded from (i.e. from a database instead of GAC/folder).
	/// </summary>
	public interface ITypeResolver
	{
		/// <summary>
		/// Gets the type activator for the specified type.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <returns></returns>
		ITypeActivator GetTypeActivator(string typeName);

		/// <summary>
		/// Gets the type activator for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		ITypeActivator GetTypeActivator(Type type);
	}
}
