using System;
using Rise.Domain.Users;

namespace Rise.Domain.Notifications
{
    public class Notification : Entity
    {
        private int _severity;
        private string _title = default!;
        private string _message = default!;
        private bool _isRead;
        private User _user = default!;

        public int Severity
        {
            get => _severity; set
            {
                Guard.Against.OutOfRange(value, nameof(Severity), 0, 3, "Severity must be between 0 and 3.");
                _severity = value;
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                Guard.Against.NullOrEmpty(value, nameof(Title), "Title can not be null or empty.");
                Guard.Against.NullOrWhiteSpace(value, nameof(Title), "Title can not be null or white space.");
                _title = value;
            }
        }

        public string Message
        {
            get => _message; set
            {
                Guard.Against.NullOrEmpty(value, nameof(Message), "Message can not be null or empty.");
                Guard.Against.NullOrWhiteSpace(value, nameof(Message), "Message can not be null or white space.");

                _message = value;
            }
        }

        public bool IsRead
        {
            get => _isRead; set
            {
                Guard.Against.Null(value, nameof(IsRead), "IsRead can not be null.");
                _isRead = value;
            }
        }

        public required User User
        {
            get => _user; set
            {
                Guard.Against.Null(value, nameof(User), "User can not be null.");
                _user = value;
            }
        }
    }
}


