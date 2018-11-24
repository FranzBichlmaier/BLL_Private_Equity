using BLL_Prism;
using Prism.Mvvm;
using BLL_DataModels;
using System;
using System.Collections.Generic;

namespace BLL_Private_Equity.Berechnungen
{
    public class CashFlowInformation: BindableBase
    {
        private int id;        
        private int reportId;
        private PeFund fund;
        private string componentText=string.Empty;
        private string componentText2 = string.Empty;
        private DateTime lastUpdated;
        private DateTime letterDate = DateTime.Now.Date;
        private string cashFlowType = string.Empty;
        private string cashFlowNumber = string.Empty;
        private DateTime effectiveDate;
        private string cashFlowDescription=string.Empty;
        private CashFlowDetail detailSummary = new CashFlowDetail();
        private List<CashFlowDetail> investorDetails = new List<CashFlowDetail>();
        private string wordDocument = string.Empty;
        private bool investorsChecked;
        private bool cashFlowDataEntered;
        private bool investorDetailsEntered;
        private bool lettersPrinted;
        private bool otherWorkDone;
        private string fileName = string.Empty;
        
        public int Id { get => id; set =>  SetProperty(ref id, value); }        
        public int ReportId { get => reportId; set => SetProperty(ref reportId,value); }   
        public PeFund Fund { get => fund; set => SetProperty(ref fund, value); }
        public string ComponentText { get => componentText; set => SetProperty(ref componentText,value) ; }
        public string ComponentText2 { get => componentText2; set => SetProperty(ref componentText2, value); }
        public DateTime LastUpdated { get => lastUpdated; set => SetProperty(ref lastUpdated, value); }
        public DateTime LetterDate { get => letterDate; set => SetProperty(ref letterDate, value); }
        public string CashFlowType { get => cashFlowType; set => SetProperty(ref cashFlowType, value); }
        public string CashFlowDescription { get => cashFlowDescription; set => SetProperty(ref cashFlowDescription, value); }          
        public string CashFlowNumber { get => cashFlowNumber; set => SetProperty(ref cashFlowNumber, value); }
        public DateTime EffectiveDate { get => effectiveDate; set => SetProperty(ref effectiveDate, value); }
        public CashFlowDetail DetailSummary { get => detailSummary; set => SetProperty(ref detailSummary, value); }
        public List<CashFlowDetail> InvestorDetails { get => investorDetails; set => SetProperty(ref investorDetails, value); }
        public string WordDocument { get => wordDocument; set => SetProperty(ref wordDocument, value); }
        public bool InvestorsChecked { get => investorsChecked; set => SetProperty(ref investorsChecked, value); }
        public bool CashFlowDataEntered { get => cashFlowDataEntered; set => SetProperty(ref cashFlowDataEntered, value); }
        public bool InvestorDetailsEntered { get => investorDetailsEntered; set => SetProperty(ref investorDetailsEntered, value); }
        public bool LettersPrinted { get => lettersPrinted; set => SetProperty(ref lettersPrinted, value); }
        public bool OtherWorkDone { get => otherWorkDone; set => SetProperty(ref otherWorkDone, value); }
        public string FileName { get => fileName; set => SetProperty(ref fileName, value); }
      

    }
}
