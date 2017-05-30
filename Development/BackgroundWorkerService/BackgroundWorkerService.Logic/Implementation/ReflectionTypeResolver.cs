using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;

namespace BackgroundWorkerService.Logic.Implementation
{
	/// <summary>
	/// This is the default .net reflection type resolver.  Always returns ReflectionTypeActivator.
	/// </summary>
	public class ReflectionTypeResolver : ITypeResolver
	{
		/// <summary>
		/// Gets the type activator for the specified type.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <returns></returns>
		public ITypeActivator GetTypeActivator(string typeName)
		{
			return new ReflectionTypeActivator();
		}

		/// <summary>
		/// Gets the type activator for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public ITypeActivator GetTypeActivator(Type type)
		{
			return new ReflectionTypeActivator();
		}
	}
}
