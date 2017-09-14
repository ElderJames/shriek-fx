using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpServiceCore.Communication
{
    public abstract class CommunicationObject : ICommunicationObject
    {
        public event Action<ICommunicationObject, CommunicationState> StateChanged;

        CommunicationState _State;
        public CommunicationState State
        {
            get { return _State; }
            private set
            {
                var changed = _State != value;
                _State = value;
                if (changed)
                    StateChanged?.Invoke(this, _State);
            }
        }

        protected SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public CommunicationObject()
        {
            this.State = CommunicationState.Created;
        }

        protected abstract Task OnOpen();
        protected abstract Task OnClose();


        public async Task Open()
        {
            await _lock.WaitAsync();
            try
            {
                if (this.State != CommunicationState.Created)
                    throw new Exception($"Can not open channel when its state is {State.ToString()}");
                this.State = CommunicationState.Openning;

                await this.OnOpen();

                if (this.State != CommunicationState.Openning)
                    throw new Exception($"Can not open channel when its state is {State.ToString()}");
                this.State = CommunicationState.Opened;
            }
            catch
            {
                this.State = CommunicationState.Faulted;
                throw;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task Close()
        {
            await _lock.WaitAsync();
            try
            {
                if (this.State < CommunicationState.Opened)
                    throw new Exception($"Can not close channel when its state is {State.ToString()}");
                this.State = CommunicationState.Closing;

                await this.OnClose();

                if (this.State != CommunicationState.Closing)
                    throw new Exception($"Can not close channel when its state is {State.ToString()}");
                this.State = CommunicationState.Closed;
            }
            catch
            {
                this.State = CommunicationState.Faulted;
                throw;
            }
            finally
            {
                _lock.Release();
            }
        }

        public virtual async Task Abort()
        {
            await this.Close();
        }

        public virtual void Dispose()
        {
            this.Close().ConfigureAwait(false);
        }

        protected void ThrowIfNotOpened()
        {
            if (this.State != CommunicationState.Opened)
                throw new Exception($"{this.GetType().Name} is not in 'Open' State");
        }
    }
}
