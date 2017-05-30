using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Logic.Helpers
{
	/// <summary>
	/// Helper utilities.
	/// </summary>
	public class Utils
	{
		/// <summary>
		/// Creates a string representation of the exception, including innerexception tree and stacktraces.
		/// </summary>
		/// <param name="ex">Exception to convert to string.</param>
		/// <returns></returns>
		public static string GetExceptionMessage(Exception ex)
		{
			string message =
				"MESSAGE: " + ex.Message +
				"\nSOURCE: " + ex.Source;

			message += "\nSTACKTRACE: " + ex.StackTrace;

			if (ex.InnerException != null)
				message += "\n\nINNEREXCEPTION: \n\n" + GetExceptionMessage(ex.InnerException);

			return message;
		}

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

		internal static string GetConnectionString(string connectionStringName)
		{
			try
			{
				return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			}
			catch
			{
				throw;
			}
		}

		internal static object CreateInstanceWithRequiredInterface(string typeName, string requiredInterfaceTypeName)
		{
			try
			{
				Type type = Type.GetType(typeName);
				if (type == null)
				{
					throw new TypeLoadException(string.Format("'{0} could not be loaded.", typeName));
				}
				if (type.GetInterface(requiredInterfaceTypeName) == null)
				{
					throw new TypeLoadException(string.Format("'{0}' does not support interface '{1}'.", typeName, requiredInterfaceTypeName));
				}
				return Activator.CreateInstance(Type.GetType(typeName));
			}
			catch (Exception ex)
			{
				BackgroundWorkerService.Logic.Configuration.ConfigurationSettings.LoggingProvider.LogException(string.Format("Failed to load : {0}", typeName), ex);
			}
			return null;
		}

		internal static object CreateInstanceWithRequiredInterface(Type type, Type requiredInterface)
		{
			return CreateInstanceWithRequiredInterface(type.AssemblyQualifiedName, requiredInterface.Name);
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
