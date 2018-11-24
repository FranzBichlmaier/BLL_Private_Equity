using BLL_Infrastructure;
using BLL_Private_Equity.Events;
using Prism.Events;
using System;
using System.IO;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Flow.FormatProviders.Docx;
using Telerik.Windows.Documents.Flow.FormatProviders.Pdf;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Windows.Documents.Flow.Model.Editing;


namespace BLL_Private_Equity.Berechnungen
{
    class CreateClientLetterTelerik
    {
        private RadFlowDocument seriesDocument = new RadFlowDocument();
        private string fileNameSeriesDocument = string.Empty;
        private readonly string sourcePath;
        private int cashFlowNumber = 0;
        public string SaveAsFile { get; }
        public CashFlowInformation information { get; }
        public IEventAggregator eventAggregator { get; }
        DocxFormatProvider provider = new DocxFormatProvider();      

        public CashFlowDetail detail { get; }
        RadFlowDocumentEditor editor;
        string confidential = string.Empty;
        string anrede = string.Empty;
        string firma = string.Empty;
        string adressAnrede = string.Empty;
        string strasse = string.Empty;
        string plzOrt = string.Empty;
        string land = string.Empty;
        string cfAmount = string.Empty;
        string cfExpenses = string.Empty;
        string cfLookback = string.Empty;
        string cfAmountPercent = string.Empty;
        string cfExpensesPercent = string.Empty;
        string cfLookbackPercent = string.Empty;
        string cfReturnOfCapital = string.Empty;
        string cfCapitalGain = string.Empty;
        string cfDividends = string.Empty;
        string cfWithholdingTax = string.Empty;
        string cfRecallable = string.Empty;
        string cashflowBetrag = string.Empty;
        string cashFlowPercent = string.Empty;
        string openCommitment = string.Empty;
        string totalCalledPercent = string.Empty;
        string totalCalled = string.Empty;
        string totalDistributed = string.Empty;
        string totalDistributedPercent = string.Empty;
        string fundCalled = string.Empty;
        string fundDistributed = string.Empty;
        string bankName = string.Empty;
        string swift = string.Empty;
        string bic = string.Empty;
        string empfaenger = string.Empty;
        string iban = string.Empty;
        string valuta = string.Empty;
        string accountHolder = string.Empty;
        string initiatorStrasse1 = string.Empty;
        string initiatorStrasse2 = string.Empty;
        string initiatorPlzOrt = string.Empty;
        string sitzStrasse1 = string.Empty;
        string sitzStrasse2 = string.Empty;
        string sitzPlzOrt = string.Empty;
        string bankAddress = string.Empty;
        string bankNumber = string.Empty;
        string additionalInformation = string.Empty;
        string ffcHolder = string.Empty;
        string ffcAccountNumber = string.Empty;
        string openPosition = string.Empty;

        public CreateClientLetterTelerik(string sourcePath, string saveAsFile, CashFlowInformation information, CashFlowDetail detail)
        {
            this.sourcePath = sourcePath;
            SaveAsFile = saveAsFile;
            this.information = information;
            this.detail = detail;

            FillStrings(detail);
            RadFlowDocument wordDocument = new RadFlowDocument();
            provider = new DocxFormatProvider();
           
            try
            {
                using (Stream input = File.OpenRead(sourcePath))
                {
                    wordDocument = provider.Import(input);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Das Word-Template konnte nicht geöffnet werden. {ex.Message}");
            }
          
            editor = new RadFlowDocumentEditor(wordDocument);
            ReplacePlaceholder(detail);

            int pos = saveAsFile.LastIndexOf(".");
            string fileName = saveAsFile.Substring(0, pos)+".docx";

            using (Stream output = File.OpenWrite(fileName))
            {
                provider.Export(editor.Document, output);
            }

           

            string pdfFile = fileName.Replace(".docx", ".pdf");
           
            // cpmversion of a flowdocument to a fixeddocument

            PdfFormatProvider pdfProvider = new PdfFormatProvider();
            RadFixedDocument fixedDocument = pdfProvider.ExportToFixedDocument(editor.Document);

            // write the fixeddocuement to file
            Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.PdfFormatProvider fixedProvider = new Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.PdfFormatProvider();


            using (Stream output = File.OpenWrite(pdfFile))
            {
                fixedProvider.Export(fixedDocument, output);
            }
        }

        public string GetFileNameOfSeriesDocument()
        {
            return fileNameSeriesDocument;
        }

        public CreateClientLetterTelerik(CashFlowInformation information, int cashFlowNumber, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;            
          

            this.information = information;
            this.cashFlowNumber = cashFlowNumber;

            provider = new DocxFormatProvider();

            ReplaceWordContent(information);
        }

        private void ReplaceWordContent(CashFlowInformation information)
        {
            DirectoryInfo directoryInfo = DirectoryHelper.GetWordTemplateDirectory();

            string source = Path.Combine(directoryInfo.FullName, information.WordDocument);
            foreach (CashFlowDetail detail in information.InvestorDetails)
            {
                FillStrings(detail);
                RadFlowDocument wordDocument = new RadFlowDocument();
                try
                {
                    using (Stream input = File.OpenRead(source))
                    {
                        wordDocument = provider.Import(input);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Das Word-Template konnte nicht geöffnet werden. {ex.Message}");
                      
                }

                editor = new RadFlowDocumentEditor(wordDocument);
                ReplacePlaceholder(detail);

                //add docuemnt to seriesDocument
                seriesDocument.Merge(editor.Document);

                // save single file
                string fileName = DirectoryHelper.FindInvestorCashFlow(detail.Investor.Id, cashFlowNumber);

                using (Stream output = File.OpenWrite(fileName))
                {
                    provider.Export(editor.Document, output);
                }
                eventAggregator.GetEvent<StatusBarEvent>().Publish($"Das Ausschüttungsschreiben für {detail.Reference} wurde erstellt.");
                detail.FileName = fileName;
            }

            string seriesFileName = DirectoryHelper.FindFundCashFlow(information.Fund.Id, cashFlowNumber);

            using (Stream output = File.OpenWrite(seriesFileName))
            {
                provider.Export(seriesDocument.Document, output);
            }

            CommonProcesses.StartWord(seriesFileName);
        }

 

        private void FillStrings(CashFlowDetail detail)
        {
            if (detail.Investor.ConfidentialLetter == true) confidential = "Persönlich / Vertraulich";
            anrede = detail.Investor.ClientAdvisor.AdvisorName.Salutation;
            firma = detail.Investor.ClientAdvisor.CompanyName;
            adressAnrede = detail.Investor.ClientAdvisor.AdvisorName.AddressName;
            strasse = detail.Investor.ClientAdvisor.Address.Street;
            plzOrt = detail.Investor.ClientAdvisor.Address.ZipCode + " " + detail.Investor.ClientAdvisor.Address.City;
            land = detail.Investor.ClientAdvisor.Address.Country;
            if (detail.CashFlowAmountCall!= 0)
            cfAmount = $"{detail.CashFlowAmountCall:n2}";
            else cfAmount = $"{detail.CashFlowAmountDistribution:n2}";
            cfExpenses = $"{detail.PartnershipExpenses:n2}";
            cfLookback = $"{detail.LookbackInterests:n2}";
            cfAmountPercent = $"{Math.Round(detail.CashFlowAmountCall / detail.CommitmentAmount * 100,1):n1} %";
            cfExpensesPercent = $"{Math.Round(detail.PartnershipExpenses / detail.CommitmentAmount * 100, 1):n1} %";
            cfLookbackPercent = $"{Math.Round(detail.LookbackInterests / detail.CommitmentAmount * 100, 1):n1} %";
            cashflowBetrag = $"{detail.CashFlowAmountCall + detail.PartnershipExpenses + detail.LookbackInterests+ detail.OpenPosition:n2}";
            cashFlowPercent = $"{Math.Round((detail.CashFlowAmountCall + detail.PartnershipExpenses + detail.LookbackInterests+ detail.OpenPosition) / detail.CommitmentAmount * 100, 1):n1} %";
            openCommitment = $"{detail.CommitmentAmount - Math.Abs(detail.TotalCalls + detail.CashFlowAmountCall - detail.RecallableAmount):n2}";
            totalCalledPercent = $"{Math.Round((Math.Abs(detail.TotalCalls+detail.CashFlowAmountCall-detail.RecallableAmount)/detail.CommitmentAmount*100),1):n1} %";
            totalCalled = $"{Math.Abs(detail.TotalCalls + detail.CashFlowAmountCall - detail.RecallableAmount):n2}";
            totalDistributed = $"{detail.TotalDistributions + detail.CashFlowAmountDistribution-detail.RecallableAmount:n2}";
            totalDistributedPercent = $"{Math.Round((detail.TotalDistributions + detail.CashFlowAmountDistribution - detail.RecallableAmount) / detail.CommitmentAmount * 100, 1):n1} %";
            fundCalled = $"{Math.Abs(information.DetailSummary.CashFlowAmountCall):n2}";
            fundDistributed = $"{information.DetailSummary.CashFlowAmountDistribution:n2}";
            valuta = $"{information.EffectiveDate:dd. MMMM yyyy}";
            bankName = information.Fund.BankAccounts[0].BankName;
            swift = information.Fund.BankAccounts[0].SwiftAddress;
            empfaenger = information.Fund.BankAccounts[0].AccountHolder;
            if (string.IsNullOrEmpty(empfaenger)) empfaenger = information.Fund.FundLegalName;
            iban = information.Fund.BankAccounts[0].Iban;
            bic = information.Fund.BankAccounts[0].Bic;
            cfReturnOfCapital = $"{detail.ReturnOfCapital:n2}";
            cfCapitalGain = $"{detail.CapitalGain:n2}";
            cfDividends = $"{detail.Dividends:n2}";
            cfRecallable = $"{detail.RecallableAmount:n2}";
            cfWithholdingTax = $"{detail.WithholdingTax:n2}";
            openPosition = $"{detail.OpenPosition:n2}";
            if (information.Fund.Initiator!= null)
            {
                sitzStrasse1 = information.Fund.Initiator.HeadQuarterAddress.Street;
                sitzStrasse2 = information.Fund.Initiator.HeadQuarterAddress.Street2;
                sitzPlzOrt = information.Fund.Initiator.HeadQuarterAddress.ZipCode + " " + information.Fund.Initiator.HeadQuarterAddress.City;
            }           
            bankAddress = information.Fund.BankAccounts[0].BankAddress;
            bankNumber = information.Fund.BankAccounts[0].BankNumberBlz;
            additionalInformation = information.Fund.BankAccounts[0].AdditionalInstructions;
            ffcHolder = information.Fund.BankAccounts[0].FfcAccountHolderName;
            ffcAccountNumber = information.Fund.BankAccounts[0].FfcAccountNumber;

        }

        private void ReplacePlaceholder(CashFlowDetail detail)
        {
            string currencyName = information.Fund.Currency.CurrencyShortName;
            editor.ReplaceText("[Beteiligungsgesellschaft]", information.Fund.FundLegalName);
            if (information.Fund.Initiator!= null)
            {
                editor.ReplaceText("[Absender Zeile 1]", information.Fund.Initiator.SenderInformation);
                editor.ReplaceText("[Absender Zeile 2]", information.Fund.Initiator.SenderInformationLine2);
                editor.ReplaceText("[Fusszeile 1]", information.Fund.Initiator.FooterLine1);
                editor.ReplaceText("[Fusszeile 2]", information.Fund.Initiator.FooterLine2);
                editor.ReplaceText("[Fusszeile 3]", information.Fund.Initiator.FooterLine3);
            }            
            editor.ReplaceText("[Vertraulich]", confidential);
            editor.ReplaceText("[Firma]", firma);
            editor.ReplaceText("[AdressAnrede]", adressAnrede);
            editor.ReplaceText("[Straße]", strasse);
            editor.ReplaceText("[PLZ Ort]", plzOrt);
            editor.ReplaceText("[Land]", land);
            editor.ReplaceText("[Text oben]", information.ComponentText);
            editor.ReplaceText("[Investor Referenz]", detail.Reference);
            editor.ReplaceText("[Betreff]", information.Fund.SubjectForLetter);
          
            editor.ReplaceText("[Anrede]", anrede);
            editor.ReplaceText("[Zeichnungsbetrag]", $"{detail.CommitmentAmount:n0}");
            editor.ReplaceText("[Whg]", currencyName);
            editor.ReplaceText("[Kapitalabruf]", information.CashFlowDescription);
            editor.ReplaceText("[Tagesdatum]", $"{information.LetterDate:dd. MMMM yyyy}");
            editor.ReplaceText("[CF Amount]", cfAmount);
            editor.ReplaceText("[CF Amount Percent]", cfAmountPercent);
            editor.ReplaceText("[CF Expenses Percent]", cfExpensesPercent);
            editor.ReplaceText("[CF Expenses]", cfExpenses);
            editor.ReplaceText("[CF Lookback]", cfLookback);
            editor.ReplaceText("[CF Lookback Percent]", cfLookbackPercent);
            editor.ReplaceText("[Cashflow Betrag]", cashflowBetrag);
            editor.ReplaceText("[Cashflow Prozent]", cashFlowPercent);
            editor.ReplaceText("[Valutatag]", valuta);
            editor.ReplaceText("[BankName]", bankName);
            editor.ReplaceText("[SwiftAdresse]", swift);
            editor.ReplaceText("[Kontoinhaber]", empfaenger);
            editor.ReplaceText("[IBAN]", iban);
            editor.ReplaceText("[BIC", bic);
            editor.ReplaceText("[BankAdresse]", bankAddress);
            editor.ReplaceText("[ZusatzInfo]", additionalInformation);
            editor.ReplaceText("[FFCKontoInhaber]", ffcHolder);
            editor.ReplaceText("[FFCKontoNummer]", ffcAccountNumber);
            editor.ReplaceText("[BankNummer]", bankNumber);
            editor.ReplaceText("[OpenCommitment]", openCommitment);
            editor.ReplaceText("[TotalCalledPercent]", totalCalledPercent);
            editor.ReplaceText("[TotalDistributed]", totalDistributed);
            editor.ReplaceText("[TotalDistributedPercent]", totalDistributedPercent);
            editor.ReplaceText("[CF ReturnOfCapital]", cfReturnOfCapital);
            editor.ReplaceText("[CF CapitalGain]", cfCapitalGain);
            editor.ReplaceText("[CF Dividends]", cfDividends);
            editor.ReplaceText("[CF Recallable]", cfRecallable);
            editor.ReplaceText("[CF WithholdingTax]", cfWithholdingTax);
            editor.ReplaceText("[SitzStrasse]", sitzStrasse1);
            editor.ReplaceText("[SitzStrasse2]", sitzStrasse2);
            editor.ReplaceText("[Sitz PLZ Ort]", sitzPlzOrt);
            editor.ReplaceText("[GesamtAbruf]", fundCalled);
            editor.ReplaceText("[GesamtAusschuettung]", fundDistributed);
            editor.ReplaceText("[offenePosition]", openPosition);


            // Absenderangaben

            var tables = editor.Document.EnumerateChildrenOfType<Table>();
            if (detail.CashFlowAmountCall != 0)
            {
                foreach (Table table in tables)
                {
                    if (table.Rows.Count < 3) continue;
                    TableRow row = table.Rows[0];
                    TableCell cell = row.Cells[0];
                    Paragraph paragraph = cell.Blocks[0] as Paragraph;
                    if (paragraph == null) continue;
                    if (paragraph.Inlines.Count == 0) continue;
                    if (paragraph.Inlines[0].ToString() == "Art")
                    {
                        // remove table rows if amounts are '0'
                        // if expenses are '0' remove row 2
                        // if lookback is '0' remove row 4
                        // start removing with the last rows!

                        if (detail.LookbackInterests == 0) table.Rows.RemoveAt(3);
                        if (detail.PartnershipExpenses == 0) table.Rows.RemoveAt(2);
                    }
                }
            }    
        }
     
    }
}

