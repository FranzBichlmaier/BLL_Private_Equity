using BLL_DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataAccess
{
    public static class AdvisorAccess
    {

        public static async Task<IEnumerable<Advisor>> GetAllHqtAdvisorsAsync()
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                List<Advisor> allInitiators = await dbContext.Advisors.
                    Include("ReportsTo").
                    OrderBy(f => f.FullName).
                    ToListAsync();
                return allInitiators;
            }
        }
        public static async Task<IEnumerable<ClientAdvisor>> GetAllClientAdvisorsAsync()
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                List<ClientAdvisor> allClientAdvisors = await dbContext.ClientAdvisors. 
                    ToListAsync();
                foreach(ClientAdvisor item in allClientAdvisors)
                {
                    // set DisplayName
                    StringBuilder builder = new StringBuilder();
                    if (!string.IsNullOrEmpty(item.AdvisorName.Title)) builder.Append(item.AdvisorName.Title + " ");
                    if (!string.IsNullOrEmpty(item.AdvisorName.FirstName)) builder.Append(item.AdvisorName.FirstName + " ");
                    if (!string.IsNullOrEmpty(item.AdvisorName.LastName)) builder.Append(item.AdvisorName.LastName);
                    item.DisplayName = builder.ToString();
                }

                return allClientAdvisors;
            }
        }

        public static bool AdvisorHasInvestor(int advisorId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                int count = dbContext.Investors.Where(i => i.AdvisorId == advisorId).Count();
                if (count == 0) return false; else return true;
            }
        }

        public static void UpdateAdvisor(Advisor advisor)
        {
            if (advisor.Id == 0)
            {
                // add new Iniitiator
                using (HqTrustData dbContext = new HqTrustData())
                {
                    dbContext.Advisors.Add(advisor);
                    try
                    {
                        dbContext.SaveChanges();
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Fehler beim Hinzufügen eines HQT Beraters. {ex.InnerException.Message}");
                    }
                }
            }
            else
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    Advisor oldAdvisor = dbContext.Advisors.FirstOrDefault(i => i.Id == advisor.Id);
                    if (oldAdvisor == null)
                        throw new Exception($"Fehler beim Lesen des HQT Beraters mit der Id {advisor.Id.ToString()}");
                    dbContext.Entry(oldAdvisor).CurrentValues.SetValues(advisor);
                    try
                    {
                        dbContext.SaveChanges();
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Fehler beim Ändern eines HQT Beraters. {ex.InnerException.Message}");
                    }
                }
            }
        }
        public static void RemoveAdvisor(Advisor advisor)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                Advisor item = dbContext.Advisors.FirstOrDefault(i => i.Id == advisor.Id);
                if (item == null)
                    throw new Exception($"Fehler beim Lesen des HQT Beraters mit der Id {advisor.Id.ToString()}");
                try
                {
                    dbContext.Advisors.Remove(item);
                    dbContext.SaveChanges();
                    return;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Löschen eines HQT Beraters. {ex.InnerException.Message}");
                }
            }
        }

        public static void RemoveClientAdvisor(ClientAdvisor clientAdvisor)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                ClientAdvisor item = dbContext.ClientAdvisors.FirstOrDefault(i => i.Id == clientAdvisor.Id);
                if (item == null)
                    throw new Exception($"Fehler beim Lesen des Kunden-Beraters mit der Id {clientAdvisor.Id.ToString()}");
                try
                {
                    dbContext.ClientAdvisors.Remove(item);
                    dbContext.SaveChanges();
                    return;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Löschen eines Kunden-Beraters. {ex.InnerException.Message}");
                }
            }
        }
        public static void UpdateClientAdvisor(ClientAdvisor clientAdvisor)
        {
            if (clientAdvisor.Id == 0)
            {
                // add new Iniitiator
                using (HqTrustData dbContext = new HqTrustData())
                {
                    dbContext.ClientAdvisors.Add(clientAdvisor);
                    try
                    {
                        dbContext.SaveChanges();
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Fehler beim Hinzufügen eines Kunden-Beraters. {ex.InnerException.Message}");
                    }
                }
            }
            else
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    ClientAdvisor oldClientAdvisor = dbContext.ClientAdvisors.FirstOrDefault(i => i.Id == clientAdvisor.Id);
                    if (oldClientAdvisor == null)
                        throw new Exception($"Fehler beim Lesen des Kunden-Beraters mit der Id {clientAdvisor.Id.ToString()}");
                    dbContext.Entry(oldClientAdvisor).CurrentValues.SetValues(clientAdvisor);
                    try
                    {
                        dbContext.SaveChanges();
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Fehler beim Ändern eines Kunden-Beraters. {ex.InnerException.Message}");
                    }
                }
            }
        }
    }
}
