using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL_DataModels;

namespace BLL_DataAccess
{
    public static class ComboboxLists
    {
        static IEnumerable<Advisor> advisors = null;
        static IEnumerable<ClientAdvisor> clientAdvisors = null;
        static IEnumerable<Currency> currencies = null;
        static IEnumerable<Country> countries = null;
        static IEnumerable<FundManager> fundManagers = null;
        static IEnumerable<FundType> fundTypes = null;
        static IEnumerable<FundGeography> geographies = null;
        static IEnumerable<FundCompanySize> companySizes = null;
        static IEnumerable<Initiator> initiators = null;
        static Currency EuroCurrency = null;
        static ComboboxLists()
        {
            
        }

        public static IEnumerable<Advisor> GetAdvisors()
        {
            if (advisors == null)
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    advisors = dbContext.Advisors.OrderBy(a => a.FullName).ToList();
                }
            }
            return advisors;
        }
        public static void RefreshAdvisors()
        {
            advisors = null;
            GetAdvisors();
        }
        public static IEnumerable<ClientAdvisor> GetClientAdvisors()
        {
            if (clientAdvisors == null)
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    clientAdvisors = dbContext.ClientAdvisors.OrderBy(a => a.AdvisorName.LastName).ToList();
                }

                // generate DisplayName using a few properties

                foreach (ClientAdvisor advisor in clientAdvisors)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    if (!string.IsNullOrEmpty(advisor.AdvisorName.Title)) stringBuilder.Append($"{advisor.AdvisorName.Title} ");
                    stringBuilder.Append($"{advisor.AdvisorName.FirstName} {advisor.AdvisorName.LastName}");
                    if (!string.IsNullOrEmpty(advisor.CompanyName)) stringBuilder.Append($" ({advisor.CompanyName})");
                    advisor.DisplayName = stringBuilder.ToString();
                }
            }
            return clientAdvisors;
        }
        public static void RefreshClientAdvisors()
        {
            clientAdvisors = null;
            GetClientAdvisors();
        }

        public static IEnumerable<Currency> GetCurrencies()
        {
            if (currencies == null)
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    currencies = dbContext.Currencies.ToList();
                }
            }
            return currencies;
        }
        public static Currency GetEuroCurrency()
        {
            if (EuroCurrency != null) return EuroCurrency;
            using (HqTrustData dbContext = new HqTrustData())
            {
                EuroCurrency = dbContext.Currencies.FirstOrDefault(c => c.CurrencyShortName == "EUR");
                if (EuroCurrency == null)
                {
                    EuroCurrency = currencies.ElementAt(0);
                }
            }
            return EuroCurrency;
        }
        public static void RefreshCurrencies()
        {
            currencies = null;
            GetCurrencies();
        }
        public static IEnumerable<Country> GetCountries()
        {
            if (countries == null)
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    countries = dbContext.Countries.ToList();
                }
            }
            return countries;
        }
        public static void RefreshCountries()
        {
            countries = null;
            GetCountries();
        }
        public static IEnumerable<FundManager> GetFundManagers()
        {
            if (fundManagers == null)
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    fundManagers = dbContext.FundManagers.OrderBy(f => f.FundManagerName).ToList();
                }
            }
            return fundManagers;
        }
        public static IEnumerable<FundType> GetFundTypes()
        {
            if (fundTypes == null)
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    fundTypes = dbContext.FundTypes.OrderBy(f => f.FundTypeName).ToList();
                }
            }
            return fundTypes;
        }
        public static IEnumerable<FundGeography> GetFundGeographies()
        {
            if (geographies == null)
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    geographies = dbContext.FundGeographies.OrderBy(f => f.Geography).ToList();
                }
            }
            return geographies;
        }
        public static IEnumerable<FundCompanySize> GetFundCompanySizes()
        {
            if (companySizes == null)
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    companySizes = dbContext.FundCompanySizes.OrderBy(f => f.CompanySize).ToList();
                }
            }
            return companySizes;
        }
        public static IEnumerable<Initiator> GetInitiators()
        {
           
                using (HqTrustData dbContext = new HqTrustData())
                {
                    initiators = dbContext.Initiators.OrderBy(i => i.InitiatorName).ToList();
                }
           
            return initiators;
        }

        public static string GetApplicationInformation(string Key)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                AnwendungsInformationen info = dbContext.AnwendungsInformationen.FirstOrDefault(a => a.Key == Key);
                if (info == null)
                {
                    // if record is not found add Anwndungsinformation-Record
                    info = new AnwendungsInformationen()
                    {
                        Key = Key,
                        Value = string.Empty
                    };
                    dbContext.AnwendungsInformationen.Add(info);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Fehler beim Eintragen einer AnwendungsInformation");
                    }
                }
                if (info.Value == null) return string.Empty; else return info.Value;
            }
        }

        public static void SetApplicationInformation(string key, string value)
        {

            using (HqTrustData dbContext = new HqTrustData())
            {
                AnwendungsInformationen info = dbContext.AnwendungsInformationen.FirstOrDefault(a => a.Key == key);
                if (info == null)
                {
                    info = new AnwendungsInformationen()
                    {
                        Key = key,
                        Value = value
                    };
                    dbContext.AnwendungsInformationen.Add(info);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Fehler beim Eintragen einer AnwendungsInformation");
                    }
                }
                else
                {
                    info.Value = value;
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Fehler beim Ändern einer AnwendungsInformation");
                    }
                }
            }
        }
    }
}
