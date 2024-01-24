using System;

namespace GymManagementSystem
{
    class Logger
    {
        public void AddLog(string st1, DateTime st2, string st3)
        {
            using (var db = new GMS_DBEntities())
            {
                Log newLog = new Log();
                newLog.UserID = Program.acc.UserID;
                newLog.Date = st2;
                newLog.Operation = st3;
                db.Logs.Add(newLog);
                db.SaveChanges();
            }
        }

        public static void Log(string operation)
        {
            using (var db = new GMS_DBEntities())
            {
                Log newLog = new Log();
                newLog.UserID = Program.acc.UserID;
                newLog.Date = DateTime.Now;
                newLog.Operation = operation;
                db.Logs.Add(newLog);
                db.SaveChanges();
            }
        }
    }
}
