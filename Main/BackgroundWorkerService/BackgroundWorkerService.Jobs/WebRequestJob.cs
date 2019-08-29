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
	/// This job simply 'Polls' a URL using a web GET request.  Use for uptime checking and also to keep your IIS application warmed up.
	/// </summary>
	public class WebRequestJob : IJob
	{
		public JobExecutionResult Execute(string jobData, string metaData)
		{
			JobExecutionResult result = new JobExecutionResult { ResultStatus = JobResultStatus.Fail };

			try
			{
				WebRequestSettings settings = Utils.DeserializeObject<WebRequestSettings>(metaData);
				if (settings == null)
				{
					throw new ArgumentException("Metadata does not contain a valid WebRequestSettings serialized object.");
				}
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(settings.Url);
				webRequest.Method = "GET";
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
