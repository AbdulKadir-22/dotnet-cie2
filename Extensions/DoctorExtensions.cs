using App131.Models;

namespace App131.Extensions
{
    public static class DoctorExtensions
    {
        public static decimal CalculateTotalCharge(this Doctor doctor)
        {
            // Adds a ₹100 service charge
            return doctor.ConsultationFee + 100m;
        }
    }
}