using System;
using System.Threading;

namespace Aberrant.SMPP.Core
{
    internal abstract class BaseLock : IDisposable
    {
        protected ReaderWriterLockSlim _Locks;

        public BaseLock(ReaderWriterLockSlim locks)
        {
            _Locks = locks;
        }

        public abstract void Dispose();
    }

    internal class ReadLock : BaseLock
    {
        public ReadLock(ReaderWriterLockSlim locks)
            : base(locks)
        {
            //if (_Log.IsDebugEnabled) _Log.DebugFormat("Entering.. ({0})", _Locks.GetHashCode());
            _Locks.EnterUpgradeableReadLock();
        }

        public override void Dispose()
        {
            //if (_Log.IsDebugEnabled) _Log.DebugFormat("Exiting.. ({0})", _Locks.GetHashCode());
            _Locks.ExitUpgradeableReadLock();
        }
    }

    internal class ReadOnlyLock : BaseLock
    {
        public ReadOnlyLock(ReaderWriterLockSlim locks)
            : base(locks)
        {
            //if (_Log.IsDebugEnabled) _Log.DebugFormat("Entering.. ({0})", _Locks.GetHashCode());
            _Locks.EnterReadLock();
        }

        public override void Dispose()
        {
            //if (_Log.IsDebugEnabled) _Log.DebugFormat("Exiting.. ({0})", _Locks.GetHashCode());
            _Locks.ExitReadLock();
        }
    }

    internal class WriteLock : BaseLock
    {
        public WriteLock(ReaderWriterLockSlim locks)
            : base(locks)
        {
            //if (_Log.IsDebugEnabled) _Log.DebugFormat("Entering.. ({0})", _Locks.GetHashCode());
            _Locks.EnterWriteLock();
        }

        public override void Dispose()
        {
            //if (_Log.IsDebugEnabled) _Log.DebugFormat("Exiting.. ({0})", _Locks.GetHashCode());
            _Locks.ExitWriteLock();
        }
    }
}