using System.ComponentModel.DataAnnotations;

namespace App131.Models
{
    public class AppointmentViewModel
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Patient name is required.")]
        public string PatientName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please select an appointment date.")]
        public DateTime AppointmentDate { get; set; }
    }
}