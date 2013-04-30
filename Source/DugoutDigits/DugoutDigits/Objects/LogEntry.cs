using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DugoutDigits.Objects {

    public enum LogType {
        NA,
        INFO,
        INVALID_REQUEST,
        DEBUG,
        ERROR
    }

    public enum LogFunction {
        NA,
        ADD_PRACTICE,
        REMOVE_TEAM,
        REMOVE_REQUEST_JOIN,
        ACCEPT_REQUEST_JOIN,
        REMOVE_INVITE,
        ACCEPT_INVITE,
        INVITE_USER,
        GET_TEAM_MEMBERS,
        GET_TEAM_COACHES,
        GET_SEASONS,
        ADD_SEASON,
        ADD_GAME
    }

    public enum LogAction {
        NA
    }

    public class LogEntry {

        // Used to classify the event being logged
        public LogType Type { get; set; }
        public LogFunction Function { get; set; }
        public LogAction Action { get; set; }

        // Used to give details about the event being logged
        public long ID { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public Person User { get; set; }

        /// <summary>
        /// Log object constructor.
        /// </summary>
        /// <param name="type">The type of the event being logged.</param>
        /// <param name="function">The function executing when the event occured.</param>
        /// <param name="action">The action being performed when the event occured.</param>
        /// <param name="Message">A specific message associated with the event being logged.</param>
        /// <param name="user">The user of the system when the event occured.</param>
        /// <param name="timestamp">The date and time when the event was logged (set in DB).</param>
        public LogEntry(LogType type, LogFunction function, LogAction action, DateTime timestamp, long id, string message, Person user) {
            Type = type;
            Function = function;
            Action = action;

            ID = id;
            Timestamp = timestamp;
            Message = message;
            User = user;
        }

        /// <summary>
        /// Default log message class.
        /// </summary>
        public LogEntry() : this(LogType.NA, LogFunction.NA, LogAction.NA, DateTime.Now, 0, "NA", new Person()) { }

        public LogEntry(LogType type, LogFunction function, LogAction action) : this(type, function, action, DateTime.Now, 0, "NA", new Person()) { }

    }
}