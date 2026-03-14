using Microsoft.EntityFrameworkCore;
using Tyrex.Domain.CRM;
using Tyrex.Domain.Fleet;
using Tyrex.Domain.Identity;
using Tyrex.Domain.Inventory;
using Tyrex.Domain.Workshop;

namespace Tyrex.Infrastructure.Persistence.Seeding;

public static class SeedData
{
    public static void ApplySeedData(this ModelBuilder modelBuilder)
    {
        // Users
        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var techId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var advisorId = Guid.Parse("00000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<User>().HasData(
            new
            {
                Id = adminId,
                Email = "admin@tyrex.com",
                PasswordHash = "admin123",
                FirstName = "Admin",
                LastName = "Tyrex",
                Role = Role.Admin
            },
            new
            {
                Id = techId,
                Email = "tech@tyrex.com",
                PasswordHash = "tech123",
                FirstName = "Jean",
                LastName = "Dupont",
                Role = Role.Technician
            },
            new
            {
                Id = advisorId,
                Email = "advisor@tyrex.com",
                PasswordHash = "advisor123",
                FirstName = "Marie",
                LastName = "Martin",
                Role = Role.Advisor
            }
        );

        // Customers
        var customer1Id = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var customer2Id = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var customer3Id = Guid.Parse("10000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<Customer>().HasData(
            new
            {
                Id = customer1Id,
                FirstName = "Pierre",
                LastName = "Dubois",
                Email = "pierre.dubois@email.com",
                Phone = "+33612345678",
                Type = CustomerType.Individual,
                CompanyName = (string?)null,
                CreatedOnUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                ModifiedOnUtc = (DateTime?)null,
                ModifiedBy = (string?)null
            },
            new
            {
                Id = customer2Id,
                FirstName = "Sophie",
                LastName = "Bernard",
                Email = "sophie.bernard@email.com",
                Phone = "+33687654321",
                Type = CustomerType.Individual,
                CompanyName = (string?)null,
                CreatedOnUtc = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                ModifiedOnUtc = (DateTime?)null,
                ModifiedBy = (string?)null
            },
            new
            {
                Id = customer3Id,
                FirstName = "Robert",
                LastName = "Petit",
                Email = "contact@transport-petit.fr",
                Phone = "+33123456789",
                Type = CustomerType.Company,
                CompanyName = "Transports Petit SARL",
                CreatedOnUtc = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                ModifiedOnUtc = (DateTime?)null,
                ModifiedBy = (string?)null
            }
        );

        // Vehicles
        var vehicle1Id = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var vehicle2Id = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var vehicle3Id = Guid.Parse("20000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<Vehicle>().HasData(
            new
            {
                Id = vehicle1Id,
                Vin = "VF7LA9HXG12345678",
                LicensePlate = "AB-123-CD",
                Make = "Peugeot",
                Model = "3008",
                Year = 2020,
                CustomerId = customer1Id,
                IsInternalFleet = false,
                CreatedOnUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                ModifiedOnUtc = (DateTime?)null,
                ModifiedBy = (string?)null
            },
            new
            {
                Id = vehicle2Id,
                Vin = "VF1R9800X12345679",
                LicensePlate = "EF-456-GH",
                Make = "Renault",
                Model = "Clio",
                Year = 2022,
                CustomerId = customer2Id,
                IsInternalFleet = false,
                CreatedOnUtc = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                ModifiedOnUtc = (DateTime?)null,
                ModifiedBy = (string?)null
            },
            new
            {
                Id = vehicle3Id,
                Vin = "WVWZZZ1KZ12345680",
                LicensePlate = "IJ-789-KL",
                Make = "Volkswagen",
                Model = "Transporter",
                Year = 2019,
                CustomerId = customer3Id,
                IsInternalFleet = false,
                CreatedOnUtc = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                ModifiedOnUtc = (DateTime?)null,
                ModifiedBy = (string?)null
            }
        );

        // Stock Items
        modelBuilder.Entity<StockItem>().HasData(
            new
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                PartNumber = "BOS-PLAQUETTE-001",
                Description = "Plaquettes de frein avant Bosch",
                Location = "A-12-03",
                QuantityOnHand = 25,
                CreatedOnUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                ModifiedOnUtc = (DateTime?)null,
                ModifiedBy = (string?)null
            },
            new
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                PartNumber = "BOS-DISQUE-001",
                Description = "Disques de frein avant 280mm",
                Location = "A-12-04",
                QuantityOnHand = 12,
                CreatedOnUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                ModifiedOnUtc = (DateTime?)null,
                ModifiedBy = (string?)null
            },
            new
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                PartNumber = "BOS-HUILE-5W40",
                Description = "Huile moteur 5W40 5L",
                Location = "B-05-01",
                QuantityOnHand = 40,
                CreatedOnUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                ModifiedOnUtc = (DateTime?)null,
                ModifiedBy = (string?)null
            },
            new
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000004"),
                PartNumber = "BOS-FILTRE-HUILE",
                Description = "Filtre à huile",
                Location = "B-05-02",
                QuantityOnHand = 60,
                CreatedOnUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                ModifiedOnUtc = (DateTime?)null,
                ModifiedBy = (string?)null
            },
            new
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000005"),
                PartNumber = "BOS-BOUGIE",
                Description = "Bougie d'allumage (jeu de 4)",
                Location = "C-08-02",
                QuantityOnHand = 18,
                CreatedOnUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                ModifiedOnUtc = (DateTime?)null,
                ModifiedBy = (string?)null
            }
        );
    }
}
