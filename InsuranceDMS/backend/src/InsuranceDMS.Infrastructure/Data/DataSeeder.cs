using InsuranceDMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsuranceDMS.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.States.AnyAsync())
        {
            db.States.AddRange(
                new State { StateCode = "AL", StateName = "Alabama" },
                new State { StateCode = "AK", StateName = "Alaska" },
                new State { StateCode = "AZ", StateName = "Arizona" },
                new State { StateCode = "AR", StateName = "Arkansas" },
                new State { StateCode = "CA", StateName = "California" },
                new State { StateCode = "CO", StateName = "Colorado" },
                new State { StateCode = "CT", StateName = "Connecticut" },
                new State { StateCode = "DE", StateName = "Delaware" },
                new State { StateCode = "FL", StateName = "Florida" },
                new State { StateCode = "GA", StateName = "Georgia" },
                new State { StateCode = "HI", StateName = "Hawaii" },
                new State { StateCode = "ID", StateName = "Idaho" },
                new State { StateCode = "IL", StateName = "Illinois" },
                new State { StateCode = "IN", StateName = "Indiana" },
                new State { StateCode = "IA", StateName = "Iowa" },
                new State { StateCode = "KS", StateName = "Kansas" },
                new State { StateCode = "KY", StateName = "Kentucky" },
                new State { StateCode = "LA", StateName = "Louisiana" },
                new State { StateCode = "ME", StateName = "Maine" },
                new State { StateCode = "MD", StateName = "Maryland" },
                new State { StateCode = "MA", StateName = "Massachusetts" },
                new State { StateCode = "MI", StateName = "Michigan" },
                new State { StateCode = "MN", StateName = "Minnesota" },
                new State { StateCode = "MS", StateName = "Mississippi" },
                new State { StateCode = "MO", StateName = "Missouri" },
                new State { StateCode = "MT", StateName = "Montana" },
                new State { StateCode = "NE", StateName = "Nebraska" },
                new State { StateCode = "NV", StateName = "Nevada" },
                new State { StateCode = "NH", StateName = "New Hampshire" },
                new State { StateCode = "NJ", StateName = "New Jersey" },
                new State { StateCode = "NM", StateName = "New Mexico" },
                new State { StateCode = "NY", StateName = "New York" },
                new State { StateCode = "NC", StateName = "North Carolina" },
                new State { StateCode = "ND", StateName = "North Dakota" },
                new State { StateCode = "OH", StateName = "Ohio" },
                new State { StateCode = "OK", StateName = "Oklahoma" },
                new State { StateCode = "OR", StateName = "Oregon" },
                new State { StateCode = "PA", StateName = "Pennsylvania" },
                new State { StateCode = "RI", StateName = "Rhode Island" },
                new State { StateCode = "SC", StateName = "South Carolina" },
                new State { StateCode = "SD", StateName = "South Dakota" },
                new State { StateCode = "TN", StateName = "Tennessee" },
                new State { StateCode = "TX", StateName = "Texas" },
                new State { StateCode = "UT", StateName = "Utah" },
                new State { StateCode = "VT", StateName = "Vermont" },
                new State { StateCode = "VA", StateName = "Virginia" },
                new State { StateCode = "WA", StateName = "Washington" },
                new State { StateCode = "WV", StateName = "West Virginia" },
                new State { StateCode = "WI", StateName = "Wisconsin" },
                new State { StateCode = "WY", StateName = "Wyoming" }
            );
        }

        if (!await db.LicenseTypes.AnyAsync())
        {
            db.LicenseTypes.AddRange(
                new LicenseType { Code = "L&H", Description = "Life and Health" },
                new LicenseType { Code = "P&C", Description = "Property and Casualty" },
                new LicenseType { Code = "Variable", Description = "Variable Products" },
                new LicenseType { Code = "Surplus", Description = "Surplus Lines" },
                new LicenseType { Code = "Title", Description = "Title Insurance" },
                new LicenseType { Code = "Credit", Description = "Credit Insurance" }
            );
        }

        await db.SaveChangesAsync();
    }
}
