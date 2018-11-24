namespace BLL_Private_Equity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSetUp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.Advisors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        EmailAddress = c.String(),
                        FaxExtension = c.String(),
                        ReportsToId = c.Int(),
                        Extension = c.String(),
                        LastName = c.String(maxLength: 50),
                        FirstName = c.String(maxLength: 50),
                        Title = c.String(maxLength: 10),
                        PrivateEquityExpert = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Advisors", t => t.ReportsToId)
                .Index(t => t.ReportsToId);
            
            CreateTable(
                "public.AnwendungsInformationens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.BankAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrencyId = c.Int(nullable: false),
                        BankName = c.String(maxLength: 75),
                        BankAddress = c.String(maxLength: 200),
                        AccountNumber = c.String(maxLength: 20),
                        BankNumberBlz = c.String(maxLength: 20),
                        Iban = c.String(maxLength: 30),
                        Bic = c.String(maxLength: 30),
                        SwiftAddress = c.String(maxLength: 15),
                        BeneficiaryBank = c.String(maxLength: 50),
                        FfcAccountHolderName = c.String(maxLength: 50),
                        FfcAccountNumber = c.String(maxLength: 25),
                        AdditionalInstructions = c.String(),
                        Signature1 = c.String(maxLength: 30),
                        Signature2 = c.String(maxLength: 30),
                        AccountHolder = c.String(),
                        InvestorId = c.Int(),
                        PefundId = c.Int(),
                        BankContactId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.ClientAdvisors", t => t.BankContactId)
                .ForeignKey("public.Currencies", t => t.CurrencyId, cascadeDelete: true)
                .ForeignKey("public.Investors", t => t.InvestorId)
                .ForeignKey("public.PeFunds", t => t.PefundId)
                .Index(t => t.CurrencyId)
                .Index(t => t.InvestorId)
                .Index(t => t.PefundId)
                .Index(t => t.BankContactId);
            
            CreateTable(
                "public.ClientAdvisors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(maxLength: 50),
                        AdvisorName_Title = c.String(maxLength: 10),
                        AdvisorName_FirstName = c.String(maxLength: 30),
                        AdvisorName_LastName = c.String(maxLength: 50),
                        AdvisorName_AddressName = c.String(maxLength: 50),
                        AdvisorName_Salutation = c.String(maxLength: 70),
                        AdvisorName_TelephoneNumber = c.String(maxLength: 20),
                        AdvisorName_TelephoneNumber2 = c.String(),
                        AdvisorName_FaxNumber = c.String(maxLength: 20),
                        AdvisorName_EmailAddress = c.String(maxLength: 70),
                        Address_Country = c.String(maxLength: 100),
                        Address_City = c.String(maxLength: 100),
                        Address_ZipCode = c.String(maxLength: 100),
                        Address_Street = c.String(maxLength: 100),
                        Address_Street2 = c.String(),
                        Department = c.String(maxLength: 50),
                        Position = c.String(maxLength: 30),
                        EarlyNotification = c.Int(nullable: false),
                        IsTaxAdvisor = c.Boolean(nullable: false),
                        IsClient = c.Boolean(nullable: false),
                        IsAdvisor = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.Currencies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrencyShortName = c.String(),
                        CurrencyName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.CashFlowBarCharts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                        Calls = c.Double(nullable: false),
                        Distributions = c.Double(nullable: false),
                        Balance = c.Double(nullable: false),
                        CumulatedBalance = c.Double(nullable: false),
                        CumulatedCapitalCalls = c.Double(nullable: false),
                        CumulatedDistributions = c.Double(nullable: false),
                        Nav = c.Double(nullable: false),
                        InvestorCommitmentId = c.Int(nullable: false),
                        ReportId = c.Int(nullable: false),
                        ReportAsOf = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CountryName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.CurrencyRates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AsOfDate = c.DateTime(nullable: false),
                        PeFundCurrencyId = c.Int(nullable: false),
                        InvestorCurrencyId = c.Int(nullable: false),
                        Rate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Currencies", t => t.InvestorCurrencyId, cascadeDelete: true)
                .ForeignKey("public.Currencies", t => t.PeFundCurrencyId, cascadeDelete: true)
                .Index(t => t.PeFundCurrencyId)
                .Index(t => t.InvestorCurrencyId);
            
            CreateTable(
                "public.DocumentAndLetters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentType = c.Int(nullable: false),
                        InvestorId = c.Int(),
                        PeFundId = c.Int(),
                        DocumentDate = c.DateTime(nullable: false),
                        DocumentDescription = c.String(maxLength: 100),
                        DocumentFileName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Investors", t => t.InvestorId)
                .ForeignKey("public.PeFunds", t => t.PeFundId)
                .Index(t => t.InvestorId)
                .Index(t => t.PeFundId);
            
            CreateTable(
                "public.EMailAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(maxLength: 100),
                        Salutation = c.String(maxLength: 100),
                        InvestorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Investors", t => t.InvestorId)
                .Index(t => t.InvestorId);
            
            CreateTable(
                "public.FundCompanySizes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanySize = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.FundGeographies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Geography = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.FundManagers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FundManagerName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.FundTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FundTypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.ImportCommitments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PeFundNumber = c.String(maxLength: 15),
                        PeFundCurrency = c.String(maxLength: 5),
                        PeFundCurrencyId = c.Int(nullable: false),
                        PeFundName = c.String(maxLength: 50),
                        InvestorNumber = c.String(maxLength: 15),
                        InvestorCurrency = c.String(maxLength: 5),
                        InvestorCurrencyId = c.Int(nullable: false),
                        AsOfDate = c.DateTime(nullable: false),
                        Commitment = c.Double(nullable: false),
                        PeFundId = c.Int(nullable: false),
                        InvestorId = c.Int(nullable: false),
                        InvestorCommitmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.Initiators",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InitiatorName = c.String(maxLength: 100),
                        AdvisorId = c.Int(nullable: false),
                        InitiatorAddress_Country = c.String(maxLength: 100),
                        InitiatorAddress_City = c.String(maxLength: 100),
                        InitiatorAddress_ZipCode = c.String(maxLength: 100),
                        InitiatorAddress_Street = c.String(maxLength: 100),
                        InitiatorAddress_Street2 = c.String(),
                        HeadQuarterAddress_Country = c.String(maxLength: 100),
                        HeadQuarterAddress_City = c.String(maxLength: 100),
                        HeadQuarterAddress_ZipCode = c.String(maxLength: 100),
                        HeadQuarterAddress_Street = c.String(maxLength: 100),
                        HeadQuarterAddress_Street2 = c.String(),
                        SenderInformation = c.String(maxLength: 150),
                        SenderInformationLine2 = c.String(maxLength: 150),
                        FooterLine1 = c.String(maxLength: 200),
                        FooterLine2 = c.String(maxLength: 200),
                        FooterLine3 = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Advisors", t => t.AdvisorId, cascadeDelete: true)
                .Index(t => t.AdvisorId);
            
            CreateTable(
                "public.InvestorCashFlows",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PsPlusId = c.Long(nullable: false),
                        CashFlowAmountInInvestorCurrency = c.Double(nullable: false),
                        InvestorCommitmentId = c.Int(nullable: false),
                        UniqueCashFlowId = c.Int(),
                        EffectiveDate = c.DateTime(nullable: false),
                        CashFlowType = c.String(),
                        CashFlowNumber = c.Int(nullable: false),
                        CashFlowDescription = c.String(),
                        CommitmentAmount = c.Double(nullable: false),
                        CashFlowAmount = c.Double(nullable: false),
                        ReturnOfCapital = c.Double(nullable: false),
                        CapitalGain = c.Double(nullable: false),
                        OpenPayments = c.Double(nullable: false),
                        Dividends = c.Double(nullable: false),
                        OtherIncome = c.Double(nullable: false),
                        RecallableAmount = c.Double(nullable: false),
                        PartnershipExpenses = c.Double(nullable: false),
                        WithholdingTax = c.Double(nullable: false),
                        InvestorExpenses = c.Double(nullable: false),
                        LookbackInterests = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.InvestorCommitments", t => t.InvestorCommitmentId, cascadeDelete: true)
                .Index(t => t.InvestorCommitmentId);
            
            CreateTable(
                "public.InvestorCommitments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateCommitmentAccepted = c.DateTime(),
                        CommitmentPlannedAmount = c.Double(nullable: false),
                        InvestorId = c.Int(),
                        BankAccountId = c.Int(),
                        PeFundId = c.Int(nullable: false),
                        CommitmentAmount = c.Double(nullable: false),
                        PeFundReference = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.BankAccounts", t => t.BankAccountId)
                .ForeignKey("public.Investors", t => t.InvestorId)
                .ForeignKey("public.PeFunds", t => t.PeFundId, cascadeDelete: true)
                .Index(t => t.InvestorId)
                .Index(t => t.BankAccountId)
                .Index(t => t.PeFundId);
            
            CreateTable(
                "public.Investors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Group = c.Int(nullable: false),
                        CurrencyId = c.Int(),
                        InvestorHqTrustAccount = c.String(maxLength: 15),
                        DirectoryPath = c.String(maxLength: 50),
                        ZeichnungsBerechtigung = c.String(),
                        IName_Title = c.String(maxLength: 10),
                        IName_FirstName = c.String(maxLength: 30),
                        IName_LastName = c.String(maxLength: 50),
                        IName_AddressName = c.String(maxLength: 50),
                        IName_Salutation = c.String(maxLength: 70),
                        IName_TelephoneNumber = c.String(maxLength: 20),
                        IName_TelephoneNumber2 = c.String(),
                        IName_FaxNumber = c.String(maxLength: 20),
                        IName_EmailAddress = c.String(maxLength: 70),
                        InvestorReference = c.String(),
                        PsPlusLevel = c.String(maxLength: 12),
                        ClientAdvisorId = c.Int(),
                        TaxAdvisorId = c.Int(),
                        ParentInvestorId = c.Int(),
                        Steuernummer = c.String(maxLength: 15),
                        SteuerIdentifikation = c.String(maxLength: 15),
                        Finanzamt = c.String(maxLength: 100),
                        TaxAdvisorCompany = c.String(maxLength: 100),
                        TaxAdvisorContactName = c.String(maxLength: 100),
                        TaxAdvisorTelephone = c.String(maxLength: 100),
                        TaxAdvisorEmail = c.String(maxLength: 100),
                        InvestorName = c.String(maxLength: 100),
                        InvestorFirstName = c.String(maxLength: 25),
                        InvestorLastName = c.String(maxLength: 50),
                        InvestorTitle = c.String(maxLength: 15),
                        InvestorEmail = c.String(maxLength: 100),
                        InvestorTelephone = c.String(maxLength: 25),
                        InvestorGender = c.String(maxLength: 5),
                        InvestorSalutation = c.String(maxLength: 100),
                        FoundationBirthday = c.DateTime(),
                        DirectorCompanyName = c.String(maxLength: 100),
                        DirectorName = c.String(maxLength: 100),
                        DirectorFirstName = c.String(maxLength: 25),
                        DirectorLastName = c.String(maxLength: 50),
                        DirectorTitle = c.String(maxLength: 15),
                        DirectorEmail = c.String(maxLength: 100),
                        DirectorTelephone = c.String(maxLength: 25),
                        DirectorGender = c.String(maxLength: 5),
                        DirectorSalutation = c.String(maxLength: 100),
                        SitzDerGesellschaft = c.String(maxLength: 50),
                        RegisterGericht = c.String(maxLength: 50),
                        HandelsregisterNummer = c.String(maxLength: 50),
                        ConfidentialLetter = c.Boolean(nullable: false),
                        SendEmail = c.Boolean(nullable: false),
                        SendForm = c.Boolean(nullable: false),
                        IsHqtClient = c.Boolean(nullable: false),
                        IsLiqidClient = c.Boolean(nullable: false),
                        ClientIsOwnAdvisor = c.Boolean(nullable: false),
                        AdvisorId = c.Int(),
                        CompanyName = c.String(maxLength: 100),
                        PrivateAddress_Country = c.String(maxLength: 100),
                        PrivateAddress_City = c.String(maxLength: 100),
                        PrivateAddress_ZipCode = c.String(maxLength: 100),
                        PrivateAddress_Street = c.String(maxLength: 100),
                        PrivateAddress_Street2 = c.String(),
                        PostalAddress_Country = c.String(maxLength: 100),
                        PostalAddress_City = c.String(maxLength: 100),
                        PostalAddress_ZipCode = c.String(maxLength: 100),
                        PostalAddress_Street = c.String(maxLength: 100),
                        PostalAddress_Street2 = c.String(),
                        StatusMifid = c.Int(nullable: false),
                        StatusKagb = c.Int(nullable: false),
                        FatcaDateAsOff = c.DateTime(),
                        WealthAmount = c.Double(nullable: false),
                        PassportAvailable = c.Boolean(nullable: false),
                        HouseOfCompanyAvailable = c.Boolean(nullable: false),
                        ContractAvailable = c.Boolean(nullable: false),
                        SignatureListAvailable = c.Boolean(nullable: false),
                        W8BenAvailable = c.Boolean(nullable: false),
                        PowerOfAttorneyAvailable = c.Boolean(nullable: false),
                        InvestorIsFeederStructur = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Advisors", t => t.AdvisorId)
                .ForeignKey("public.ClientAdvisors", t => t.ClientAdvisorId)
                .ForeignKey("public.Currencies", t => t.CurrencyId)
                .ForeignKey("public.Investors", t => t.ParentInvestorId)
                .ForeignKey("public.ClientAdvisors", t => t.TaxAdvisorId)
                .Index(t => t.CurrencyId)
                .Index(t => t.ClientAdvisorId)
                .Index(t => t.TaxAdvisorId)
                .Index(t => t.ParentInvestorId)
                .Index(t => t.AdvisorId);
            
            CreateTable(
                "public.TaxInformations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvestorId = c.Int(),
                        CountryId = c.Int(),
                        TaxStatus = c.String(maxLength: 50),
                        TaxIdentificationNumber = c.String(maxLength: 50),
                        Remarks = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Countries", t => t.CountryId)
                .ForeignKey("public.Investors", t => t.InvestorId)
                .Index(t => t.InvestorId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "public.InvestorCommitmentDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SecondaryCutOffDate = c.DateTime(),
                        SecondaryCallsAfterCutOff = c.Double(nullable: false),
                        SecondaryDistributionsAfterCutOff = c.Double(nullable: false),
                        SecondaryPurchaseAmount = c.Double(nullable: false),
                        SecondaryOpenCommitment = c.Double(nullable: false),
                        InvestorCommitmentId = c.Int(nullable: false),
                        CommitmentDate = c.DateTime(nullable: false),
                        CommitmentAmount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.InvestorCommitments", t => t.InvestorCommitmentId, cascadeDelete: true)
                .Index(t => t.InvestorCommitmentId);
            
            CreateTable(
                "public.InvestorPcaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FinalPcapAmountInInvestorCurrency = c.Double(nullable: false),
                        InvestorCommitmentId = c.Int(nullable: false),
                        AsOfDate = c.DateTime(nullable: false),
                        EstimatedPcapAmount = c.Double(nullable: false),
                        DateOfFinalPcap = c.DateTime(nullable: false),
                        FinalPcapAmount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.InvestorCommitments", t => t.InvestorCommitmentId, cascadeDelete: true)
                .Index(t => t.InvestorCommitmentId);
            
            CreateTable(
                "public.PeFunds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InitiatorId = c.Int(),
                        SubjectForLetter = c.String(maxLength: 300),
                        DirectoryPath = c.String(maxLength: 50),
                        FundHqTrustNumber = c.String(maxLength: 15),
                        VintageYear = c.Int(nullable: false),
                        IsHqTrustFund = c.Boolean(nullable: false),
                        IsLiqidFund = c.Boolean(nullable: false),
                        IsExternalAdministrated = c.Boolean(nullable: false),
                        FundTypeId = c.Int(),
                        FundGeographyId = c.Int(),
                        FundCompanySizeId = c.Int(),
                        FundName = c.String(maxLength: 100),
                        FundShortName = c.String(maxLength: 100),
                        FundLegalName = c.String(maxLength: 200),
                        FundManagerId = c.Int(),
                        CurrencyId = c.Int(),
                        FundVolume = c.Double(nullable: false),
                        IsFeederFund = c.Boolean(nullable: false),
                        PeFundId = c.Int(),
                        PositiveIrr = c.Double(nullable: false),
                        PositiveMM = c.Double(nullable: false),
                        NegativeIrr = c.Double(nullable: false),
                        NegativeMM = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Currencies", t => t.CurrencyId)
                .ForeignKey("public.PeFunds", t => t.PeFundId)
                .ForeignKey("public.FundCompanySizes", t => t.FundCompanySizeId)
                .ForeignKey("public.FundGeographies", t => t.FundGeographyId)
                .ForeignKey("public.FundManagers", t => t.FundManagerId)
                .ForeignKey("public.FundTypes", t => t.FundTypeId)
                .ForeignKey("public.Initiators", t => t.InitiatorId)
                .Index(t => t.InitiatorId)
                .Index(t => t.FundTypeId)
                .Index(t => t.FundGeographyId)
                .Index(t => t.FundCompanySizeId)
                .Index(t => t.FundManagerId)
                .Index(t => t.CurrencyId)
                .Index(t => t.PeFundId);
            
            CreateTable(
                "public.PeFundCashFlows",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EffectiveDate = c.DateTime(nullable: false),
                        PeFundId = c.Int(nullable: false),
                        CashFlowType = c.String(),
                        CashFlowNumber = c.Int(nullable: false),
                        CashFlowDescription = c.String(),
                        CommitmentAmount = c.Double(nullable: false),
                        CashFlowAmount = c.Double(nullable: false),
                        ReturnOfCapital = c.Double(nullable: false),
                        CapitalGain = c.Double(nullable: false),
                        Interests = c.Double(nullable: false),
                        Dividends = c.Double(nullable: false),
                        OtherIncome = c.Double(nullable: false),
                        RecallableAmount = c.Double(nullable: false),
                        PartnershipExpenses = c.Double(nullable: false),
                        WithholdingTax = c.Double(nullable: false),
                        InvestorExpenses = c.Double(nullable: false),
                        LookbackInterests = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.PeFunds", t => t.PeFundId, cascadeDelete: true)
                .Index(t => t.PeFundId);
            
            CreateTable(
                "public.PeFundPcaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PeFundId = c.Int(nullable: false),
                        AsOfDate = c.DateTime(nullable: false),
                        EstimatedPcapAmount = c.Double(nullable: false),
                        DateOfFinalPcap = c.DateTime(nullable: false),
                        FinalPcapAmount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.PeFunds", t => t.PeFundId, cascadeDelete: true)
                .Index(t => t.PeFundId);
            
            CreateTable(
                "public.InvestorToDoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvestorId = c.Int(nullable: false),
                        TodoText = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessageText = c.String(),
                        CreationDateTime = c.DateTime(nullable: false),
                        CreationType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.PrintPcaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PeFundName = c.String(),
                        PeFundId = c.Int(nullable: false),
                        FundCommitment = c.Double(nullable: false),
                        YearEnd = c.DateTime(nullable: false),
                        LastQuarterEnd = c.DateTime(nullable: false),
                        CurrentQuarterEnd = c.DateTime(nullable: false),
                        FundPcapYearEnd = c.Double(nullable: false),
                        FundPcapLastQuarter = c.Double(nullable: false),
                        FundPcapCurrentQuarter = c.Double(nullable: false),
                        InvestorName = c.String(),
                        InvestorId = c.Int(nullable: false),
                        InvestorCommitment = c.Double(nullable: false),
                        InvestorPcapYearEnd = c.Double(nullable: false),
                        InvestorPcapLastQuarter = c.Double(nullable: false),
                        InvestorPcapCurrentQuarter = c.Double(nullable: false),
                        FundContributionsInception = c.Double(nullable: false),
                        FundContributionsYearEnd = c.Double(nullable: false),
                        FundContributionsCurrentQuarter = c.Double(nullable: false),
                        FundResultYearEnd = c.Double(nullable: false),
                        FundResultQuarter = c.Double(nullable: false),
                        InvestorContributionsInception = c.Double(nullable: false),
                        InvestorContributionsYearEnd = c.Double(nullable: false),
                        InvestorContributionsCurrentQuarter = c.Double(nullable: false),
                        InvestorResultYearEnd = c.Double(nullable: false),
                        InvestorResultQuarter = c.Double(nullable: false),
                        FundDistributionsYearEnd_Id = c.Int(),
                        InvestorDistributionsYearEnd_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.PeFundCashFlows", t => t.FundDistributionsYearEnd_Id)
                .ForeignKey("public.InvestorCashFlows", t => t.InvestorDistributionsYearEnd_Id)
                .Index(t => t.FundDistributionsYearEnd_Id)
                .Index(t => t.InvestorDistributionsYearEnd_Id);
            
            CreateTable(
                "public.ReportInformationCapitalCalls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReportId = c.Int(nullable: false),
                        PeFundId = c.Int(nullable: false),
                        InvestorId = c.Int(nullable: false),
                        InvestorCommitmentId = c.Int(nullable: false),
                        AdvisorId = c.Int(nullable: false),
                        BankAccountId = c.Int(nullable: false),
                        ReportDate = c.DateTime(nullable: false),
                        CashFlowDueDate = c.DateTime(nullable: false),
                        SecondSignature = c.String(),
                        FundReference = c.String(),
                        CashFlowSubject = c.String(),
                        CashFlowNumberText = c.String(),
                        CashFlowAmount = c.Double(nullable: false),
                        CashFlowAmountInPercent = c.Double(nullable: false),
                        CommitmentAmount = c.Double(nullable: false),
                        OpenCommitment = c.Double(nullable: false),
                        CurrencyShortName = c.String(),
                        IndividualHtmlText = c.String(),
                        IndividualHtmlText2 = c.String(),
                        CashFlowAmountCall = c.Double(nullable: false),
                        CashFlowAmountDistribution = c.Double(nullable: false),
                        ReturnOfCapital = c.Double(nullable: false),
                        CapitalGain = c.Double(nullable: false),
                        Interests = c.Double(nullable: false),
                        Dividends = c.Double(nullable: false),
                        OtherIncome = c.Double(nullable: false),
                        RecallableAmount = c.Double(nullable: false),
                        PartnershipExpenses = c.Double(nullable: false),
                        WithholdingTax = c.Double(nullable: false),
                        InvestorExpenses = c.Double(nullable: false),
                        LookBackInterests = c.Double(nullable: false),
                        CapitalCallNumber = c.Int(nullable: false),
                        DistributionNumber = c.Int(nullable: false),
                        TotalDistributionPaidIn = c.Double(nullable: false),
                        TotalDistributionCommitment = c.Double(nullable: false),
                        TotalContributionCommitment = c.Double(nullable: false),
                        TotalDistribution = c.Double(nullable: false),
                        TotalContribution = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.ReportNumbers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateReportGenerated = c.DateTime(nullable: false),
                        ReportName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.TextComponents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 100),
                        TextContent = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "public.UniqueCashFlowNumbers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CashFlowDate = c.DateTime(nullable: false),
                        PeFundId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("public.PrintPcaps", "InvestorDistributionsYearEnd_Id", "public.InvestorCashFlows");
            DropForeignKey("public.PrintPcaps", "FundDistributionsYearEnd_Id", "public.PeFundCashFlows");
            DropForeignKey("public.InvestorCommitments", "PeFundId", "public.PeFunds");
            DropForeignKey("public.PeFundPcaps", "PeFundId", "public.PeFunds");
            DropForeignKey("public.PeFunds", "InitiatorId", "public.Initiators");
            DropForeignKey("public.PeFunds", "FundTypeId", "public.FundTypes");
            DropForeignKey("public.PeFunds", "FundManagerId", "public.FundManagers");
            DropForeignKey("public.PeFunds", "FundGeographyId", "public.FundGeographies");
            DropForeignKey("public.PeFunds", "FundCompanySizeId", "public.FundCompanySizes");
            DropForeignKey("public.PeFunds", "PeFundId", "public.PeFunds");
            DropForeignKey("public.DocumentAndLetters", "PeFundId", "public.PeFunds");
            DropForeignKey("public.PeFunds", "CurrencyId", "public.Currencies");
            DropForeignKey("public.PeFundCashFlows", "PeFundId", "public.PeFunds");
            DropForeignKey("public.BankAccounts", "PefundId", "public.PeFunds");
            DropForeignKey("public.InvestorPcaps", "InvestorCommitmentId", "public.InvestorCommitments");
            DropForeignKey("public.InvestorCommitmentDetails", "InvestorCommitmentId", "public.InvestorCommitments");
            DropForeignKey("public.InvestorCashFlows", "InvestorCommitmentId", "public.InvestorCommitments");
            DropForeignKey("public.TaxInformations", "InvestorId", "public.Investors");
            DropForeignKey("public.TaxInformations", "CountryId", "public.Countries");
            DropForeignKey("public.Investors", "TaxAdvisorId", "public.ClientAdvisors");
            DropForeignKey("public.Investors", "ParentInvestorId", "public.Investors");
            DropForeignKey("public.EMailAccounts", "InvestorId", "public.Investors");
            DropForeignKey("public.DocumentAndLetters", "InvestorId", "public.Investors");
            DropForeignKey("public.Investors", "CurrencyId", "public.Currencies");
            DropForeignKey("public.InvestorCommitments", "InvestorId", "public.Investors");
            DropForeignKey("public.Investors", "ClientAdvisorId", "public.ClientAdvisors");
            DropForeignKey("public.BankAccounts", "InvestorId", "public.Investors");
            DropForeignKey("public.Investors", "AdvisorId", "public.Advisors");
            DropForeignKey("public.InvestorCommitments", "BankAccountId", "public.BankAccounts");
            DropForeignKey("public.Initiators", "AdvisorId", "public.Advisors");
            DropForeignKey("public.CurrencyRates", "PeFundCurrencyId", "public.Currencies");
            DropForeignKey("public.CurrencyRates", "InvestorCurrencyId", "public.Currencies");
            DropForeignKey("public.BankAccounts", "CurrencyId", "public.Currencies");
            DropForeignKey("public.BankAccounts", "BankContactId", "public.ClientAdvisors");
            DropForeignKey("public.Advisors", "ReportsToId", "public.Advisors");
            DropIndex("public.PrintPcaps", new[] { "InvestorDistributionsYearEnd_Id" });
            DropIndex("public.PrintPcaps", new[] { "FundDistributionsYearEnd_Id" });
            DropIndex("public.PeFundPcaps", new[] { "PeFundId" });
            DropIndex("public.PeFundCashFlows", new[] { "PeFundId" });
            DropIndex("public.PeFunds", new[] { "PeFundId" });
            DropIndex("public.PeFunds", new[] { "CurrencyId" });
            DropIndex("public.PeFunds", new[] { "FundManagerId" });
            DropIndex("public.PeFunds", new[] { "FundCompanySizeId" });
            DropIndex("public.PeFunds", new[] { "FundGeographyId" });
            DropIndex("public.PeFunds", new[] { "FundTypeId" });
            DropIndex("public.PeFunds", new[] { "InitiatorId" });
            DropIndex("public.InvestorPcaps", new[] { "InvestorCommitmentId" });
            DropIndex("public.InvestorCommitmentDetails", new[] { "InvestorCommitmentId" });
            DropIndex("public.TaxInformations", new[] { "CountryId" });
            DropIndex("public.TaxInformations", new[] { "InvestorId" });
            DropIndex("public.Investors", new[] { "AdvisorId" });
            DropIndex("public.Investors", new[] { "ParentInvestorId" });
            DropIndex("public.Investors", new[] { "TaxAdvisorId" });
            DropIndex("public.Investors", new[] { "ClientAdvisorId" });
            DropIndex("public.Investors", new[] { "CurrencyId" });
            DropIndex("public.InvestorCommitments", new[] { "PeFundId" });
            DropIndex("public.InvestorCommitments", new[] { "BankAccountId" });
            DropIndex("public.InvestorCommitments", new[] { "InvestorId" });
            DropIndex("public.InvestorCashFlows", new[] { "InvestorCommitmentId" });
            DropIndex("public.Initiators", new[] { "AdvisorId" });
            DropIndex("public.EMailAccounts", new[] { "InvestorId" });
            DropIndex("public.DocumentAndLetters", new[] { "PeFundId" });
            DropIndex("public.DocumentAndLetters", new[] { "InvestorId" });
            DropIndex("public.CurrencyRates", new[] { "InvestorCurrencyId" });
            DropIndex("public.CurrencyRates", new[] { "PeFundCurrencyId" });
            DropIndex("public.BankAccounts", new[] { "BankContactId" });
            DropIndex("public.BankAccounts", new[] { "PefundId" });
            DropIndex("public.BankAccounts", new[] { "InvestorId" });
            DropIndex("public.BankAccounts", new[] { "CurrencyId" });
            DropIndex("public.Advisors", new[] { "ReportsToId" });
            DropTable("public.UniqueCashFlowNumbers");
            DropTable("public.TextComponents");
            DropTable("public.ReportNumbers");
            DropTable("public.ReportInformationCapitalCalls");
            DropTable("public.PrintPcaps");
            DropTable("public.Messages");
            DropTable("public.InvestorToDoes");
            DropTable("public.PeFundPcaps");
            DropTable("public.PeFundCashFlows");
            DropTable("public.PeFunds");
            DropTable("public.InvestorPcaps");
            DropTable("public.InvestorCommitmentDetails");
            DropTable("public.TaxInformations");
            DropTable("public.Investors");
            DropTable("public.InvestorCommitments");
            DropTable("public.InvestorCashFlows");
            DropTable("public.Initiators");
            DropTable("public.ImportCommitments");
            DropTable("public.FundTypes");
            DropTable("public.FundManagers");
            DropTable("public.FundGeographies");
            DropTable("public.FundCompanySizes");
            DropTable("public.EMailAccounts");
            DropTable("public.DocumentAndLetters");
            DropTable("public.CurrencyRates");
            DropTable("public.Countries");
            DropTable("public.CashFlowBarCharts");
            DropTable("public.Currencies");
            DropTable("public.ClientAdvisors");
            DropTable("public.BankAccounts");
            DropTable("public.AnwendungsInformationens");
            DropTable("public.Advisors");
        }
    }
}
