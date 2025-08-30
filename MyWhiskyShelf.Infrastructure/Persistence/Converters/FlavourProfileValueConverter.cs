using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Infrastructure.Encoding;

namespace MyWhiskyShelf.Infrastructure.Persistence.Converters;

public sealed class FlavourProfileValueConverter() : ValueConverter<FlavourProfile, ulong>(
    flavourProfile => FlavourProfileEncoder.Encode(flavourProfile),
    encodedFlavourProfile => FlavourProfileEncoder.Decode(encodedFlavourProfile));