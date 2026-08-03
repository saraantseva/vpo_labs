using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Domain.Entities
{
    public class Bloger : Entity
    {

        private List<Story> _stories = new();
        public Person PersonalData { get; private set; }
        public string Nickname { get; private set; }
        public int FollowersNumber { get; private set; }

        private Bloger() { }

        public void UpdatePersonalData(Person person) => PersonalData = person; 
        public void UpdateNickname(string nickname)
        {
            if (System.String.IsNullOrEmpty(nickname))
            {
                throw new ArgumentNullException("Nickname musn't be empty!", nameof(nickname));
            }
            Nickname = nickname;
        }
        public void UpdateFollowersNumber(int? followersNumber)
        {
            FollowersNumber = followersNumber ?? 0;

            if (FollowersNumber < 0)
            {
                throw new ArgumentOutOfRangeException("Count of followers must be greater than zero!", nameof(followersNumber));
            }
        }
        public Bloger(Person person, string nickname, int? followersNumber = 0) {
            PersonalData = person;
            if (System.String.IsNullOrEmpty(nickname))
            {
                throw new ArgumentNullException("Nickname musn't be empty!", nameof(nickname));
            }
            Nickname = nickname;
            FollowersNumber = followersNumber ?? 0;

            if (FollowersNumber < 0)
            {
                throw new ArgumentOutOfRangeException("Count of followers must be greater than zero!", nameof(followersNumber));
            }

        }
        public IReadOnlyList<Story> Stories { get => _stories.AsReadOnly(); }

        public void ChangeFollowersNumber(int number)
        {
            if (number < 0) return; 
            FollowersNumber = number;
        }

    }

}
