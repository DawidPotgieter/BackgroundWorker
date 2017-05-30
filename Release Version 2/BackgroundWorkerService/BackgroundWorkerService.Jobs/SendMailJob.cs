using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackgroundWorkerService.Logic.Interfaces;
using BackgroundWorkerService.Logic.DataModel.Jobs;
using System.Net.Mail;
using BackgroundWorkerService.Jobs.DataModel;
using BackgroundWorkerService.Logic.Helpers;
using System.Net;
using System.Xml;

namespace BackgroundWorkerService.Jobs
{
	public class SendMailJob : IJob
	{
		private void Execute(MailMessage mailMessage, MailSettings settings)
		{
			SmtpClient smtpClient = null;
			try
			{
				smtpClient = new SmtpClient();
				if (settings != null)
				{
					if (!string.IsNullOrEmpty(settings.SmtpServer))	smtpClient.Host = settings.SmtpServer;
					if (settings.Port > 0) smtpClient.Port = settings.Port;
					smtpClient.EnableSsl = settings.EnableSsl;
					if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password)) smtpClient.Credentials = new NetworkCredential(settings.Username, settings.Password);
				}
				smtpClient.Send(mailMessage);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (smtpClient != null)
				{
					smtpClient.Dispose();
					smtpClient = null;
				}
			}
		}

		public JobExecutionResult Execute(string jobData, string metaData)
		{
			JobExecutionResult result = new JobExecutionResult { MetaData = metaData, ResultStatus = JobResultStatus.Fail };

			MailMessage mailMessage = null;
			if (string.IsNullOrEmpty(jobData))
			{
				result.ErrorMessage = "JobData is not a valid SerializableMailMessageWrapper object.";
				return result;
			}
			try
			{
				mailMessage = new SerializableMailMessageWrapper(jobData).GetBase();
			}
			catch (Exception ex)
			{
				result.ErrorMessage = "JobData is not a valid SerializableMailMessageWrapper object.\n\n" + Utils.GetExceptionMessage(ex);
				return result;
			}

			MailSettings mailSettings = null;
			if (!string.IsNullOrEmpty(metaData))
			{
				try
				{
					mailSettings = Utils.DeserializeObject<MailSettings>(metaData);
				}
				catch (Exception ex)
				{
					result.ErrorMessage = Utils.GetExceptionMessage(ex);
					return result;
				}
			}

			try
			{
				Execute(mailMessage, mailSettings);
				result.ResultStatus = JobResultStatus.Success;
			}
			catch (Exception ex)
			{
				result.ErrorMessage = Utils.GetExceptionMessage(ex);
			}
			return result;
		}
	}
}
