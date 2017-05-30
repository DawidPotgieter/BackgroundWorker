using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using BackgroundWorkerService.Logic.Helpers;

namespace BackgroundWorkerService.Jobs.DataModel
{
	public class CompositeBasicHttpCallbackJobMetaData : BasicHttpCallbackJobMetaData
	{
		public string MethodName { get; set; }

		public new string Serialize()
		{
			return Utils.SerializeObject<CompositeBasicHttpCallbackJobMetaData>(this);
		}
	}
}
