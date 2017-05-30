BackgroundWorkerService is a Windows Service that allows you to execute multiple "Jobs" (each a simple class in .Net) concurrently in a separate process.  The service exposes it's admin interface via wcf so it is easy to control via an external process and it also has a simple UI that it exposes via a built in webserver (Cassini Derivative).
In addition to this, you can also schedule your jobs to execute at specific times (see a bit later about the scheduling supported).

If you're looking to use this project, you should also be looking at the excellent Quartz scheduler [http://quartznet.sourceforge.net/](http://quartznet.sourceforge.net/), since they have very similar goals in mind.  BackgroundWorker focuses on being simpler, easier to use, more reliable and cleaner code, but doesn't support as many features.

Some features you might be interested in :
* Firstly, it's designed to be easily extensible using provider patterns everywhere.  However, this shouldn't really be necessary
* Provides it's own little management UI (Web based that you can apply security to etc.)
* Provides an admin WCF interface to control via an external process (so you can build your own UI or control jobs from your process)
* Currently has 2 job stores, but it's simple to add more
	* RamJobStore - in memory ofc, not too useful except for testing
	* Linq2SqlJobStore - this should support any of SQL Server, SQL Express (Not tested)
* Supports multiple "Thread Pools" called "Execution Queues" (You can have up to 255 different queues of the different types below)
	* ThreadPoolExecutionQueue - This is the .net threadpool, bit fire and forget and limited in the amount of actual active threads it can execute
	* ThreadExecutionQueue - Allows any number of threads concurrently and also allows thread shutdown (very important if you don't have control of the process being called)
	* TimedThreadExecutionQueue - This uses 2 threads per job, but allows the job to have an absolute timeout and thread shutdown capability
* One feature I use very often is Callbacks.  The main gist is this : Your process registers a job (or it's scheduled externally).  When the job executes, it calls a webservice via wcf.  Benefits of using callbacks :
	* This allows you to keep domain specific logic inside each application and have one central job service delegate when to call these
	* A good example is when you have more than one website that have jobs/schedules to execute.  You keep each website's logic inside the website, host a little webservice inside each website (very simple and examples provided) and you never have to deploy dll's to the central scheduling service.  More on this later.
* You have an execution history.  This has usually been a requirement where I've used it, but you can switch this off on a job by job basis.
* Uses very few external dependencies - just CassiniDev and Common.Logging currently.
* Quite simple to add your own logging provider using Common.Logging (Uses nLog out of the box).
* Alert generation - All job failures generate an alert that's available both via the UI and the WCF admin endpoint.

![](https://github.com/DawidPotgieter/BackgroundWorker/blob/master/docs/Home_ss1.png)
![](https://github.com/DawidPotgieter/BackgroundWorker/blob/master/docs/Home_s2.png)
![](https://github.com/DawidPotgieter/BackgroundWorker/blob/master/docs/Home_ss3.png)
![](https://github.com/DawidPotgieter/BackgroundWorker/blob/master/docs/Home_ss4.png)
![](https://github.com/DawidPotgieter/BackgroundWorker/blob/master/docs/Home_ss5.png)
