using BLL_DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Events
{
    public class AccountCollection
    {
        public string Action = string.Empty;
        public int InvestorId = 0;
        public int PeFundId = 0;
        public int CurrencyId = 0;
        public List<BankAccount> AccountList = new List<BankAccount>();
    }
}
