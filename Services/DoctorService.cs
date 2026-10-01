using App131.Models;
using Microsoft.Extensions.Caching.Memory;

namespace App131.Services
{
    public class DoctorService
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;

        public DoctorService(IMemoryCache cache, HttpClient httpClient)
        {
            _cache = cache;
            _httpClient = httpClient;
        }

        public List<Doctor> GetDoctors()
        {
            const string cacheKey = "DoctorListCache";

            // Cache the doctor list using IMemoryCache with a 3-minute expiration
            if (!_cache.TryGetValue(cacheKey, out List<Doctor>? doctors))
            {
                doctors = new List<Doctor>
                {
                    new Doctor { DoctorId = 1, DoctorName = "Dr. Rohan Sharma", Specialization = "Cardiology", Experience = 10, ConsultationFee = 800m },
                    new Doctor { DoctorId = 2, DoctorName = "Dr. Priya Patel", Specialization = "Dermatology", Experience = 6, ConsultationFee = 600m },
                    new Doctor { DoctorId = 3, DoctorName = "Dr. Amit Verma", Specialization = "Orthopedics", Experience = 12, ConsultationFee = 1000m },
                    new Doctor { DoctorId = 4, DoctorName = "Dr. Sneha Gupta", Specialization = "Pediatrics", Experience = 8, ConsultationFee = 700m }
                };

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                _cache.Set(cacheKey, doctors, cacheEntryOptions);
            }

            return doctors ?? new List<Doctor>();
        }

        public async Task<List<PostViewModel>> GetExternalPostsAsync()
        {
            try
            {
                var posts = await _httpClient.GetFromJsonAsync<List<PostViewModel>>("https://jsonplaceholder.typicode.com/posts");
                return posts?.Take(10).ToList() ?? new List<PostViewModel>();
            }
            catch
            {
                return new List<PostViewModel>();
            }
        }
    }
}