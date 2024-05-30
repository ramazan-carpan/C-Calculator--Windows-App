using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OO_programming
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TaxThreshold { get; set; }


        public override string ToString()
        {
            return $"{EmployeeID} {FirstName} {LastName}";
        }

    }


    /// <summary>
    /// Class a capture details accociated with an employee's pay slip record
    /// </summary>
    public class PaySlip
    {
        // Employee details
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        // Week details
        public int WeekNumber { get; set; }
        public decimal WeekHours { get; set; }
        public decimal TaxNumber { get; set; }
        public decimal TaxThreshold { get; set; }

        // Submission details
        public string SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; }

        // Approval details
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        // Payment details
        public decimal PayGrossCalculated { get; set; }
        public decimal TaxCalculated { get; set; }
        public decimal SuperCalculated { get; set; }
        public decimal PayNetCalculated { get; set; }

    }

    /// <summary>
    /// Base class to hold all Pay calculation functions
    /// Default class behaviour is tax calculated with tax threshold applied
    /// </summary>
    public class PayCalculator
    {
        public decimal CalculateGrossPay(decimal hoursWorked, decimal hourlyRate)
        {
            return hoursWorked * hourlyRate;
        }

        public decimal CalculateTax(decimal grossPay, decimal taxThreshold)
        {
            return grossPay * (taxThreshold / 100);
        }

        public decimal CalculateNetPay(decimal grossPay, decimal tax)
        {
            return grossPay - tax;
        }

        public decimal CalculateSuperannuation(decimal grossPay)
        {
            return grossPay * 0.09m; // Assuming superannuation rate is 9%
        }

        /// <summary>
        /// Extends PayCalculator class handling No tax threshold
        /// </summary>
        public class PayCalculatorNoThreshold : PayCalculator
    {
     
    }

    /// <summary>
    /// Extends PayCalculator class handling With tax threshold
    /// </summary>
    public class PayCalculatorWithThreshold : PayCalculator
    {
     
    }
}
