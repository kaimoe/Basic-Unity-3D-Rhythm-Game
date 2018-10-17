namespace CosmicMovement {
    public struct Song {
        public string ID { get; set; }
        public string Title { get; set; }
        public int BPM { get; set; }
        public string Length { get; set; }
        public string Artist { get; set; }
        public string Description { get; set; }
        public int Difficulty { get; set; }
        public string Theme { get; set; }
        public bool Ortho { get; set; }
    }
    public struct Note {
        public string Type { get; set; }
        public float Beat { get; set; }
        public float Duration { get; set; }
        public float End { get; set; }
        public char Pos { get; set; }
        public bool Up { get; set; }
    }
    public struct SongEvent {
        public string Type { get; set; }
        public float Beat { get; set; }
        public string Parameter { get; set; }
        public float Duration { get; set; }
    }
}