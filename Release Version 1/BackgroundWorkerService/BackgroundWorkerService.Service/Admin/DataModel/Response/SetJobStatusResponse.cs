﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Response
{
	[DataContract(Name = "SetJobStatusResponse", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class SetJobStatusResponse
	{
		[DataMember(Name = "Success", IsRequired = true)]
		public bool Success { get; set; }
	}
}
