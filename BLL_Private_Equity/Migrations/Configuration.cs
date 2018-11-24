namespace BLL_Private_Equity.Migrations
{
    using System.Data.Entity.Migrations;
    using BLL_DataModels;

    internal sealed class Configuration : DbMigrationsConfiguration<BLL_Private_Equity.BllData>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BLL_Private_Equity.BllData context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.Currencies.AddOrUpdate(c => c.CurrencyShortName,
                new Currency { CurrencyName = "US Dollar", CurrencyShortName = "USD" },
                new Currency { CurrencyName = "Euro", CurrencyShortName = "EUR" },
                new Currency { CurrencyName = "British Pound", CurrencyShortName = "GBP" });

            context.Countries.AddOrUpdate(c => c.CountryName,
                          new Country { CountryName = "USA" },
                          new Country { CountryName = "Kanada" },
                          new Country { CountryName = "Deutschland" },
                           new Country { CountryName = "Österreich" },
                            new Country { CountryName = "Schweiz" },
                            new Country { CountryName = "Niederlande" },
                          new Country { CountryName = "Schweden" },
                          new Country { CountryName = "Finnland" },
                          new Country { CountryName = "Norwegen" },
                          new Country { CountryName = "Frankreich" },
                          new Country { CountryName = "Italien" },
                          new Country { CountryName = "Spanien" },
                           new Country { CountryName = "Europe" },
                          new Country { CountryName = "Asien" },
                           new Country { CountryName = "China" });

            context.FundCompanySizes.AddOrUpdate(c => c.CompanySize,
               new FundCompanySize { CompanySize="Large Cap" },
               new FundCompanySize { CompanySize = "Mid Cap" },
               new FundCompanySize { CompanySize = "Small Cap" });

            context.FundGeographies.AddOrUpdate(c => c.Geography,
    new FundGeography { Geography = "Nordamerika" },
    new FundGeography { Geography = "Europa" },
    new FundGeography { Geography = "Asien" });

            context.FundTypes.AddOrUpdate(c => c.FundTypeName,
new FundType { FundTypeName="Buyout" },
new FundType { FundTypeName = "Secondary" },
new FundType { FundTypeName = "Venture" });
        }
    }
}
