using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL_DataModels;

namespace BLL_DataAccess
{
    public static class PefundAccess
    {
        private static List<PeFund> allFunds = null;
        private static List<InvestorCommitment> commitmentsForAFund = null;

        public static async Task<IEnumerable<PeFund>> GetAllPefundsAsync()
        {
           
            using (HqTrustData dbContext = new HqTrustData())
            {
                allFunds = await dbContext.PeFunds.
                    Include("BankAccounts").
                    Include("Currency").
                    Include("FundManager").
                    Include("Initiator").
                    Include("Initiator.Advisor").
                    Include("DocumentAndLetters").
                    OrderBy(f => f.FundShortName).
                    ToListAsync();
            }            
            return allFunds;
        }
        public static List<PeFund> GetAllPefunds()
        {

            using (HqTrustData dbContext = new HqTrustData())
            {
                allFunds = dbContext.PeFunds.
                    Include("BankAccounts").
                    Include("Currency").
                    Include("FundManager").
                    Include("Initiator").
                    Include("Initiator.Advisor").
                    Include("DocumentAndLetters").
                    OrderBy(f => f.FundShortName).
                    ToList();
            }
            return allFunds;
        }
        public static async Task<IEnumerable<Initiator>> GetAllInitiatorsAsync()
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                List<Initiator> allInitiators = await dbContext.Initiators.
                    Include("Advisor").                   
                    OrderBy(f => f.InitiatorName).
                    ToListAsync();
                return allInitiators;
            }
        }

        public static bool IsInitiatorUsed(int id)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                int count = dbContext.PeFunds.Where(f => f.InitiatorId == id).Count();
                if (count > 0) return true; else return false;
            }
        }
        public static void UpdateInitiator(Initiator initiator)
        {
            if (initiator.Id == 0)
            {
                // add new Iniitiator
                using (HqTrustData dbContext = new HqTrustData())
                {
                    dbContext.Initiators.Add(initiator);
                    try
                    {
                        dbContext.SaveChanges();
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Fehler beim Hinzufügen eines Initiators. {ex.InnerException.Message}");
                    }
                }
            }
            else
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    Initiator oldInitiator = dbContext.Initiators.FirstOrDefault(i => i.Id == initiator.Id);
                    if (oldInitiator == null)
                        throw new Exception($"Fehler beim Lesen des Initiators mit der Id {initiator.Id.ToString()}");
                    dbContext.Entry(oldInitiator).CurrentValues.SetValues(initiator);
                    try
                    {
                        dbContext.SaveChanges();
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Fehler beim Ändern eines Initiators. {ex.InnerException.Message}");
                    }
                }
            }
        }
        public static void RemoveInitiator(Initiator initiator)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                Initiator item = dbContext.Initiators.FirstOrDefault(i => i.Id == initiator.Id);
                if (item == null)
                    throw new Exception($"Fehler beim Lesen des Initiators mit der Id {initiator.Id.ToString()}");
                try
                {
                    dbContext.Initiators.Remove(item);
                    dbContext.SaveChanges();
                    return;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Löschen eines Initiators. {ex.InnerException.Message}");
                }
            }
        }

        public static void InsertPeFund(PeFund fund)
        {
            PeFund newFund = fund.Copy(fund);
            newFund.BankAccounts = new System.Collections.ObjectModel.ObservableCollection<BankAccount>();
            newFund.DocumentAndLetters = new System.Collections.ObjectModel.ObservableCollection<DocumentAndLetter>();           

            using (HqTrustData dbContext = new HqTrustData())
            {
                dbContext.PeFunds.Add(newFund);
                try
                {
                    dbContext.SaveChanges();
                    fund.Id = newFund.Id;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Einfügen eines neuen Private Equity Funds: {ex.InnerException.Message}");
                }
            }
            UpdateBankAccountsForPeFund(fund);
            UpdateDocumentAndLettersForPeFund(fund);            
        }

        public static Initiator GetInitiatorById(int id)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.Initiators.FirstOrDefault(i => i.Id == id);
            }
        }

        public static void UpdatePeFund(PeFund fund)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {

                PeFund updatePeFund = dbContext.PeFunds.FirstOrDefault(i => i.Id == fund.Id);
                if (updatePeFund == null)
                {
                    throw new Exception($"Der Fund mit der Id {fund.Id} wurde nicht in der Datenbank gefunden");
                }
                dbContext.Entry(updatePeFund).CurrentValues.SetValues(fund);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Ändern eines PeFunds: {ex.InnerException.Message}");
                }
            }
            UpdateBankAccountsForPeFund(fund);
            UpdateDocumentAndLettersForPeFund(fund);
        }

        public static void RemovePeFund(PeFund fund)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                PeFund removefund = dbContext.PeFunds.FirstOrDefault(i => i.Id == fund.Id);
                if (removefund == null)
                {
                    throw new Exception($"Der Fund mit der Id {fund.Id} wurde in der Datenbank nicht gefunden.");
                }
                dbContext.PeFunds.Remove(removefund);

                try
                {
                    // remove related BankAccounts
                    var results = dbContext.BankAccounts.Where(b => b.PefundId == fund.Id).ToList();
                    if (results.Count > 0)
                        dbContext.BankAccounts.RemoveRange(results);

                    // remove related Documents and LetterInformation
                    var docs = dbContext.DocumentAndLetters.Where(e => e.PeFundId == fund.Id).ToList();
                    if (docs.Count > 0)
                        dbContext.DocumentAndLetters.RemoveRange(docs);
                }

                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Löschen von abhängigen Tabellen: {ex.InnerException.Message}");

                }
                // save changes
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Löschen des Funds. Ursache: {ex.InnerException.Message}");
                }
            }
        }

        private static void UpdateDocumentAndLettersForPeFund(PeFund fund)
        {

            using (HqTrustData dbContext = new HqTrustData())
            {
                var existingDocs = dbContext.DocumentAndLetters.Where(b => b.PeFundId == fund.Id).ToList();

                // foreach account in existingAccounts try to find a record in investor.BankAccounts
                // if found --> update properties
                // if not found --> record was deleted by user --> remove account

                foreach (DocumentAndLetter document in existingDocs)
                {
                    DocumentAndLetter newDoc = fund.DocumentAndLetters.FirstOrDefault(b => b.Id == document.Id);
                    if (newDoc == null)
                    {
                        // not found --> remove
                        dbContext.DocumentAndLetters.Remove(document);
                    }
                    else
                    {
                        // found --> update properties
                        document.DocumentDate = newDoc.DocumentDate;
                        document.DocumentDescription = newDoc.DocumentDescription;
                        document.DocumentFileName = newDoc.DocumentFileName;
                        document.DocumentType = newDoc.DocumentType;
                    }
                }


                foreach (DocumentAndLetter document in fund.DocumentAndLetters)
                {
                    if (document.Id != 0) continue;
                    document.PeFundId = fund.Id;
                    document.InvestorId = null;
                    dbContext.DocumentAndLetters.Add(document);
                }
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Einfügen eines neuen Private Equity Funds (Tabelle: DocumentAndLetters) Fehler: {ex.InnerException.Message}");
                }
            }
        }
        private static void UpdateBankAccountsForPeFund(PeFund fund)
        {

            using (HqTrustData dbContext = new HqTrustData())
            {
                var existingAccounts = dbContext.BankAccounts.Where(b => b.PefundId == fund.Id).ToList();

                // foreach account in existingAccounts try to find a record in investor.BankAccounts
                // if found --> update properties
                // if not found --> record was deleted by user --> remove account

                foreach (BankAccount account in existingAccounts)
                {
                    BankAccount newAccount = fund.BankAccounts.FirstOrDefault(b => b.Id == account.Id);
                    if (newAccount == null)
                    {
                        // not found --> remove
                        dbContext.BankAccounts.Remove(account);
                    }
                    else
                    {
                        // found --> update properties
                        account.AccountHolder = newAccount.AccountHolder;
                        account.AccountNumber = newAccount.AccountNumber;
                        account.AdditionalInstructions = newAccount.AdditionalInstructions;
                        account.BankAddress = newAccount.BankAddress;
                        account.BankContactId = newAccount.BankContactId;
                        account.BankName = newAccount.BankName;
                        account.BankNumberBlz = newAccount.BankNumberBlz;
                        account.BeneficiaryBank = newAccount.BeneficiaryBank;
                        account.CurrencyId = newAccount.CurrencyId;
                        account.FfcAccountHolderName = newAccount.FfcAccountHolderName;
                        account.FfcAccountNumber = newAccount.FfcAccountNumber;
                        account.Iban = newAccount.Iban;
                        account.InvestorId = null;
                        account.PefundId = fund.Id;
                        account.Signature1 = newAccount.Signature1;
                        account.Signature2 = newAccount.Signature2;
                        account.SwiftAddress = newAccount.SwiftAddress;
                    }
                }


                foreach (BankAccount account in fund.BankAccounts)
                {
                    if (account.Id != 0) continue;
                    account.PefundId = fund.Id;
                    account.InvestorId = null;
                    dbContext.BankAccounts.Add(account);
                }
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Einfügen eines neuen Private Equity Funds (Tabelle: BankAccounts) Fehler: {ex.InnerException.Message}");
                }
            }
        }

        public static bool FundHasCommitments(PeFund fund)
        {           
            int numberOfCommitments = 0;
            using (HqTrustData dbContext = new HqTrustData())
            {
                numberOfCommitments = dbContext.InvestorCommitmnents.Where(c => c.PeFundId == fund.Id).Count();
            }
            if (numberOfCommitments > 0) return true; else return false;
        }

        public static PeFund GetPeFundById(int peFundId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.PeFunds.
                       Include("BankAccounts").
                       Include("Currency").
                       Include("FundManager").
                       Include("Initiator").
                       Include("Initiator.Advisor").
                       Include("DocumentAndLetters").
                       Include("FeederFunds").
                       FirstOrDefault(p => p.Id == peFundId);
            }
        }
        public static PeFund GetPeFundByBeteiligungsnummer(string beteiligung)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.PeFunds.                       
                       FirstOrDefault(p => p.FundHqTrustNumber.Equals(beteiligung));
            }
        }
        public static PeFund GetFundByFundName(string partialName)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.PeFunds.Where(p => p.FundName.Contains(partialName)).
                       FirstOrDefault();
            }
        }
        public static List<PeFund> GetFundListForBeteiligungsnumer(string beteiligung)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.PeFunds.Where(p => p.FundHqTrustNumber.Equals(beteiligung)).ToList();                       
            }
        }

        public static List<InvestorCommitment> GetCommitmentsForPeFund(int fundId)
        {
            List<InvestorCommitment> returnList = new List<InvestorCommitment>();
            using (HqTrustData dbContext = new HqTrustData())
            {
                returnList= dbContext.InvestorCommitmnents.
                    Include("Investor").
                    Include("Investor.ClientAdvisor").
                    Include("InvestorCashFlows").
                    Include("InvestorPcaps").
                    Include("Investor.EMailAccounts").
                    Include("PeFund").
                    Include("PeFund.Currency").
                    Include("BankAccount").
                    Include("BankAccount.BankContact").
                    Where(c => c.PeFundId == fundId).
                    ToList();
            }
            //Add additional Information to properties
            foreach (InvestorCommitment commitment in returnList)
            {
                FillFullName(commitment.Investor);
                if (commitment.Investor.ClientAdvisorId > 0) FillDisplayName(commitment.Investor.ClientAdvisor);
            }
            return returnList;
        }
        public static async Task<IEnumerable<InvestorCommitment>> GetCommitmentsForPeFundAsync(int fundId)
        {

            using (HqTrustData dbContext = new HqTrustData())
            {
                commitmentsForAFund = await dbContext.InvestorCommitmnents.
                    Include("Investor").
                    Include("Investor.Advisor").
                    Include("Investor.ClientAdvisor").
                    Include("Investor.TaxAdvisor").
                    Include("PeFund").
                    Include("PeFund.Currency").
                    Include("BankAccount").
                    Include("BankAccount.Currency").
                    Include("BankAccount.BankContact").
                    Where(c => c.PeFundId == fundId).
                    ToListAsync();

                //Add additional Information to properties
                foreach (InvestorCommitment commitment in commitmentsForAFund)
                {
                    FillFullName(commitment.Investor);
                    if (commitment.Investor.ClientAdvisorId > 0) FillDisplayName(commitment.Investor.ClientAdvisor);
                }
                return commitmentsForAFund;


            }
        }
        public static async Task<IEnumerable<InvestorCommitment>> GetCommitmentsForPeFundIncludingCashFlowsAsync(int fundId)
        {

            using (HqTrustData dbContext = new HqTrustData())
            {
                commitmentsForAFund = await dbContext.InvestorCommitmnents.
                    Include("Investor").
                    Include("InvestorCashFlows").
                    Where(c => c.PeFundId == fundId).
                    ToListAsync();

                //Add additional Information to properties
  
                return commitmentsForAFund;
            }
        }

        private static void FillDisplayName(ClientAdvisor clientAdvisor)
        {
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrEmpty(clientAdvisor.AdvisorName.Title))
                builder.Append(clientAdvisor.AdvisorName.Title + " ");
            if (!string.IsNullOrEmpty(clientAdvisor.AdvisorName.FirstName))
                builder.Append(clientAdvisor.AdvisorName.FirstName + " ");
            if (!string.IsNullOrEmpty(clientAdvisor.AdvisorName.LastName))
                builder.Append(clientAdvisor.AdvisorName.LastName);
            clientAdvisor.DisplayName = builder.ToString();
        }

        private static void FillFullName(Investor investor)
        {
            // fill FullNameProperty of IName
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrEmpty(investor.IName.Title))
                builder.Append(investor.IName.Title + " ");
            if (!string.IsNullOrEmpty(investor.IName.FirstName))
                builder.Append(investor.IName.FirstName + " ");
            if (!string.IsNullOrEmpty(investor.IName.LastName))
                builder.Append(investor.IName.LastName);
            investor.IName.FullName = builder.ToString();
            
        }
        public static int GetUniqueCashFlowNumber(DateTime cfDate, int fundId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                UniqueCashFlowNumber record = new UniqueCashFlowNumber()
                {
                    CashFlowDate = cfDate,
                    PeFundId = fundId
                };
                dbContext.UniqueCashFlowNumbers.Add(record);
                dbContext.SaveChanges();
                return record.Id;
            }
        }

        public static bool CashFlowExists(DateTime cfDate, int fundId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                UniqueCashFlowNumber record = dbContext.UniqueCashFlowNumbers.FirstOrDefault(u => u.CashFlowDate == cfDate.Date && u.PeFundId == fundId);
                if (record == null) return false; else return true;
            }
        }
        /// <summary>
        /// returns true if more than one Fund uses the same Beteiligungsnummer
        /// (PS Plus makes no difference between legal entities)
        /// </summary>
        /// <param name="fund"></param>
        /// <returns></returns>
        public static bool BeteiligungsNummerExistsMoreThanOnce(PeFund fund)
        {
            bool morethanone = false;
            using (HqTrustData dbContext = new HqTrustData())
            {
                int count = dbContext.PeFunds.Where(p => p.FundHqTrustNumber == fund.FundHqTrustNumber).Count();
                if (count > 1) morethanone = true;
            }
            return morethanone;
        }
        public static List<UniqueCashFlowNumber>GetCashFlowsForFund(int fundId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                List<UniqueCashFlowNumber> listOfCashFlows = dbContext.UniqueCashFlowNumbers.OrderBy(u => u.CashFlowDate).Where(u => u.PeFundId == fundId).ToList();
                return listOfCashFlows;
            }
        }
        #region Pcaps

        /// <summary>
        /// returns InvestorPcap for an InvestorCommitment and a date
        /// returns null if not found
        /// </summary>
        /// <param name="commitmentId"></param>
        /// <param name="quarterend"></param>
        /// <returns></returns>
        public static InvestorPcap GetPcapForCommitmentAndDate(int commitmentId, DateTime quarterend)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.InvestorPcaps.FirstOrDefault(p => p.InvestorCommitmentId == commitmentId && p.AsOfDate == quarterend);
            }
        }
        /// <summary>
        /// returns investorpcap for a commitmnet prior to a date
        /// returns null if not available
        /// </summary>
        /// <param name="c"></param>
        /// <param name="selectedQuarter"></param>
        /// <returns></returns>
        public static InvestorPcap GetLastPCap(InvestorCommitment c, DateTime selectedQuarter)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.InvestorPcaps.OrderByDescending(p => p.AsOfDate).
                    Where(p => p.InvestorCommitmentId == c.Id && p.AsOfDate < selectedQuarter).FirstOrDefault();
            }
        }
        /// <summary>
        /// return a calculated NAV using a Investorpcap and cashflows between investorpcap and date
        /// </summary>
        /// <param name="lastPcap"></param>
        /// <param name="selectedQuarter"></param>
        /// <returns></returns>
       public static double NavCalculation(InvestorPcap lastPcap, DateTime selectedQuarter)
        {
            
            double nav = lastPcap.FinalPcapAmount;
            using (HqTrustData dbContext = new HqTrustData())
            {
                List<InvestorCashFlow> cfList = dbContext.InvestorCashFlows.
                    Where(c => c.InvestorCommitmentId == lastPcap.InvestorCommitmentId && c.EffectiveDate > lastPcap.AsOfDate && c.EffectiveDate <= selectedQuarter).ToList();
                foreach(InvestorCashFlow cashflow in cfList)
                {
                   if (cashflow.CashFlowType == "Capital Call")
                    {
                        nav -= cashflow.CashFlowAmount;
                    }
                   if(cashflow.CashFlowType == "Distribution")
                    {
                        nav -= cashflow.CashFlowAmount;                       
                    }
                }                    
            }
            return nav;
        }
        /// <summary>
        /// takes each element within a List of InvestorPcap and inserts or updates the database
        /// </summary>
        /// <param name="pcaps">List<InvestorPcap></InvestorPcap> </param>
        public static void UpdateOrInsertPcaps(List<InvestorPcap> pcaps)
        {
            if (pcaps.Count() == 0) return;
            using (HqTrustData dbContext = new HqTrustData())
            {
                foreach(InvestorPcap pcap in pcaps)
                {
                    if (pcap.Id ==0)
                    {
                        dbContext.InvestorPcaps.Add(pcap);
                    }
                    else
                    {
                        InvestorPcap existingPcap = dbContext.InvestorPcaps.FirstOrDefault(p => p.Id == pcap.Id);
                        if (existingPcap==null)
                        {
                            throw new Exception($"Das PCAP mit der Id {pcap.Id} wurde nicht gefunden");
                        }
                        dbContext.Entry(existingPcap).CurrentValues.SetValues(pcap);
                    }
                }
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Ändern der Tabelle InvestorPcaps. {ex.InnerException.Message}");
                }
            }
        }
        #endregion
    }
}
