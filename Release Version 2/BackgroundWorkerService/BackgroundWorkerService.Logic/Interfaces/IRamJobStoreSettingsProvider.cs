using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackgroundWorkerService.Logic.Interfaces
{
	/// <summary>
	/// An interface that is required to provide settings to the RamJobStore.
	/// </summary>
	public interface IRamJobStoreSettingsProvider
	{
		long MaxHistoryRecords { get; }
	}
}
