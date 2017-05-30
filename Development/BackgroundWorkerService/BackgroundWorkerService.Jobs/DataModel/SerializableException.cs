using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace BackgroundWorkerService.Jobs.DataModel
{
	[DataContract(Name = "SerializableException", Namespace = "http://backgroundworkerservice/jobs/DataModel/01/01/12")]
	public class SerializableException
	{
		[DataMember(Name = "Data", IsRequired = true)]
		public byte[] Data { get; set; }

		public SerializableException()
		{
		}

		public SerializableException(Exception e)
		{
			SetBase(e);
		}

		public Exception GetBase()
		{
			Exception result;
			BinaryFormatter bf = new BinaryFormatter();
			MemoryStream stream = new MemoryStream(Data);
			result = (Exception)bf.Deserialize(stream);
			stream.Close();
			return result;
		}

		public void SetBase(Exception e)
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(stream, e);
			Data = stream.ToArray();
			stream.Close();
		}
	}
}
