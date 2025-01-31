using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HandHistories.Objects.Players
{
    [DataContract]
    public class PlayerList : IEnumerable<Player>
    {
        [DataMember]
        private List<Player> _players = new List<Player>(10); 

        public PlayerList()
        {
            
        }

        public PlayerList(IEnumerable<Player> players)
        {
            AddRange(players);
        }

        class PlayerSorter : IComparer<Player>
        {
            public static readonly PlayerSorter Default = new PlayerSorter();

            private PlayerSorter()
            {
            }

            public int Compare(Player x, Player y)
            {
                return x.SeatNumber.CompareTo(y.SeatNumber);
            }
        }

        public void Add(Player player)
        {
            int newIndex = Math.Abs(_players.BinarySearch(player, PlayerSorter.Default)) - 1;
            _players.Insert(newIndex, player);
        }

        public void AddRange(IEnumerable<Player> players)
        {
            foreach (var item in players)
            {
                int newIndex = Math.Abs(_players.BinarySearch(item, PlayerSorter.Default)) - 1;
                _players.Insert(newIndex, item);
            }
        }

        public override bool Equals(object obj)
        {
            PlayerList playerList = obj as PlayerList;
            if (playerList == null) return false;

            if (playerList.Count != this.Count) return false;
            for (int i = 0; i < playerList.Count; i ++)
            {
                if (!playerList[i].Equals(this[i]))  
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int res = 0x2D2816FE;
            foreach (var item in this)
            {
                res = res * 31 + (item == null ? 0 : item.GetHashCode());
            }
            return res;
        }

        public List<Player> GetPlayers() { return _players; }

        public void SortList()
        {
            _players = _players.OrderBy(p => p.SeatNumber).ToList();
        }

        public void Remove(int index)
        {
            _players.RemoveAt(index);
        }

        public Player this[int index]
        {
            get { return _players[index]; }
        }        

        public Player this[string name]
        {
            get { return _players.FirstOrDefault(p => p.PlayerName == name); }
        }
       
        public int Count
        {
            get { return _players.Count; }
        }

        public IEnumerator<Player> GetEnumerator()
        {
            return _players.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}