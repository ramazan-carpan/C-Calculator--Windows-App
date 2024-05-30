using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace OO_programming
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    // this classs for identify for csv file head 
    public class TaxRate
    {
        public decimal MinEarnings { get; set; } 
        public decimal MaxEarnings { get; set; }
        public decimal Rate { get; set; }
        public decimal SubtractAmount { get; set; }
    }

    public class TaxRateMap : ClassMap<TaxRate>
    {
        public TaxRateMap()
        {
            Map(m => m.MinEarnings).Index(0);
            Map(m => m.MaxEarnings).Index(1);
            Map(m => m.Rate).Index(2);
            Map(m => m.SubtractAmount).Index(3);
        }
    }

    public class TaxRateLoader
    {
        public List<TaxRate> LoadTaxRates(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            }))
            {
                csv.Context.RegisterClassMap<TaxRateMap>();
                return new List<TaxRate>(csv.GetRecords<TaxRate>());
            }
        }
    }
    public class PaySlip
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal HourlyRate { get; set; } 
        public string TaxThreshold { get; set; }
        public decimal WeekHours { get; set; }
        public decimal GrossPay { get; private set; }
        public decimal Tax { get; private set; }
        public decimal NetPay { get; private set; }
        public decimal Superannuation { get; private set; }

        public override string ToString()
        {
            return $"{EmployeeID} - {FirstName} {LastName}";
        }

        public void CalculatePay(PayCalculator calculator)
        {
            // Calculate GrossPay based on HourlyRate and WeekHours
            GrossPay = calculator.CalculatePay(HourlyRate, WeekHours);

            // Calculate Tax, NetPay, and Superannuation based on GrossPay and TaxThreshold
            Tax = calculator.CalculateTax(GrossPay, TaxThreshold);
            NetPay = calculator.CalculateNetPay(GrossPay, Tax);
            Superannuation = calculator.CalculateSuperannuation(GrossPay);
        }
    }

    public class PayCalculator
    {
        public decimal SuperRate = 0.11m;

        public virtual decimal CalculatePay(decimal hourlyRate, decimal hoursWorked)
        {
            return hoursWorked * hourlyRate;
        }

        public virtual decimal CalculateTax(decimal grossPay, string taxThreshold)
        {
            return 0; // Base class returns no tax
        }

        public decimal CalculateNetPay(decimal grossPay, decimal tax)
        {
            return grossPay - tax;
        }

        public decimal CalculateSuperannuation(decimal grossPay)
        {
            return grossPay * SuperRate;
        }
    }

    public class PayCalculatorWithThreshold : PayCalculator //divide class
    {
        private List<TaxRate> _taxRatesWithThreshold;

        public PayCalculatorWithThreshold(string taxRatesWithThresholdFilePath)
        {
            var loader = new TaxRateLoader();
            _taxRatesWithThreshold = loader.LoadTaxRates(taxRatesWithThresholdFilePath);//"../../../taxrate-withthreshold.csv"
        }

        public override decimal CalculateTax(decimal grossPay, string taxThreshold)
        {
            if (taxThreshold.ToLower() == "y")
            {
                foreach (var rate in _taxRatesWithThreshold)
                {
                    if (grossPay >= rate.MinEarnings && grossPay < rate.MaxEarnings)
                    {
                        return grossPay * rate.Rate - rate.SubtractAmount;
                    }
                }
            }
            return 0m; // Default return if no matching rate found
        }
    }
    public class PayCalculatorNoThreshold : PayCalculator //divied class from PayCalculator
    {
        private List<TaxRate> _taxRatesNoThreshold;

        public PayCalculatorNoThreshold(string taxRatesNoThresholdFilePath)//string taxRatesNoThresholdFilePath
        {
            var loader = new TaxRateLoader();
            _taxRatesNoThreshold = loader.LoadTaxRates(taxRatesNoThresholdFilePath); //// "../../../taxrate-nothreshold.csv""
        }

        public override decimal CalculateTax(decimal grossPay, string taxThreshold)
        {
            if (taxThreshold.ToLower() == "n")
            {
                foreach (var rate in _taxRatesNoThreshold) // compare  gross pay between min and max until finding 
                {
                    if (grossPay >= rate.MinEarnings && grossPay < rate.MaxEarnings)
                    {
                        return grossPay * rate.Rate - rate.SubtractAmount;
                    }
                }
            }
            return 0m; // Default return if no matching rate found
        }
    }
}
