using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Berechnungen
{
    public enum ImportColumns
    {
        pfnr =0,
        wkn,
        wpName,
        effectiveDate,
        trxId,
        trxInputDate,
        trxStorno,
        flowInvestor,
        flowFund,
        emptyDetail,
        payoutInvestor,
        payoutFund,
        distributionInvestor,
        distributionFund,
        emptyTrades,
        trx50000CallInvestor,
        trx50032RecallableInvestor,
        trx50000CallFund, 
        trx50032RecallableFund,
        trx50021DistributionInvestor,
        trx50021ReturnOfCapitalInvestor,
        trx50021CapitalGainInvestor,
        trx50020IncomeInvestor,
        trx50020TaxesInvestor,
        trx50022ReturnOfCapitalNRInvestor, 
        trx50041AcquistionCostInvestor, 
        trx50040ExpensesCorrectionInvestor,
        trx50023CancellRecallableInvestor,
        trx50021DistributionFund,
        trx50021ReturnOfCapitalFund,
        trx50021CapitalGainFund,
        trx50020IncomeFund,
        trx50020TaxesFund,
        trx50022ReturnOfCapitalNRFund,
        trx50041AcquistionCostFund,
        trx50040ExpensesCorrectionFund,
        trx50023CancellRecallableFund
    }
}
