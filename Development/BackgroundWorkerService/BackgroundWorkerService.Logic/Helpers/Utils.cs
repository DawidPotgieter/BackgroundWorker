using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Common.Logging;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.Implementation;

namespace BackgroundWorkerService.Logic.Helpers
{
	/// <summary>
	/// Helper class that doesn't fit anywhere else (or I was to lazy to implement).
	/// </summary>
	public class Utils
	{
		/// <summary>
		/// This is a very important line.  It determines what type resolver and activator will be used to load types.  Reflection is the default and uses 
		/// the reflection activator (normal Activator.CreateInstance), but you can change this one line to have it load types from a database or remoting etc.
		/// </summary>
		private static ITypeResolver typeResolver = new ReflectionTypeResolver();

		/// <summary>
		/// Creates a string representation of the exception, including innerexception tree and stacktraces.
		/// </summary>
		/// <param name="ex">Exception to convert to string.</param>
		/// <returns></returns>
		public static string GetExceptionMessage(Exception ex)
		{
			return ex.ToString();
		}

		/// <summary>
		/// Tries to get the specified configuration section from the running process' configuration file.
		/// </summary>
		/// <typeparam name="U">The configuration section implementation type</typeparam>
		/// <returns></returns>
		internal static U GetConfigurationSection<U>() where U : ConfigurationSection
		{
			U ret = null;
			try
			{
				ret = ConfigurationManager.GetSection(typeof(U).FullName) as U;
				if (ret == null)
				{
					throw new ConfigurationErrorsException("The Configuration Section has not been setup for " +
																					 typeof(U).FullName);
				}
				return ret;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the specified named connection string from the running process' configuration file.
		/// </summary>
		/// <param name="connectionStringName">Name of the connection string.</param>
		internal static string GetConnectionString(string connectionStringName)
		{
			return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
		}

		/// <summary>
		/// Creates an instance of T from the specified typeName, and ensures it implements the required interface type.
		/// </summary>
		/// <typeparam name="T">Type of class to return.</typeparam>
		/// <param name="typeName">Name of the type to try and load.</param>
		/// <param name="requiredInterfaceTypeName">Name of the required interface that type must implement.</param>
		/// <returns>
		/// Null if the object could not be loaded.
		/// </returns>
		internal static T CreateInstanceWithRequiredInterface<T>(string typeName, string requiredInterfaceTypeName) where T : class
		{
			ITypeActivator typeActivator = typeResolver.GetTypeActivator(typeName);
			return typeActivator.CreateInstanceWithRequiredInterface<T>(typeName, requiredInterfaceTypeName);
		}

		/// <summary>
		/// Creates an instance of T of the specified type, and ensures it implements the required interface type.
		/// </summary>
		/// <typeparam name="T">Type of class to return.</typeparam>
		/// <param name="type">The type to try and load.</param>
		/// <param name="requiredInterface">The required interface that type must implement.</param>
		/// <returns>
		/// Null if the object could not be loaded.
		/// </returns>
		internal static T CreateInstanceWithRequiredInterface<T>(Type type, Type requiredInterface) where T : class
		{
			ITypeActivator typeActivator = typeResolver.GetTypeActivator(type);
			return typeActivator.CreateInstanceWithRequiredInterface<T>(type, requiredInterface);
		}

		/// <summary>
		/// Serializes the object using <see cref="XmlSerializer"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataObject">The data object.</param>
		/// <returns></returns>
		public static string SerializeObject<T>(object dataObject) where T : class
		{
			return SerializeObject(dataObject, typeof(T));
		}

		/// <summary>
		/// Serializes the object using <see cref="XmlSerializer"/>.
		/// </summary>
		/// <param name="dataObject">The data object.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <returns></returns>
		public static string SerializeObject(object dataObject, Type objectType)
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(objectType);
				StringBuilder stringBuilder = new StringBuilder();
				using (StringWriter writer = new StringWriter(stringBuilder))
				{
					serializer.Serialize(writer, dataObject);
					return stringBuilder.ToString();
				}
			}
			catch { }
			return null;
		}

		/// <summary>
		/// Deserializes the object using <see cref="XmlSerializer"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public static T DeserializeObject<T>(string data) where T : class
		{
			return (T)DeserializeObject(data, typeof(T));
		}

		/// <summary>
		/// Deserializes the object using <see cref="XmlSerializer"/>.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <returns></returns>
		public static object DeserializeObject(string data, Type targetType)
		{
			XmlSerializer serializer = new XmlSerializer(targetType);
			using (StringReader reader = new StringReader(data))
			{
				object returnValue = serializer.Deserialize(reader);
				return returnValue;
			}
		}
	}
}
