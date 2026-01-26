using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace WrathTools.Deprecated
{
    [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
    public class SaveCollection<TSave, TLoad> : SaveObject<IEnumerable<TLoad>> where TSave : SaveObject<TLoad>
    {
        protected override Task<IEnumerable<TLoad>> LoadAsyncProtected()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<TLoad> LoadProtected()
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