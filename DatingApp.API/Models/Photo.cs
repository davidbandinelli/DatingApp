using System;

namespace DatingApp.API.Models
{
    public class Photo {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }

        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        // in EF core aggiungendo la relazione inversa otteniamo la delete cascade (sulla tabella Photo) e lo UserId not nullable
        public User User { get; set; }
        public int UserId { get; set; }
        public bool IsApproved { get; set; }
    }
}