using KillProcesses.Scripts.Events.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillProcesses.Scripts.Events {

    public class OnEdit : EventArgs {

        public OnEditEventArgs Args;

        public OnEdit (OnEditEventArgs args) {
            Args = args;
        }

    }
}
