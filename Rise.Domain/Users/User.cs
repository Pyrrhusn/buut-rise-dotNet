// using Rise.Domain.Reservations;

using Rise.Domain.Notifications;
using Rise.Domain.Boats;
using Rise.Domain.Reservations;

namespace Rise.Domain.Users
{
    /// <summary>
    /// User base class
    /// </summary>
    public class User : Entity
    {
        private string _email = default!;
        private string _firstName = default!;
        private string _familyName = default!;
        private string _phoneNumber = default!;
        private DateTime _dateOfBirth = default!;
        private UserAddress _address = default!;

        public required string Email
        {
            get => _email;
            set
            {
                value = value.Trim();
                Guard.Against.NullOrWhiteSpace(value, nameof(Email));
                Guard.Against.LengthOutOfRange(value, 1, 69, nameof(Email)); //Parameter name does nothing, probably bug in package
                Guard.Against.InvalidFormat(value, nameof(Email), "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
                _email = value;
            }
        }

        public required string FirstName
        {
            get => _firstName;
            set
            {
                value = value.Trim();
                Guard.Against.NullOrWhiteSpace(value, nameof(FirstName));
                Guard.Against.LengthOutOfRange(value, 1, 100, nameof(FirstName)); //Parameter name does nothing, probably bug in package
                _firstName = value;
            }
        }
        public required string FamilyName
        {
            get => _familyName;
            set
            {
                value = value.Trim();
                Guard.Against.NullOrWhiteSpace(value, nameof(FamilyName));
                Guard.Against.LengthOutOfRange(value, 1, 100, nameof(FamilyName)); //Parameter name does nothing, probably bug in package
                _familyName = value;
            }
        }
        public string FullName { get; private set; } = default!;
        public required string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                value = value.Trim();
                Guard.Against.NullOrWhiteSpace(value, nameof(PhoneNumber));
                Guard.Against.LengthOutOfRange(value, 1, 100, nameof(PhoneNumber)); //Parameter name does nothing, probably bug in package
                Guard.Against.InvalidFormat(value, nameof(PhoneNumber), "^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\\s\\./0-9]*$");
                _phoneNumber = value;
            }
        }

        public required DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                Guard.Against.Null(value, nameof(DateOfBirth));
                Guard.Against.OutOfSQLDateRange(value, nameof(DateOfBirth));
                Guard.Against.Expression((x) => x > DateTime.Today.AddYears(-18), value, nameof(DateOfBirth));
                _dateOfBirth = value;
            }
        }

        public required UserAddress Address
        {
            get => _address;
            set
            {
                Guard.Against.Null(value, nameof(Address));
                _address = value;
            }
        }

        public class UserAddress
        {
            private string _street = default!;
            private string _number = default!;
            private string _city = default!;
            private string _postalCode = default!;
            private string _country = default!;
            public required string Street
            {
                get => _street; set
                {
                    value = value.Trim();
                    Guard.Against.NullOrWhiteSpace(value, nameof(Street));
                    Guard.Against.LengthOutOfRange(value, 1, 200, nameof(Street)); //Parameter name does nothing, probably bug in package
                    _street = value;
                }
            }
            public required string Number
            {
                get => _number; set
                {
                    value = value.Trim();
                    Guard.Against.NullOrWhiteSpace(value, nameof(Number));
                    Guard.Against.LengthOutOfRange(value, 1, 200, nameof(Number)); //Parameter name does nothing, probably bug in package
                    _number = value;
                }
            }
            public required string City
            {
                get => _city; set
                {
                    value = value.Trim();
                    Guard.Against.NullOrWhiteSpace(value, nameof(City));
                    Guard.Against.LengthOutOfRange(value, 1, 200, nameof(City)); //Parameter name does nothing, probably bug in package
                    _city = value;
                }
            }
            public required string PostalCode
            {
                get => _postalCode; set
                {
                    value = value.Trim();
                    Guard.Against.NullOrWhiteSpace(value, nameof(PostalCode));
                    Guard.Against.LengthOutOfRange(value, 1, 100, nameof(PostalCode)); //Parameter name does nothing, probably bug in package
                    _postalCode = value;
                }
            }
            public required string Country
            {
                get => _country; set
                {
                    value = value.Trim();
                    Guard.Against.NullOrWhiteSpace(value, nameof(Country));
                    Guard.Against.LengthOutOfRange(value, 1, 100, nameof(Country)); //Parameter name does nothing, probably bug in package
                    _country = value;
                }
            }

        }

        private readonly List<Reservation> reservations = [];
        public IReadOnlyList<Reservation> Reservations => reservations.AsReadOnly();
        private readonly List<Battery> guardedBatteries = [];
        public IReadOnlyList<Battery> GuardedBatteries => guardedBatteries.AsReadOnly();
        private readonly List<Notification> notifications = [];
        public IReadOnlyList<Notification> Notifications => notifications.AsReadOnly();

        private readonly List<Reservation> holdsBatteries = [];
        public IReadOnlyList<Reservation> HoldsBatteries => holdsBatteries.AsReadOnly();
    }
}