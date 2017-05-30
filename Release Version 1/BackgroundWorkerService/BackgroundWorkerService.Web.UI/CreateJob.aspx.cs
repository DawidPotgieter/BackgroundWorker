using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using WebUI.BackgroundWorkerService.Service;
using WebUI.datamodel;

namespace WebUI
{
	public partial class CreateJob : System.Web.UI.Page
	{
		[WebMethod]
		public static string Create(string name, string description, string data, string metaData, string jobType, string absoluteTimeout, string queueId, string application, string group, bool suppressHistory, bool deleteWhenDone, List<KeyValuePairStringString> additionalData, SimpleSchedule schedule)
		{
			try
			{
				TimeSpan? absoluteTimeoutValue = !string.IsNullOrEmpty(absoluteTimeout) ? (TimeSpan?)TimeSpan.Parse(absoluteTimeout) : null;
				byte queueIdValue = !string.IsNullOrEmpty(queueId) ? byte.Parse(queueId) : (byte)0;

				switch (jobType)
				{
					case "BackgroundWorkerService.Jobs.BasicHttpSoapCallbackJob, BackgroundWorkerService.Jobs":
						string callbackUrl = additionalData.First(d => d.Key == "callbackUrl").Value;
						string contractType = additionalData.First(d => d.Key == "contractType").Value;
						string securityMode = additionalData.First(d => d.Key == "securityMode").Value;
						string credentialType = additionalData.First(d => d.Key == "credentialType").Value;
						string domain = additionalData.First(d => d.Key == "domain").Value;
						string username = additionalData.First(d => d.Key == "username").Value;
						string password = additionalData.First(d => d.Key == "password").Value;
						string methodName = additionalData.First(d => d.Key == "methodName").Value;
						bool ignoreCertificateErrors = bool.Parse(additionalData.First(d => d.Key == "ignoreCertificateErrors").Value);

						try
						{
							Uri uri = new Uri(callbackUrl);
						}
						catch
						{
							throw new ArgumentException("Callback Url specified was not a valid Uri.");
						}

						if (contractType == "Basic")
						{
							metaData =
								global::BackgroundWorkerService.Jobs.JobBuilder.CreateBasicHttpSoap_BasicCallbackJobMetaData(
										metaData,
										callbackUrl,
										(System.ServiceModel.BasicHttpSecurityMode)Enum.Parse(typeof(System.ServiceModel.BasicHttpSecurityMode), securityMode),
										(System.ServiceModel.HttpClientCredentialType)Enum.Parse(typeof(System.ServiceModel.HttpClientCredentialType), credentialType),
										null,
										domain,
										username,
										password,
										ignoreCertificateErrors).Serialize();
						}
						else
						{
							if (string.IsNullOrEmpty(methodName))
								throw new ArgumentException("Method Name must be specified for Composite contracts.");
							metaData =
								global::BackgroundWorkerService.Jobs.JobBuilder.CreateBasicHttpSoap_CompositeBasicCallbackJobMetaData(
										metaData,
										methodName,
										callbackUrl,
										(System.ServiceModel.BasicHttpSecurityMode)Enum.Parse(typeof(System.ServiceModel.BasicHttpSecurityMode), securityMode),
										(System.ServiceModel.HttpClientCredentialType)Enum.Parse(typeof(System.ServiceModel.HttpClientCredentialType), credentialType),
										null,
										domain,
										username,
										password,
										ignoreCertificateErrors).Serialize();
						}
						break;
					case "BackgroundWorkerService.Jobs.SendMailJob, BackgroundWorkerService.Jobs":
						string sendFrom = additionalData.First(d => d.Key == "sendFrom").Value;
						string sendTo = additionalData.First(d => d.Key == "sendTo").Value;
						string sendSubject = additionalData.First(d => d.Key == "sendSubject").Value;
						string emailBody = additionalData.First(d => d.Key == "emailBody").Value;

						using (System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(sendFrom, sendTo, sendSubject, emailBody))
						{
							global::BackgroundWorkerService.Jobs.JobBuilder.GetSendMailJobDataAndMetaData(message, null, out data, out metaData);
						}
						break;
				}

				if (schedule == null)
				{
					CreateNewJob(name, description, data, metaData, jobType, absoluteTimeoutValue, queueIdValue, application, group, suppressHistory, deleteWhenDone);
				}
				else
				{
					ScheduleJob(name, description, data, metaData, jobType, absoluteTimeoutValue, queueIdValue, application, group, suppressHistory, deleteWhenDone, schedule);
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
			return string.Empty;
		}

		private static void CreateNewJob(string name, string description, string data, string metaData, string jobType, TimeSpan? absoluteTimeout, byte queueId, string application, string group, bool suppressHistory, bool deleteWhenDone)
		{
			CreateJobRequest request = new CreateJobRequest
			{
				Application = application,
				DeleteWhenDone = deleteWhenDone,
				Description = description,
				Name = name,
				QueueId = queueId,
				Type = jobType,
				MetaData = metaData,
				Data = data,
				SuppressHistory = suppressHistory,
				AbsoluteTimeout = absoluteTimeout,
				Group = group,
			};

			using (AccessPointClient client = new AccessPointClient())
			{
				client.CreateJob(request);
			}
		}

		private static void ScheduleJob(string name, string description, string data, string metaData, string jobType, TimeSpan? absoluteTimeout, byte queueId, string application, string group, bool suppressHistory, bool deleteWhenDone, SimpleSchedule schedule)
		{
			if (!schedule.StartDailyAt.HasValue)
				schedule.StartDailyAt = new TimeSpan();
			ScheduleJobRequest request = new ScheduleJobRequest
			{
				Application = application,
				DeleteWhenDone = deleteWhenDone,
				Description = description,
				Name = name,
				QueueId = queueId,
				Type = jobType,
				MetaData = metaData,
				Data = data,
				AbsoluteTimeout = absoluteTimeout,
				Group = group,
				CalendarSchedule = new CalendarSchedule
				{
					ScheduleType = typeof(global::BackgroundWorkerService.Logic.DataModel.Scheduling.CalendarSchedule).AssemblyQualifiedName,
					DaysOfWeek = schedule.DaysOfWeek.ToArray(),
					StartDailyAt = new TimeOfDay { Hour = schedule.StartDailyAt.Value.Hours, Minute = schedule.StartDailyAt.Value.Minutes, Second = schedule.StartDailyAt.Value.Seconds },
					RepeatInterval = schedule.RepeatInterval,
					EndDateTime = null,
					StartDateTime = DateTime.Now,
				},
			};

			using (AccessPointClient client = new AccessPointClient())
			{
				client.ScheduleJob(request);
			}
		}
	}
}