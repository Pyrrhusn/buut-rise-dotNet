using System;
using FluentValidation;

namespace Rise.Shared.Address;

public record class AddressDto
{
    public required string Street { get; set; }
    public required string Number { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
}
