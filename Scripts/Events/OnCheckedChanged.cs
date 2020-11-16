using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillProcesses.Scripts.Events {

    public class OnCheckedChanged : EventArgs {

        public OnCheckedChanged (bool new_state) {
            NewState = new_state;
        }

        public bool NewState { get; }

    }
}
