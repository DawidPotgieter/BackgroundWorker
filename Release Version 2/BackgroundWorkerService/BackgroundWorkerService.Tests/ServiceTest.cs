using BackgroundWorkerService.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BackgroundWorkerService.Tests
{
    
    
    /// <summary>
    ///This is a test class for ServiceTest and is intended
    ///to contain all ServiceTest Unit Tests
    ///</summary>
	[TestClass()]
	public class ServiceTest
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
		///A test for Service Constructor
		///</summary>
		[TestMethod()]
		public void ServiceConstructorTest()
		{
			Logic.Service target = new Logic.Service();
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for Instance
		///</summary>
		[TestMethod()]
		public void InstanceTest()
		{
			Logic.Service service = new Logic.Service();
			//service.JobManager.JobStore.CreateJob("BackgroundWorkerService.Logic.TestJobs.InfiniteRunningJob, BackgroundWorkerService.Logic", "", "", 0);
			service.Start();
			//service.JobManager.JobStore.CreateJob("BackgroundWorkerService.Logic.TestJobs.ShortRunningJob, BackgroundWorkerService.Logic", "", "", 0);
			service.Stop(true);
			service = null;
		}
	}
}
