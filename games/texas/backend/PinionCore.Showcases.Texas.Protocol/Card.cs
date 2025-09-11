using System;
using System.Collections.Generic;
namespace PinionCore.Showcases.Texas.Protocol
{
    public readonly struct Card : IEquatable<Card>, IComparable<Card>
    {
        // 核心存儲：單一 byte (0-51) 用於序列化
        public readonly byte Id;

        // 計算屬性：提供可讀性
        public Suit Suit => (Suit)(Id / 13);
        public Rank Rank => (Rank)((Id % 13) + 2);

        public Card(Suit suit, Rank rank)
        {
            var r = (int)rank;
            var s = (int)suit;
            if (r < 2 || r > 14) throw new ArgumentOutOfRangeException(nameof(rank));
            if (s < 0 || s > 3) throw new ArgumentOutOfRangeException(nameof(suit));
            Id = (byte)(s * 13 + (r - 2));
        }

        private Card(byte id)
        {
            if (id > 51) throw new ArgumentOutOfRangeException(nameof(id));
            Id = id;
        }

        public static Card FromId(byte id) => new Card(id);

        // 比較和等值
        public bool Equals(Card other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Card c && Equals(c);
        public override int GetHashCode() => Id;
        public int CompareTo(Card other) => Id.CompareTo(other.Id);

        // 可讀性支援
        public override string ToString() => $"{Rank} of {Suit}";
        public void Deconstruct(out Rank rank, out Suit suit) { rank = Rank; suit = Suit; }

        // 德州撲克常用比較器
        public static readonly IComparer<Card> RankThenSuitComparer =
            Comparer<Card>.Create((a, b) => {
                var rankCompare = a.Rank.CompareTo(b.Rank);
                return rankCompare != 0 ? rankCompare : a.Suit.CompareTo(b.Suit);
            });
    }
}
