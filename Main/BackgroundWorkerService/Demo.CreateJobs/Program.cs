using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demo.CreateJobs.BWS;
using BackgroundWorkerService.Jobs;

namespace Demo.CreateJobs
{
	class Program
	{
		static void Main(string[] args)
		{
			using (AccessPointClient client = new AccessPointClient())
			{
				for (int i = 0; i < 100; i++)
				{
					var job = client.CreateJob(new CreateJobRequest
					{
						Type = typeof(BackgroundWorkerService.Logic.TestJobs.ShortRunningJob).AssemblyQualifiedName,
						QueueId = (byte)new Random().Next(3),
						Name = "Job " + new Random().Next(int.MaxValue),
						Description = "This is a test description to see what it looks like : " + new Random().Next(int.MaxValue),
						Status = JobStatus.Ready,
					});
				}

				//for (int i = 0; i < 20; i++)
				//{
				//  var job = client.CreateJob(new CreateJobRequest
				//  {
				//    Data = "bob",
				//    MetaData =
				//      JobBuilder.CreateBasicHttpSoap_BasicCallbackJobMetaData(
				//        "metaData",
				//        "http://localhost:50889/BasicJobCallback.svc",
				//        System.ServiceModel.BasicHttpSecurityMode.None,
				//        System.ServiceModel.HttpClientCredentialType.None,
				//        null).Serialize(),
				//    Type = typeof(BasicHttpSoapCallbackJob).AssemblyQualifiedName,
				//    QueueId = (byte)new Random().Next(3),
				//    CalendarSchedule = new CalendarSchedule
				//    {
				//      DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday }.ToArray(),
				//      StartDateTime = DateTime.Now,
				//      StartDailyAt = new TimeOfDay { Hour = 0, Minute = 0, Second = 0 },
				//      RepeatInterval = new TimeSpan(0, 0, 10),
				//    }
				//  });
				//}
				//for (int i = 0; i < 100; i++)
				//{
				//  var job = client.CreateJob(new CreateJobRequest
				//  {
				//    Data = "bob",
				//    MetaData =
				//      JobBuilder.CreateBasicHttpSoap_BasicCallbackJobMetaData(
				//        "metaData",
				//        "http://localhost:50889/BasicJobCallback.svc",
				//        System.ServiceModel.BasicHttpSecurityMode.None,
				//        System.ServiceModel.HttpClientCredentialType.None,
				//        null).Serialize(),
				//    Type = typeof(BasicHttpSoapCallbackJob).AssemblyQualifiedName,
				//    QueueId = 0,// (byte)new Random().Next(3),
				//    CalendarSchedule = new CalendarSchedule
				//    {
				//      ScheduleType = typeof(BackgroundWorkerService.Logic.DataModel.Scheduling.CalendarSchedule).AssemblyQualifiedName, //Must set this
				//      DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday }.ToArray(),
				//      StartDateTime = DateTime.Now,
				//      StartDailyAt = new TimeOfDay { Hour = 0, Minute = 0, Second = 0 },
				//      RepeatInterval = new TimeSpan(0, 0, 10),
				//    }
				//  });
				//}

				//var job = client.CreateJob(new CreateJobRequest
				//{
				//  Data = "bob",
				//  MetaData =
				//    JobBuilder.CreateBasicHttpSoap_BasicCallbackJobMetaData(
				//      "metaData",
				//      "http://localhost:50889/BasicJobCallback.svc",
				//      System.ServiceModel.BasicHttpSecurityMode.None,
				//      System.ServiceModel.HttpClientCredentialType.None,
				//      null).Serialize(),
				//  Type = typeof(BasicHttpSoapCallbackJob).AssemblyQualifiedName,
				//  QueueId = (byte)new Random().Next(3),
				//  CalendarSchedule = new CalendarSchedule
				//  {
				//    ScheduleType = typeof(BackgroundWorkerService.Logic.DataModel.Scheduling.CalendarSchedule).AssemblyQualifiedName, //Must set this
				//    DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday }.ToArray(),
				//    StartDateTime = DateTime.Now,
				//    StartDailyAt = new TimeOfDay { Hour = 0, Minute = 0, Second = 0 },
				//    RepeatInterval = new TimeSpan(0, 1, 0),
				//  },
				//});

				//var job2 = client.CreateJob(new CreateJobRequest
				//{
				//  Data = "bob",
				//  MetaData =
				//    JobBuilder.CreateBasicHttpSoap_BasicCallbackJobMetaData(
				//      "metaData",
				//      "http://localhost:50889/BasicJobCallbackCodeOnly.svc",
				//      System.ServiceModel.BasicHttpSecurityMode.None,
				//      System.ServiceModel.HttpClientCredentialType.None,
				//      null).Serialize(),
				//  Type = typeof(BasicHttpSoapCallbackJob).AssemblyQualifiedName,
				//  QueueId = 0,
				//});
				//string data;
				//string metadata;
				//JobBuilder.GetSendMailJobDataAndMetaData(new System.Net.Mail.MailMessage("a@b.com", "a@b.com"), null, out data, out metadata);
				//var job3 = client.CreateJob(new CreateJobRequest
				//{
				//  Data = data,
				//  MetaData = metadata,
				//  Type = typeof(SendMailJob).AssemblyQualifiedName,
				//  QueueId = 1,
				//  CalendarSchedule = new CalendarSchedule
				//  {
				//    ScheduleType = typeof(BackgroundWorkerService.Logic.DataModel.Scheduling.CalendarSchedule).AssemblyQualifiedName, //Must set this
				//    DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday }.ToArray(),
				//    StartDateTime = DateTime.Now,
				//    StartDailyAt = new TimeOfDay { Hour = 0, Minute = 30, Second = 0 },
				//  }
				//});
				//var job4 = client.CreateJob(new CreateJobRequest
				//{
				//  Data = "",
				//  MetaData = 
				//    JobBuilder.GetWebRequestJobMetaData(
				//      "http://localhost:2048/default.aspx",
				//      System.Net.HttpStatusCode.OK,
				//      true).Serialize(),
				//  Type = typeof(WebRequestJob).AssemblyQualifiedName,
				//  QueueId = 0,
				//  DeleteWhenDone = false,
				//  SuppressHistory = false,
				//  Name = "Ping Service WebUI",
				//  Description = "This pings the web ui to check whether it's still running.",
				//});
			}
		}
	}
}
