using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using BackgroundWorkerService.Jobs.DataModel;
using System.Net.Mail;
using BackgroundWorkerService.Logic.Helpers;
using System.Net;

namespace BackgroundWorkerService.Jobs
{
	public static class JobBuilder
	{
		public static BasicHttpCallbackJobMetaData CreateBasicHttpSoap_BasicCallbackJobMetaData(string metaData, string callbackUrl, BasicHttpSecurityMode securityMode, HttpClientCredentialType transportCredentialType, BasicHttpMessageCredentialType? messageCredentialType, string domain = null, string username = null, string password = null, bool ignoreCertificateErrors = false)
		{
			return new BasicHttpCallbackJobMetaData
			{
				CallbackUrl = callbackUrl,
				ContractType = ContractType.Basic,
				TransportCredentialType = transportCredentialType,
				MessageCredentialType = messageCredentialType,
				Domain = domain,
				MetaData = metaData,
				Password = password,
				SecurityMode = securityMode,
				Username = username,
				IgnoreCertificateErrors = ignoreCertificateErrors,
			};
		}

		public static CompositeBasicHttpCallbackJobMetaData CreateBasicHttpSoap_CompositeBasicCallbackJobMetaData(string metaData, string methodName, string callbackUrl, BasicHttpSecurityMode securityMode, HttpClientCredentialType transportCredentialType, BasicHttpMessageCredentialType? messageCredentialType, string domain = null, string username = null, string password = null, bool ignoreCertificateErrors = false)
		{
			return new CompositeBasicHttpCallbackJobMetaData
			{
				CallbackUrl = callbackUrl,
				MethodName = methodName,
				ContractType = ContractType.Composite,
				TransportCredentialType = transportCredentialType,
				MessageCredentialType = messageCredentialType,
				Domain = domain,
				MetaData = metaData,
				Password = password,
				SecurityMode = securityMode,
				Username = username,
				IgnoreCertificateErrors = ignoreCertificateErrors,
			};
		}

		public static void GetSendMailJobDataAndMetaData(MailMessage mailMessage, MailSettings optionalSettings, out string jobData, out string metaData)
		{
			SerializableMailMessageWrapper serializableMailMessage = new SerializableMailMessageWrapper(mailMessage);
			jobData = serializableMailMessage.ToString();
			metaData = null;
			if (optionalSettings != null)
			{
				metaData = Utils.SerializeObject<MailSettings>(optionalSettings);
			}
		}

		public static WebRequestSettings GetWebRequestJobMetaData(string url, HttpStatusCode expectedResponseCode, bool useDefaultCredentials, CredentialType? credentialType = null, string username = null, string password = null, string domain = null)
		{
			return new WebRequestSettings
			{
				Url = url,
				ExpectedResponseCode = expectedResponseCode,
				UseDefaultCredentials = useDefaultCredentials,
				CredentialType = credentialType,
				Username = username,
				Password = password,
				Domain = domain,
			};
		}
	}
}
