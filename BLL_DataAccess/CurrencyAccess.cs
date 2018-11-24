using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL_DataModels;

namespace BLL_DataAccess
{
    public class CurrencyAccess
    {
        public Currency GetCurrencyById(int currencyId)
        {
            using (HqTrustData dbContext = new HqTrustData())
            {
                return dbContext.Currencies.FirstOrDefault(c => c.Id == currencyId);
            }
        }
        
    }
}
