using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Domain.Entities
{
    public class Story : Entity
    {
        private Story() { }
        public string Description { get; private set; }
        public int LikesCount { get; private set; }
        public int ViewCount { get; private set; }
        public DateTime CreationDate { get; private set; }

        public int? BlogerId {  get; private set; }
        public Story(string description, int? likes, int? views, int? blogerId) {
            if (System.String.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException("Nickname musn't be empty!", nameof(description));
            }

            Description = description;
            LikesCount = likes ?? 0;
            ViewCount = views ?? 0;
            BlogerId = blogerId;

            if (LikesCount < 0) {
                throw new ArgumentOutOfRangeException("Count of likes must be greater than zero!", nameof(likes));
            }
            if (ViewCount < 0)
            {
                throw new ArgumentOutOfRangeException("Count of views must be greater than zero!", nameof(views));
            }

            CreationDate = DateTime.Now;
        }

        public void Changedescription(string descr)
        {
            if (String.IsNullOrEmpty(descr)) return;
            Description = descr;
        }
        public void AssociateWith(int blogerId)
        {
            if (blogerId < 0) return;
            BlogerId = blogerId;
        }
        public void StopAssociateWith()
        {
            BlogerId = null;
        }
        public void ChangeViewCount(int count)
        {
            if (count <= ViewCount) return;
            ViewCount = count;
        }
        public void ChangeLikesCount(int count)
        {
            if (count < 0) return;
            LikesCount = count;
        }
       
    }
}
