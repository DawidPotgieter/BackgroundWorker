using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.Interfaces
{
	/// <summary>
	/// This interface is implemented to create a type activator.  A type activator is used to get an instance of a specified type of object, optionally
	/// requiring an interface to be implemented by the specified type.
	/// </summary>
	public interface ITypeActivator
	{
		/// <summary>
		/// Creates an instance of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeName">Name of the type to instantiate.</param>
		/// <returns>
		/// Returns null if the instance could not be created.
		/// </returns>
		T CreateInstance<T>(string typeName) where T : class;

		/// <summary>
		/// Creates an instance of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type">The type to instantiate.</param>
		/// <returns></returns>
		/// <returns>
		/// Returns null if the instance could not be created.
		/// </returns>
		T CreateInstance<T>(Type type) where T : class;

		/// <summary>
		/// Creates an instance of the specified type, requiring that the type implements the specified interface.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeName">Name of the type to instantiate.</param>
		/// <param name="requiredInterfaceTypeName">Name of the required interface that the type must implement.</param>
		/// <returns></returns>
		/// <returns>
		/// Returns null if the instance could not be created.
		/// </returns>
		T CreateInstanceWithRequiredInterface<T>(string typeName, string requiredInterfaceTypeName) where T : class;

		/// <summary>
		/// Creates an instance of the specified type, requiring that the type implements the specified interface.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type">The type to instantiate.</param>
		/// <param name="requiredInterface">The required interface that the type must implement.</param>
		/// <returns></returns>
		/// <returns>
		/// Returns null if the instance could not be created.
		/// </returns>
		T CreateInstanceWithRequiredInterface<T>(Type type, Type requiredInterface) where T : class;
	}
}
