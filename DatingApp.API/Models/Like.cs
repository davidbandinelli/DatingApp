namespace DatingApp.API.Models
{
    public class Like {
        // Liker: l'utente a cui piace un altro utente
        // Likee: l'utente che piace ad un altro utente
        public int LikerId { get; set; }
        public int LikeeId { get; set; }
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}