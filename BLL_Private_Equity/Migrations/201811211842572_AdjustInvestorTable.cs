namespace BLL_Private_Equity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustInvestorTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("public.Investors", "IsFatca", c => c.Boolean(nullable: false));
            AddColumn("public.Investors", "IsCrs", c => c.Boolean(nullable: false));
            AddColumn("public.Investors", "RechtsNachfolgerVonId", c => c.Int());
            DropColumn("public.Investors", "PsPlusLevel");
            DropColumn("public.Investors", "TaxAdvisorCompany");
            DropColumn("public.Investors", "TaxAdvisorContactName");
            DropColumn("public.Investors", "TaxAdvisorTelephone");
            DropColumn("public.Investors", "TaxAdvisorEmail");
            DropColumn("public.Investors", "InvestorName");
            DropColumn("public.Investors", "InvestorFirstName");
            DropColumn("public.Investors", "InvestorLastName");
            DropColumn("public.Investors", "InvestorTitle");
            DropColumn("public.Investors", "InvestorEmail");
            DropColumn("public.Investors", "InvestorTelephone");
            DropColumn("public.Investors", "InvestorGender");
            DropColumn("public.Investors", "InvestorSalutation");
            DropColumn("public.Investors", "DirectorCompanyName");
            DropColumn("public.Investors", "DirectorName");
            DropColumn("public.Investors", "DirectorFirstName");
            DropColumn("public.Investors", "DirectorLastName");
            DropColumn("public.Investors", "DirectorTitle");
            DropColumn("public.Investors", "DirectorEmail");
            DropColumn("public.Investors", "DirectorTelephone");
            DropColumn("public.Investors", "DirectorGender");
            DropColumn("public.Investors", "DirectorSalutation");
        }
        
        public override void Down()
        {
            AddColumn("public.Investors", "DirectorSalutation", c => c.String(maxLength: 100));
            AddColumn("public.Investors", "DirectorGender", c => c.String(maxLength: 5));
            AddColumn("public.Investors", "DirectorTelephone", c => c.String(maxLength: 25));
            AddColumn("public.Investors", "DirectorEmail", c => c.String(maxLength: 100));
            AddColumn("public.Investors", "DirectorTitle", c => c.String(maxLength: 15));
            AddColumn("public.Investors", "DirectorLastName", c => c.String(maxLength: 50));
            AddColumn("public.Investors", "DirectorFirstName", c => c.String(maxLength: 25));
            AddColumn("public.Investors", "DirectorName", c => c.String(maxLength: 100));
            AddColumn("public.Investors", "DirectorCompanyName", c => c.String(maxLength: 100));
            AddColumn("public.Investors", "InvestorSalutation", c => c.String(maxLength: 100));
            AddColumn("public.Investors", "InvestorGender", c => c.String(maxLength: 5));
            AddColumn("public.Investors", "InvestorTelephone", c => c.String(maxLength: 25));
            AddColumn("public.Investors", "InvestorEmail", c => c.String(maxLength: 100));
            AddColumn("public.Investors", "InvestorTitle", c => c.String(maxLength: 15));
            AddColumn("public.Investors", "InvestorLastName", c => c.String(maxLength: 50));
            AddColumn("public.Investors", "InvestorFirstName", c => c.String(maxLength: 25));
            AddColumn("public.Investors", "InvestorName", c => c.String(maxLength: 100));
            AddColumn("public.Investors", "TaxAdvisorEmail", c => c.String(maxLength: 100));
            AddColumn("public.Investors", "TaxAdvisorTelephone", c => c.String(maxLength: 100));
            AddColumn("public.Investors", "TaxAdvisorContactName", c => c.String(maxLength: 100));
            AddColumn("public.Investors", "TaxAdvisorCompany", c => c.String(maxLength: 100));
            AddColumn("public.Investors", "PsPlusLevel", c => c.String(maxLength: 12));
            DropColumn("public.Investors", "RechtsNachfolgerVonId");
            DropColumn("public.Investors", "IsCrs");
            DropColumn("public.Investors", "IsFatca");
        }
    }
}
