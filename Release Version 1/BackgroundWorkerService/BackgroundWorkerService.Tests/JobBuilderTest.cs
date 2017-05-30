using BackgroundWorkerService.Jobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Mail;
using BackgroundWorkerService.Jobs.DataModel;

namespace BackgroundWorkerService.Tests
{
    
    
    /// <summary>
    ///This is a test class for JobBuilderTest and is intended
    ///to contain all JobBuilderTest Unit Tests
    ///</summary>
	[TestClass()]
	public class JobBuilderTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for GetSendMailJobDataAndMetaData
		///</summary>
		[TestMethod()]
		public void GetSendMailJobDataAndMetaDataTest()
		{
			MailMessage mailMessage = new MailMessage("a@b.com", "a@b.com", "subject", "body");
			MailSettings optionalSettings = null; // TODO: Initialize to an appropriate value
			string jobData = string.Empty; // TODO: Initialize to an appropriate value
			string jobDataExpected = string.Empty; // TODO: Initialize to an appropriate value
			string metaData = string.Empty; // TODO: Initialize to an appropriate value
			string metaDataExpected = string.Empty; // TODO: Initialize to an appropriate value
			JobBuilder.GetSendMailJobDataAndMetaData(mailMessage, optionalSettings, out jobData, out metaData);

			var newmsg = new SerializableMailMessageWrapper(jobData).GetBase();

			Assert.AreEqual(jobDataExpected, jobData);
			Assert.AreEqual(metaDataExpected, metaData);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}
	}
}
