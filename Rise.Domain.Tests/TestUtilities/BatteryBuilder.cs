using Rise.Domain.Boats;
using Rise.Domain.Users;

namespace Rise.Domain.Tests.TestUtilities
{
    public class BatteryBuilder
    {
        public const string ValidBatteryType = "Lithium-Ion";
        public static readonly Boat ValidBoat = new BoatBuilder().Build();
        public static readonly User ValidMentor = new UserBuilder().Build();

        private string batteryType = ValidBatteryType;
        private Boat boat = ValidBoat;
        private User mentor = ValidMentor;

        public BatteryBuilder WithBatteryType(string batteryType)
        {
            this.batteryType = batteryType;
            return this;
        }

        public BatteryBuilder WithBoat(Boat boat)
        {
            this.boat = boat;
            return this;
        }

        public BatteryBuilder WithMentor(User mentor)
        {
            this.mentor = mentor;
            return this;
        }

        public Battery Build()
        {
            return new Battery
            {
                Type = batteryType,
                Boat = boat,
                Mentor = mentor
            };
        }
    }
}
