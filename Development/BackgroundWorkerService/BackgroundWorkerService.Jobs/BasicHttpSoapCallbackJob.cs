using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using BackgroundWorkerService.Jobs.DataModel;
using System.ServiceModel;
using BackgroundWorkerService.Jobs.DataModel.Response;
using BackgroundWorkerService.Logic.Helpers;
using BackgroundWorkerService.Jobs.DataModel.Request;
using BackgroundWorkerService.Jobs.Contracts;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.IdentityModel.Selectors;
using System.ServiceModel.Description;
using System.Web;

namespace BackgroundWorkerService.Jobs
{
	public class BasicHttpSoapCallbackJob : IJob
	{
		private static Dictionary<string, int> IgnoreSSLCertificatesForUriList = InitializeServerCertificateValidationCallback();

		private static Dictionary<string, int> InitializeServerCertificateValidationCallback()
		{
			//This convoluted bit is used to ensure SSL (transport security) certificates are only ignored for urls where the job has it specified.
			ServicePointManager.ServerCertificateValidationCallback = delegate(object certsender, X509Certificate cert, X509Chain chain, System.Net.Security.SslPolicyErrors errors)
			{
				HttpWebRequest request = certsender as HttpWebRequest;
				if (request != null)
				{
					string uri = request.RequestUri.ToString();
					bool ignoreCertificateError = false;
					lock (IgnoreSSLCertificatesForUriList)
					{
						ignoreCertificateError = IgnoreSSLCertificatesForUriList.ContainsKey(uri) && IgnoreSSLCertificatesForUriList[uri] > 0;
					}
					if (ignoreCertificateError)
					{
						lock (IgnoreSSLCertificatesForUriList)
						{
							if (IgnoreSSLCertificatesForUriList[uri] > 1)
							{
								IgnoreSSLCertificatesForUriList[uri]--;
							}
							else
							{
								//Since we're now blindly accepting the ssl cert, we have to make sure that as soon as the connection is closed, the underlying socket is closed too once the call counter has reached 0.
								//This has a slight performance penalty, but only when you have different jobs using the same callback url with different IgnoreCertificateErrors settings.
								ServicePoint svcPoint = ServicePointManager.FindServicePoint(request.RequestUri);
								svcPoint.MaxIdleTime = 0;
								IgnoreSSLCertificatesForUriList.Remove(uri);
							}
						}

						return true;
					}
				}
				return errors == SslPolicyErrors.None;
			};

			return new Dictionary<string, int>();
		}

		private JobExecutionResult Execute(string jobData, BasicHttpCallbackJobMetaData callbackJobData)
		{
			JobExecutionResult result = new JobExecutionResult();

			BasicHttpBinding binding = new BasicHttpBinding(callbackJobData.SecurityMode);
			binding.MaxReceivedMessageSize = callbackJobData.MessageSize;
			binding.ReaderQuotas.MaxStringContentLength = callbackJobData.MessageSize;
			binding.MaxBufferSize = callbackJobData.MessageSize;
			binding.ReceiveTimeout = TimeSpan.FromMilliseconds(callbackJobData.TimeoutMilliseconds);
			binding.SendTimeout = TimeSpan.FromMilliseconds(callbackJobData.TimeoutMilliseconds);
			binding.Security.Transport.ClientCredentialType = callbackJobData.TransportCredentialType;
			if (callbackJobData.MessageCredentialType.HasValue)
			{
				binding.Security.Message.ClientCredentialType = callbackJobData.MessageCredentialType.Value;
			}

			//This convoluted bit is used to ensure SSL (transport security) certificates are only ignored for urls where the job has it specified.
			if (callbackJobData.IgnoreCertificateErrors)
			{
				//The logic is not foolproof.  If you have 2 requests at the same time to the same host with different IgnoreCertificateErrors settings, the first one in will win.
				//The fix is to not use different settings for IgnoreCertificateErrors when calling the same host.
				lock (IgnoreSSLCertificatesForUriList)
				{
					if (!IgnoreSSLCertificatesForUriList.ContainsKey(callbackJobData.CallbackUrl))
					{
						IgnoreSSLCertificatesForUriList.Add(callbackJobData.CallbackUrl, 1);
					}
					else
					{
						//Store a counter for how many times this url has been called with the ignore bit set.
						IgnoreSSLCertificatesForUriList[callbackJobData.CallbackUrl]++;
					}
				}
			}

			EndpointAddress endpoint = new EndpointAddress(callbackJobData.CallbackUrl);

			Response response = null;

			switch (callbackJobData.ContractType)
			{
				case ContractType.Basic:
					ChannelFactory<IBasicJobCallback> channelFactory = new ChannelFactory<IBasicJobCallback>(binding, endpoint);

					if (callbackJobData.IgnoreCertificateErrors)
					{
						//Ignore X509 certs for message security
						channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
						channelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
					}

					if (!string.IsNullOrEmpty(callbackJobData.Username) && !string.IsNullOrEmpty(callbackJobData.Password))
					{
						//Even though Credentials and Credentials.Windows is used for different authentication mechanisms, it works just setting all like this
						channelFactory.Credentials.UserName.UserName = callbackJobData.Username;
						channelFactory.Credentials.UserName.Password = callbackJobData.Password;
						channelFactory.Credentials.Windows.ClientCredential.Domain = callbackJobData.Domain;
						channelFactory.Credentials.Windows.ClientCredential.UserName = callbackJobData.Username;
						channelFactory.Credentials.Windows.ClientCredential.Password = callbackJobData.Password;
					}

					IBasicJobCallback basicJobCallbackService = channelFactory.CreateChannel();
					try
					{
						response = basicJobCallbackService.Execute(new BasicJobCallbackRequest { Data = jobData, MetaData = callbackJobData.MetaData });
					}
					catch
					{
						throw;
					}
					finally
					{
						try
						{
							channelFactory.Close();
						}
						catch { }
					}
					break;
				case ContractType.Composite:
					ChannelFactory<ICompositeJobCallback> compositeChannelFactory = new ChannelFactory<ICompositeJobCallback>(binding, endpoint);

					if (callbackJobData.IgnoreCertificateErrors)
					{
						//Ignore X509 certs for message security
						compositeChannelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
						compositeChannelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
					}

					if (!string.IsNullOrEmpty(callbackJobData.Username) && !string.IsNullOrEmpty(callbackJobData.Password))
					{
						//Even though Credentials and Credentials.Windows is used for different authentication mechanisms, it works just setting all like this
						compositeChannelFactory.Credentials.UserName.UserName = callbackJobData.Username;
						compositeChannelFactory.Credentials.UserName.Password = callbackJobData.Password;
						compositeChannelFactory.Credentials.Windows.ClientCredential.Domain = callbackJobData.Domain;
						compositeChannelFactory.Credentials.Windows.ClientCredential.UserName = callbackJobData.Username;
						compositeChannelFactory.Credentials.Windows.ClientCredential.Password = callbackJobData.Password;
					}

					CompositeBasicHttpCallbackJobMetaData metaData = (CompositeBasicHttpCallbackJobMetaData)callbackJobData;

					ICompositeJobCallback compositeJobCallbackService = compositeChannelFactory.CreateChannel();
					try
					{
						response = compositeJobCallbackService.Execute(new CompositeBasicJobCallbackRequest { Data = jobData, MetaData = metaData.MetaData, MethodName = metaData.MethodName });
					}
					catch
					{
						throw;
					}
					finally
					{
						try
						{
							compositeChannelFactory.Close();
						}
						catch { }
					}
					break;
			}

			if (response != null)
			{
				if (response.Result != null)
				{
					callbackJobData.MetaData = response.Result.MetaData;
					if (callbackJobData.ContractType == ContractType.Basic)
					{
						result.MetaData = callbackJobData.Serialize();
					}
					else
					{
						result.MetaData = ((CompositeBasicHttpCallbackJobMetaData)callbackJobData).Serialize();
					}
				}
				else
				{
					if (callbackJobData.ContractType == ContractType.Basic)
					{
						result.MetaData = callbackJobData.Serialize();
					}
					else
					{
						result.MetaData = ((CompositeBasicHttpCallbackJobMetaData)callbackJobData).Serialize();
					}
					result.ResultStatus = JobResultStatus.Fail;
					result.ErrorMessage = "Response.Result was null.";
				}

				if (response.Exception != null)
				{
					Exception ex = response.Exception.GetBase();
					result.ResultStatus = JobResultStatus.Fail;
					result.ErrorMessage = Utils.GetExceptionMessage(ex);
				}
				else
				{
					result.ResultStatus = response.Result.ResultStatus;
					result.ErrorMessage = response.Result.ErrorMessage;
				}
			}

			return result;
		}

		public JobExecutionResult Execute(string jobData, string metaData)
		{
			JobExecutionResult result = new JobExecutionResult { ResultStatus = JobResultStatus.Fail, ErrorMessage = "Initialization Error.", MetaData = metaData };
			CompositeBasicHttpCallbackJobMetaData compositeCallbackJobData = null;
			BasicHttpCallbackJobMetaData basicCallbackJobData = null;
			try
			{
				if (string.IsNullOrEmpty(metaData))
				{
					throw new ArgumentException("MetaData does not contain valid data.");
				}
				if (metaData.Contains("<CompositeBasicHttpCallbackJobMetaData "))
				{
					compositeCallbackJobData = Utils.DeserializeObject<CompositeBasicHttpCallbackJobMetaData>(metaData);
				}
				else if (metaData.Contains("<BasicHttpCallbackJobMetaData "))
				{
					basicCallbackJobData = Utils.DeserializeObject<BasicHttpCallbackJobMetaData>(metaData);
				}
				else
				{
					throw new ArgumentException("MetaData does not contain valid data.");
				}
			}
			catch (Exception ex)
			{
				result.ErrorMessage = Utils.GetExceptionMessage(ex);
			}
			if (compositeCallbackJobData != null)
			{
				result = Execute(jobData, compositeCallbackJobData);
			}
			else if (basicCallbackJobData != null)
			{
				result = Execute(jobData, basicCallbackJobData);
			}

			return result;
		}
	}
}