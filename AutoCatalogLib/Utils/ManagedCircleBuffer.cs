using System;
using System.Collections.Generic;
using System.Threading;

namespace AutoCatalogLib.Utils
{
    public class ManagedCircleBuffer<TElement> 
    {
        private volatile CircleBuffer<TElement> _writerElement;
        private volatile CircleBuffer<TElement> _readerElement;

        private volatile EventWaitHandle _writerWaitHandle;
        private volatile bool _writerWaiting;
        private volatile EventWaitHandle _readerWaitHandle;
        private volatile bool _readerWaiting;

        //private volatile bool _writing;

        private volatile uint _writenCount;
        private volatile uint _readedCount;

        public readonly int Size;
        public readonly int Timeout;

        public long Writen
        {
            get { return _writenCount; }
        }
        public long Readed
        {
            get { return _readedCount; }
        }

        private volatile bool _prepareToFinish;
        public bool Finished { get { return _prepareToFinish && !HasElements; } }

        public ManagedCircleBuffer(int size, int timeout = 1000)
        {
            Size = size;
            Timeout = timeout;

            var buffer = CircleBuffer<TElement>.BuildBuffer(size);
            _writerElement = buffer;
            _readerElement = buffer;

            _writerWaitHandle = new AutoResetEvent(false);
            _readerWaitHandle = new AutoResetEvent(false);
        }

        public void Finish()
        {
            _prepareToFinish = true;
        }

        public bool HasElements {
            get { return _writenCount > _readedCount; }
        }

        public bool TryPop(out TElement element, int timeout = -1)
        {
            element = default(TElement);
            if (_readerWaiting || !WaitForElement(timeout))
                return false;

            element = _readerElement.Element;
            _readerElement.Empty = true;
            _readerElement = _readerElement.Next;
            _readedCount++;
            if (_writerWaiting)
            {
                _writerWaitHandle.Set();
                _writerWaiting = false;
            }

            return true;
        }

        public IEnumerable<TElement> PopAsEnumerable()
        {
            while (!Finished)
            {
                if (_readerElement.Empty)
                {
                    _readerWaitHandle.WaitOne(Timeout);
                    continue;
                }

                var element = _readerElement.Element;
                //_readerElement.Element = null;
                _readerElement.Empty = true;
                _readerElement = _readerElement.Next;

                _readedCount++;

                yield return element;
            }
        }

        public void Push(TElement element, int timeout = -1)
        {
            if (Finished)
                throw new Exception("Collection finished");

#if DEBUG
            if (element == null)
                throw new Exception();
#endif

            //_writing = true;

            while (!_writerElement.Empty)
            {
                _writerWaiting = true;
                _writerWaitHandle.WaitOne(timeout == -1 ? Timeout : timeout);
            }

            _writerElement.Element = element;
            _writerElement.Empty = false;
            _writerElement = _writerElement.Next;
            _writenCount++;
            //_writing = false;

            if (_readerWaiting)
            {
                _readerWaitHandle.Set();
                _readerWaiting = false;
            }
        }

        public void PushRange(IEnumerable<TElement> elements)
        {
            if(Finished)
                throw new Exception("Collection finished");

            foreach (var element in elements)
            {
                while (!Finished && !_writerElement.Empty)
                {
                    _writerWaiting = true;
                    _writerWaitHandle.WaitOne(Timeout);
                }

                if(Finished)
                    throw new Exception("Collection finished");

                _writerElement.Element = element;
                _writerElement.Empty = false;
                _writerElement = _writerElement.Next;
                _writenCount++;

                if (_readerWaiting)
                {
                    _readerWaitHandle.Set();
                    _readerWaiting = false;
                }
            }
        }

        public bool WaitForElement(int timeout = -1)
        {
            if (!HasElements && !Finished)
            {
                _readerWaiting = true;
                _readerWaitHandle.WaitOne(timeout == -1 ? Timeout : timeout);
            }

            return HasElements;
        }
    }
}