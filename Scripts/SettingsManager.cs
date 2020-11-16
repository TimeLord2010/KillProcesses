using SqliteHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KillProcesses.Scripts {

    public class SettingsManager {

        public SettingsManager(string filename) {
            SqliteHandler = new SqliteHandler(filename);
            if (!File.Exists(SqliteHandler.DatabaseName)) {
                EnsureTable();
            }
        }

        const string ProcessesTable = "ProceessName";

        SqliteHandler SqliteHandler {
            get;
        }

        public void EnsureTable() {
            SqliteHandler.NonQuery(
                "create table if not exists " + ProcessesTable + @" (
                    [ProcessName] text not null,
                    [IsEnabled] integer not null,
                    primary key ([ProcessName])
                );");
        }

        public void GetProcesses(Action<string, bool> process_name) {
            if (process_name == null) {
                throw new NullReferenceException("Process function was nullp was null in GetProcesses().");
            }
            if (SqliteHandler == null) {
                throw new NullReferenceException("SqliteHandler was null in GetProcesses().");
            }
            SqliteHandler.QueryLoop($"select * from {ProcessesTable};", (r) => {
                if (r == null) {
                    throw new NullReferenceException("SqlReader was null in GetProcesses().");
                }
                var pn = r.IsDBNull(0)? null : r.GetString(0);
                var s = !r.IsDBNull(1) && (r.GetInt32(1) == 1);
                process_name(pn, s);
            });
        }

        public void AddProcess(string process_name) {
            if (process_name == null) {
                throw new ArgumentException("Process_name cannot be null in db.");
            }
            SqliteHandler.NonQuery($"insert or ignore into {ProcessesTable} values (@process, 0);", ("@process", process_name));
        }

        public void SetEnabled(string process_name, bool is_enabled) {
            SqliteHandler.NonQuery($"update {ProcessesTable} set IsEnabled = @enabled where ProcessName = @process", ("@enabled", is_enabled ? 1 : 0), ("@process", process_name));
        }

        public void UpdateProcess(string old_process_name, string new_process_name) {
            if (old_process_name == null) {
                AddProcess(new_process_name);
            } else {
                SqliteHandler.NonQuery($"update {ProcessesTable} set ProcessName = @new where ProcessName = @old;", ("@old", old_process_name), ("@new", new_process_name));
            }
        }

        public void DeleteProcess(string process_name) {
            SqliteHandler.NonQuery($"delete from {ProcessesTable} where [ProcessName] = @process;", ("@process", process_name));
        }

    }
}
