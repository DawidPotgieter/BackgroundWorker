using BackgroundWorkerService.Logic.DataModel.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BackgroundWorkerService.Tests
{
    
    
    /// <summary>
    ///This is a test class for CalendarScheduleTest and is intended
    ///to contain all CalendarScheduleTest Unit Tests
    ///</summary>
	[TestClass()]
	public class CalendarScheduleTest
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
		///A test for GetNextOccurrence
		///</summary>
		[TestMethod()]
		public void GetNextOccurrenceTest()
		{
			CalendarSchedule target = new CalendarSchedule();
			//target
				//.StartDailyAt(new TimeOfDay { Hour = 9, Minute = 52 })
				//.EndingAt(new DateTime(2012, 1, 6, 11, 02, 59))
				//.WithRepeatInterval(new TimeSpan(0, 3, 0))
				//.OnDaysOfWeek(new System.Collections.Generic.HashSet<DayOfWeek> { DayOfWeek.Friday });
				//.OnDaysOfMonth(new System.Collections.Generic.HashSet<int> { 7 });
			DateTime afterDateTime = DateTime.Now;
			Nullable<DateTime> expected = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
			Nullable<DateTime> actual;
			actual = target.GetNextOccurrence(afterDateTime);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
	}
}
