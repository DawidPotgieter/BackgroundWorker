using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using System.Net;
using BackgroundWorkerService.Jobs.DataModel;
using BackgroundWorkerService.Logic.Helpers;

namespace BackgroundWorkerService.Jobs
{
	/// <summary>
	/// This job sends data to an endpoint using a web POST request. Use for polling Post endpoints.
	/// </summary>
	public class WebPostJob : IJob
	{
		public JobExecutionResult Execute(string jobData, string metaData)
		{
			JobExecutionResult result = new JobExecutionResult { ResultStatus = JobResultStatus.Fail };

			try
			{
				WebPostSettings settings = Utils.DeserializeObject<WebPostSettings>(metaData);
				if (settings == null)
				{
					throw new ArgumentException("Metadata does not contain a valid WebPostSettings serialized object.");
				}
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(settings.Url);
				webRequest.Method = "POST";
				if (settings.UseDefaultCredentials)
				{
					webRequest.UseDefaultCredentials = true;
				}
				else
				{
					if (!settings.CredentialType.HasValue)
					{
						throw new ArgumentException("If UseDefaultCredentials == true, then CredentialType has to be specified");
					}

					webRequest.UseDefaultCredentials = false;
					NetworkCredential networkCredential = new NetworkCredential(settings.Username, settings.Password);
					if (!string.IsNullOrEmpty(settings.Domain)) networkCredential.Domain = settings.Domain;

					CredentialCache credentialCache = new CredentialCache();
					credentialCache.Add(webRequest.RequestUri, settings.CredentialType.Value.ToString(), networkCredential);
					webRequest.Credentials = networkCredential;
				}

				JobBuilder.AddRequestHeaders(webRequest, settings.Headers);

				if (settings.TimeoutMilliseconds > 0)
				{
					webRequest.Timeout = settings.TimeoutMilliseconds;
				}

				byte[] postData = new ASCIIEncoding().GetBytes(settings.Content);
				var length = postData.Length;
				webRequest.ContentLength = length;
				if (length > 0)
				{
					webRequest.GetRequestStream().Write(postData, 0, length);
				}

				HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
				if (webResponse.StatusCode == settings.ExpectedResponseCode)
				{
					result.ResultStatus = JobResultStatus.Success;
				}
			}
			catch (Exception ex)
			{
				result.ErrorMessage = Utils.GetExceptionMessage(ex);
			}

			return result;
		}
	}
}
