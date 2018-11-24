using BLL_DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Events
{
    public class EditInvestorCommitmentResponse
    {
        public string ActionType = string.Empty;
        public InvestorCommitment editedCommitment = new InvestorCommitment();
    }
}
