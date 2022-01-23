using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using varbyte.encryption.Models;

namespace varbyte.encryption.Interfaces
{
    public interface IConfigurationManager
    {
        public Globals Globals { get; }
        public Globals InitializeConfiguration();
        public void SetPrimaryKeyPath(string keyPath);
    }
}
