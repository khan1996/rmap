using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace rMap
{
    static class AsyncLoader
    {
        public static List<AsyncLoaderBase> runningInstances = new List<AsyncLoaderBase>();
        public static object runningLock = new object();

        public static void Start(Action action, Action onSuccess, Action<Exception> onError, bool useWaitCursor)
        {
            if (useWaitCursor)
            {
                rMapForm.Instance.UseWaitCursor = true;
                rMapForm.Instance.statusProgress.Visible = true;
                rMapForm.Instance.statusText.Text = "Working...";
            }

            var runningInstance = new AsyncLoaderA(action, delegate
            {
                if (useWaitCursor)
                {
                    bool anyundone = false;
                    lock (runningLock)
                        anyundone = runningInstances.Any(x => x.Msg && !x.Done);

                    if (!anyundone)
                    {
                        rMapForm.Instance.UseWaitCursor = false;
                        rMapForm.Instance.statusProgress.Visible = false;
                        rMapForm.Instance.statusText.Text = "Ready";
                    }
                }
            },
            onSuccess, onError) { Msg = useWaitCursor };

            lock(runningLock)
                runningInstances.Add(runningInstance);

            runningInstance.Start();
        }

        public static void Shutdown()
        {
            lock(runningLock)
                foreach (AsyncLoaderBase async in runningInstances.ToArray())
                    async.Dispose();
        }
    }


    class AsyncLoaderA : AsyncLoaderBase
    {
        Action run;
        Action runDone;
        Action runSuccess;
        Action<Exception> runError;

        public AsyncLoaderA(Action func, Action done, Action success, Action<Exception> error)
        {
            run = func;
            runDone = done;
            runError = error;
            runSuccess = success;
        }

        protected override void Run() { run(); }
        protected override void OnDone() 
        {
            if (error == null && runSuccess != null)
                runSuccess();

            if (runDone != null) runDone();

            if (error != null && runError != null)
                runError(error);
        }
    }

    abstract class AsyncLoaderBase : IDisposable
    {
        public bool Msg;
        public Thread runningThread;
        public Exception error;
        private bool disposing = false;

        protected abstract void Run();
        protected abstract void OnDone();

        public bool Done { get; private set; }

        private void ThreadStart()
        {
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                error = ex;
            }
            Done = true;

            if (!disposing)
                rMapForm.Instance.Invoke(new Action(OnDone));
            runningThread = null;

            lock(AsyncLoader.runningLock)
                AsyncLoader.runningInstances.Remove(this);
        }

        public void Start()
        {
            runningThread = new Thread(new System.Threading.ThreadStart(ThreadStart));
            runningThread.Start();
            //ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadStart));
        }

        public void Dispose()
        {
            if (!Done)
            {
                disposing = true;
                runningThread.Abort();
            }
        }
    }
}
