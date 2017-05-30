using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Mail;

namespace BackgroundWorkerService.Jobs.DataModel
{
  [DataContract(Name = "SerializableMailMessage", Namespace = "http://backgroundworkerservice/jobs/DataModel/01/01/12")]
  public class SerializableMailMessageWrapper
  {
    [DataMember(Name = "Data", IsRequired = true)]
    public byte[] Data { get; set; }

    public SerializableMailMessageWrapper()
    {
    }

    public SerializableMailMessageWrapper(MailMessage mailMessage)
    {
      SetBase(mailMessage);
    }

		public SerializableMailMessageWrapper(string base64Data)
    {
      try
      {
				Data = Convert.FromBase64String(base64Data);
        var message = GetBase();
        if (message == null) throw new ArgumentException("base64Data");
      }
      catch (Exception ex)
      {
        throw new FormatException("base64Data is not a valid MailMessage", ex);
      }
    }

		public override string ToString()
		{
			return Convert.ToBase64String(Data);
		}

    public MailMessage GetBase()
    {
			SerializeableMailMessage msg;
      BinaryFormatter bf = new BinaryFormatter();
      MemoryStream stream = new MemoryStream(Data);
			msg = (SerializeableMailMessage)bf.Deserialize(stream);
      stream.Close();
      return msg.GetMailMessage();
    }

    public void SetBase(MailMessage mailMessage)
    {
			SerializeableMailMessage msg = new SerializeableMailMessage(mailMessage);
      MemoryStream stream = new MemoryStream();
      BinaryFormatter bf = new BinaryFormatter();
      bf.Serialize(stream, msg);
      Data = stream.ToArray();
      stream.Close();
    }
  }
}