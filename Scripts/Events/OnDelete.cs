using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillProcesses.Scripts.Events {
    
    public class OnDelete {

        public OnDelete(string name) {
            ProcessName = name;
        }

        public string ProcessName { get; }

    }
}
