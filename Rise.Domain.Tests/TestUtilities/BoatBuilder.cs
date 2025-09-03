using Rise.Domain.Boats;

namespace Rise.Domain.Tests.TestUtilities
{
    public class BoatBuilder
    {
        public const string ValidPersonalName = "Limba";

        private string personalName = ValidPersonalName;

        public BoatBuilder WithPersonalName(string personalName)
        {
            this.personalName = personalName;
            return this;
        }

        public Boat Build()
        {
            var boat = new Boat
            {
                PersonalName = personalName
            };

            return boat;
        }

    }

}

