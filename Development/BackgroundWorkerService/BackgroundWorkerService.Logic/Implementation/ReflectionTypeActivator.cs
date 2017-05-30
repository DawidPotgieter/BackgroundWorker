using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;
using Common.Logging;

namespace BackgroundWorkerService.Logic.Implementation
{
	/// <summary>
	/// This type activator uses the default .net System.Activator.CreateInstance to create instances of object types.
	/// </summary>
	public class ReflectionTypeActivator : ITypeActivator
	{
		/// <summary>
		/// Creates an instance of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeName">Name of the type to instantiate.</param>
		/// <returns>
		/// Returns null if the instance could not be created.
		/// </returns>
		public T CreateInstance<T>(string typeName) where T : class
		{
			Type type = Type.GetType(typeName);
			return CreateInstance<T>(type);
		}

		/// <summary>
		/// Creates an instance of the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type">The type to instantiate.</param>
		/// <returns>
		/// Returns null if the instance could not be created.
		/// </returns>
		public T CreateInstance<T>(Type type) where T : class
		{
			try
			{
				if (type == null)
				{
					throw new ArgumentNullException("type");
				}
				return (T)Activator.CreateInstance(type);
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(string.Format("Failed to load : {0}", (type != null ? type.AssemblyQualifiedName : "null")), ex);
			}
			return null;
		}

		/// <summary>
		/// Creates an instance of the specified type, requiring that the type implements the specified interface.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeName">Name of the type to instantiate.</param>
		/// <param name="requiredInterfaceTypeName">Name of the required interface that the type must implement.</param>
		/// <returns>
		/// Returns null if the instance could not be created.
		/// </returns>
		public T CreateInstanceWithRequiredInterface<T>(string typeName, string requiredInterfaceTypeName) where T : class
		{
			Type type = Type.GetType(typeName);
			Type requiredInterfaceType = Type.GetType(requiredInterfaceTypeName);
			return CreateInstanceWithRequiredInterface<T>(type, requiredInterfaceType);
		}

		/// <summary>
		/// Creates an instance of the specified type, requiring that the type implements the specified interface.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type">The type to instantiate.</param>
		/// <param name="requiredInterface">The required interface that the type must implement.</param>
		/// <returns>
		/// Returns null if the instance could not be created.
		/// </returns>
		public T CreateInstanceWithRequiredInterface<T>(Type type, Type requiredInterface) where T : class
		{
			try
			{
				if (type == null)
				{
					throw new ArgumentNullException("type");
				}
				if (requiredInterface == null)
				{
					throw new ArgumentNullException("requiredInterface");
				}
				if (type.GetInterface(requiredInterface.Name) == null)
				{
					throw new TypeLoadException(string.Format("'{0}' does not support interface '{1}'.", type.AssemblyQualifiedName, requiredInterface.AssemblyQualifiedName));
				}
				return (T)Activator.CreateInstance(type);
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(string.Format("Failed to load : {0}", (type != null ? type.AssemblyQualifiedName : "null")), ex);
			}
			return null;
		}
	}
}
