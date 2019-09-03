using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using BackgroundWorkerService.Jobs.DataModel;
using System.Net.Mail;
using BackgroundWorkerService.Logic.Helpers;
using System.Net;
using BackgroundWorkerService.Logic.DataModel;

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

		public static WebRequestSettings GetWebRequestJobMetaData(string url, HttpStatusCode expectedResponseCode, bool useDefaultCredentials, CredentialType? credentialType = null, string username = null, string password = null, string domain = null, List<WebRequestHeader> headers = null, int timeoutMilliseconds = 0)
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
				Headers = headers,
				TimeoutMilliseconds = timeoutMilliseconds
			};
		}

		public static WebPostSettings GetWebPostJobMetaData(string url, HttpStatusCode expectedResponseCode, bool useDefaultCredentials, List<WebRequestHeader> headers, string content = "", CredentialType? credentialType = null, string username = null, string password = null, string domain = null, int timeoutMilliseconds = 0)
		{
			return new WebPostSettings
			{
				Url = url,
				ExpectedResponseCode = expectedResponseCode,
				UseDefaultCredentials = useDefaultCredentials,
				CredentialType = credentialType,
				Username = username,
				Password = password,
				Domain = domain,
				Headers = headers,
				Content = content,
				TimeoutMilliseconds = timeoutMilliseconds
			};
		}

		public static void AddRequestHeaders(HttpWebRequest webRequest, List<WebRequestHeader> headers)
		{
			foreach (var header in headers)
			{
				switch (header.Header)
				{
					case HttpRequestHeader.Accept:
						webRequest.Accept = header.Value;
						break;
					case HttpRequestHeader.Connection:
						webRequest.Connection = header.Value;
						break;
					case HttpRequestHeader.ContentType:
						webRequest.ContentType = header.Value;
						break;
					case HttpRequestHeader.Expect:
						webRequest.Expect = header.Value;
						break;
					case HttpRequestHeader.Date:
						webRequest.Date = DateTime.Parse(header.Value);
						break;
					case HttpRequestHeader.Host:
						webRequest.Host = header.Value;
						break;
					case HttpRequestHeader.IfModifiedSince:
						webRequest.IfModifiedSince = DateTime.Parse(header.Value);
						break;
					case HttpRequestHeader.Range:
						try
						{
							var range = header.Value.Split(',');
							if (range.Length != 2) throw new Exception("Range header value not formatted as two comma separated integers");
							var from = int.Parse(range[0]);
							var to = int.Parse(range[1]);
							webRequest.AddRange(from, to);
						}
						catch (Exception e)
						{
							throw new ArgumentException("Range Header Value is invalid. See inner exception for details.", e);
						}
						break;
					case HttpRequestHeader.Referer:
						webRequest.Referer = header.Value;
						break;
					case HttpRequestHeader.TransferEncoding:
						webRequest.TransferEncoding = header.Value;
						break;
					case HttpRequestHeader.UserAgent:
						webRequest.UserAgent = header.Value;
						break;
					default:
						webRequest.Headers.Add(header.Header, header.Value);
						break;
				}
			}
		}
	}
}
