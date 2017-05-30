﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BackgroundWorkerService.Service.Admin.DataModel.Response
{
	[DataContract(Name = "UpdateJobResponse", Namespace = "http://backgroundworkerservice/DataModel/01/01/12")]
	public class UpdateJobResponse
	{
		[DataMember(Name = "Success", IsRequired = true)]
		public bool Success { get; set; }
	}
}
