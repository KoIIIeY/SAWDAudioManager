using System;

namespace SAWDAudio.Models
{
    [Serializable]
    public class Character {
        public int character_id;
        public int user_id;
        public int game_id;
        public DateTime created_at;
        public DateTime updated_at;
        public string name;
    }
}
