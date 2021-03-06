﻿
namespace com.spacepuppy.Async
{

    public abstract class RadicalAsyncOperation : IRadicalYieldInstruction
    {

        #region Fields

        private bool _complete;

        #endregion

        #region CONSTRUCTOR

        public RadicalAsyncOperation()
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Begin()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(this.AsyncCallback, null);
        }
        
        private void AsyncCallback(object state)
        {
            this.DoAsyncWork();
        }

        protected abstract void DoAsyncWork();

        protected void SetSignal()
        {
            _complete = true;
        }

        protected virtual bool Tick(out object yieldObject)
        {
            yieldObject = null;
            return !_complete;
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        public bool IsComplete
        {
            get { return _complete; }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            if(_complete)
            {
                yieldObject = null;
                return false;
            }

            return this.Tick(out yieldObject);
        }

        #endregion

        #region Factory
        
        public static RadicalAsyncOperation QueueUserWorkItem(System.Threading.WaitCallback callback, object token = null)
        {
            var op = new SimpleAsyncAction();
            op._callback = callback;
            op._token = token;
            op.Begin();
            return op;
        }
        
        private class SimpleAsyncAction : RadicalAsyncOperation
        {

            internal System.Threading.WaitCallback _callback;
            internal object _token;
            
            protected override void DoAsyncWork()
            {
                if(_callback != null)
                {
                    _callback(_token);
                }
                this.SetSignal();
            }

        }

        #endregion

    }

}
