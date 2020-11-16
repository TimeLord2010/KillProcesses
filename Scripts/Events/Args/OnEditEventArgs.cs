using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillProcesses.Scripts.Events.Args {

    public class OnEditEventArgs {

        public OnEditEventArgs (string old_name, string new_name) {
            OldName = old_name;
            NewName = new_name;
        }

        public string OldName { get; }
        public string NewName { get; set; }
        public bool Cancel { get; set; }

    }
}
