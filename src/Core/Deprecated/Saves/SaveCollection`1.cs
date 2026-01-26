using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace WrathTools.Deprecated
{
    [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
    public sealed class SaveCollection<T> : SaveObject<IEnumerable<T>> where T : SaveObject
    {
        protected override Task<IEnumerable<T>> LoadAsyncProtected()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<T> LoadProtected()
        {
            throw new NotImplementedException();
        }

        protected override void Read(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        protected override Task ReadAsync(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void Write(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override Task WriteAsync(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
