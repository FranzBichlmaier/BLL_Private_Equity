using BLL_DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Events
{
    public class InvokeEditInvestorCommitment
    {
        public InvestorCommitment investorCommitment = new InvestorCommitment();
        public ObservableCollection<PeFund> availableFunds = new ObservableCollection<PeFund>();
        public ObservableCollection<BankAccount> bankAccounts = new ObservableCollection<BankAccount>();
    }
}
