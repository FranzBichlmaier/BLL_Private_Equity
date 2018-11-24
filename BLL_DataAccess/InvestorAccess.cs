using BLL_DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLL_DataAccess
{
    public  class InvestorAccess 
    {
        List<Investor> allInvestors = null;
        public async Task<IEnumerable<Investor>> GetAllInvestorsAsync()
        {
            if (allInvestors == null)
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    allInvestors = await dbContext.Investors.
                       Include("Advisor").
                       Include("ClientAdvisor").
                       Include("Commitments").
                       ToListAsync();
                }
                foreach(Investor investor in allInvestors)
                {
                    FillFullName(investor);
                    if (investor.ClientAdvisor != null)
                    {
                        FillDisplayName(investor.ClientAdvisor);
                    }
                }
            }          
            return allInvestors;
        }
        public List<Investor> GetInvestorsByClientAdvisorId(int clientAdvisorId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.Investors.
                    Include("ClientAdvisor").
                    Where(i => i.ClientAdvisorId == clientAdvisorId).
                    ToList();
            }
        }

        public void InsertInvestor(Investor investor)
        {

            //
            // UpdateClientAdvisor has updated investor.ClientAdvisor to a new clientadvisor
            //
            if (investor.ClientIsOwnAdvisor == true) UpdateClientAdvisor(investor);
            Investor newInvestor = investor.Copy(investor);      
            newInvestor.BankAccounts = new List<BankAccount>();
            newInvestor.DocumentAndLetters = new List<DocumentAndLetter>();
            newInvestor.TaxInformations = new List<TaxInformation>();
            newInvestor.EMailAccounts = new List<EMailAccount>();
            newInvestor.Currency = new Currency();

            using (HqTrustData dbContext = new HqTrustData())
            {
                dbContext.Investors.Add(newInvestor);
                try
                {
                    dbContext.SaveChanges();
                    investor.Id = newInvestor.Id;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Einfügen eines neuen Investors: {ex.InnerException.Message}");
                }
            }
            UpdateBankAccountsForInvestor(investor);
            UpdateDocumentAndLettersForInvestor(investor);
            UpdateTaxInformationsForInvestor(investor);
            UpdateEMailAccountsForInvestor(investor);
        }

        private void UpdateEMailAccountsForInvestor(Investor investor)
        {
           

            using (HqTrustData dbContext = new HqTrustData())
            {
                var existingEmails = dbContext.EmailAccounts.Where(b => b.InvestorId == investor.Id).ToList();

                // foreach account in existingAccounts try to find a record in investor.BankAccounts
                // if found --> update properties
                // if not found --> record was deleted by user --> remove account

                foreach (EMailAccount document in existingEmails)
                {
                    EMailAccount newDoc = investor.EMailAccounts.FirstOrDefault(b => b.Id == document.Id);
                    if (newDoc == null)
                    {
                        // not found --> remove
                        dbContext.EmailAccounts.Remove(document);
                    }
                    else
                    {
                        // found --> update properties
                        document.EmailAddress = newDoc.EmailAddress;
                        document.Salutation = newDoc.Salutation;
                    }
                }


                foreach (EMailAccount document in investor.EMailAccounts)
                {
                    if (document.Id != 0) continue;
                    document.InvestorId = investor.Id;
                    dbContext.EmailAccounts.Add(document);
                }
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Einfügen eines neuen Investors (Tabelle: EMailAccounts) Fehler: {ex.InnerException.Message}");
                }
            }
        }

        private void UpdateTaxInformationsForInvestor(Investor investor)
        {

            using (HqTrustData dbContext = new HqTrustData())
            {
                var existingTax = dbContext.TaxInformations.Where(b => b.InvestorId == investor.Id).ToList();

                // foreach account in existingAccounts try to find a record in investor.BankAccounts
                // if found --> update properties
                // if not found --> record was deleted by user --> remove account

                foreach (TaxInformation document in existingTax)
                {
                    TaxInformation newDoc = investor.TaxInformations.FirstOrDefault(b => b.Id == document.Id);
                    if (newDoc == null)
                    {
                        // not found --> remove
                        dbContext.TaxInformations.Remove(document);
                    }
                    else
                    {
                        // found --> update properties
                        document.CountryId = newDoc.CountryId;
                        document.Remarks = newDoc.Remarks;
                        document.TaxIdentificationNumber = newDoc.TaxIdentificationNumber;
                        document.TaxStatus = newDoc.TaxStatus;                      
                    }
                }


                foreach (TaxInformation document in investor.TaxInformations)
                {
                    if (document.Id != 0) continue;                    
                    document.InvestorId = investor.Id;
                    dbContext.TaxInformations.Add(document);
                }
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Einfügen eines neuen Investors (Tabelle: TaxInformations) Fehler: {ex.InnerException.Message}");
                }
            }
        }

        private void UpdateDocumentAndLettersForInvestor(Investor investor)
        {

            using (HqTrustData dbContext = new HqTrustData())
            {
                var existingDocs = dbContext.DocumentAndLetters.Where(b => b.InvestorId == investor.Id).ToList();

                // foreach account in existingAccounts try to find a record in investor.BankAccounts
                // if found --> update properties
                // if not found --> record was deleted by user --> remove account

                foreach (DocumentAndLetter document in existingDocs)
                {
                    DocumentAndLetter   newDoc = investor.DocumentAndLetters.FirstOrDefault(b => b.Id == document.Id);
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


                foreach (DocumentAndLetter document in investor.DocumentAndLetters)
                {
                    if (document.Id != 0) continue;
                    document.PeFundId = null;
                    document.InvestorId = investor.Id;
                    dbContext.DocumentAndLetters.Add(document);
                }
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Einfügen eines neuen Investors (Tabelle: DocumentAndLetters) Fehler: {ex.InnerException.Message}");
                }
            }
        }

        public void UpdateInvestor(Investor investor)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {

                Investor updateInvestor = dbContext.Investors.FirstOrDefault(i => i.Id == investor.Id);
                if (updateInvestor == null)
                {
                    throw new Exception($"Der Investor mit der Id {investor.Id} wurde nicht in der Datenbank gefunden");
                }
                dbContext.Entry(updateInvestor).CurrentValues.SetValues(investor);
                try
                {
                    dbContext.SaveChanges();                    
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Ändern eines Investors: {ex.InnerException.Message}");
                }
            }
            UpdateBankAccountsForInvestor(investor);
            UpdateDocumentAndLettersForInvestor(investor);
            UpdateTaxInformationsForInvestor(investor);
            UpdateEMailAccountsForInvestor(investor);
        }

        private void UpdateBankAccountsForInvestor(Investor investor)
        {
            
            using (HqTrustData dbContext = new HqTrustData())
            {
                var existingAccounts = dbContext.BankAccounts.Where(b => b.InvestorId == investor.Id).ToList();

                // foreach account in existingAccounts try to find a record in investor.BankAccounts
                // if found --> update properties
                // if not found --> record was deleted by user --> remove account

                foreach (BankAccount account in existingAccounts)
                {
                    BankAccount newAccount = investor.BankAccounts.FirstOrDefault(b => b.Id == account.Id);
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
                        account.InvestorId = investor.Id;
                        account.PefundId = null;
                        account.Signature1 = newAccount.Signature1;
                        account.Signature2 = newAccount.Signature2;
                        account.SwiftAddress = newAccount.SwiftAddress;                        
                    }
                }
               

                    foreach (BankAccount account in investor.BankAccounts)
                    {
                        if (account.Id != 0) continue;
                        account.PefundId = null;
                        account.InvestorId = investor.Id;
                        dbContext.BankAccounts.Add(account);
                    }
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Einfügen eines neuen Investors (Tabelle: BankAccounts) Fehler: {ex.InnerException.Message}");
                }
            }
        }
        private void UpdateClientAdvisor(Investor investor)
        {
            // if investor.ClientAdvisorId == 0 Insert ClientAdviosr
            //                              else Update ClientAdvisor

            if (investor.ClientAdvisorId == null || investor.ClientAdvisorId == 0)
            {
                ClientAdvisor newAdvisor = new ClientAdvisor()
                {
                    Address = investor.PrivateAddress,
                    AdvisorName = investor.IName,
                    IsClient = true
                };
                using (HqTrustData dbContext = new HqTrustData())
                {
                    dbContext.ClientAdvisors.Add(newAdvisor);
                    try
                    {
                        dbContext.SaveChanges();
                        investor.ClientAdvisorId = newAdvisor.Id;
                        return;
                    }
                    catch (Exception)
                    {
                        throw new Exception("Fehler beim Einfügen eines Ansprechpartners");
                    }
                }
            }

            // Read Clientadvisor and set Address of Investor
            using (HqTrustData dbContext = new HqTrustData())
            {
                ClientAdvisor newAdvisor = dbContext.ClientAdvisors.FirstOrDefault(c => c.Id == investor.ClientAdvisorId);
                if (newAdvisor == null) throw new Exception("Ansprechpartner wurde nicht gefunden");
                newAdvisor.AdvisorName = investor.IName;
                newAdvisor.Address = investor.PrivateAddress;
                newAdvisor.IsClient = true;
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Removes an investor and the related bankaccounts, EMailAccounts, taxinformation, Todos and documents from the database
        /// </summary>
        /// <param name="investor"></param>
        public void RemoveInvestor(Investor investor)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                Investor removeInvestor = dbContext.Investors.FirstOrDefault(i => i.Id == investor.Id);
                if (removeInvestor ==null)
                {
                    throw new Exception($"Der Investor mit der Id {investor.Id} wurde in der Datenbank nicht gefunden.");
                }
                dbContext.Investors.Remove(removeInvestor);

                try
                {
                    // remove related BankAccounts
                    var results = dbContext.BankAccounts.Where(b => b.InvestorId == investor.Id).ToList();
                    if (results.Count > 0)
                        dbContext.BankAccounts.RemoveRange(results);

                    // remove related E-MailAccounts
                    var emails = dbContext.EmailAccounts.Where(e => e.InvestorId == investor.Id).ToList();
                    if (emails.Count > 0)
                        dbContext.EmailAccounts.RemoveRange(emails);

                    // remove related TaxInformation
                    var taxes = dbContext.TaxInformations.Where(e => e.InvestorId == investor.Id).ToList();
                    if (taxes.Count > 0)
                        dbContext.TaxInformations.RemoveRange(taxes);

                    // remove related Documents and LetterInformation
                    var docs = dbContext.DocumentAndLetters.Where(e => e.InvestorId == investor.Id).ToList();
                    if (docs.Count > 0)
                        dbContext.DocumentAndLetters.RemoveRange(docs);

                    // remove related Todos
                    var todos = dbContext.InvestorToDos.Where(e => e.InvestorId == investor.Id).ToList();
                    if (todos.Count > 0)
                        dbContext.InvestorToDos.RemoveRange(todos);
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
                    throw new Exception($"Fehler beim Löschen des Investors. Ursache: {ex.InnerException.Message}");
                }
            }
        }

        public async Task<IEnumerable<InvestorCommitment>> GetHqtCommitmentsAsync()
        {
            List<InvestorCommitment> commitments = new List<InvestorCommitment>();
           
            using (HqTrustData dbContext = new HqTrustData())
            {
                 var investors = await dbContext.Investors.
                                Include("Commitments").
                                Include("Commitments.PeFund").
                                Include("Commitments.PeFund.Currency").
                                Where( i => string.IsNullOrEmpty(i.InvestorHqTrustAccount) != true).
                                ToListAsync();

                foreach(Investor investor in investors)
                { 
                    foreach(InvestorCommitment commitment in investor.Commitments)
                    {     
                        commitment.Investor = investor;
                        commitments.Add(commitment);
                    }
                }
            }
            return commitments;
        }
        public void TruncateImportCommitment()
        {
            // the existing content of ImportCommitments is truncated
            // foreach Commitment where Investor is HqtClient
            //  insert record in ImportCommitments
            using (HqTrustData dbContext = new HqTrustData())
            {
                string sqlTruncate = "Truncate Table ImportCommitments";
                try
                {
                    dbContext.Database.ExecuteSqlCommand(sqlTruncate);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Leeren der Tabelle ImportCommiments. {ex.Message}");
                }
               
            }
        }
        public ImportCommitment FindImportCommitment(string peNumber, string investorNumber)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.ImportCommitments.FirstOrDefault(i => i.PeFundNumber.Equals(peNumber) && i.InvestorNumber.Equals(investorNumber));
            }
        }
        public void InsertImportCommitment(ImportCommitment import)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                dbContext.ImportCommitments.Add(import);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Fehler beim Einfügen eines PS Plus Commitments {ex.Message}");
                }
            }
        }

        private void FillDisplayName(ClientAdvisor clientAdvisor)
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

        public void UpdateInvestorList(Investor investor)
        {
            Investor existingInvestor = allInvestors.FirstOrDefault(i => i.Id == investor.Id);
            if (existingInvestor == null)
            {
                allInvestors.Add(investor);
                return;
            }
            throw new NotImplementedException();
        }

        public Investor GetInvestorById(int investorId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.Investors.FirstOrDefault(i => i.Id == investorId);
            }
        }
        public Investor GetInvestorByHqTrustNumber(string hqTrustNumber)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.Investors.FirstOrDefault(i => i.InvestorHqTrustAccount.Equals(hqTrustNumber));
            }
        }

        public Investor GetInvestorDetailsById (int investorId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
               Investor investor= dbContext.Investors.
                    Include("Commitments").
                    Include("BankAccounts").
                    Include("BankAccounts.Currency").
                    Include("EMailAccounts").
                    Include("Advisor").
                    Include("ClientAdvisor").
                    Include("TaxInformations").
                    Include("Currency").
                    Include("DocumentAndLetters").
                    FirstOrDefault(i => i.Id == investorId);
                    FillFullName(investor);
                    if (investor.ClientAdvisor != null)
                    {
                        FillDisplayName(investor.ClientAdvisor);
                    }
                    return investor;
            }

        }
        public List<InvestorToDo> GetToDosForInvestor(int investorid)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.InvestorToDos.Where(t => t.InvestorId == investorid).ToList();
            }
        }
        public IEnumerable<InvestorCommitment> GetCommitmentsForInvestor(int investorId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.InvestorCommitmnents.
                    Include("PeFund").
                    Include("PeFund.Currency").
                    Include("InvestorCommitmentDetails").
                    Include("InvestorCashFlows").
                    Include("InvestorPcaps").
                    Where(c => c.InvestorId == investorId).ToList();
            }
        }
        public InvestorCommitment GetInvestorCommitmentById(int id)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.InvestorCommitmnents.FirstOrDefault(c => c.Id == id);
            }
        }
        public InvestorCommitment GetInvestorCommitmentByFundAndInvestor(int fundId, int investorId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                var result = dbContext.InvestorCommitmnents.Where(c => c.PeFundId == fundId && c.InvestorId == investorId).ToList();
                if (result.Count>1)
                {

                }
                if (result.Count == 1) return result.ElementAt(0);
                return null;
            }
        }
        public InvestorCommitment UpdateInvestorCommitments(InvestorCommitment commitment)
        {
            if (commitment.Id ==0)
            {
                // add new commitment
                using (HqTrustData dbContext = new HqTrustData())
                {
                    InvestorCommitment newCommitment = new InvestorCommitment(commitment);
                    newCommitment.PeFund = null;
                    dbContext.InvestorCommitmnents.Add(newCommitment);
                    try
                    {
                        dbContext.SaveChanges();
                        return newCommitment;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Das Commitment konnte nicht in der Datenbank gespeichert werden" + Environment.NewLine + ex.InnerException.Message);                        
                    }
                }
            }
            else
            {
                // update commitments
                using (HqTrustData dbContext = new HqTrustData())
                {
                    InvestorCommitment old = dbContext.InvestorCommitmnents.
                        Include("InvestorCommitmentDetails").
                        FirstOrDefault(c => c.Id == commitment.Id);
                    if (old == null)
                    {
                        throw new Exception($"Das Commitment mit der Id {commitment.Id} für den Investor {commitment.InvestorId} wurde nicht in der Datenbank gefunden");
                    }
                    old.BankAccountId = commitment.BankAccountId;
                    old.CommitmentAmount = commitment.CommitmentAmount;
                    old.CommitmentPlannedAmount = commitment.CommitmentPlannedAmount;
                    old.DateCommitmentAccepted = commitment.DateCommitmentAccepted;                    
                    old.PeFundReference = commitment.PeFundReference;

                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Das Commitment konnte in der Datenbank nicht geändert werden" + Environment.NewLine + ex.InnerException.Message);
                    }
                    

                    //foreach(InvestorCommitmentDetail detail in old.InvestorCommitmentDetails)
                    for (int i =0; i<old.InvestorCommitmentDetails.Count;i++)
                    {
                        InvestorCommitmentDetail detail = old.InvestorCommitmentDetails.ElementAt(i);
                        InvestorCommitmentDetail oldDetail = dbContext.InvestorCommitmentDetails.FirstOrDefault(d => d.Id == detail.Id);
                        if (oldDetail == null)
                        {
                            throw new Exception($"Das CommitmentDetail mit der Id {detail.Id} für das Commitment {old.Id} wurde nicht in der Datenbank gefunden");
                        }
                        InvestorCommitmentDetail newDetail = commitment.InvestorCommitmentDetails.FirstOrDefault(n => n.Id == detail.Id);
                        if (newDetail == null)
                        {
                            // record has been delete by the user:
                            dbContext.InvestorCommitmentDetails.Remove(oldDetail);

                            dbContext.SaveChanges();
                        }
                        else
                        {
                            oldDetail.CommitmentAmount = newDetail.CommitmentAmount;
                            oldDetail.CommitmentDate = newDetail.CommitmentDate;
                            oldDetail.SecondaryCallsAfterCutOff = newDetail.SecondaryCallsAfterCutOff;
                            oldDetail.SecondaryCutOffDate = newDetail.SecondaryCutOffDate;
                            oldDetail.SecondaryDistributionsAfterCutOff = newDetail.SecondaryDistributionsAfterCutOff;
                            oldDetail.SecondaryOpenCommitment = newDetail.SecondaryOpenCommitment;
                            oldDetail.SecondaryPurchaseAmount = newDetail.SecondaryPurchaseAmount;

                            dbContext.SaveChanges();
                        }
                    }

                    foreach (InvestorCommitmentDetail detail in commitment.InvestorCommitmentDetails)
                    {
                        if (detail.Id > 0) continue;
                        dbContext.InvestorCommitmentDetails.Add(detail);
                    }

                    try
                    {
                        dbContext.SaveChanges();
                        return commitment;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Das Commitment konnte in der Datenbank nicht geändert werden" + Environment.NewLine + ex.InnerException.Message);
                    }
                }
            }
        }

        public void DeleteInvestorCommitment(InvestorCommitment commitment)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                InvestorCommitment old = dbContext.InvestorCommitmnents.                    
                    FirstOrDefault(c => c.Id == commitment.Id);
                if (old == null)
                {
                    throw new Exception($"Das Commitment mit der Id {commitment.Id} für den Investor {commitment.InvestorId} wurde nicht in der Datenbank gefunden");
                }

                dbContext.InvestorCommitmnents.Remove(old);

                foreach(InvestorCommitmentDetail detail in commitment.InvestorCommitmentDetails)
                {
                    InvestorCommitmentDetail icDetail = dbContext.InvestorCommitmentDetails.FirstOrDefault(d => d.Id == detail.Id);
                    if (icDetail == null)
                    {
                        throw new Exception($"Das CommitmentDetail mit der Id {icDetail.Id} für das Commitment {old.Id} wurde nicht in der Datenbank gefunden");
                    }
                    dbContext.InvestorCommitmentDetails.Remove(icDetail);
                }
                try
                {
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {

                    throw new Exception("Das Commitment konnte in der Datenbank nicht gelöscht werden" + Environment.NewLine + ex.InnerException.Message);
                }
            }
        }public InvestorCommitment GetFullyLoadedInvestorCommitment(int commitmentId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.InvestorCommitmnents.
                    Include("Investor").
                    Include("PeFund").
                    Include("PeFund.Currency").
                    Include("InvestorCashFlows").
                    Include("InvestorPcaps").
                    FirstOrDefault(c => c.Id == commitmentId);
            }
        }
        public List<InvestorCashFlow> GetCashFlowsForCommitment(int commitmentId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                List<InvestorCashFlow> resultList = dbContext.InvestorCashFlows                    
                    .Where(c => c.InvestorCommitmentId == commitmentId).ToList();
                return resultList;
            }
        }

        public void UpdateInvestorCashFlow(InvestorCashFlow cashflow)
        {
            if (cashflow.Id == 0)
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    dbContext.InvestorCashFlows.Add(cashflow);
                    try
                    {
                        dbContext.SaveChanges();
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Fehler beim Einfügen eines CashFlows. {ex.InnerException.Message}");
                    }
                }
            }
            else
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    InvestorCashFlow cf = dbContext.InvestorCashFlows.FirstOrDefault(c => c.Id == cashflow.Id);
                    if (cf == null)
                    {
                        throw new Exception($"Fehler beim Lesen eines CashFlows mit der Id {cashflow.Id}.");
                    }
                    dbContext.Entry(cf).CurrentValues.SetValues(cashflow);
                    try
                    {
                        dbContext.SaveChanges();
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Fehler beim Ändern des CashFlows mit der Id {cashflow.Id}. {ex.InnerException.Message}");
                    }
                }
            }
        }

        public List<InvestorCashFlow> GetInvestorCashFlowsByUniqueNumber(int unique)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                List<InvestorCashFlow> resultList = dbContext.InvestorCashFlows
                    .Include("InvestorCommitment")
                    .Include("InvestorCommitment.Investor")
                    .Where(c => c.UniqueCashFlowId == unique).ToList();
                return resultList;
            }
        }
        public async Task<List<InvestorCashFlow>> GetInvestorCashFlowsByUniqueNumberAsync(int unique)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                List<InvestorCashFlow> cashFlows = await dbContext.InvestorCashFlows
                     .Include("InvestorCommitment")
                    .Include("InvestorCommitment.Investor")
                    .Where(c => c.UniqueCashFlowId == unique).ToListAsync();
                return cashFlows;
            }
        }

        public int DeleteInvestorCashFlows(List<InvestorCashFlow> cashFlows)
        {
            int counter = 0;
            using (HqTrustData dbContext = new HqTrustData())
            {
                foreach (InvestorCashFlow cf in cashFlows)
                {
                    InvestorCashFlow item = dbContext.InvestorCashFlows.FirstOrDefault(c => c.Id == cf.Id);
                    if (item != null)
                        dbContext.InvestorCashFlows.Remove(item);
                    counter++;
                }
                try
                {
                    dbContext.SaveChanges();
                    return counter;
                }
                catch (Exception ex)
                {

                    throw new Exception($"Fehler beim Löschen von Cashflows. {ex.InnerException.Message}");
                }
            }
        }

        public int AddInvestorCashFlows(List<InvestorCashFlow> cashFlows)
        {
           
            using (HqTrustData dbContext = new HqTrustData())
            {
                var entityList = dbContext.InvestorCashFlows.AddRange(cashFlows);
                try
                {
                    dbContext.SaveChanges();
                    return entityList.Count();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Beim Eintragen von CashFlows ist ein Fehler aufgetreten: {ex.InnerException.Message}");
                }
            }
        }
        public Investor GetSampleInvestor()
        {
            Investor sample = new Investor()
            {
                IName = new Name()
                {
                    Title = "Dr.",
                    FirstName = "Max",
                    LastName = "Mustermann",
                    AddressName = "Herrn Dr. Mustermann",
                    Salutation = "Sehr geehrter Herr Dr. Mustermann",
                    EmailAddress = "max@mustermann.de",
                    FullName = "Dr. Max Mustermann"
                },
                PrivateAddress = new Address()
                {
                    Street = "Hauptstraße 10",
                    ZipCode = "12345",
                    City = "Musterstadt",
                    Country = "Deutschland"
                },
                ConfidentialLetter = true         
            };
            BankAccount account = new BankAccount()
            {
                Iban = "DE12 3456 7890 9876 5432 10",
                BankName = "Sparkasse Musterstadt",
                CurrencyName = "USD"
            };
            sample.BankAccounts.Add(account);
            ClientAdvisor advisor = new ClientAdvisor()
            {
                AdvisorName = new Name()
                {
                    Title = "Dr.",
                    FirstName = "Anton",
                    LastName = "Kundenberater",
                    AddressName = "Herrn Dr. Anton Kundenberater",
                    Salutation = "Sehr geehrter Herr Dr. Kundenberater",
                },
                Address = new Address()
                {
                    Street = "Am Stadtrand 5",
                    ZipCode = "12345",
                    City = "Musterstadt",
                    Country = "Deutschland"
                }
            };
            sample.ClientAdvisorId = 1;
            sample.ClientAdvisor = advisor;
            sample.InvestorReference = "Beispiel-Kunde";
            return sample;
        }
        #region PcapFunctions

        public InvestorPcap GetPcapForInvestorByCommitmentAndDate(int commitmentId, DateTime navDate)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.InvestorPcaps.FirstOrDefault(p => p.InvestorCommitmentId == commitmentId && p.AsOfDate == navDate);
            }
        }
        public void InsertOrUpdateInvestorPcap(InvestorPcap pcap)
        {
            if (pcap.Id == 0)
            {
                // inser new record
                using (HqTrustData dbContext = new HqTrustData())
                {
                    dbContext.InvestorPcaps.Add(pcap);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Fehler beim Eintragen eines PCap. {ex.InnerException.Message}");
                    }
                }
            }
            else
            {
                using (HqTrustData dbContext = new HqTrustData())
                {
                    InvestorPcap existingPcap = dbContext.InvestorPcaps.FirstOrDefault(p => p.Id == pcap.Id);
                    if(existingPcap == null)
                    {
                        throw new Exception($"Ein NAV für das Commitment {pcap.InvestorCommitmentId} mit dem Datum {pcap.AsOfDate:d} wurde nicht gefunden.");
                    }
                    dbContext.Entry(existingPcap).CurrentValues.SetValues(pcap);
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Fehler beim Ändern eines PCap. {ex.InnerException.Message}");
                    }
                }

            }
        }
        #endregion

    }
}

