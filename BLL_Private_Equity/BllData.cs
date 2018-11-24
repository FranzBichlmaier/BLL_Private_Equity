using BLL_DataModels;
using System;
using System.Data.Entity;

namespace BLL_Private_Equity
{
    public class BllData: DbContext
    {
        private static string connectionInformation = "name=BllConnectionString";
        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<FundManager> FundManagers { get; set; }
        public DbSet<Investor> Investors { get; set; }
        public DbSet<InvestorCommitment> InvestorCommitmnents { get; set; }
        public DbSet<InvestorCommitmentDetail> InvestorCommitmentDetails { get; set; }
        public DbSet<PeFund> PeFunds { get; set; }
        public DbSet<FundType> FundTypes { get; set; }
        public DbSet<FundCompanySize> FundCompanySizes { get; set; }
        public DbSet<FundGeography> FundGeographies { get; set; }
        public DbSet<InvestorCashFlow> InvestorCashFlows { get; set; }
        public DbSet<InvestorPcap> InvestorPcaps { get; set; }
        public DbSet<PeFundCashFlow> PeFundCashFlows { get; set; }
        public DbSet<PeFundPcap> PeFundPcaps { get; set; }
        public DbSet<PrintPcap> PrintPcaps { get; set; }
        public DbSet<Initiator> Initiators { get; set; }
        public DbSet<DocumentAndLetter> DocumentAndLetters { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<ReportInformationCapitalCall> ReportInformationCapitalCalls { get; set; }
        public DbSet<ReportNumber> ReportNumbers { get; set; }
        //public DbSet<PeFundPerformance> PeFundPerformances { get; set; }
        public DbSet<AnwendungsInformationen> AnwendungsInformationen { get; set; }
        public DbSet<CashFlowBarChart> CashFlowBarCharts { get; set; }
        public DbSet<TextComponent> TextComponents { get; set; }
        public DbSet<InvestorToDo> InvestorToDos { get; set; }
        public DbSet<EMailAccount> EmailAccounts { get; set; }
        public DbSet<UniqueCashFlowNumber> UniqueCashFlowNumbers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<TaxInformation> TaxInformations { get; set; }
        //public DbSet<PrepCashFlow> PrepCashFlows { get; set; }
        public DbSet<ClientAdvisor> ClientAdvisors { get; set; }
        public DbSet<ImportCommitment> ImportCommitments { get; set; }
        public DbSet<Message> Messages { get; set; }

        public BllData():base(connectionInformation)
        {
            try
            {
                //Database.CreateIfNotExists();
                //Database.SetInitializer(new DropCreateDatabaseAlways<HqTrustData, HqTrustPeAdmin.Migrations.Configuration>());
                //Database.SetInitializer<HqTrustData>(new DropCreateDatabaseAlways<HqTrustData>());
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<BllData, Migrations.Configuration>());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.ToString());
            }

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // PostgreSQL uses the public schema by default - not dbo.
            modelBuilder.HasDefaultSchema("public");

            // all decimal values are set to Precision 18.2; exeptions will be overwritten by code thereafter

            modelBuilder.Properties<decimal>().Configure(c => c.HasPrecision(18, 2));

            // set precision for currencyrates to 18.5

            //modelBuilder.Entity<FundCashFlow>().Property(x => x.CurrencyRate).HasPrecision(18, 5);
            //modelBuilder.Entity<ParticipationCashFlow>().Property(x => x.CurrencyRate).HasPrecision(18, 5);
            //modelBuilder.Entity<ParticipationReport>().Property(x => x.CurrencyRate).HasPrecision(18, 5);

            base.OnModelCreating(modelBuilder);
        }
    }
}
